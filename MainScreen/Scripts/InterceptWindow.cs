using Godot;

namespace BtlEditor.MainScreen.Scripts;

public partial class InterceptWindow : Window
{
    public override void _Ready()
    {
    }

    public void StartIntercept(string btlPath)
    {
        Show();
    }
}