using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public partial class AffairItemList : BaseItemList
{
    public override void _Ready()
    {
        ItemActivated += _ => Set();
        foreach (Affair affair in MapController.Affairs)
            AddItem(GetText(affair));
    }

    public override void Delete()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Affairs.TryGetValue(index, out Affair affair)) return;
        Game.Dialog.Builder($"确定要删除这个事件？\n({GetText(affair)})", () =>
        {
            MapController.Affairs.RemoveAt(index);
            RemoveItem(index);
        });
    }

    public override void Add()
    {
        Game.Dialog.Builder("确定要新建事件？", () =>
        {
            Affair affair = new();
            MapController.Affairs.Add(affair);
            AddItem(GetText(affair));
        });
    }

    private static string GetText(Affair affair) => $"事件ID:{affair.事件ID}  类型:{affair.事件类型}  触发回合:{affair.触发回合}  对话代码:{affair.对话代码}";

    public override void Set()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Affairs.TryGetValue(index, out Affair affair)) return;
        Game.BtlObjWindow.CreateEdit(affair, w =>
        {
            MapController.Affairs[index] = w;
            SetItemText(index, GetText(w));
        });
    }

    public override void Copy()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Affairs.TryGetValue(index, out Affair affair)) return;
        Game.Dialog.Builder($"确定要复制这个事件？\n({GetText(affair)})", () =>
        {
            var newWeather = (Affair)affair.Clone();
            MapController.Affairs.Add(newWeather);
            AddItem(GetText(newWeather));
        });
    }
}