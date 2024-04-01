using System.Collections.Generic;
using BtlEditor.CoreScripts;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.Parser.ParserHelper;
using static BtlEditor.CoreScripts.StaticRes;
using static BtlEditor.CoreScripts.TileSetDictionary;

namespace BtlEditor.GameScreen.Scripts;

public class LandUnit(MapController mapController)
{
    private LandUnit[] LandUnits => mapController.LandUnits;
    private TileMap TileMap => mapController.TileMap;
    private TileSet TileSet => TileMap.TileSet;
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
                mapController.SetUvColorV(Coords, color);
                _color = color;
            }
        }
    }

    #endregion

    #region 省规划

    public short Province { get; set; }

    public void UpdateProvince(short province)
    {
        if (!Sea)
        {
            if (LandUnits.TryGetValue(GetBtlIndex(province), out LandUnit landUnit))
                LandColor = Btl.Countries.TryGetValue(landUnit.Belong, out Country country)
                    ? Color.Color8(country.R, country.G, country.B)
                    : Colors.White;
            else LandColor = Colors.White;
            Province = province;
        }
    }

    #endregion

    #region 归属

    public byte Belong { get; set; }

    public void UpdateBelong(byte belong)
    {
        TileMap.EraseCell(MapController.FlagLayer, Coords);
        foreach (LandUnit landUnit in LandUnits)
            if (landUnit.Province == RegionIndex)
                landUnit.LandColor = Colors.White;

        if (mapController.Countries.TryGetValue(belong, out Country country))
        {
            foreach (Wc4ResourceElement wc4ResourceElement in Tacticalmap.Images.ImageList)
                if (wc4ResourceElement.Name == $"f_{country.国家:D2}.png")
                {
                    Vector2I atlasCoords = new(wc4ResourceElement.X, wc4ResourceElement.Y);
                    TileMap.SetCell(MapController.FlagLayer, Coords, MapController.TacticalmapTileSetAtlasId, atlasCoords);
                    break;
                }

            Color color = Color.Color8(country.R, country.G, country.B);
            foreach (LandUnit landUnit in LandUnits)
                if (landUnit.Province == RegionIndex)
                    landUnit.LandColor = color;
        }

        Belong = belong;
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
    private readonly SpriteFrames _spriteFrames = ResourceLoader.Load<SpriteFrames>("res://Assets/Textures/army_frame.tres");
    private AnimatedSprite2D _armySprite;
    public ArmyJson ArmyJson { get; private set; }
    public GeneralJson GeneralJson { get; private set; }

    public Army Army
    {
        get => _army;

        set
        {
            if (value != null)
            {
                _army = value;
                if (_armySprite == null)
                {
                    _armySprite = new() { SpriteFrames = _spriteFrames };
                    TileMap.AddChild(_armySprite);
                }

                UpdateArmy();
            }
            else
            {
                _army = null;
                ArmyJson = null;
                GeneralJson = null;
                _cityLabel?.QueueFree();
                _cityLabel = null;
                TileMap.EraseCell(MapController.ArmyLayer, Coords);
                TileMap.EraseCell(MapController.LevelLayer, Coords);
                TileMap.EraseCell(MapController.StackLayer, Coords);
            }
        }
    }

    public void UpdateArmy()
    {
        _armySprite.Frame = ArmyDictionary.GetValueOrDefault(Army.兵种, 0);
        TileMap.SetCell(MapController.ArmyLayer, Coords, MapController.ArmyTileSetAtlasId, vector2I, Army.方向);
        else TileMap.EraseCell(MapController.ArmyLayer, Coords);
        TileMap.SetCell(MapController.LevelLayer, Coords, MapController.LevelTileSetAtlasId, new(Army.等级 - 2, 0));
        TileMap.SetCell(MapController.StackLayer, Coords, MapController.StackTileSetAtlasId, new(Army.编制 - 1, 0));

        foreach (GeneralJson generalJson in GeneralSettings.GeneralJsons)
        {
            if (generalJson.Id == Army.将领 && generalJson.Name != null)
            {
                GeneralJson = generalJson;
                mapController.Additional.QueueRedraw();
                break;
            }

            GeneralJson = null;
        }

        foreach (ArmyJson armyJson in ArmySettings.ArmyJsons)
            if (armyJson.Army == Army.兵种)
            {
                ArmyJson = armyJson;
                break;
            }
    }

    #endregion

    public void UpdateRender() => mapController.ApplyShader();
}