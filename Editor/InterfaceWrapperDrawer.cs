using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AlexMalyutinDev.InterfaceSerialization
{
	[CustomPropertyDrawer(typeof(InterfaceWrapper), true)]
	public class InterfaceWrapperDrawer : PropertyDrawer
	{
		private List<Type> implementations;
		private string[] implementationsNames;
		private InterfaceWrapper wrapper;
		private float height;

		private bool cached = false;

		//// Maybe someday it will work...
		// public override VisualElement CreatePropertyGUI(SerializedProperty property)
		// {
		// 	Debug.Log("CreatePropertyGUI");
		// 	var container = new VisualElement();

		// 	var testField = new PropertyField(property.FindPropertyRelative("FullTypeName"));
		// 	container.Add(testField);

		// 	return container;
		// }

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.serializedObject.Update();

			CacheImplementations(property);

			position.height = EditorGUIUtility.singleLineHeight;
			height = EditorGUIUtility.singleLineHeight;

			EditorGUI.BeginChangeCheck();
			var typeID = EditorGUI.Popup(position, property.displayName, wrapper.TypeID, implementationsNames);
			Undo.RecordObject(property.serializedObject.targetObject, "popup");
			if (EditorGUI.EndChangeCheck())
			{
				wrapper.TypeID = typeID;
				wrapper.Type = implementations[wrapper.TypeID];
				property.serializedObject.Update();
				EditorUtility.SetDirty(property.serializedObject.targetObject);
			}

			position.x += 15;
			position.width -= 15;
			foreach (var field in wrapper.Type.GetFields())
			{
				height += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
				if (field.FieldType == typeof(int))
					field.SetValue(wrapper.Value, EditorGUI.IntField(position, field.Name, (int)field.GetValue(wrapper.Value)));
			}

			property.serializedObject.ApplyModifiedProperties();
		}

		private void CacheImplementations(SerializedProperty property)
		{
			if (!cached)
			{
				var propertyType = fieldInfo.FieldType;
				var interfaceType = propertyType.BaseType.GetGenericArguments()[0];

				implementations = propertyType.Assembly.GetTypes()
					.Where(type => type.GetInterfaces().Contains(interfaceType)).ToList();
				implementations.Insert(0, interfaceType);

				implementationsNames = implementations.Select(type => type.IsInterface ? "<none>" : type.Name).ToArray();

				wrapper = (InterfaceWrapper)GetValue(property);
				try
				{
					wrapper.TypeID = implementations.FindIndex(type => type == wrapper.Type);
				}
				catch (Exception)
				{
					Debug.LogWarning("Type is missing!", property.serializedObject.targetObject);
					wrapper.TypeID = 0;
				}
				cached = true;
			}
		}

		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return height;
		}

		public object GetValue(SerializedProperty property)
		{
			return fieldInfo.GetValue(property.serializedObject.targetObject);
		}

		public void SetPrpertyValue(SerializedProperty property, object value)
		{
			fieldInfo.SetValue(property.serializedObject.targetObject, value);
		}
	}
}
