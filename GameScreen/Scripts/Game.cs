using BtlEditor.GameScreen.Scripts.MapUIScripts;
using Godot;

namespace BtlEditor.GameScreen.Scripts;

public partial class Game : Node2D
{
    public MapController MapController { get; private set; }
    public CameraController CameraController { get; private set; }
    public MapUI MapUI { get; private set; }
    public DataUIScripts.DataUI DataUI { get; private set; }
    public static Dialog Dialog { get; private set; }
    public static Game Instance { get; private set; }

    private bool _dataMode;

    private bool DataMode
    {
        get => _dataMode;
        set
        {
            _dataMode = value;
            if (value)
            {
                DataUI.Visible = true;
                MapUI.Visible = false;
            }
            else
            {
                DataUI.Visible = false;
                MapUI.Visible = true;
            }
        }
    }

    public override void _EnterTree()
    {
        MapController = GetNode<MapController>("MapController");
        CameraController = GetNode<CameraController>("Camera2D");
        Instance = this;
        MapUI = GetNode<MapUI>("MapUI");
        DataUI = GetNode<DataUIScripts.DataUI>("DataUI");
        Dialog = GetNode<Dialog>("Dialog");
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent) return;
        if (keyEvent.Pressed && keyEvent.KeyLabel == Key.Tab)
            DataMode = !DataMode;
    }
}