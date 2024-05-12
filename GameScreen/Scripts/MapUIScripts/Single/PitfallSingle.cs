using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class PitfallSingle : BaseSingle
{
    protected override void UserInface()
    {
        if (GameLandUnit.Pitfall is { } pitfall)
        {
            ReflexStruct(pitfall, Container, Save);
            Button delete = CreateButton("删除陷阱", () =>
            {
                GameLandUnit.Pitfall = null;
                Update();
            });
            Container.AddChild(delete);
        }
        else
        {
            Button add = CreateButton("添加陷阱", () =>
            {
                GameLandUnit.Pitfall = new()
                {
                    坐标 = GameLandUnit.RegionIndex
                };
                Update();
            });
            Container.AddChild(add);
        }
    }

    private static void Save()
    {
    }
}