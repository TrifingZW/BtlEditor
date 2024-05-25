using System.Linq;
using BtlEditor.CoreScripts;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.Globals;
using static BtlEditor.CoreScripts.StaticRes;
using static BtlEditor.InterceptScreen.Scripts.InterceptHelper;

namespace BtlEditor.InterceptScreen.Scripts;

public partial class Intercept : Node2D
{
    public TileMap TileMap { get; private set; }
    private Sprite2D _landRender;
    private SubViewport _subViewport;
    private Sprite2D _land;
    private Node2D _topography;
    public InterceptUiLayer InterceptUiLayer { get; private set; }
    public BtlParser Btl { get; private set; }
    public BinParser Bin { get; private set; }
    public LandUnit[] LandUnits { get; private set; }
    private Master Master => Btl.Master;

    public const int TileWidth = 148;
    public const int TileHeight = 129;
    public const int SideLength = 76;
    public static Vector2 OffsetSize => new(TileWidth / 2f, TileHeight / 2f);

    public Vector2I CanvasSize => new Vector2(TileMap.MapToLocal(new(Master.地图宽 - 1, 1)).X,
        TileMap.MapToLocal(new(1, Master.地图高 - 1)).Y).ToVector2I() + OffsetSize.ToVector2I();

    public static Intercept Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;

        TileMap = GetNode<TileMap>("%TileMapLayer");

        //SubViewport
        _subViewport = GetNode<SubViewport>("%SubViewport");
        _land = GetNode<Sprite2D>("%Land");
        _topography = GetNode<Node2D>("%Topography");

        _landRender = GetNode<Sprite2D>("%LandRender");

        InterceptUiLayer = GetNode<InterceptUiLayer>("InterceptUiLayer");
        Btl = new BtlParser().Parser(@"C:\Users\TrifingZW\Godot\BtlEditor\files\WorkPath\assets\stage\conquest3.btl");
        SetData();
        SetSize();
        _topography.Draw += () => GlobalHelper.DrawTerrain(_topography, LandUnits);
    }

    private void SetData()
    {
        InterceptUiLayer.MasterContainer.Initialize(Master);

        Topography[] topographies;
        //读取地形
        if (Btl.IndependentTerrain)
            topographies = Btl.Topographies;
        else
        {
            Map map = MapConfig.Maps.MapArray.First(m => m.Id == Btl.Master.地图序号);
            Bin = new BinParser().Parser(map.File);

            topographies = new Topography[Master.地块总数];
            var index = 0;
            for (var y = Master.地图截取y; y < Master.地图截取y + Master.地图高; y++)
            for (var x = Master.地图截取x; x < Master.地图截取x + Master.地图宽; x++)
            {
                topographies[index] = Bin.Topographies[InterceptHelper.GetIndex(x, y, Bin.Width)];
                index++;
            }
        }

        LandUnits = new LandUnit[Master.地块总数];
        for (var y = 0; y < Master.地图高; y++)
        {
            for (var x = 0; x < Master.地图宽; x++)
            {
                var landIndex = InterceptHelper.GetIndex(x, y, Master.地图宽);
                LandUnit landUnit = new()
                {
                    Index = (short)landIndex,
                    RegionIndex =
                        (short)(Btl.IndependentTerrain ? landIndex : InterceptHelper.GetIndex(x + Master.地图截取x, y + Master.地图截取y, Bin.Width)),
                    X = x,
                    Y = y,
                    Position = TileMap.MapToLocal(new(x, y)),
                    Topography = topographies[landIndex],
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
                    if (army.兵种 == 39) landUnit.CityHp = army;
                    else landUnit.Army = army;
        if (Btl.Version2 || Btl.Version3)
            foreach (Army2 army in Btl.Armies2)
                if (LandUnits.TryGetValue(GetBtlIndex(army.坐标), out LandUnit landUnit))
                    if (army.兵种 == 39) landUnit.CityHp = army;
                    else landUnit.Army = army;

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

        /*//读取空袭
        foreach (AirRaid btlParserAirRaid in Btl.AirRaids)
            if (LandUnits.TryGetValue(GetBtlIndex(btlParserAirRaid.坐标), out LandUnit landUnit))
                landUnit.AirRaids = btlParserAirRaid;*/

        //读取首都
        foreach (Capital capital in Btl.Capitals)
            if (LandUnits.TryGetValue(GetBtlIndex(capital.坐标), out LandUnit landUnit))
                landUnit.Capital = capital;

        //读取归属
        for (var index = 0; index < Btl.Belongs.Length; index++)
            LandUnits[index].Belong = Btl.Belongs[index];
    }

    private void SetSize()
    {
        Vector2I size = CanvasSize;
        _subViewport.Size = (new Vector2(size.X, size.Y) * RenderScaleValue).ToVector2I();
        _land.RegionRect = new() { Size = _subViewport.Size };
        _landRender.Scale /= RenderScaleValue;
    }
}