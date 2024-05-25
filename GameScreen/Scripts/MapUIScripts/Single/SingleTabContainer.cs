using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class SingleTabContainer : TabContainer, IInput
{
    private static MapController MapController => Game.Instance.MapController;
    private static GameLandUnit[] LandUnits => MapController.LandUnits;
    private static TileMapLayer TileMapLayer => MapController.TileMapLayer;
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
    private bool _selectReleased;
    private Vector2 _selectDelta;
    private GameLandUnit _oldSelectLand;
    private Vector2 _oldMousePosition;
    private Vector2 _pressedMousePosition;
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
        _selectReleased = true;
        _pressedMousePosition = MousePosition;

        Vector2I vector2I = TileMapLayer.LocalToMap(MousePosition);
        if (!LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit landUnit)) return;

        if (landUnit != _oldSelectLand) return;
        if (_oldSelectLand?.ArmySprite is not { Select: true }) return;
        _oldMousePosition = MousePosition;
        Indicator.Visible = false;
        _selectMotion = true;
        Game.Instance.MapUI.Visible = false;
        Game.Instance.CameraController.AndroidCameraController = false;
    }

    private void Released()
    {
        if (!_selectMotion)
        {
            if (!_selectReleased) return;
            Vector2I vector2I = TileMapLayer.LocalToMap(MousePosition);
            if (!LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit landUnit)) return;

            if (landUnit == _oldSelectLand)
                if (_oldSelectLand?.ArmySprite is { Select: true })
                    return;

            Game.Instance.AudioStreamPlayer.Play();
            SingeGameLandUnit = landUnit;

            if (_oldSelectLand?.ArmySprite is { } oldArmySprite) oldArmySprite.Select = false;
            if (landUnit.ArmySprite is { } armySprite) armySprite.Select = true;

            _oldSelectLand = landUnit;
        }
        else
        {
            //军队交换
            Vector2I vector2I = TileMapLayer.LocalToMap(_oldSelectLand.Position + _selectDelta);
            if (LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit landUnit))
            {
                _oldSelectLand.ArmySprite.Select = false;
                var belong = _oldSelectLand.Belong;
                (_oldSelectLand.Army, landUnit.Army) = (landUnit.Army, _oldSelectLand.Army);
                if (landUnit.Belong == 0xff) landUnit.Belong = belong;
                SingeGameLandUnit = landUnit;
            }
            else _oldSelectLand.UpdateArmy();

            //清除记录
            _oldMousePosition = default;
            _selectDelta = default;
            _selectMotion = false;
            Game.Instance.MapUI.Visible = true;
            Game.Instance.CameraController.AndroidCameraController = true;
            TileMapLayer.Clear();
        }
    }

    private void Motion()
    {
        if (!_selectMotion)
        {
            _selectReleased = _pressedMousePosition.DistanceTo(MousePosition) < 1f;
            return;
        }

        //计算位置
        _selectDelta = MousePosition - _oldMousePosition;
        Vector2I vector2I = TileMapLayer.LocalToMap(_oldSelectLand.Position + _selectDelta);
        TileMapLayer.Clear();
        TileMapLayer.SetCell(vector2I, MapController.SingleTileSetAtlasId, new(), 1);

        if (_oldSelectLand.ArmySprite is { } armySprite)
            armySprite.Position = _oldSelectLand.Position + _selectDelta;
        if (_oldSelectLand.GeneralSprite is { } generalSprite)
            generalSprite.Position = _oldSelectLand.Position + _selectDelta;
    }
}