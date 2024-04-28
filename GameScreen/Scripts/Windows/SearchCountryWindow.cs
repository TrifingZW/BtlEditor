using System;
using System.Linq;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.Windows;

public partial class SearchCountryWindow : Window
{
    private ItemList ItemList { get; set; }
    private Button Button { get; set; }

    private Action<byte> _callback;

    public override void _Ready()
    {
        CloseRequested += () => Visible = false;
        ItemList = GetNode<ItemList>("MarginContainer/VBoxContainer/PanelContainer/ItemList");
        Button = GetNode<Button>("MarginContainer/VBoxContainer/Button");

        ItemList.ItemActivated += _ => Save();
        Button.Pressed += Save;
    }

    private void Save()
    {
        if (!ItemList.GetSelectedItems().TryGetValue(0, out var index)) return;
        _callback?.Invoke((byte)index);
        Visible = false;
    }

    private void Update()
    {
        ItemList.Clear();
        foreach (Country country in Game.Instance.MapController.Countries)
            SetItem(ItemList.AddItem("无数据"), country);
    }

    private void SetItem(int index, Country country)
    {
        ItemList.SetItemText(index, $"序号:{country.序号}  {Stringtable.CountryName[country.国家]}");
        SetIcon(index, country);
    }

    private void SetIcon(int index, Country country)
    {
        foreach (Rect2 rect in from element in Tacticalmap.Images.ImageList
                 where element.Name == $"f_{country.国家:D2}.png"
                 select new Rect2
                 {
                     Position = new(element.X, element.Y),
                     Size = new(element.Width, element.Height)
                 })
        {
            ItemList.SetItemIcon(index, Tacticalmap.Texture2D);
            ItemList.SetItemIconRegion(index, rect);
            break;
        }
    }

    public void CreateEdit(Action<byte> callback)
    {
        Update();
        _callback = callback;
        Show();
    }
}