using System;
using System.Reflection;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.GameScreen.Scripts.LandScripts;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Multi;

public partial class CityMulti : BaseMulti
{
    public override void Initialize()
    {
        ReflexStruct<City>();
    }
    
    protected override void Update(FieldInfo field, int value, GameLandUnit gameLandUnit)
    {
        if (gameLandUnit.City is { } city)
        {
            field.SetValue(city, Convert.ChangeType(value, field.FieldType));
            gameLandUnit.UpdateCity();
        }
    }
}