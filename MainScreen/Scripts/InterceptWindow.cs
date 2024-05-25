using Godot;

namespace BtlEditor.MainScreen.Scripts;

public partial class InterceptWindow : Window
{
    public override void _Ready()
    {
        Position += new Vector2I(0, 34);
    }

    public void StartIntercept(string btlPath)
    {
        Show();
    }
}