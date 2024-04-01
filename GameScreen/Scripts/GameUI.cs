using System.Collections.Generic;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.GameScreen.Scripts.Single;
using BtlEditor.GameScreen.Scripts.Windows;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts;

public partial class GameUI : CanvasLayer
{
    public LandUnit SingeLandUnit
    {
        set
        {
            foreach (Node node in _dataTabContainer.GetChildren())
                if (node is BaseSingle baseContainer)
                    if (value != null)
                        baseContainer.LandUnit = value;
                    else baseContainer.Clear();
        }
    }

    public List<LandUnit> MultiLandUnit { get; } = [];

    private MapController _mapController;

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
    public static GameUI Instance { get; private set; }

    public override void _Ready()
    {
        Instance = this;

        _mapController = GetNode<MapController>("%CanvasGroup");
        _dataContainer = GetNode<PanelContainer>("%DataContainer");
        _dataTabContainer = GetNode<TabContainer>("%DataTabContainer");
        _multiContainer = GetNode<PanelContainer>("%MultiContainer");

        _utils = GetNode<TabBar>("%Utils");
        _utils.TabSelected += UtilsSelect;

        EditWindow = GetNode<EditWindow>("%EditWindow");
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

        _mapController.UtilMode = (int)index;
    }

    private void MultiModeTabSelect(long index) => _mapController.MultiMode = (int)index;

    private void MultiModeClear()
    {
        MultiLandUnit.Clear();
        _mapController.TileMap.ClearLayer(MapController.MultiLayer);
    }

    private void MultiModeCreateArmy()
    {
        foreach (LandUnit landUnit in MultiLandUnit)
        {
            if (landUnit.Army != null) continue;
            if (Btl.Version1) landUnit.Army = new Army1 { 坐标 = landUnit.RegionIndex };
            if (Btl.Version2 || Btl.Version3) landUnit.Army = new Army3 { 坐标 = landUnit.RegionIndex };
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
            landUnit.UpdateProvince(landUnit.RegionIndex);
        }

        _mapController.ApplyShader();
    }

    private void MultiModeSetProvince()
    {
        EditWindow.CreateEdit("设置省规划", province =>
        {
            foreach (LandUnit landUnit in MultiLandUnit)
                landUnit.UpdateProvince((short)province);
            _mapController.ApplyShader();
        });
    }

    private void MultiModeSetBelong()
    {
        EditWindow.CreateEdit("设置归属", belong =>
        {
            foreach (LandUnit landUnit in MultiLandUnit)
                landUnit.UpdateBelong((byte)belong);
            _mapController.ApplyShader();
        });
    }

    private void MultiModeDeleteArmy()
    {
        foreach (LandUnit landUnit in MultiLandUnit)
            landUnit.Army = null;
        _mapController.Additional.QueueRedraw();
    }

    private void MultiModeDeleteCity()
    {
        foreach (LandUnit landUnit in MultiLandUnit)
            landUnit.City = null;
    }
}