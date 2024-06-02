using BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;
using Godot;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts;

public partial class DataUI : InterceptScreen.Scripts.InterceptUiLayer
{
    private TabContainer _tabContainer;

    public AirSupportTree AirSupportTree { get; set; }
    private static MapController MapController => Game.Instance.MapController;

    public override void _Ready()
    {
        _tabContainer = GetNode<TabContainer>("%TabContainer");
        AirSupportTree = GetNode<AirSupportTree>("%AirSupportTree");
    }

    private void TabSelect(long index)
    {
        _tabContainer.CurrentTab = (int)index;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event is not InputEventKey keyEvent) return;
        if (keyEvent.Pressed && keyEvent.KeyLabel == Key.Delete)
            Delete();
    }

    private void Close() => Hide();

    private void Add()
    {
        if (_tabContainer.GetCurrentTabControl() is IDataOperation baseItemList)
            baseItemList.Add();
    }

    private void Delete()
    {
        if (_tabContainer.GetCurrentTabControl() is IDataOperation baseItemList)
            baseItemList.Delete();
    }

    private void Set()
    {
        if (_tabContainer.GetCurrentTabControl() is IDataOperation baseItemList)
            baseItemList.Set();
    }

    private void Copy()
    {
        if (_tabContainer.GetCurrentTabControl() is IDataOperation baseItemList)
            baseItemList.Copy();
    }
}