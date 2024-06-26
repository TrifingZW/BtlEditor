using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using Godot;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public partial class StrategyItemList : ItemList, IDataOperation
{
    private static MapController MapController => Game.Instance.MapController;

    public override void _Ready()
    {
        ItemActivated += _ => Set();
        foreach (Strategy strategy in MapController.Strategies)
            AddItem(GetText(strategy));
    }

    public void Delete()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Strategies.TryGetValue(index, out Strategy strategy)) return;
        Game.Instance.Dialog.Builder($"确定要删除这个战略？\n({GetText(strategy)})", () =>
        {
            MapController.Strategies.RemoveAt(index);
            RemoveItem(index);
        });
    }

    public void Add()
    {
        Game.Instance.Dialog.Builder("确定要新建战略？", () =>
        {
            Strategy strategy = new();
            MapController.Strategies.Add(strategy);
            AddItem(GetText(strategy));
        });
    }

    private static string GetText(Strategy strategy) => $"军团编号:{strategy.军团序号}  回合:{strategy.回合}  建设代码:{strategy.建设代码}";

    public void Set()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Strategies.TryGetValue(index, out Strategy strategy)) return;
        Game.Instance.BtlObjWindow.CreateEdit(strategy, s =>
        {
            MapController.Strategies[index] = s;
            SetItemText(index, GetText(s));
        });
    }

    public void Copy()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Strategies.TryGetValue(index, out Strategy strategy)) return;
        Game.Instance.Dialog.Builder($"确定要复制这个战略？\n({GetText(strategy)})", () =>
        {
            var newStrategy = (Strategy)strategy.Clone();
            MapController.Strategies.Add(newStrategy);
            AddItem(GetText(newStrategy));
        });
    }
}