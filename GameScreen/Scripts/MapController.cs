using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using BtlEditor.GameScreen.Scripts.MapUIScripts;
using Godot;
using static BtlEditor.CoreScripts.Globals;
using static BtlEditor.CoreScripts.Parser.ParserHelper;
using static BtlEditor.CoreScripts.StaticRes;
using FileAccess = Godot.FileAccess;

namespace BtlEditor.GameScreen.Scripts;

public partial class MapController : CanvasGroup
{
    //节点
    private static Game Game => Game.Instance;
    public static CameraController CameraController => Game.CameraController;

    private SubViewport _subViewport;
    private Sprite2D _land;
    private Node2D _topography;

    private Sprite2D _seaRender;
    private Sprite2D _landRender;
    private TileSet _tileSet;
    private static MapUI MapUI => Game.MapUI;
    private static DataUIScripts.DataUI DataUI => Game.DataUI;
    public Node2D ArmyRender { get; private set; }
    public Node2D FlagRender { get; private set; }
    public Node2D GeneralRender { get; private set; }
    public TileMap TileMap { get; private set; }
    public Indicator Indicator { get; private set; }

    //单位
    public LandUnit[] LandUnits { get; private set; }

    //图块集ID
    public static int BuildTileSetAtlasId { get; private set; } = -1;
    public const int MultiTileSetAtlasId = 1;
    public const int SingleTileSetAtlasId = 2;

    //tileLayer
    public const int MultiLayer = 0;
    public const int SingleLayer = 1;
    public const int BuildLayer = 2;
    public const int PitfallLayer = 3;

    //Btl数据
    public List<Country> Countries { get; } = Btl.Countries.ToList();
    public Topography[] Topographies { get; private set; }
    public List<Weather> Weathers { get; } = Btl.Weathers.ToList();
    public List<Affair> Affairs { get; } = Btl.Affairs.ToList();
    public List<Strategy> Strategies { get; } = Btl.Strategies.ToList();
    public List<AirSupport> AirSupports { get; } = Btl.AirSupports.ToList();
    public List<ArmyPlacement> ArmyPlacements { get; } = Btl.ArmyPlacements.ToList();

    private static Master Master => Btl.Master;

    public const int TileWidth = 148;
    public const int TileHeight = 129;
    public const int SideLength = 76;
    public static Vector2 OffsetSize => new(TileWidth / 2f, TileHeight / 2f);

    public Vector2I CanvasSize => new Vector2(TileMap.MapToLocal(new(Master.地图宽 - 1, 1)).X,
        TileMap.MapToLocal(new(1, Master.地图高 - 1)).Y).ToVector2I() + OffsetSize.ToVector2I();


    public override void _Ready()
    {
        //SubViewport
        _subViewport = GetNode<SubViewport>("%SubViewport");
        _land = GetNode<Sprite2D>("%Land");
        _topography = GetNode<Node2D>("%Topography");

        //CanvasGroup
        _seaRender = GetNode<Sprite2D>("%SeaRender");
        _landRender = GetNode<Sprite2D>("%LandRender");
        TileMap = GetNode<TileMap>("%TileMap");
        Indicator = GetNode<Indicator>("%Indicator");
        ArmyRender = GetNode<Node2D>("%ArmyRender");
        FlagRender = GetNode<Node2D>("%FlagRender");
        GeneralRender = GetNode<Node2D>("%GeneralRender");
        _tileSet = TileMap.TileSet;

        //读取图块集
        BuildTileSetAtlasId = LoadTileSetAtlasSource(BuildingsHd);

        //创建ColorUv射影图
        CreateColorUv();

        //设置数据
        SetData();

        //设置绘制
        _topography.Draw += DrawTopography;

        //设置大小
        SetSize();

        //应用着色器
        UpdateColorUV();
    }

    //更新
    private IEnumerable<Vector2I> _points;

    //设置数据
    private void SetData()
    {
        //读取地形
        if (Btl.IndependentTerrain)
            Topographies = Btl.Topographies;
        else
        {
            Topographies = new Topography[Master.地块总数];
            var index = 0;
            for (var y = Master.地图截取y; y < Master.地图截取y + Master.地图高; y++)
            for (var x = Master.地图截取x; x < Master.地图截取x + Master.地图宽; x++)
            {
                Topographies[index] = Bin.Topographies[ParserHelper.GetIndex(x, y, Bin.Width)];
                index++;
            }
        }

        LandUnits = new LandUnit[Master.地块总数];
        for (var y = 0; y < Master.地图高; y++)
        {
            for (var x = 0; x < Master.地图宽; x++)
            {
                var landIndex = ParserHelper.GetIndex(x, y, Master.地图宽);
                LandUnit landUnit = new()
                {
                    Index = (short)landIndex,
                    RegionIndex = (short)(Btl.IndependentTerrain ? landIndex : ParserHelper.GetIndex(x + Master.地图截取x, y + Master.地图截取y, Bin.Width)),
                    X = x,
                    Y = y,
                    Position = TileMap.MapToLocal(new(x, y)),
                    Topography = Topographies[landIndex],
                    Province = Btl.Provinces[landIndex],
                };
                LandUnits[landIndex] = landUnit;
            }
        }

        //读取城市
        foreach (City city in Btl.Cities)
            if (LandUnits.TryGetValue(GetBtlIndex(city.坐标), out LandUnit landUnit))
                landUnit.City = city;

        //读取军队
        if (Btl.Version1)
            foreach (Army1 army in Btl.Armies1)
                if (LandUnits.TryGetValue(GetBtlIndex(army.坐标), out LandUnit landUnit))
                    landUnit.Army = army;
        if (Btl.Version2 || Btl.Version3)
            foreach (Army2 army in Btl.Armies2)
                if (LandUnits.TryGetValue(GetBtlIndex(army.坐标), out LandUnit landUnit))
                    landUnit.Army = army;

        //读取陷阱
        foreach (Pitfall btlParserPitfall in Btl.Pitfalls)
            if (LandUnits.TryGetValue(GetBtlIndex(btlParserPitfall.坐标), out LandUnit landUnit))
                landUnit.Pitfall = btlParserPitfall;

        //读取方案
        foreach (Scheme btlParserScheme in Btl.Schemes)
            if (LandUnits.TryGetValue(GetBtlIndex(btlParserScheme.目标地块), out LandUnit landUnit))
                landUnit.Scheme = btlParserScheme;

        //读取援军
        if (Btl.Version1 || Btl.Version2)
            foreach (Reinforcement1 btlParserReinforcement in Btl.Reinforcements1)
                if (LandUnits.TryGetValue(GetBtlIndex(btlParserReinforcement.坐标), out LandUnit landUnit))
                    landUnit.Reinforcements.Add(btlParserReinforcement);
        if (Btl.Version3)
            foreach (Reinforcement3 btlParserReinforcement in Btl.Reinforcements3)
                if (LandUnits.TryGetValue(GetBtlIndex(btlParserReinforcement.坐标), out LandUnit landUnit))
                    landUnit.Reinforcements.Add(btlParserReinforcement);

        //读取空袭
        foreach (AirRaid btlParserAirRaid in Btl.AirRaids)
            if (LandUnits.TryGetValue(GetBtlIndex(btlParserAirRaid.坐标), out LandUnit landUnit))
                landUnit.AirRaid = btlParserAirRaid;

        //读取首都
        foreach (Capital capital in Btl.Capitals)
            if (LandUnits.TryGetValue(GetBtlIndex(capital.坐标), out LandUnit landUnit))
                landUnit.Capital = capital;

        //读取归属
        for (var index = 0; index < Btl.Belongs.Length; index++)
            LandUnits[index].Belong = Btl.Belongs[index];

        //更新绘制
        var tasks = Master.地图高; //任务数
        var handleCount = Master.地图宽; //每个任务的遍历数
        using CountdownEvent countdown = new(tasks);
        for (var i = 0; i < tasks; i++)
        {
            var start = i * handleCount;
            var end = (i + 1) * handleCount;
            // 确保最后一个任务能够处理所有剩余的列表
            if (i == tasks - 1)
                end = LandUnits.Length;
            ThreadPool.QueueUserWorkItem(_ =>
            {
                foreach (LandUnit landUnit in LandUnits[start..end])
                    landUnit.UpdateProvinceColor();

                // ReSharper disable once AccessToDisposedClosure
                countdown.Signal();
            });
        }

        countdown.Wait();
    }

    //地形绘制
    private void DrawTopography()
    {
        Terrains terrains = TerrainConfig.Terrains;

        foreach (LandUnit landUnit in LandUnits)
        {
            Topography topography = landUnit.Topography;
            foreach (Terrain terrain in terrains.ImageList)
                if (topography.装饰类型B == terrain.TerrainG)
                    foreach (Tile tile in terrain.Tiles)
                        if (topography.装饰BID == tile.Idx)
                            SetTexture(landUnit, new((sbyte)topography.装饰BX, (sbyte)topography.装饰BY), tile);
        }

        foreach (LandUnit landUnit in LandUnits)
        {
            Topography topography = landUnit.Topography;
            foreach (Terrain terrain in terrains.ImageList)
                if (topography.装饰类型A == terrain.TerrainG)
                    foreach (Tile tile in terrain.Tiles)
                        if (topography.装饰AID == tile.Idx)
                            SetTexture(landUnit, new((sbyte)topography.装饰AX, (sbyte)topography.装饰AY), tile);
        }

        foreach (LandUnit landUnit in LandUnits)
        {
            Topography topography = landUnit.Topography;
            foreach (Terrain terrain in terrains.ImageList)
                if (topography.地块类型 == terrain.TerrainG)
                    foreach (Tile tile in terrain.Tiles)
                        if (topography.地块ID == tile.Idx)
                            SetTexture(landUnit, new((sbyte)topography.地块X, (sbyte)topography.地块X), tile);
        }

        return;

        void SetTexture(LandUnit landUnit, Vector2 offset, Tile tile)
        {
            if (TerrainHd.Images.ImageList.FirstOrDefault(e => e.Name == tile.Image) is { } element)
            {
                Rect2 scrRect = new()
                {
                    Position = new(element.X, element.Y),
                    Size = new(element.Width, element.Height)
                };
                Rect2 rect = new()
                {
                    Position = (landUnit.Position - new Vector2(element.RefX, element.RefY) + offset) * RenderScaleValue,
                    Size = new Vector2(element.Width, element.Height) * RenderScaleValue
                };
                _topography.DrawTextureRectRegion(TerrainHd.Texture2D, rect, scrRect);
            }

            if (PlantHd.Images.ImageList.FirstOrDefault(e => e.Name == tile.Image) is { } element2)
            {
                Rect2 scrRect = new()
                {
                    Position = new(element2.X, element2.Y),
                    Size = new(element2.Width, element2.Height)
                };
                Rect2 rect = new()
                {
                    Position = (landUnit.Position - new Vector2(element2.RefX, element2.RefY) + offset) * RenderScaleValue,
                    Size = new Vector2(element2.Width, element2.Height) * RenderScaleValue
                };
                _topography.DrawTextureRectRegion(PlantHd.Texture2D, rect, scrRect);
            }
        }
    }

    //读取图块集
    private int LoadTileSetAtlasSource(Wc4ResourceParser wc4ResourceParser, Vector2I offset = default)
    {
        TileSetAtlasSource buildTileSetAtlasSource = new();
        buildTileSetAtlasSource.Texture = wc4ResourceParser.Texture2D;
        buildTileSetAtlasSource.TextureRegionSize = Vector2I.One;
        foreach (Wc4ResourceElement element in wc4ResourceParser.Images.ImageList)
        {
            Vector2I atlasCoords = new(element.X, element.Y);
            Vector2I size = new(element.Width, element.Height);
            if (atlasCoords + size < wc4ResourceParser.Texture2D.GetSize())
            {
                try
                {
                    buildTileSetAtlasSource.CreateTile(atlasCoords, size);
                    buildTileSetAtlasSource.GetTileData(atlasCoords, 0).TextureOrigin = offset;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        return _tileSet.AddSource(buildTileSetAtlasSource);
    }

    //生成ColorUV图
    private void CreateColorUv()
    {
        LoadHex(out var points);
        _points = points;
        ColorUvImage = Image.Create(Mathf.CeilToInt(CanvasSize.X / 2f), Mathf.CeilToInt(CanvasSize.Y / 2f), false,
            Image.Format.Rgba8);
        _currentTexture = ImageTexture.CreateFromImage(ColorUvImage);
        var sprite2DMaterial = (ShaderMaterial)_landRender.Material;
        sprite2DMaterial.SetShaderParameter("color_texture", _currentTexture);
    }

    private void SetSize()
    {
        Vector2I size = CanvasSize;
        _subViewport.Size = (new Vector2(size.X, size.Y) * RenderScaleValue).ToVector2I();
        _land.RegionRect = new() { Size = _subViewport.Size };
        _landRender.Scale /= RenderScaleValue;
        _seaRender.RegionRect = new() { Size = size };
    }

    #region 设置UV颜色

    public void SetUvColor(int w, int h, Color color)
    {
        Vector2 position = (TileMap.MapToLocal(new(w, h)) - OffsetSize) / 2f;
        foreach (Vector2I vector2I in _points)
            ColorUvImage.SetPixelv(vector2I + position.ToVector2I(), color);
    }

    public void SetUvColorV(Vector2I vector2I, Color color) => SetUvColor(vector2I.X, vector2I.Y, color);

    #endregion

    //读取Hex
    private static void LoadHex(out List<Vector2I> points)
    {
        List<Vector2I> vector2Is = [];
        Image image = new();
        const string hexName = "hex_small.bin";
        image.LoadPngFromBuffer(FileAccess.GetFileAsBytes($"res://Assets/Textures/{hexName}"));
        for (var x = 0; x < image.GetWidth(); x++)
        for (var y = 0; y < image.GetHeight(); y++)
            if (image.GetPixel(x, y).A != 0)
                vector2Is.Add(new(x, y));
        points = vector2Is;
    }

    //保存
    public void Save()
    {
        var 军团总数 = Countries.Count;
        var 建筑总数 = 0;
        var 军队总数 = 0;
        var 方案总数 = 0;
        var 事件总数 = Affairs.Count;
        var 天气总数 = Weathers.Count;
        var 援军总数 = 0;
        var 空袭总数 = 0;
        var 国家首都 = 0;
        var 陷阱总数 = 0;
        var 战略总数 = Strategies.Count;

        var provinces = new short[LandUnits.Length];
        var belongs = new byte[LandUnits.Length];
        List<City> cities = [];
        List<Army> armies = [];
        List<Scheme> schemes = [];
        List<Reinforcement> reinforcements = [];
        List<AirRaid> airRaids = [];
        List<Capital> capitals = [];
        List<Pitfall> pitfalls = [];

        foreach (LandUnit landUnit in LandUnits)
        {
            provinces[landUnit.Index] = landUnit.Province;
            belongs[landUnit.Index] = landUnit.Belong;

            if (landUnit.City is { } city)
            {
                建筑总数++;
                cities.Add(city);
            }

            if (landUnit.Army is { } army)
            {
                军队总数++;
                armies.Add(army);
            }

            if (landUnit.Scheme is { } scheme)
            {
                方案总数++;
                schemes.Add(scheme);
            }

            foreach (Reinforcement reinforcement in landUnit.Reinforcements)
            {
                reinforcements.Add(reinforcement);
                援军总数++;
            }

            if (landUnit.AirRaid is { } airRaid)
            {
                空袭总数++;
                airRaids.Add(airRaid);
            }

            if (landUnit.Capital is { } capital)
            {
                国家首都++;
                capitals.Add(capital);
            }

            if (landUnit.Pitfall is { } pitfall)
            {
                陷阱总数++;
                pitfalls.Add(pitfall);
            }
        }

        Master.军团总数 = 军团总数;
        Master.建筑总数 = 建筑总数;
        Master.军队总数 = 军队总数;
        Master.方案总数 = 方案总数;
        Master.事件总数 = 事件总数;
        Master.天气总数 = 天气总数;
        Master.援军总数 = 援军总数;
        Master.空袭总数 = 空袭总数;
        Master.国家首都 = 国家首都;
        Master.陷阱总数 = 陷阱总数;
        Master.战略总数 = 战略总数;

        using BinaryWriter binaryWriter = new(File.Create(BtlPath));

        Master.Serializable(binaryWriter);

        foreach (Country country in Countries)
            country.Serializable(binaryWriter);

        if (Btl.IndependentTerrain)
            foreach (Topography topography in Topographies)
                topography.Serializable(binaryWriter);

        foreach (var province in provinces)
            binaryWriter.Write(province);

        foreach (var belong in belongs)
            binaryWriter.Write(belong);

        foreach (City city in cities)
            city.Serializable(binaryWriter);

        foreach (Army army in armies)
            switch (army)
            {
                case Army1 army1:
                    army1.Serializable(binaryWriter);
                    break;
                case Army2 army2:
                    army2.Serializable(binaryWriter);
                    break;
            }

        foreach (Pitfall pitfall in pitfalls)
            pitfall.Serializable(binaryWriter);

        foreach (Scheme scheme in schemes)
            scheme.Serializable(binaryWriter);

        foreach (Weather weather in Weathers)
            weather.Serializable(binaryWriter);

        foreach (Affair affair in Affairs)
            affair.Serializable(binaryWriter);

        foreach (Reinforcement reinforcement in reinforcements)
            switch (reinforcement)
            {
                case Reinforcement1 reinforcement1:
                    reinforcement1.Serializable(binaryWriter);
                    break;
                case Reinforcement3 reinforcement3:
                    reinforcement3.Serializable(binaryWriter);
                    break;
            }

        foreach (AirRaid airRaid in airRaids)
            airRaid.Serializable(binaryWriter);

        foreach (ArmyPlacement armyPlacement in ArmyPlacements)
            armyPlacement.Serializable(binaryWriter);

        foreach (Capital capital in capitals)
            capital.Serializable(binaryWriter);

        foreach (Strategy strategy in Strategies)
            strategy.Serializable(binaryWriter);

        foreach (AirSupport airSupport in AirSupports)
            airSupport.Serializable(binaryWriter);
    }

    #region 更新函数

    public void UpdateCountryColor(int index, Color color)
    {
        Country country = Countries[index];
        country.R8 = (byte)color.R8;
        country.G8 = (byte)color.G8;
        country.B8 = (byte)color.B8;
        foreach (LandUnit landUnit in LandUnits)
            if (landUnit.Belong == index)
                landUnit.UpdateBelongColor();
        UpdateColorUV();
    }

    //更新着色器

    private ImageTexture _currentTexture;

    public void UpdateColorUV() => _currentTexture?.Update(ColorUvImage);

    #endregion

    #region 工具模式

    private LandUnit _oldSelectLand;
    private bool _selectMotion;
    private Vector2 _mousePosition;
    private Vector2 _androidMousePosition;
    private bool _androidSelect = true;
    private Vector2 _delta;
    private int _utilMode;

    public int UtilMode
    {
        get => _utilMode;
        set
        {
            _utilMode = value;
            TileMap.ClearLayer(SingleLayer);
        }
    }

    public int MultiMode { get; set; }
    private bool _multiPressed;

    public override void _UnhandledInput(InputEvent @event)
    {
        switch (UtilMode)
        {
            case 0:
            {
                if (@event is InputEventMouseButton { ButtonIndex: MouseButton.Left } mouseButton)
                {
                    if (mouseButton.Pressed)
                    {
                        _androidMousePosition = GetGlobalMousePosition();

                        Vector2I vector2I = TileMap.LocalToMap(GetGlobalMousePosition());
                        if (LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Master.地图宽), out LandUnit landUnit))
                        {
                            if (landUnit == _oldSelectLand)
                            {
                                if (_oldSelectLand?.ArmySprite is { Select: true })
                                {
                                    Game.Instance.MapUI.TemporarilyHidden = true;
                                    CameraController.AndroidCameraController = false;
                                    _mousePosition = GetGlobalMousePosition();
                                    Indicator.Visible = false;
                                    _selectMotion = true;
                                }

                                return;
                            }
                        }
                    }
                    else
                    {
                        if (_androidSelect)
                        {
                            Vector2I vector2I = TileMap.LocalToMap(GetGlobalMousePosition());
                            if (LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Master.地图宽), out LandUnit landUnit))
                            {
                                if (landUnit == _oldSelectLand)
                                    return;

                                TileMap.ClearLayer(SingleLayer);
                                Game.Instance.AudioStreamPlayer.Play();

                                MapUI.SingeLandUnit = landUnit;
                                Indicator.Visible = true;
                                Indicator.Position = landUnit.Position;

                                foreach (LandUnit unit in LandUnits)
                                    if (unit.Province == landUnit.RegionIndex)
                                        TileMap.SetCell(SingleLayer, unit.Coords, SingleTileSetAtlasId, new());

                                if (_oldSelectLand?.ArmySprite is { } oldArmySprite) oldArmySprite.Select = false;
                                if (landUnit.ArmySprite is { } armySprite) armySprite.Select = true;

                                _oldSelectLand = landUnit;
                            }
                        }
                        else _androidSelect = true;

                        if (!_selectMotion) return;

                        #region 军队交换

                        {
                            Vector2I vector2I = TileMap.LocalToMap(_oldSelectLand.Position + _delta);
                            if (LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Master.地图宽), out LandUnit landUnit))
                            {
                                var belong = _oldSelectLand.Belong;
                                (_oldSelectLand.Army, landUnit.Army) = (landUnit.Army, _oldSelectLand.Army);
                                if (landUnit.Belong == 0xff) landUnit.Belong = belong;
                            }
                            else _oldSelectLand.UpdateArmy();

                            //清除记录
                            Game.Instance.MapUI.TemporarilyHidden = false;
                            CameraController.AndroidCameraController = true;
                            _selectMotion = false;
                            _mousePosition = default;
                            _delta = default;

                            //清除TileMap选中
                            TileMap.ClearLayer(SingleLayer);
                        }

                        #endregion
                    }
                }

                if (@event is InputEventMouseMotion)
                {
                    if (_androidMousePosition.DistanceTo(GetGlobalMousePosition()) > 1f) _androidSelect = false;

                    if (_selectMotion)
                    {
                        //计算位置
                        _delta = GetGlobalMousePosition() - _mousePosition;
                        if (_oldSelectLand.ArmySprite is { } armySprite)
                            armySprite.Position = _oldSelectLand.Position + _delta;
                        if (_oldSelectLand.GeneralSprite is { } generalSprite)
                            generalSprite.Position = _oldSelectLand.Position + _delta;

                        //设置TileMap选中
                        Vector2I vector2I = TileMap.LocalToMap(_oldSelectLand.Position + _delta);
                        TileMap.ClearLayer(SingleLayer);
                        TileMap.SetCell(SingleLayer, vector2I, SingleTileSetAtlasId, new(), 1);
                    }
                }

                break;
            }

            case 1:
            {
                switch (@event)
                {
                    case InputEventMouseButton inputEventMouseButton:
                    {
                        if (inputEventMouseButton.ButtonIndex == MouseButton.Right)
                        {
                            _multiPressed = inputEventMouseButton.Pressed;
                            Vector2I vector2I = TileMap.LocalToMap(GetGlobalMousePosition());
                            if (LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Master.地图宽), out LandUnit landUnit))
                                switch (MultiMode)
                                {
                                    case 0:
                                        if (!MapUI.MultiLandUnit.Contains(landUnit))
                                        {
                                            TileMap.SetCell(MultiLayer, vector2I, MultiTileSetAtlasId, new());
                                            MapUI.MultiLandUnit.Add(landUnit);
                                        }

                                        break;
                                    case 1:
                                        if (MapUI.MultiLandUnit.Contains(landUnit))
                                        {
                                            TileMap.EraseCell(MultiLayer, vector2I);
                                            MapUI.MultiLandUnit.Remove(landUnit);
                                        }

                                        break;
                                }
                        }

                        break;
                    }
                    case InputEventMouseMotion:
                    {
                        if (_multiPressed)
                        {
                            Vector2I vector2I = TileMap.LocalToMap(GetGlobalMousePosition());
                            if (LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Master.地图宽), out LandUnit landUnit))
                                switch (MultiMode)
                                {
                                    case 0:
                                        if (!MapUI.MultiLandUnit.Contains(landUnit))
                                        {
                                            TileMap.SetCell(MultiLayer, vector2I, MultiTileSetAtlasId, new());
                                            MapUI.MultiLandUnit.Add(landUnit);
                                        }

                                        break;
                                    case 1:
                                        if (MapUI.MultiLandUnit.Contains(landUnit))
                                        {
                                            TileMap.EraseCell(MultiLayer, vector2I);
                                            MapUI.MultiLandUnit.Remove(landUnit);
                                        }

                                        break;
                                }
                        }

                        break;
                    }
                }

                break;
            }
        }
    }

    #endregion
}