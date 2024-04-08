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

    private void MultiModeTabSelect(long index) => MapController.MultiMode = (int)index;

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
            if (Btl.Version2 || Btl.Version3) landUnit.Army = new Army3 { 坐标 = landUnit.RegionIndex };
        }
    }

    private void MultiModeCreateCity()
    {
        foreach (LandUnit landUnit in MultiLandUnit)
        {
            if (landUnit.City != null) continue;
            landUnit.City = new City
            {
                坐标 = landUnit.RegionIndex
            };
            landUnit.Province = landUnit.RegionIndex;
            landUnit.UpdateProvince();
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
                landUnit.UpdateProvince();
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
                landUnit.UpdateBelong();
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