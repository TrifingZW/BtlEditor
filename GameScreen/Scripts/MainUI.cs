using Godot;

namespace BtlEditor.GameScreen.Scripts;

public partial class MainUI : InterceptScreen.Scripts.InterceptUiLayer
{
    public override void _Ready()
    {
        GetNode<MenuButton>("%MenuButton").GetPopup().IdPressed += id =>
        {
            switch (id)
            {
                case 0:
                    break;
                case 1:
                    Game.Instance.MapController.Save();
                    break;
                case 2:
                    Game.Instance.Exit();
                    break;
            }
        };
        GetNode<MenuButton>("%MenuButton2").GetPopup().IdPressed += id =>
        {
            switch (id)
            {
                case 0:
                    Game.Instance.MapUI.Show();
                    break;
                case 1:
                    Game.Instance.DataUI.Show();
                    break;
                case 2:
                    Game.Instance.InterceptMode = true;
                    break;
            }
        };
    }
}