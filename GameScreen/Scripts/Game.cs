using BtlEditor.CoreScripts.Structures;
using BtlEditor.GameScreen.Scripts.DataUIScripts;
using BtlEditor.GameScreen.Scripts.MapUIScripts;
using BtlEditor.GameScreen.Scripts.Windows;
using Godot;

namespace BtlEditor.GameScreen.Scripts;

public partial class Game : Node2D
{
    public MapController MapController { get; private set; }
    public CameraController CameraController { get; private set; }
    public MapUI MapUI { get; private set; }
    public DataUI DataUI { get; private set; }
    public EditUI EditUI { get; private set; }
    public Dialog Dialog { get; private set; }
    public EditWindow EditWindow { get; private set; }
    public BtlObjWindow BtlObjWindow { get; private set; }
    public SearchGeneralWindow SearchGeneralWindow { get; private set; }
    public SearchArmyWindow SearchArmyWindow { get; private set; }
    public SearchCountryWindow SearchCountryWindow { get; private set; }
    public AcceptDialog AcceptDialog { get; private set; }
    public AudioStreamPlayer AudioStreamPlayer { get; private set; }
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

    public bool ProvinceMode { get; set; }

    public void StartProvinceMode(short coords)
    {
        ProvinceMode = true;
        MapUI.Visible = false;
        EditUI.Visible = true;
        EditUI.Start(coords);
    }

    public void StopProvinceMode()
    {
        ProvinceMode = false;
        MapUI.Visible = true;
        EditUI.Visible = false;
    }

    #region 粘贴板

    public static Reinforcement ReinforcementCopy { get; set; }
    public static Army ArmyCopy { get; set; }
    public static City CityCopy { get; set; }
    public static byte BelongCopy { get; set; }

    #endregion

    public override void _EnterTree()
    {
        Instance = this;
        MapController = GetNode<MapController>("MapController");
        CameraController = GetNode<CameraController>("Camera2D");
        MapUI = GetNode<MapUI>("MapUI");
        DataUI = GetNode<DataUI>("DataUI");
        EditUI = GetNode<EditUI>("EditUI");
        BtlObjWindow = GetNode<BtlObjWindow>("BtlObjWindow");
        SearchGeneralWindow = GetNode<SearchGeneralWindow>("SearchGeneralWindow");
        SearchArmyWindow = GetNode<SearchArmyWindow>("SearchArmyWindow");
        SearchCountryWindow = GetNode<SearchCountryWindow>("SearchCountryWindow");
        AcceptDialog = GetNode<AcceptDialog>("AcceptDialog");
        Dialog = GetNode<Dialog>("Dialog");
        EditWindow = GetNode<EditWindow>("EditWindow");
        AudioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }

    public override void _Input(InputEvent @event)
    {
        if (ProvinceMode) return;
        if (@event is not InputEventKey keyEvent) return;
        if (keyEvent.Pressed && keyEvent.KeyLabel == Key.Capslock)
            DataMode = !DataMode;
        if (keyEvent.Pressed && keyEvent.KeyLabel == Key.S)
            if (keyEvent.CtrlPressed)
                MapController.Save();
    }
}