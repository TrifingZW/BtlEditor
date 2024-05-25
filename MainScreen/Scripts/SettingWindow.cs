using BtlEditor.CoreScripts;
using Godot;
using static BtlEditor.CoreScripts.Globals;

namespace BtlEditor.MainScreen.Scripts;

public partial class SettingWindow : Window
{
    private OptionButton _size;
    private OptionButton _windowMode;

    public override void _Ready()
    {
        CloseRequested += Hide;
        _size = GetNode<OptionButton>("%Size");
        _size.Selected = RenderScale;
        _size.ItemSelected += index =>
        {
            RenderScale = (int)index;
            Save();
        };

        _windowMode = GetNode<OptionButton>("%WindowMode");
        _windowMode.Selected = (int)WindowMode;
        _windowMode.ItemSelected += index =>
        {
            WindowMode = index;
            Save();
        };
    }

    public void StartSetting()
    {
        Show();
    }
}