using Godot;

namespace BtlEditor.MainScreen.Scripts;

public partial class ToolScript : VBoxContainer
{
    public override void _Ready()
    {
        //设置
        Button setting = GetNode<Button>("Setting");
        setting.Pressed += () =>
        {
            GetNode<SettingWindowScript>("SettingWindow").Load();
            GetNode<SettingWindowScript>("SettingWindow").Show();
        };

        //退出
        Button exit = GetNode<Button>("Exit");
        exit.Pressed += () => { GetTree().Quit(); };
    }
}