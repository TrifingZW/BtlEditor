using System.Collections.Generic;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.GameScreen.Scripts.LandScripts;
using BtlEditor.GameScreen.Scripts.Windows;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;
using BaseSingle = BtlEditor.GameScreen.Scripts.MapUIScripts.Single.BaseSingle;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts;

public partial class MapUI : CanvasLayer
{
    private LandUnit _singeLandUnit;

    public LandUnit SingeLandUnit
    {
        get => _singeLandUnit;
        set
        {
            _singeLandUnit = value;
            foreach (Node node in _dataTabContainer.GetChildren())
                if (node is BaseSingle baseContainer)
                    if (SingeLandUnit != null)
                        baseContainer.LandUnit = SingeLandUnit;
                    else baseContainer.Clear();
        }
    }

    public List<LandUnit> MultiLandUnit { get; } = [];

    private static MapController MapController => Game.Instance.MapController;

    //数据面板
    private PanelContainer _dataContainer;
    private PanelContainer _multiContainer;
    private TabContainer _dataTabContainer;

    // private ReinforcementContainer _reinforcementContainer;
    // private AirRaidContainer _airRaidContainer;
    // private CapitalContainer _capitalContainer;
    // private TerrainContainer _terrainContainer;
    private TabBar _utils;

    //Windows
    public EditWindow EditWindow { get; private set; }

    public override void _Ready()
    {
        _dataContainer = GetNode<PanelContainer>("%DataContainer");
        _dataTabContainer = GetNode<TabContainer>("%DataTabContainer");
        _multiContainer = GetNode<PanelContainer>("%MultiContainer");

        _utils = GetNode<TabBar>("%Utils");
        _utils.TabSelected += UtilsSelect;

        EditWindow = GetNode<EditWindow>("%EditWindow");

        var androidCameraController = GetNode<Button>("%AndroidCameraController");
        androidCameraController.Pressed += () => { Game.Instance.CameraController.AndroidCameraController = !androidCameraController.ButtonPressed; };
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent) return;
        if (keyEvent.Pressed && keyEvent.KeyLabel == Key.F1)
            _utils.CurrentTab = 0;
        if (keyEvent.Pressed && keyEvent.KeyLabel == Key.F2)
            _utils.CurrentTab = 1;
        if (keyEvent.Pressed && keyEvent.KeyLabel == Key.F3)
            _utils.CurrentTab = 2;
    }

    private void UtilsSelect(long index)
    {
        _dataContainer.Visible = false;
        _multiContainer.Visible = false;
        SingeLandUnit = null;
        switch (index)
        {
            case 0:
                break;
            case 1:
                _dataContainer.Visible = true;
                break;
            case 2:
                _multiContainer.Visible = true;
                break;
        }

        MapController.UtilMode = (int)index;
    }

    private void MultiModeClear()
    {
        MultiLandUnit.Clear();
        MapController.TileMap.ClearLayer(MapController.MultiLayer);
    }

    private void MultiModeCreateArmy()
    {
        foreach (LandUnit landUnit in MultiLandUnit)
        {
            if (landUnit.Army != null) continue;
            if (Btl.Version1) landUnit.Army = new Army1 { 坐标 = landUnit.RegionIndex };
            if (Btl.Version2 || Btl.Version3) landUnit.Army = new Army2 { 坐标 = landUnit.RegionIndex };
        }
    }

    private void MultiModeCreateCity()
    {
        foreach (LandUnit landUnit in MultiLandUnit)
        {
            if (landUnit.City != null) continue;
            landUnit.City = new()
            {
                坐标 = landUnit.RegionIndex
            };
            landUnit.Province = landUnit.RegionIndex;
            landUnit.UpdateProvinceColor();
        }

        MapController.UpdateShader();
    }

    private void MultiModeSetProvince()
    {
        EditWindow.CreateEdit("设置省规划", province =>
        {
            foreach (LandUnit landUnit in MultiLandUnit)
            {
                landUnit.Province = (short)province;
                landUnit.UpdateProvinceColor();
            }

            MapController.UpdateShader();
        });
    }

    private void MultiModeSetBelong()
    {
        EditWindow.CreateEdit("设置归属", belong =>
        {
            foreach (LandUnit landUnit in MultiLandUnit)
            {
                landUnit.Belong = (byte)belong;
                landUnit.UpdateBelongColor();
            }

            MapController.UpdateShader();
        });
    }

    private void MultiModeDeleteArmy()
    {
        foreach (LandUnit landUnit in MultiLandUnit)
            landUnit.Army = null;
    }

    private void MultiModeDeleteCity()
    {
        foreach (LandUnit landUnit in MultiLandUnit)
            landUnit.City = null;
    }
}