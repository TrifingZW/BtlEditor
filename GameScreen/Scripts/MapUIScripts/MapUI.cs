using System.Collections.Generic;
using BtlEditor.GameScreen.Scripts.LandScripts;
using BtlEditor.GameScreen.Scripts.MapUIScripts.Multi;
using BtlEditor.GameScreen.Scripts.MapUIScripts.Single;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts;

public partial class MapUI : CanvasLayer, IInput
{
    public List<LandUnit> MultiLandUnit
    {
        get => _multiContainer.MultiLandUnit;
        set => _multiContainer.MultiLandUnit = value;
    }

    public LandUnit SingeLandUnit
    {
        get => _singleContainer.SingeLandUnit;
        set => _singleContainer.SingeLandUnit = value;
    }

    //数据面板
    private SingleContainer _singleContainer;
    private MultiContainer _multiContainer;

    // private ReinforcementContainer _reinforcementContainer;
    // private AirRaidContainer _airRaidContainer;
    // private CapitalContainer _capitalContainer;
    // private TerrainContainer _terrainContainer;
    private TabBar _utils;
    private CheckButton _panelVisible;

    public override void _Ready()
    {
        _singleContainer = GetNode<SingleContainer>("%SingleContainer");
        _multiContainer = GetNode<MultiContainer>("%MultiContainer");

        _utils = GetNode<TabBar>("%Utils");
        _utils.TabSelected += UtilsSelect;

        _panelVisible = GetNode<CheckButton>("%PanelVisible");
        _panelVisible.Pressed += () => { UtilsSelect(_utils.CurrentTab); };
    }

    private void UtilsSelect(long index)
    {
        _singleContainer.Visible = false;
        _multiContainer.Visible = false;
        if (!_panelVisible.ButtonPressed) return;
        switch (index)
        {
            case 0:
                _singleContainer.Visible = true;
                break;
            case 1:
                _multiContainer.Visible = true;
                break;
        }
    }

    public void Input(InputEvent @event)
    {
        if (Game.Instance.ProvinceMode) return;
        switch (_utils.CurrentTab)
        {
            case 0:
                _singleContainer.Input(@event);
                break;
            case 1:
                _multiContainer.Input(@event);
                break;
        }
    }
}

public interface IInput
{
    void Input(InputEvent @event);
}