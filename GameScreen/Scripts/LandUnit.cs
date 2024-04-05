using System.Collections.Generic;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.Parser.ParserHelper;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts;

public class LandUnit
{
    private static MapController MapController => MapController.Instance;
    private LandUnit[] LandUnits => MapController.LandUnits;
    private TileMap TileMap => MapController.TileMap;
    public short Index { get; init; }
    public short RegionIndex { get; init; }
    public int X { get; init; }
    public int Y { get; init; }
    public Vector2 Position { get; init; }
    public Vector2I Coords => new(X, Y);
    public Topography Topography { get; init; }
    public bool Sea => Topography.地块类型 == 1;
    public Pitfall Pitfall { get; set; }
    public Scheme Scheme { get; set; }
    public List<Reinforcement> Reinforcements { get; } = [];
    public AirRaid AirRaid { get; set; }
    public Capital Capital { get; set; }

    #region 颜色

    private Color? _color;

    public Color? LandColor
    {
        get => _color;
        private set
        {
            if (value is { } color)
            {
                MapController.SetUvColorV(Coords, color);
                _color = color;
            }
        }
    }

    #endregion

    #region 省规划

    public short Province { get; set; }

    public void UpdateProvince()
    {
        if (!Sea)
            if (LandUnits.TryGetValue(GetBtlIndex(Province), out LandUnit landUnit))
                LandColor = Btl.Countries.TryGetValue(landUnit.Belong, out Country country)
                    ? Color.Color8(country.R, country.G, country.B)
                    : Colors.White;
    }

    #endregion

    #region 归属

    public FlagSprite FlagSprite { get; private set; }
    private byte _belong;

    public byte Belong
    {
        get => _belong;
        set
        {
            _belong = value;
            if (MapController.Instance.Countries.TryGetValue(Belong, out Country country))
            {
                if (FlagSprite == null)
                {
                    FlagSprite = new();
                    MapController.FlagRender.AddChild(FlagSprite);
                }

                FlagSprite.Position = Position;
                FlagSprite.Flag = country.国家;
            }
            else
            {
                FlagSprite?.QueueFree();
                FlagSprite = null;
            }
        }
    }

    public void UpdateBelong()
    {
        if (MapController.Instance.Countries.TryGetValue(Belong, out Country country))
        {
            Color color = Color.Color8(country.R, country.G, country.B);
            foreach (LandUnit landUnit in LandUnits)
                if (landUnit.Province == RegionIndex)
                    landUnit.LandColor = color;
        }
        else
        {
            foreach (LandUnit landUnit in LandUnits)
                if (landUnit.Province == RegionIndex)
                    landUnit.LandColor = Colors.White;
        }
    }

    #endregion

    #region 城市

    private City _city;
    private Label _cityLabel;

    public City City
    {
        get => _city;
        set
        {
            if (value != null)
            {
                _city = value;
                UpdateCity();
            }
            else
            {
                _city = null;
                _cityLabel?.QueueFree();
                _cityLabel = null;
                TileMap.EraseCell(MapController.BuildLayer, Coords);
            }
        }
    }

    public void UpdateCity()
    {
        TileMap.EraseCell(MapController.BuildLayer, Coords);
        City.坐标 = RegionIndex;
        foreach (Wc4ResourceElement wc4ResourceElement in BuildingsHd.Images.ImageList)
        {
            var level = City.等级;
            if (level is 16 or 17 or 18 or 19 or 20)
                level = 15;
            if (wc4ResourceElement.Name == $"building_{level}.png")
            {
                TileMap.SetCell(MapController.BuildLayer, Coords, MapController.BuildTileSetAtlasId,
                    new Vector2I(wc4ResourceElement.X, wc4ResourceElement.Y));
                break;
            }
        }

        if (Stringtable.CityName[City.名称.ToString("D3")] is { } name)
        {
            if (_cityLabel == null)
            {
                _cityLabel = new();
                TileMap.AddChild(_cityLabel);
            }

            _cityLabel.Text = name;
            _cityLabel.ResetSize();
            _cityLabel.Position = Position - _cityLabel.Size / 2 + new Vector2(0f, 60f);
        }
        else
        {
            _cityLabel?.QueueFree();
            _cityLabel = null;
        }
    }

    #endregion

    #region 军队

    private Army _army;
    public ArmySprite ArmySprite { get; private set; }
    public GeneralSprite GeneralSprite { get; private set; }
    public ArmyJson ArmyJson { get; private set; }
    public GeneralJson GeneralJson { get; private set; }

    public Army Army
    {
        get => _army;

        set
        {
            _army = value;
            UpdateArmy();
        }
    }

    public void UpdateArmy()
    {
        if (Army != null)
        {
            GeneralJson = null;
            foreach (GeneralJson generalJson in GeneralSettings.GeneralJsons)
                if (generalJson.Id == Army.将领 && generalJson.Name != null)
                {
                    if (GeneralSprite == null)
                    {
                        GeneralSprite = new();
                        MapController.GeneralRender.AddChild(GeneralSprite);
                    }

                    GeneralJson = generalJson;
                    GeneralSprite.Position = Position;
                    GeneralSprite.GeneralJson = generalJson;
                    break;
                }

            if (GeneralJson == null)
            {
                GeneralSprite?.QueueFree();
                GeneralSprite = null;
            }

            foreach (ArmyJson armyJson in ArmySettings.ArmyJsons)
                if (armyJson.Army == Army.兵种)
                {
                    ArmyJson = armyJson;
                    break;
                }

            if (ArmySprite == null)
            {
                ArmySprite = new();
                MapController.ArmyRender.AddChild(ArmySprite);
            }

            Army.坐标 = RegionIndex;
            ArmySprite.Army = Army;
            ArmySprite.Position = Position;
        }
        else
        {
            _army = null;
            ArmyJson = null;
            GeneralJson = null;
            GeneralSprite?.QueueFree();
            GeneralSprite = null;
            ArmySprite?.QueueFree();
            ArmySprite = null;
        }
    }

    #endregion

    public static void UpdateRender() => MapController.ApplyShader();
}