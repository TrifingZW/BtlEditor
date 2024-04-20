using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public partial class AirSupportItemList : BaseItemList
{
    public override void _Ready()
    {
        ItemActivated += _ => Set();
        foreach (AirSupport airSupport in MapController.AirSupports)
            AddItem(GetText(airSupport));
    }

    public override void Delete()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.AirSupports.TryGetValue(index, out AirSupport airSupport)) return;
        Game.Instance.Dialog.Builder($"确定要删除这个空中支援？\n({GetText(airSupport)})", () =>
        {
            MapController.AirSupports.RemoveAt(index);
            RemoveItem(index);
        });
    }

    public override void Add()
    {
        Game.Instance.Dialog.Builder("确定要新建空中支援？", () =>
        {
            AirSupport airSupport = new();
            MapController.AirSupports.Add(airSupport);
            AddItem(GetText(airSupport));
        });
    }

    private static string GetText(AirSupport airSupport) => $"兵种:{airSupport.兵种}  弹药:{airSupport.弹药}  军团:{airSupport.军团}  回合:{airSupport.回合}";

    public override void Set()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.AirSupports.TryGetValue(index, out AirSupport airSupport)) return;
        Game.Instance.BtlObjWindow.CreateEdit(airSupport, a =>
        {
            MapController.AirSupports[index] = a;
            SetItemText(index, GetText(a));
        });
    }

    public override void Copy()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.AirSupports.TryGetValue(index, out AirSupport airSupport)) return;
        Game.Instance.Dialog.Builder($"确定要复制这个空中支援？\n({GetText(airSupport)})", () =>
        {
            var newAirSupport = (AirSupport)airSupport.Clone();
            MapController.AirSupports.Add(newAirSupport);
            AddItem(GetText(airSupport));
        });
    }
}