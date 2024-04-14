using System;
using System.Collections.Generic;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.Windows;

public partial class SearchArmyWindow : Window
{
    private LineEdit LineEdit { get; set; }
    private ItemList ItemList { get; set; }
    private Button Button { get; set; }
    private List<int> SelectedArmies { get; } = [];

    private Action<ArmyJson> _callback;

    public override void _Ready()
    {
        CloseRequested += () => Visible = false;
        LineEdit = GetNode<LineEdit>("MarginContainer/VBoxContainer/LineEdit");
        ItemList = GetNode<ItemList>("MarginContainer/VBoxContainer/PanelContainer/ItemList");
        Button = GetNode<Button>("MarginContainer/VBoxContainer/Button");

        LineEdit.TextChanged += Update;
        Button.Pressed += () =>
        {
            if (!ItemList.GetSelectedItems().TryGetValue(0, out var index)) return;
            _callback?.Invoke(ArmySettings.ArmyJsons[SelectedArmies[index]]);
            Visible = false;
        };

        Update("");
    }

    private void Update(string text)
    {
        ItemList.Clear();
        SelectedArmies.Clear();
        for (var index = 0; index < ArmySettings.ArmyJsons.Length; index++)
        {
            ArmyJson armyJson = ArmySettings.ArmyJsons[index];
            if (!armyJson.Name.Contains(text)) continue;
            ItemList.AddItem($"{armyJson.Name} {armyJson.Army}");
            SelectedArmies.Add(index);
        }
    }

    public void CreateEdit(Action<ArmyJson> callback)
    {
        _callback = callback;
        Show();
    }
}