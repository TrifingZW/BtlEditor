using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class PitfallSingle : BaseSingle
{
    protected override void Update()
    {
        if (LandUnit.Pitfall is { } pitfall)
            ReflexStruct(pitfall, TreeContainer, Save);
        else
        {
            Button add = new();
            add.Pressed += () =>
            {
                LandUnit.Pitfall = new()
                {
                    坐标 = LandUnit.RegionIndex
                };
                Clear();
                Update();
            };
            add.Text = "添加陷阱";
            EndContainer.AddChild(add);
        }
    }

    private static void Save()
    {
    }
}