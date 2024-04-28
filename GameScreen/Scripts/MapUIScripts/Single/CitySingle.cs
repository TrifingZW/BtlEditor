using BtlEditor.CoreScripts.Structures;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class CitySingle : BaseSingle
{
    protected override void Update()
    {
        if (LandUnit.City is { } city)
        {
            ReflexStruct(city, TreeContainer, Save);
            
            Button copy = CreateButton("复制城市", () =>
            {
                Game.CityCopy = (City)city.Clone();
                Game.BelongCopy = LandUnit.Belong;
            });
            EndContainer.AddChild(copy);
            
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
            Button paste = CreateButton("粘贴城市", () =>
            {
                if (Game.CityCopy is null) return;
                LandUnit.City = (City)Game.CityCopy?.Clone();
                LandUnit.Belong = LandUnit.ProvinceBelong;
                LandUnit.Province = LandUnit.RegionIndex;
                LandUnit.UpdateProvinceColor();
                Game.Instance.MapController.UpdateColorUV();
                Clear();
                Update();
            });
            EndContainer.AddChild(paste);


            Button add = CreateButton("添加城市", () =>
            {
                LandUnit.City = new() { 坐标 = LandUnit.RegionIndex };
                LandUnit.Belong = LandUnit.ProvinceBelong;
                LandUnit.Province = LandUnit.RegionIndex;
                LandUnit.UpdateProvinceColor();
                Game.Instance.MapController.UpdateColorUV();
                Clear();
                Update();
            });
            EndContainer.AddChild(add);
        }
    }

    private void Save()
    {
        LandUnit.UpdateCity();
    }
}