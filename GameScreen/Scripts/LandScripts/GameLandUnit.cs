using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts.CustomSprite;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;
using static BtlEditor.GameScreen.Scripts.MapHelper;

namespace BtlEditor.GameScreen.Scripts.LandScripts;

public class GameLandUnit : LandUnit
{
    private static MapController MapController => Game.Instance.MapController;
    private static GameLandUnit[] LandUnits => MapController.LandUnits;
    private static TileMapLayer TileMapLayer => MapController.TileMapLayer;

    #region 颜色

    private Color? _color;

    public Color? LandColor
    {
        get => _color;
        set
        {
            if (Sea) return;
            if (value is not { } color) return;
            MapController.SetUvColorV(Coords, color);
            _color = color;
        }
    }

    #endregion

    #region 省规划

    public void UpdateProvinceColor()
    {
        if (Sea) return;
        LandColor = LandUnits.TryGetValue(GetBtlIndex(Province), out GameLandUnit landUnit)
            ? Btl.Countries.TryGetValue(landUnit.Belong, out Country country)
                ? Color.Color8(country.R8, country.G8, country.B8)
                : Colors.White
            : Colors.White;
    }

    #endregion

    #region 归属

    public FlagSprite FlagSprite { get; private set; }

    public new byte Belong
    {
        get => base.Belong;
        set
        {
            if (Army is null && City is null) base.Belong = 0xff;
            else
            {
                base.Belong = value;

                foreach (Reinforcement reinforcement in Reinforcements)
                    switch (reinforcement)
                    {
                        case Reinforcement1 reinforcement1:
                            reinforcement1.所属国家 = Belong;
                            break;
                        case Reinforcement3 reinforcement3:
                            reinforcement3.所属国家 = Belong;
                            break;
                    }
            }

            if (MapController.Countries.TryGetValue(Belong, out Country country))
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

    public byte ProvinceBelong => LandUnits.TryGetValue(GetBtlIndex(Province), out GameLandUnit landUnit) ? landUnit.Belong : (byte)0xff;
    public Country BelongCountry => MapController.Countries.TryGetValue(ProvinceBelong, out Country country) ? country : null;

    public void UpdateBelongColor()
    {
        if (MapController.Countries.TryGetValue(Belong, out Country country))
        {
            var color = Color.Color8(country.R8, country.G8, country.B8);
            foreach (GameLandUnit landUnit in LandUnits)
                if (landUnit.Province == RegionIndex)
                    landUnit.LandColor = color;
        }
        else
        {
            foreach (GameLandUnit landUnit in LandUnits)
                if (landUnit.Province == RegionIndex)
                    landUnit.LandColor = Colors.White;
        }
    }

    public void ExamineBelong()
    {
        if (Army is not null || City is not null) return;
        Belong = 0xff;
        UpdateBelongColor();
    }

    #endregion

    #region 城市

    private CitySprite _citySprite;
    private Label _cityLabel;

    public new City City
    {
        get => base.City;
        set
        {
            base.City = value;
            UpdateCity();
        }
    }

    public void UpdateCity()
    {
        if (City != null)
        {
            City.坐标 = RegionIndex;
            if(_citySprite == null)
            {
                _citySprite = new();
                MapController.CityRender.AddChild(_citySprite);
                _citySprite.Position = Position;
                _citySprite.City = City;
            }

            if (Stringtable.CityName[City.名称.ToString("D3")] is { } name)
            {
                if (_cityLabel == null)
                {
                    _cityLabel = new();
                    MapController.CityRender.AddChild(_cityLabel);
                }

                _cityLabel.Text = name;
                _cityLabel.ResetSize();
                _cityLabel.Position = Position - _cityLabel.Size / 2 + new Vector2(0f, 65f);
            }
            else
            {
                _cityLabel?.QueueFree();
                _cityLabel = null;
            }
        }
        else
        {
            ClearCity();
            ExamineBelong();
            MapController.UpdateColorUV();
        }
    }

    public void ClearCity()
    {
        base.City = null;
        _citySprite?.QueueFree();
        _cityLabel = null;
        _cityLabel?.QueueFree();
        _cityLabel = null;
        TileMapLayer.EraseCell(Coords);
    }

    #endregion

    #region 军队

    public ArmySprite ArmySprite { get; private set; }
    public GeneralSprite GeneralSprite { get; private set; }
    public ArmyJson ArmyJson { get; private set; }
    public GeneralJson GeneralJson { get; private set; }

    public new Army Army
    {
        get => base.Army;

        set
        {
            base.Army = value;
            UpdateArmy();
        }
    }

    public void UpdateArmy()
    {
        if (Army != null)
        {
            if (Army.方向 > 1) Army.方向 = 1;

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

            ArmyJson = null;
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
            ClearArmy();
            ExamineBelong();
            MapController.UpdateColorUV();
        }
    }

    public void ClearArmy()
    {
        base.Army = null;
        ArmyJson = null;
        GeneralJson = null;
        GeneralSprite?.QueueFree();
        GeneralSprite = null;
        ArmySprite?.QueueFree();
        ArmySprite = null;
    }

    #endregion
}