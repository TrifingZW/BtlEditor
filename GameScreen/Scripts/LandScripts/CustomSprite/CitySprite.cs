using System.Linq;
using BtlEditor.CoreScripts.Structures;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.LandScripts.CustomSprite;

public partial class CitySprite : Sprite2D
{
    private City _city;

    public City City
    {
        get => _city;
        set
        {
            _city = value;
            if (BuildingsHd.Images.ImageList.FirstOrDefault(e =>
                {
                    if (City.奇观 != 0)
                        return e.Name == $"capital_{City.奇观:D2}.png";
                    
                    var level = City.等级;
                    switch (level)
                    {
                        case 31 or 32 or 33 or 34:
                            return e.Name == $"building_31_{City.外观}.png";
                        case 16 or 17 or 18 or 19 or 20:
                            level = 15;
                            break;
                    }

                    return e.Name == $"building_{level}.png";
                }) is { } image)
                RegionRect = image.GetRect2();
            else RegionRect= new(0, 0, 0, 0);
        }
    }

    public override void _Ready()
    {
        RegionEnabled = true;
        Texture = BuildingsHd.Texture2D;
    }
}