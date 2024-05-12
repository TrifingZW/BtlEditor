using BtlEditor.CoreScripts.Structures;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class CitySingle : BaseSingle
{
    protected override void UserInface()
    {
        if (GameLandUnit.City is { } city)
        {
            ReflexStruct(city, Container, Save);

            Button copy = CreateButton("复制城市", () =>
            {
                Game.CityCopy = (City)city.Clone();
                Game.BelongCopy = GameLandUnit.Belong;
            });
            Container.AddChild(copy);

            Button delete = CreateButton("删除城市", () =>
            {
                GameLandUnit.City = null;
                Update();
            });
            Container.AddChild(delete);
        }
        else
        {
            Button paste = CreateButton("粘贴城市", () =>
            {
                if (Game.CityCopy is null) return;
                GameLandUnit.City = (City)Game.CityCopy?.Clone();
                GameLandUnit.Belong = GameLandUnit.ProvinceBelong;
                GameLandUnit.Province = GameLandUnit.RegionIndex;
                GameLandUnit.UpdateProvinceColor();
                Game.Instance.MapController.UpdateColorUV();
                Update();
            });
            Container.AddChild(paste);


            Button add = CreateButton("添加城市", () =>
            {
                GameLandUnit.City = new() { 坐标 = GameLandUnit.RegionIndex };
                GameLandUnit.Belong = GameLandUnit.ProvinceBelong;
                GameLandUnit.Province = GameLandUnit.RegionIndex;
                GameLandUnit.UpdateProvinceColor();
                Game.Instance.MapController.UpdateColorUV();
                Update();
            });
            Container.AddChild(add);
        }
    }

    private void Save()
    {
        GameLandUnit.UpdateCity();
    }
}