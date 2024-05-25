using System.Collections.Generic;
using System.IO;
using System.Linq;
using BtlEditor.CoreScripts;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using BtlEditor.UserInterface;
using Godot;
using static BtlEditor.GameScreen.Scripts.MapHelper;

namespace BtlEditor.GameScreen.Scripts;

public partial class InterceptUI : CanvasLayer
{
    private TabPanel _tabPanel;
    private MapController MapController => Game.Instance.MapController;
    private TileMapLayer TileMapLayer => MapController.TileMapLayer;

    private Vector2I _startPoint = Vector2I.Zero;

    private Vector2I StartPoint
    {
        get => _startPoint;
        set
        {
            _startPoint = value;
            UpdateTileMap();
        }
    }

    private Vector2I _endPoint = Vector2I.Zero;

    private Vector2I EndPoint
    {
        get => _endPoint;
        set
        {
            _endPoint = value;
            UpdateTileMap();
        }
    }

    private bool _start;

    private bool _selectMode;

    private bool SelectMode
    {
        get => _selectMode;
        set
        {
            _selectMode = value;
            if (SelectMode)
                Hide();
            else
                Show();
        }
    }

    private LineEdit _name;
    private SpinBox _startX;
    private SpinBox _startY;
    private SpinBox _endX;
    private SpinBox _endY;

    public override void _Ready()
    {
        _tabPanel = GetNode<TabPanel>("%TabPanel");
        _tabPanel.Close.Pressed += Close;
        _name = GetNode<LineEdit>("%Name");
        _startX = GetNode<SpinBox>("%StartX");
        _startY = GetNode<SpinBox>("%StartY");
        _endX = GetNode<SpinBox>("%EndX");
        _endY = GetNode<SpinBox>("%EndY");
    }

    private void Close()
    {
        Game.Instance.InterceptMode = false;
        TileMapLayer.Clear();
        StartPoint = Vector2I.Zero;
        EndPoint = Vector2I.Zero;
        _startX.SetValueNoSignal(0);
        _startY.SetValueNoSignal(0);
        _endX.SetValueNoSignal(0);
        _endY.SetValueNoSignal(0);
    }

    private void StartVec()
    {
        _start = true;
        SelectMode = true;
    }

    private void EndVec()
    {
        _start = false;
        SelectMode = true;
    }

    private void StartX(double value) => StartPoint = new((int)value, StartPoint.Y);
    private void StartY(double value) => StartPoint = new(StartPoint.X, (int)value);
    private void EndX(double value) => EndPoint = new((int)value, EndPoint.Y);
    private void EndY(double value) => EndPoint = new(EndPoint.X, (int)value);


    private void UpdateTileMap()
    {
        TileMapLayer.Clear();
        for (var x = StartPoint.X; x <= EndPoint.X; x++)
        for (var y = StartPoint.Y; y <= EndPoint.Y; y++)
            TileMapLayer.SetCell(new(x, y), MapController.SingleTileSetAtlasId, new(), 2);

        TileMapLayer.SetCell(StartPoint, MapController.SingleTileSetAtlasId, new());
        TileMapLayer.SetCell(EndPoint, MapController.SingleTileSetAtlasId, new(), 1);
    }


    private Vector2 MousePosition => MapController.GetGlobalMousePosition();

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!SelectMode) return;
        switch (@event)
        {
            case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }:
                Vector2I vector2I = TileMapLayer.LocalToMap(MousePosition);
                if (!MapController.LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit _)) return;
                if (_start)
                    StartPoint = vector2I;
                else
                    EndPoint = vector2I;
                SelectMode = false;
                _startX.SetValueNoSignal(StartPoint.X);
                _startY.SetValueNoSignal(StartPoint.Y);
                _endX.SetValueNoSignal(EndPoint.X);
                _endY.SetValueNoSignal(EndPoint.Y);
                break;
        }
    }

    private void Save()
    {
        //重置主数据
        var master = (Master)Btl.Master.Clone();
        master.地图序号 = 0;
        master.地图截取x = 0;
        master.地图截取y = 0;
        master.地图宽 = EndPoint.X - StartPoint.X + 1;
        master.地图高 = EndPoint.Y - StartPoint.Y + 1;
        master.地块总数 = master.地图宽 * master.地图高;
        master.军团总数 = 0;
        master.建筑总数 = 0;
        master.军队总数 = 0;
        master.方案总数 = 0;
        master.事件总数 = 0;
        master.天气总数 = 0;
        master.援军总数 = 0;
        master.空袭总数 = 0;
        master.国家首都 = 0;
        master.陷阱总数 = 0;
        master.战略总数 = 0;
        master.空中支援 = 0;

        //读取地块数据
        List<Country> countries = [];
        var topographies = new Topography[master.地块总数];
        var provinces = new short[master.地块总数];
        var belongs = new byte[master.地块总数];
        List<City> cities = [];
        List<Army> armies = [];
        List<Scheme> schemes = [];
        List<Reinforcement> reinforcements = [];
        List<AirRaid> airRaids = [];
        List<Pitfall> pitfalls = [];
        for (var x = 0; x <= EndPoint.X - StartPoint.X; x++)
        for (var y = 0; y <= EndPoint.Y - StartPoint.Y; y++)
        {
            var index = GlobalHelper.GetIndex(x, y, master.地图宽);
            GameLandUnit landUnit = MapController.LandUnits[MapHelper.GetIndex(x + StartPoint.X, y + StartPoint.Y)];

            if (landUnit.BelongCountry is { } country)
            {
                if (countries.All(c => c.序号 != country.序号))
                {
                    var nCountry = (Country)country.Clone();
                    countries.Add(nCountry);
                    master.军团总数++;
                }
            }

            topographies[index] = landUnit.Topography;
            provinces[index] = 0;
            if (MapController.LandUnits.TryGetValue(GetBtlIndex(landUnit.Province), out GameLandUnit pLandUnit))
                if (pLandUnit.Coords >= StartPoint && pLandUnit.Coords <= EndPoint)
                    provinces[index] = (short)GlobalHelper.GetIndex(pLandUnit.Coords.X - StartPoint.X, pLandUnit.Coords.Y - StartPoint.Y, master.地图宽);

            belongs[index] = landUnit.Belong;

            if (landUnit.City is { } city)
            {
                var nCity = (City)city.Clone();
                nCity.坐标 = (short)index;
                cities.Add(nCity);
                master.建筑总数++;
            }

            if (landUnit.Army is { } army)
            {
                var nArmy = (Army)army.Clone();
                nArmy.坐标 = (short)index;
                armies.Add(nArmy);
                master.军队总数++;
            }

            if (landUnit.Scheme is { } scheme)
            {
                var nScheme = (Scheme)scheme.Clone();
                nScheme.目标地块 = (short)index;
                schemes.Add(nScheme);
                master.方案总数++;
            }

            foreach (Reinforcement reinforcement in landUnit.Reinforcements)
            {
                var nReinforcement = (Reinforcement)reinforcement.Clone();
                nReinforcement.坐标 = (short)index;
                reinforcements.Add(nReinforcement);
                master.援军总数++;
            }

            if (landUnit.AirRaids is { } lAirRaids)
            {
                foreach (AirRaid airRaid in lAirRaids)
                {
                    var nAirRaid = (AirRaid)airRaid.Clone();
                    nAirRaid.坐标 = (short)index;
                    airRaids.Add(nAirRaid);
                    master.空袭总数++;
                }
            }

            if (landUnit.Pitfall is { } pitfall)
            {
                var nPitfall = (Pitfall)pitfall.Clone();
                nPitfall.坐标 = (short)index;
                pitfalls.Add(nPitfall);
                master.陷阱总数++;
            }
        }

        //读取非地块数据
        var weathers = (Weather[])Btl.Weathers.Clone();
        master.天气总数 = weathers.Length;

        var affairs = Btl.Affairs
            .Where(affair => countries.Any(country => country.序号 == affair.触发军团) && countries.Any(country => country.序号 == affair.影响军团)).ToList();
        master.事件总数 = affairs.Count;

        var strategies = Btl.Strategies.Where(strategy => countries.Any(country => country.序号 == strategy.军团序号)).ToList();
        master.战略总数 = strategies.Count;

        var airSupports = Btl.AirSupports.Where(airSupport => countries.Any(country => country.序号 == airSupport.军团)).ToList();
        master.空中支援 = strategies.Count;

        //同步归属
        for (var i = 0; i < belongs.Length; i++)
            if (countries.FirstOrDefault(c => c.序号 == belongs[i]) is { } country)
                belongs[i] = (byte)countries.IndexOf(country);
            else belongs[i] = 0xff;

        //同步序号
        for (var i = 0; i < countries.Count; i++)
        {
            foreach (Reinforcement reinforcement in reinforcements)
                switch (reinforcement)
                {
                    case Reinforcement1 reinforcement1:
                        if (reinforcement1.所属国家 == countries[i].序号)
                            reinforcement1.所属国家 = (short)i;
                        break;
                    case Reinforcement3 reinforcement3:
                        if (reinforcement3.所属国家 == countries[i].序号)
                            reinforcement3.所属国家 = (short)i;
                        break;
                }

            foreach (Pitfall pitfall in pitfalls)
                if (pitfall.所属军团 == countries[i].序号)
                    pitfall.所属军团 = (short)i;

            foreach (Affair affair in affairs)
            {
                if (affair.触发军团 == countries[i].序号)
                    affair.触发军团 = i;
                if (affair.影响军团 == countries[i].序号)
                    affair.影响军团 = i;
            }

            foreach (Strategy strategy in strategies)
                if (strategy.军团序号 == countries[i].序号)
                    strategy.军团序号 = i;

            foreach (AirSupport airSupport in airSupports)
                if (airSupport.军团 == countries[i].序号)
                    airSupport.军团 = i;

            countries[i].序号 = i;
        }

        using BinaryWriter binaryWriter = new(File.Create($"{StaticRes.StagePath}/{_name.Text}.btl"));

        master.Serializable(binaryWriter);

        foreach (Country country in countries)
            country.Serializable(binaryWriter);

        foreach (Topography topography in topographies)
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

        foreach (Weather weather in Btl.Weathers)
            weather.Serializable(binaryWriter);

        foreach (Affair affair in affairs)
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

        foreach (ArmyPlacement armyPlacement in Btl.ArmyPlacements)
            armyPlacement.Serializable(binaryWriter);

        foreach (Strategy strategy in strategies)
            strategy.Serializable(binaryWriter);

        foreach (AirSupport airSupport in airSupports)
            airSupport.Serializable(binaryWriter);

        Game.Instance.AcceptDialog.DialogText = "保存成功";
        Game.Instance.AcceptDialog.Show();

        Close();
    }
}