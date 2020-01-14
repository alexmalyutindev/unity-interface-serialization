using System;
using UnityEngine;

namespace AlexMalyutinDev.InterfaceSerialization
{
	[Serializable]
	public class InterfaceWrapper
	{
		[SerializeField]
		protected string FullTypeName;
		[SerializeField]
		protected string FullAssemblyName;
		[SerializeField]
		protected string JSON;

		[SerializeField]
		public int TypeID;

		public virtual object Value { get; }
		public virtual Type Type { get; set; }
	}

	[Serializable]
	public class InterfaceWrapper<I> : InterfaceWrapper, ISerializationCallbackReceiver
	{
		internal InterfaceWrapper()
		{
			Type = typeof(I);
		}

		private I _value;
		public override object Value => _value;

		public override Type Type
		{
			get => !String.IsNullOrEmpty(FullTypeName) ?
				Type.GetType($"{FullTypeName}, {FullAssemblyName}") ?? typeof(I) :
				typeof(I);
			set
			{
				FullTypeName = value.FullName;
				FullAssemblyName = value.Assembly.FullName;
				if (!value.IsInterface)
					_value = (I)Activator.CreateInstance(value);
			}
		}

		void ISerializationCallbackReceiver.OnBeforeSerialize()
		{
			if (!Type.IsInterface)
				JSON = JsonUtility.ToJson(Convert.ChangeType(_value, Type));
		}

		void ISerializationCallbackReceiver.OnAfterDeserialize()
		{
			if (!Type.IsInterface)
				_value = (I)JsonUtility.FromJson(JSON, Type);
		}

		public static explicit operator I(InterfaceWrapper<I> wrapper) => wrapper._value;
	}
}
