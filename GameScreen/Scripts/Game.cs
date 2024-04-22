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
    public Dialog Dialog { get; private set; }
    public BtlObjWindow BtlObjWindow { get; private set; }
    public SearchGeneralWindow SearchGeneralWindow { get; private set; }
    public SearchArmyWindow SearchArmyWindow { get; private set; }
    public AcceptDialog AcceptDialog { get; private set; }
    public AudioStreamPlayer AudioStreamPlayer { get;private set; }
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

    private void Handoff() => DataMode = !DataMode;

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
        BtlObjWindow = GetNode<BtlObjWindow>("BtlObjWindow");
        SearchGeneralWindow = GetNode<SearchGeneralWindow>("SearchGeneralWindow");
        SearchArmyWindow = GetNode<SearchArmyWindow>("SearchArmyWindow");
        AcceptDialog = GetNode<AcceptDialog>("AcceptDialog");
        Dialog = GetNode<Dialog>("Dialog");
        AudioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
    }
}