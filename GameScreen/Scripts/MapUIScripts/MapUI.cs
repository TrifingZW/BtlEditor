using BtlEditor.GameScreen.Scripts.LandScripts;
using BtlEditor.GameScreen.Scripts.MapUIScripts.Single;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts;

public partial class MapUI : InterceptScreen.Scripts.InterceptUiLayer, IInput
{
    /*public List<LandUnit> MultiLandUnit
    {
        get => _multiContainer.MultiLandUnit;
        set => _multiContainer.MultiLandUnit = value;
    }*/

    public GameLandUnit SingeGameLandUnit
    {
        get => _singleTabContainer.SingeGameLandUnit;
        set => _singleTabContainer.SingeGameLandUnit = value;
    }

    //数据面板
    private SingleTabContainer _singleTabContainer;
    // private MultiContainer _multiContainer;

    // private ReinforcementContainer _reinforcementContainer;
    // private AirRaidContainer _airRaidContainer;
    // private CapitalContainer _capitalContainer;
    // private TerrainContainer _terrainContainer;
    private CheckButton _panelVisible;

    public override void _Ready()
    {
        _singleTabContainer = GetNode<SingleTabContainer>("%SingleTabContainer");
        // _multiContainer = GetNode<MultiContainer>("%多选");
    }

    private void TabSelect(long index)
    {
        _singleTabContainer.CurrentTab = (int)index;
    }

    private void Close() => Hide();

    public void Input(InputEvent @event)
    {
        if (Game.Instance.EditMode) return;

        _singleTabContainer.Input(@event);

        /*
        switch (_tabContainer.CurrentTab)
        {
            case 0:
                break;
            case 1:
                _multiContainer.Input(@event);
                break;
        }
    */
    }
}

public interface IInput
{
    void Input(InputEvent @event);
}