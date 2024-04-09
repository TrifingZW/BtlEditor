using System;
using System.Reflection;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.GameScreen.Scripts.LandScripts;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Multi;

public partial class ArmyMulti : BaseMulti
{
    public override void Initialize()
    {
        if (Btl.Version1) ReflexStruct<Army1>();
        if (Btl.Version2 || Btl.Version3) ReflexStruct<Army2>();
    }

    protected override void Update(FieldInfo field, int value, LandUnit landUnit)
    {
        if (landUnit.Army is { } army)
        {
            field.SetValue(army, Convert.ChangeType(value, field.FieldType));
            landUnit.UpdateArmy();
        }
    }
}