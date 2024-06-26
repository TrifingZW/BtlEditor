using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using Godot;

namespace BtlEditor.GameScreen.Scripts;

public partial class EditUI : InterceptScreen.Scripts.InterceptUiLayer
{
    private TabBar _tabBar;
    private Button _clear;
    private Button _save;
    private Button _cancel;
    private short _coords;

    private static MapController MapController => Game.Instance.MapController;
    private static TileMapLayer TileMapLayer => MapController.TileMapLayer;

    public override void _Ready()
    {
        _tabBar = GetNode<TabBar>("%ProvinceModeTab");
        _clear = GetNode<Button>("%ProvinceModeClear");
        _save = GetNode<Button>("%ProvinceModeSave");
        _cancel = GetNode<Button>("%ProvinceModeCancel");
        _clear.Pressed += () => TileMapLayer.Clear();
        _save.Pressed += () =>
        {
            var vector2Is = TileMapLayer.GetUsedCells();
            foreach (GameLandUnit landUnit in MapController.LandUnits)
            {
                if (landUnit.Province == _coords)
                {
                    landUnit.Province = 0;
                    landUnit.UpdateProvinceColor();
                }

                if (vector2Is.Contains(landUnit.Coords))
                {
                    landUnit.Province = _coords;
                    landUnit.UpdateProvinceColor();
                }
            }

            MapController.UpdateColorUV();

            TileMapLayer.Clear();
            Game.Instance.ProvinceMode = false;
        };
        _cancel.Pressed += () =>
        {
            TileMapLayer.Clear();
            Game.Instance.ProvinceMode = false;
        };
    }

    public void Start()
    {
        _coords = Game.Instance.MapUI.SingeGameLandUnit.RegionIndex;
        foreach (GameLandUnit landUnit in MapController.LandUnits)
            if (landUnit.Province == _coords)
                TileMapLayer.SetCell(landUnit.Coords, MapController.SingleTileSetAtlasId, new());
    }

    private bool _multiPressed;
    private static Vector2 MousePosition => MapController.GetGlobalMousePosition();

    public override void _UnhandledInput(InputEvent @event)
    {
        if (!Game.Instance.ProvinceMode) return;
        switch (@event)
        {
            case InputEventMouseButton inputEventMouseButton:
            {
                if (inputEventMouseButton.ButtonIndex == MouseButton.Left)
                {
                    _multiPressed = inputEventMouseButton.Pressed;
                    Vector2I vector2I = TileMapLayer.LocalToMap(MousePosition);
                    if (MapController.LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit _))
                        switch (_tabBar.CurrentTab)
                        {
                            case 0:
                                TileMapLayer.SetCell(vector2I, MapController.SingleTileSetAtlasId, new());
                                break;
                            case 1:
                                TileMapLayer.EraseCell(vector2I);
                                break;
                        }
                }

                break;
            }
            case InputEventMouseMotion:
            {
                if (_multiPressed)
                {
                    Vector2I vector2I = TileMapLayer.LocalToMap(MousePosition);
                    if (MapController.LandUnits.TryGetValue(MapHelper.GetIndex(vector2I), out GameLandUnit _))
                        switch (_tabBar.CurrentTab)
                        {
                            case 0:
                                TileMapLayer.SetCell(vector2I, MapController.SingleTileSetAtlasId, new());
                                break;
                            case 1:
                                TileMapLayer.EraseCell(vector2I);
                                break;
                        }
                }

                break;
            }
        }
    }
}