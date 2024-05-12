using BtlEditor.CoreScripts.Structures;
using BtlEditor.GameScreen.Scripts.DataUIScripts;
using BtlEditor.GameScreen.Scripts.MapUIScripts;
using BtlEditor.GameScreen.Scripts.Windows;
using Godot;

namespace BtlEditor.GameScreen.Scripts;

public partial class Game : Node2D
{
    public MapController MapController { get; private set; }
    public CoreScripts.CameraController CameraController { get; private set; }
    public MainUI MainUI { get; private set; }
    public MapUI MapUI { get; private set; }
    public DataUI DataUI { get; private set; }
    public InterceptUI InterceptUI { get; private set; }
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

    private bool _editMode;

    public bool EditMode
    {
        get => _editMode;
        set
        {
            _editMode = value;
            if (EditMode)
            {
                MainUI.Hide();
                MapUI.Hide();
            }
            else
                MainUI.Show();
        }
    }

    private bool _provinceMode;

    public bool ProvinceMode
    {
        get => _provinceMode;
        set
        {
            _provinceMode = value;
            if (ProvinceMode)
            {
                EditMode = true;
                EditUI.Start();
                EditUI.Show();
            }
            else
            {
                EditMode = false;
                EditUI.Hide();
            }
        }
    }

    private bool _interceptMode;

    public bool InterceptMode
    {
        get => _interceptMode;
        set
        {
            _interceptMode = value;
            if (InterceptMode)
            {
                EditMode = true;
                InterceptUI.Show();
            }
            else
            {
                EditMode = false;
                InterceptUI.Hide();
            }
        }
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
        CameraController = GetNode<CoreScripts.CameraController>("Camera2D");
        MainUI = GetNode<MainUI>("MainUI");
        MapUI = GetNode<MapUI>("MapUI");
        DataUI = GetNode<DataUI>("DataUI");
        InterceptUI = GetNode<InterceptUI>("InterceptUI");
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

    public void Exit()
    {
        MapController.FreeResources();
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://MainScreen/MainInterface.tscn"));
    }
}