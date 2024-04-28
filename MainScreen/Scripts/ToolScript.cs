using Godot;

namespace BtlEditor.MainScreen.Scripts;

public partial class ToolScript : VBoxContainer
{
    public override void _Ready()
    {
        //设置
        var setting = GetNode<Button>("Setting");
        setting.Pressed += () =>
        {
            GetNode<SettingWindowScript>("%SettingWindow").Load();
            GetNode<SettingWindowScript>("%SettingWindow").Show();
        };

        //退出
        var exit = GetNode<Button>("Exit");
        exit.Pressed += () => { GetTree().Quit(); };
    }
}