using Godot;

namespace BtlEditor.GameScreen.Scripts.Single;

public partial class CitySingle : BaseSingle
{
    protected override void Update()
    {
        Clear();
        if (LandUnit.City is { } city)
            ReflexStruct(city, TreeContainer, Save);
        else
        {
            Button addCity = new();
            addCity.Pressed += () =>
            {
                LandUnit.City = new()
                {
                    坐标 = LandUnit.RegionIndex
                };
                LandUnit.UpdateProvince(LandUnit.RegionIndex);
                LandUnit.UpdateRender();
                Update();
            };
            addCity.Text = "添加城市";
            EndContainer.AddChild(addCity);
        }
    }

    private void Save()
    {
        LandUnit.UpdateCity();
    }
}