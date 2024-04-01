using System;
using System.Reflection;
using BtlEditor.CoreScripts.Structures;

namespace BtlEditor.GameScreen.Scripts.Multi;

public partial class CityMulti : BaseMulti
{
    public override void Initialize()
    {
        ReflexStruct<City>();
    }
    
    protected override void Update(FieldInfo field, int value, LandUnit landUnit)
    {
        if (landUnit.City is { } city)
        {
            field.SetValue(city, Convert.ChangeType(value, field.FieldType));
            landUnit.UpdateCity();
        }
    }
}