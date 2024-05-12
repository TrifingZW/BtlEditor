using Godot;

namespace BtlEditor.UserInterface;

public partial class TabPanel : Panel
{
    public TabBar TabBar { get; private set; }
    public Button Close { get; private set; }
    public TabContainer TabContainer { get; private set; }

    public override void _Ready()
    {
        TabBar = GetNode<TabBar>("%TabBar");
        Close = GetNode<Button>("%Close");
        TabContainer = GetNode<TabContainer>("%TabContainer");
    }

    private void TabSelect(long index) => TabContainer.CurrentTab = (int)index;
}