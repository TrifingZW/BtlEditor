using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class CapitalSingle : BaseSingle
{
    protected override void UserInface()
    {
        if (GameLandUnit.Capital is { } capital)
        {
            ReflexStruct(capital, Container);
            Button delete = CreateButton("删除国旗", () =>
            {
                GameLandUnit.Capital = null;
                Update();
            });
            Container.AddChild(delete);
        }
        else
        {
            Button add = CreateButton("添加国旗", () =>
            {
                GameLandUnit.Capital = new() { 坐标 = GameLandUnit.RegionIndex };
                Update();
            });
            Container.AddChild(add);
        }
    }
}