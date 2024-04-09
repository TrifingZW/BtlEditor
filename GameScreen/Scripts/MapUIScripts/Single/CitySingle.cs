using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class CitySingle : BaseSingle
{
    protected override void Update()
    {
        if (LandUnit.City is { } city)
        {
            ReflexStruct(city, TreeContainer, Save);
            Button delete = new();
            delete.Pressed += () =>
            {
                LandUnit.City = null;
                Clear();
                Update();
            };
            delete.Text = "删除城市";
            EndContainer.AddChild(delete);
        }
        else
        {
            Button add = new();
            add.Pressed += () =>
            {
                LandUnit.City = new() { 坐标 = LandUnit.RegionIndex };
                LandUnit.Province = LandUnit.RegionIndex;
                LandUnit.UpdateProvinceColor();
                Game.Instance.MapController.UpdateShader();
                Clear();
                Update();
            };
            add.Text = "添加城市";
            EndContainer.AddChild(add);
        }
    }

    private void Save()
    {
        LandUnit.UpdateCity();
    }
}