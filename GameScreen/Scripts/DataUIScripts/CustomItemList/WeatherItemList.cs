using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using Godot;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public partial class WeatherItemList : ItemList, IDataOperation
{
    private static MapController MapController => Game.Instance.MapController;

    public override void _Ready()
    {
        base._Ready();
        ItemActivated += _ => Set();
        foreach (Weather weather in MapController.Weathers)
            AddItem(GetText(weather));
    }

    public void Delete()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Weathers.TryGetValue(index, out Weather weather)) return;
        Game.Instance.Dialog.Builder($"确定要删除这个天气？\n({GetText(weather)})", () =>
        {
            MapController.Weathers.RemoveAt(index);
            RemoveItem(index);
        });
    }

    public void Add()
    {
        Game.Instance.Dialog.Builder("确定要新建天气？", () =>
        {
            Weather weather = new()
            {
                触发回合 = 1,
                持续回合 = 3,
            };
            MapController.Weathers.Add(weather);
            AddItem(GetText(weather));
        });
    }

    private static string GetText(Weather weather) => $"类型:{weather.天气类型}  触发回合:{weather.触发回合}  持续回合:{weather.持续回合}";

    public void Set()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Weathers.TryGetValue(index, out Weather weather)) return;
        Game.Instance.BtlObjWindow.CreateEdit(weather, w =>
        {
            MapController.Weathers[index] = w;
            SetItemText(index, GetText(w));
        });
    }

    public void Copy()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Weathers.TryGetValue(index, out Weather weather)) return;
        Game.Instance.Dialog.Builder($"确定要复制这个天气？\n({GetText(weather)})", () =>
        {
            var newWeather = (Weather)weather.Clone();
            MapController.Weathers.Add(newWeather);
            AddItem(GetText(newWeather));
        });
    }
}