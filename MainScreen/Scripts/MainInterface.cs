using System.IO;
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
    private ItemList _itemList;
    private string[] _btlList;
    private OptionButton _translationOption;

    public override void _EnterTree()
    {
        _btlList = Directory.GetFiles(StagePath);
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

        _itemList = GetNode<ItemList>("%ItemList");
        foreach (var btl in _btlList)
            _itemList.AddItem(btl);
    }

    private void EditChanged(string value)
    {
        _itemList.Clear();
        foreach (var btl in _btlList)
            if (btl.Contains(value))
                _itemList.AddItem(btl);
    }

    private void Setting() => _settingWindow.StartSetting();
    private void Intercept() => GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://InterceptScreen/Intercept.tscn"));

    private void StartBtl()
    {
        if (!_itemList.GetSelectedItems().TryGetValue(0, out var index)) return;
        MapHelper.BtlPath = _itemList.GetItemText(index);
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://GameScreen/Game.tscn"));
    }

    private void StartBin()
    {
        if (!_itemList.GetSelectedItems().TryGetValue(0, out var index)) return;
    }

    private void StartBinScene() => GetTree().ChangeSceneToFile("res://BinScreen/Bin.tscn");
}