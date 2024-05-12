using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using Godot;
using static BtlEditor.GameScreen.Scripts.MapHelper;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class SingleTabContainer : TabContainer, IInput
{
    private static MapController MapController => Game.Instance.MapController;
    private static GameLandUnit[] LandUnits => MapController.LandUnits;
    private static TileMap TileMap => MapController.TileMap;
    private static Indicator Indicator => MapController.Indicator;
    private GameLandUnit _singeGameLandUnit;

    public GameLandUnit SingeGameLandUnit
    {
        get => _singeGameLandUnit;
        set
        {
            _singeGameLandUnit = value;
            MapController.Indicator.Position = SingeGameLandUnit.Position;
            MapController.Indicator.Visible = true;
            ((BaseSingle)GetCurrentTabControl()).GameLandUnit = SingeGameLandUnit;
        }
    }

    public override void _Ready()
    {
        TabChanged += select =>
        {
            if (SingeGameLandUnit != null)
                GetChild<BaseSingle>((int)select).GameLandUnit = SingeGameLandUnit;
        };
        
    }


    private bool _selectMotion;
    private Vector2 _delta;
    private GameLandUnit _oldSelectGameLand;
    private Vector2 _oldMousePosition;
    private static Vector2 MousePosition => MapController.GetGlobalMousePosition();

    public void Input(InputEvent @event)
    {
        switch (@event)
        {
            case InputEventMouseButton { ButtonIndex: MouseButton.Left, Pressed: true }:
                Pressed();
                break;
            case InputEventMouseButton { ButtonIndex: MouseButton.Left }:
                Released();
                break;
            case InputEventMouseMotion:
                Motion();
                break;
        }
    }

    private void Pressed()
    {
        Vector2I vector2I = TileMap.LocalToMap(MousePosition);
        if (!LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit landUnit)) return;

        if (landUnit == _oldSelectGameLand)
            if (_oldSelectGameLand?.ArmySprite is { Select: true })
            {
                _oldMousePosition = MousePosition;
                Indicator.Visible = false;
                _selectMotion = true;
                Game.Instance.MapUI.Visible = false;
                return;
            }

        Game.Instance.AudioStreamPlayer.Play();
        SingeGameLandUnit = landUnit;

        if (_oldSelectGameLand?.ArmySprite is { } oldArmySprite) oldArmySprite.Select = false;
        if (landUnit.ArmySprite is { } armySprite) armySprite.Select = true;

        _oldSelectGameLand = landUnit;
    }

    private void Released()
    {
        if (!_selectMotion) return;

        //军队交换
        Vector2I vector2I = TileMap.LocalToMap(_oldSelectGameLand.Position + _delta);
        if (LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit landUnit))
        {
            _oldSelectGameLand.ArmySprite.Select = false;
            var belong = _oldSelectGameLand.Belong;
            (_oldSelectGameLand.Army, landUnit.Army) = (landUnit.Army, _oldSelectGameLand.Army);
            if (landUnit.Belong == 0xff) landUnit.Belong = belong;
            SingeGameLandUnit = landUnit;
        }
        else _oldSelectGameLand.UpdateArmy();

        //清除记录
        _oldMousePosition = default;
        _delta = default;
        _selectMotion = false;
        Game.Instance.MapUI.Visible = true;
        TileMap.ClearLayer(MapController.SingleLayer);
    }

    private void Motion()
    {
        if (!_selectMotion) return;
        //计算位置
        _delta = MousePosition - _oldMousePosition;
        Vector2I vector2I = TileMap.LocalToMap(_oldSelectGameLand.Position + _delta);
        TileMap.ClearLayer(MapController.SingleLayer);
        TileMap.SetCell(MapController.SingleLayer, vector2I, MapController.SingleTileSetAtlasId, new(), 1);

        if (_oldSelectGameLand.ArmySprite is { } armySprite)
            armySprite.Position = _oldSelectGameLand.Position + _delta;
        if (_oldSelectGameLand.GeneralSprite is { } generalSprite)
            generalSprite.Position = _oldSelectGameLand.Position + _delta;
    }
}