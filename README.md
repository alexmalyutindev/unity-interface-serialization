# Unity Interface serialization

## Example
```
public interface IEffect 
{
    public void Apply(ICreature current, ICreature target);
}

public class MagicEffect : IEffect
{
    [SerializeField]
    private float _power;
    
    public void Apply(ICreature current, ICreature target) 
    {
        ...
    }
}

public class HealEffect : IEffect
{
    [SerializeField]
    private float _heal;
    [SerializeField]
    private float _time;
    
    public void Apply(ICreature current, ICreature target) 
    {
        ...
    }
}
```
```
[Serializable]
public class Effect : InterfaceWrapper<IEffect> { }

[CreateAssetMenu(fileName = "WizardModel", menuName = "Data/Creature/WizardModel")]
public class WizardModel : ScriptableObject, ICreature
{
    public Effect effect;
    ...
}
```
