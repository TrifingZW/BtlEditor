using BtlEditor.UserInterface;

namespace BtlEditor.InterceptScreen.Scripts;

public partial class InterceptUiLayer : Godot.CanvasLayer
{
    private TabPanel _tabPanel;
    public MasterContainer MasterContainer { get; private set; }

    public override void _Ready()
    {
        _tabPanel = GetNode<TabPanel>("%TabPanel");
        MasterContainer = GetNode<MasterContainer>("%MasterContainer");
        _tabPanel.Close.Pressed += Hide;
    }
}