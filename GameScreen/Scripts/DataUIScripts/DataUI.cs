using BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;
using BtlEditor.GameScreen.Scripts.Windows;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts;

public partial class DataUI : CanvasLayer
{
    public BtlObjWindow BtlObjWindow { get; private set; }
    private CountryItemList _countryItemList;
    private TabContainer _tabContainer;
    private static MapController MapController => Game.Instance.MapController;

    public override void _Ready()
    {
        BtlObjWindow = GetNode<BtlObjWindow>("%BtlObjWindow");
        _countryItemList = GetNode<CountryItemList>("%CountryItemList");
        _tabContainer = GetNode<TabContainer>("%TabContainer");
        for (var index = 0; index < Btl.Countries.Length; index++)
            _countryItemList.AddItem(index, Btl.Countries[index]);
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

    private void Add()
    {
        if (_tabContainer.GetCurrentTabControl() is BaseItemList baseItemList)
            baseItemList.Add();
    }

    private void Delete()
    {
        if (_tabContainer.GetCurrentTabControl() is BaseItemList baseItemList)
            baseItemList.Delete();
    }

    private void Set()
    {
        if (_tabContainer.GetCurrentTabControl() is BaseItemList baseItemList)
            baseItemList.Set();
    }
}