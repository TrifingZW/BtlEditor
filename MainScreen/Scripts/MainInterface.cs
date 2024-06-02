using System.IO;
using System.Linq;
using BtlEditor.CoreScripts;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;
using Translation = BtlEditor.CoreScripts.Translation;

namespace BtlEditor.MainScreen.Scripts;

public partial class MainInterface : Control
{
    private LoadWindow _loadWindow;
    private SettingWindow _settingWindow;
    private InterceptWindow _interceptWindow;
    private Tree _tree;
    private TreeItem _treeRoot;
    private string[] _btlList;
    private OptionButton _translationOption;

    public override void _EnterTree()
    {
        _btlList = Directory.GetFiles(StagePath).Select(Path.GetFileName).ToArray();
    }

    public override void _Ready()
    {
        _translationOption = GetNode<OptionButton>("%TranslationOption");
        _translationOption.Selected = (int)Globals.Translation;
        _translationOption.ItemSelected += index =>
        {
            Globals.Translation = (Translation)index;
            Globals.Save();
        };
        _loadWindow = GetNode<LoadWindow>("LoadWindow");
        _settingWindow = GetNode<SettingWindow>("SettingWindow");
        _interceptWindow = GetNode<InterceptWindow>("InterceptWindow");
        if (!StaticRes.Ready)
        {
            _loadWindow.Load(success =>
            {
                if (success) StaticRes.Ready = true;
            });
        }

        _tree = GetNode<Tree>("%Tree");
        _treeRoot = _tree.CreateItem();
        TreeItem conquest = CreateFloder("Conquest 征服");
        TreeItem stage = CreateFloder("Stage 战役");
        TreeItem generalStage = CreateFloder("GeneralStage 将领战役");
        TreeItem frontier = CreateFloder("Frontier 前线");
        TreeItem invadecorps = CreateFloder("Invadecorps 入侵");
        TreeItem warzone = CreateFloder("warzone 不知道");
        TreeItem qevent = CreateFloder("event 事件");
        stage.SetSelectable(0, false);
        foreach (var btl in _btlList)
        {
            if (btl.Contains("conquest"))
                CreateFile(conquest, btl);

            if (btl.Contains("stage")&&!btl.Contains("general"))
                CreateFile(stage, btl);
            
            if (btl.Contains("generalstage"))
                CreateFile(generalStage, btl);
            
            if (btl.Contains("frontier"))
                CreateFile(frontier, btl);
            
            if (btl.Contains("invadecorps"))
                CreateFile(invadecorps, btl);
            
            if (btl.Contains("warzone"))
                CreateFile(warzone, btl);
            
            if (btl.Contains("event"))
                CreateFile(qevent, btl);
        }
    }

    private TreeItem CreateFloder(string name)
    {
        TreeItem floder = _treeRoot.CreateChild();
        floder.Collapsed = true;
        floder.SetText(0, name);
        floder.SetIcon(0, ResourceLoader.Load<Texture2D>("res://Assets/Textures/UI/folder.svg"));
        floder.SetSelectable(0, false);
        return floder;
    }

    private void CreateFile(TreeItem root, string name)
    {
        TreeItem file = root.CreateChild();
        file.SetText(0, name);
        file.SetIcon(0, ResourceLoader.Load<Texture2D>("res://Assets/Textures/UI/file.svg"));
    }

    private void EditChanged(string value) => _treeRoot.Screen(value);


    private void Setting() => _settingWindow.StartSetting();
    private void Intercept() => GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://InterceptScreen/Intercept.tscn"));

    private void StartBtl()
    {
        if (_tree.GetSelected().GetText(0) is not { } path) return;
        MapHelper.BtlPath = $"{StagePath}/{path}";
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://GameScreen/Game.tscn"));
    }

    private void StartBin()
    {
    }

    private void StartBinScene() => GetTree().ChangeSceneToFile("res://BinScreen/Bin.tscn");
}