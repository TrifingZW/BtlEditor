using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class SingleContainer : PanelContainer, IInput
{
    private static MapController MapController => Game.Instance.MapController;
    private static LandUnit[] LandUnits => MapController.LandUnits;
    private static TileMap TileMap => MapController.TileMap;
    private static Indicator Indicator => MapController.Indicator;
    private LandUnit _singeLandUnit;

    private TabContainer _singleTabContainer;

    public LandUnit SingeLandUnit
    {
        get => _singeLandUnit;
        set
        {
            _singeLandUnit = value;
            MapController.Indicator.Position = SingeLandUnit.Position;
            MapController.Indicator.Visible = true;
            ((BaseSingle)_singleTabContainer.GetCurrentTabControl()).LandUnit = SingeLandUnit;
        }
    }

    public override void _Ready()
    {
        _singleTabContainer = GetNode<TabContainer>("%SingleTabContainer");
        _singleTabContainer.TabChanged += select =>
        {
            if (SingeLandUnit != null)
                _singleTabContainer.GetChild<BaseSingle>((int)select).LandUnit = SingeLandUnit;
        };
    }


    private bool _selectMotion;
    private bool _selectReleased;
    private Vector2 _selectDelta;
    private LandUnit _oldSelectLand;
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

        Vector2I vector2I = TileMap.LocalToMap(MousePosition);
        if (!LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Btl.Master.地图宽), out LandUnit landUnit)) return;

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
            Vector2I vector2I = TileMap.LocalToMap(MousePosition);
            if (!LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Btl.Master.地图宽), out LandUnit landUnit)) return;

            if (landUnit == _oldSelectLand)
                if (_oldSelectLand?.ArmySprite is { Select: true })
                    return;

            Game.Instance.AudioStreamPlayer.Play();
            SingeLandUnit = landUnit;

            if (_oldSelectLand?.ArmySprite is { } oldArmySprite) oldArmySprite.Select = false;
            if (landUnit.ArmySprite is { } armySprite) armySprite.Select = true;

            _oldSelectLand = landUnit;
        }
        else
        {
            //军队交换
            Vector2I vector2I = TileMap.LocalToMap(_oldSelectLand.Position + _selectDelta);
            if (LandUnits.TryGetValue(ParserHelper.GetIndex(vector2I, Btl.Master.地图宽), out LandUnit landUnit))
            {
                _oldSelectLand.ArmySprite.Select = false;
                var belong = _oldSelectLand.Belong;
                (_oldSelectLand.Army, landUnit.Army) = (landUnit.Army, _oldSelectLand.Army);
                if (landUnit.Belong == 0xff) landUnit.Belong = belong;
                SingeLandUnit = landUnit;
            }
            else _oldSelectLand.UpdateArmy();

            //清除记录
            _oldMousePosition = default;
            _selectDelta = default;
            _selectMotion = false;
            Game.Instance.MapUI.Visible = true;
            Game.Instance.CameraController.AndroidCameraController = true;
            TileMap.ClearLayer(MapController.SingleLayer);
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
        Vector2I vector2I = TileMap.LocalToMap(_oldSelectLand.Position + _selectDelta);
        TileMap.ClearLayer(MapController.SingleLayer);
        TileMap.SetCell(MapController.SingleLayer, vector2I, MapController.SingleTileSetAtlasId, new(), 1);

        if (_oldSelectLand.ArmySprite is { } armySprite)
            armySprite.Position = _oldSelectLand.Position + _selectDelta;
        if (_oldSelectLand.GeneralSprite is { } generalSprite)
            generalSprite.Position = _oldSelectLand.Position + _selectDelta;
    }
}