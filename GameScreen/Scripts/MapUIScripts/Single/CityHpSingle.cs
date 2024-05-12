using BtlEditor.CoreScripts.Structures;
using Godot;
using static BtlEditor.GameScreen.Scripts.MapHelper;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class CityHpSingle : BaseSingle
{
    protected override void UserInface()
    {
        if (GameLandUnit.CityHp is { } cityHp)
        {
            ReflexStruct(cityHp, Container, false, "血量加成","当前血量","血量上限");
            Button delete = CreateButton("删除血率", () =>
            {
                GameLandUnit.CityHp = null;
                Update();
            });
            Container.AddChild(delete);
        }
        else
        {
            if (GameLandUnit.City == null) return;
            Button add = CreateButton("添加血率", () =>
            {
                if (Btl.Version1) GameLandUnit.CityHp = new Army1();
                if (Btl.Version2 || Btl.Version3) GameLandUnit.CityHp = new Army2();
                GameLandUnit.CityHp.坐标 = GameLandUnit.RegionIndex;
                GameLandUnit.CityHp.兵种 = 39;
                GameLandUnit.Belong = GameLandUnit.ProvinceBelong;
                Update();
            });
            Container.AddChild(add);
        }
    }
}