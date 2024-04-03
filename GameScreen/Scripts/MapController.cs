using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using BtlEditor.CoreScripts;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.Parser.ParserHelper;
using static BtlEditor.CoreScripts.StaticRes;
using FileAccess = Godot.FileAccess;

namespace BtlEditor.GameScreen.Scripts;

public partial class MapController : CanvasGroup
{
    //节点
    private SubViewport _subViewport;
    private Sprite2D _land;
    private Node2D _topography;

    private Sprite2D _seaRender;
    private Sprite2D _landRender;
    private TileSet _tileSet;
    private GameUI _gameUI;
    public Node2D Additional { get; private set; }
    public TileMap TileMap { get; private set; }

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
    public List<Country> Countries { get; private set; }
    public Topography[] Topographies { get; private set; }
    public List<Weather> Weathers { get; private set; }
    public List<Affair> Affairs { get; private set; }
    public List<Strategy> Strategies { get; private set; }
    public List<AirSupport> AirSupports { get; private set; }
    public List<ArmyPlacement> ArmyPlacements { get; private set; }

    private static Master Master => Btl.Master;

    public const int TileWidth = 148;
    public const int TileHeight = 129;
    public const int SideLength = 76;
    public static Vector2 OffsetSize => new(TileWidth / 2f, TileHeight / 2f);

    public Vector2I CanvasSize => new Vector2(TileMap.MapToLocal(new(Master.地图宽 - 1, 1)).X,
        TileMap.MapToLocal(new(1, Master.地图高 - 1)).Y).ToVector2I() + OffsetSize.ToVector2I();

    public override void _Ready()
    {
        _gameUI = GetNode<GameUI>("%CanvasLayer");

        //SubViewport
        _subViewport = GetNode<SubViewport>("%SubViewport");
        _land = GetNode<Sprite2D>("%Land");
        _topography = GetNode<Node2D>("%Topography");

        //CanvasGroup
        _seaRender = GetNode<Sprite2D>("%SeaRender");
        _landRender = GetNode<Sprite2D>("%LandRender");
        TileMap = GetNode<TileMap>("%TileMap");
        Additional = GetNode<Node2D>("%Additional");
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
        ApplyShader();
    }

    //更新
    private IEnumerable<Vector2I> _points;

    //设置数据
    private void SetData()
    {
        Countries = Btl.Countries.ToList();
        Weathers = Btl.Weathers.ToList();
        Affairs = Btl.Affairs.ToList();
        Strategies = Btl.Strategies.ToList();
        AirSupports = Btl.AirSupports.ToList();
        ArmyPlacements = Btl.ArmyPlacements.ToList();

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
                LandUnit landUnit = new(this)
                {
                    Index = (short)landIndex,
                    RegionIndex = (short)ParserHelper.GetIndex(x + Master.地图截取x, y + Master.地图截取y, Bin.Width),
                    X = x,
                    Y = y,
                    Position = TileMap.MapToLocal(new(x, y)),
                    Topography = Topographies[landIndex],
                    Province = Btl.Provinces[landIndex],
                    Belong = Btl.Belongs[landIndex]
                };
                LandUnits[landIndex] = landUnit;
            }
        }

        var tasks = Master.地图高; //任务数
        var handleCount = Master.地图宽; //每个任务的遍历数
        using (CountdownEvent countdown = new(tasks))
        {
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
                    {
                        landUnit.UpdateProvince(landUnit.Province);
                        if (Countries.TryGetValue(landUnit.Belong, out Country country))
                            foreach (Wc4ResourceElement wc4ResourceElement in Tacticalmap.Images.ImageList)
                                if (wc4ResourceElement.Name == $"f_{country.国家:D2}.png")
                                {
                                    Vector2I atlasCoords = new(wc4ResourceElement.X, wc4ResourceElement.Y);
                                    // TileMap.SetCell(FlagLayer, landUnit.Coords, TacticalmapTileSetAtlasId, atlasCoords);
                                    break;
                                }
                    }

                    // ReSharper disable once AccessToDisposedClosure
                    countdown.Signal();
                });
            }

            countdown.Wait();
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
            foreach (Army3 army in Btl.Armies3)
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
        if (Btl.Version1)
            if (Btl.Version2 || Btl.Version3)
                foreach (Reinforcement3 btlParserReinforcement in Btl.Reinforcements1)
                    if (LandUnits.TryGetValue(GetBtlIndex(btlParserReinforcement.坐标), out LandUnit landUnit))
                        landUnit.Reinforcements.Add(btlParserReinforcement);
        if (Btl.Version2 || Btl.Version3)
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
    }
    
    //地形绘制
    private void DrawTopography()
    {
        Terrains terrains = TerrainConfig.Terrains;

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
                if (topography.装饰类型B == terrain.TerrainG)
                    foreach (Tile tile in terrain.Tiles)
                        if (topography.装饰BID == tile.Idx)
                            SetTexture(landUnit, new((sbyte)topography.装饰BX, (sbyte)topography.装饰BY), tile);
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
                    Position = landUnit.Position - new Vector2(element.RefX, element.RefY) + offset,
                    Size = new(element.Width, element.Height)
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
                    Position = landUnit.Position - new Vector2(element2.RefX, element2.RefY) + offset,
                    Size = new(element2.Width, element2.Height)
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
        ColorUvImage = Image.Create(Mathf.CeilToInt(CanvasSize.X * Globals.ShaderScale), Mathf.CeilToInt(CanvasSize.Y * Globals.ShaderScale), false,
            Image.Format.Rgba8);
    }

    private void SetSize()
    {
        Vector2I size = CanvasSize;
        _subViewport.Size = size;
        _seaRender.RegionRect = new() { Size = size };
        _land.RegionRect = new() { Size = size };
    }

    #region 设置UV颜色

    public void SetUvColor(int w, int h, Color color)
    {
        Vector2 position = (TileMap.MapToLocal(new(w, h)) - OffsetSize) * Globals.ShaderScale;
        foreach (Vector2I vector2I in _points)
            ColorUvImage.SetPixelv(vector2I + position.ToVector2I(), color);
    }

    public void SetUvColorV(Vector2I vector2I, Color color) => SetUvColor(vector2I.X, vector2I.Y, color);

    #endregion

    //应用着色器
    public void ApplyShader()
    {
        ShaderMaterial sprite2DMaterial1 = (ShaderMaterial)_landRender.Material;
        sprite2DMaterial1.SetShaderParameter("color_texture", ImageTexture.CreateFromImage(ColorUvImage));
    }

    //读取Hex
    private static void LoadHex(out List<Vector2I> points)
    {
        List<Vector2I> vector2Is = [];
        Image image = new();
        var hexName = Globals.Shader switch
        {
            0 => "hex_min.bin",
            1 => "hex_small.bin",
            2 => "hex_large.bin",
            _ => "hex_small.bin"
        };
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

        using BinaryWriter binaryWriter = new(File.Create(Globals.BtlPath));

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
                case Army3 army3:
                    army3.Serializable(binaryWriter);
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

    #region 工具模式

    private LandUnit _selectLand;
    private bool _selectMotion;
    private Vector2 _mousePosition;
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
                switch (@event)
                {
                    case InputEventMouseButton mouseButton:
                    {
                        if (mouseButton.ButtonIndex == MouseButton.Left)
                        {
                            //选中地块
                            if (mouseButton.Pressed)
                            {
                                Vector2I vector2I = TileMap.LocalToMap(GetGlobalMousePosition());
                                if (LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Master.地图宽), out LandUnit landUnit))
                                    if (landUnit.Army != null)
                                    {
                                        _selectLand = landUnit;
                                        _mousePosition = GetGlobalMousePosition();
                                        _selectMotion = true;
                                    }
                            }

                            else //释放
                            {
                                if (_selectLand == null) return;
                                
                                //军队交换
                                Vector2I vector2I = TileMap.LocalToMap(_selectLand.Position + _delta);
                                if (LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Master.地图宽), out LandUnit landUnit))
                                    (_selectLand.Army, landUnit.Army) = (landUnit.Army, _selectLand.Army);
                                else _selectLand.UpdateArmy();

                                //清除记录
                                _selectMotion = false;
                                _mousePosition = default;
                                _delta = default;

                                //清除TileMap选中
                                TileMap.ClearLayer(SingleLayer);
                            }
                        }

                        break;
                    }
                    case InputEventMouseMotion:
                        if (_selectMotion)
                        {
                            //计算位置
                            _delta = GetGlobalMousePosition() - _mousePosition;
                            _selectLand.ArmySprite.Position = _selectLand.Position + _delta;

                            //设置TileMap选中
                            Vector2I vector2I = TileMap.LocalToMap(_selectLand.Position + _delta);
                            TileMap.ClearLayer(SingleLayer);
                            TileMap.SetCell(SingleLayer, vector2I, SingleTileSetAtlasId, new());
                        }

                        break;
                }

                break;
            }

            case 1:
            {
                if (@event is InputEventMouseButton inputEventMouseButton)
                    if (inputEventMouseButton.ButtonIndex == MouseButton.Left && inputEventMouseButton.Pressed)
                    {
                        Vector2I vector2I = TileMap.LocalToMap(GetGlobalMousePosition());
                        if (LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Master.地图宽), out LandUnit landUnit))
                            _gameUI.SingeLandUnit = landUnit;

                        //设置TileMap选中
                        TileMap.ClearLayer(SingleLayer);
                        TileMap.SetCell(SingleLayer, vector2I, SingleTileSetAtlasId, new());
                    }

                break;
            }

            case 2:
            {
                switch (@event)
                {
                    case InputEventMouseButton inputEventMouseButton:
                    {
                        if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                            _multiPressed = inputEventMouseButton.Pressed;
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
                                        if (!_gameUI.MultiLandUnit.Contains(landUnit))
                                        {
                                            TileMap.SetCell(MultiLayer, vector2I, MultiTileSetAtlasId, new());
                                            _gameUI.MultiLandUnit.Add(landUnit);
                                        }

                                        break;
                                    case 1:
                                        if (_gameUI.MultiLandUnit.Contains(landUnit))
                                        {
                                            TileMap.EraseCell(MultiLayer, vector2I);
                                            _gameUI.MultiLandUnit.Remove(landUnit);
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