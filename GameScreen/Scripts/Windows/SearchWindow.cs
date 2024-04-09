using System;
using System.Collections.Generic;
using System.IO;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.UserInterface;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.Windows;

public partial class SearchWindow : Window
{
    private LineEdit LineEdit { get; set; }
    private ItemList ItemList { get; set; }
    private Button Button { get; set; }
    private TextureRect GeneralTexture => GetNode<TextureRect>("%GeneralTexture");
    private Label GeneralName => GetNode<Label>("%GeneralName");
    private StarRect Star1 => GetNode<StarRect>("%Star1");
    private StarRect Star2 => GetNode<StarRect>("%Star2");
    private StarRect Star3 => GetNode<StarRect>("%Star3");
    private StarRect Star4 => GetNode<StarRect>("%Star4");
    private StarRect Star5 => GetNode<StarRect>("%Star5");
    private StarRect Star6 => GetNode<StarRect>("%Star6");
    private List<int> SelectedGenerals { get; } = [];

    private Action<GeneralJson> _callback;

    public override void _Ready()
    {
        CloseRequested += () => Visible = false;
        LineEdit = GetNode<LineEdit>("%LineEdit");
        ItemList = GetNode<ItemList>("%ItemList");
        Button = GetNode<Button>("%Button");

        LineEdit.TextChanged += Update;
        ItemList.ItemSelected += ItemSelect;
        Button.Pressed += () =>
        {
            if (!ItemList.GetSelectedItems().TryGetValue(0, out var index)) return;
            _callback?.Invoke(GeneralSettings.GeneralJsons[SelectedGenerals[index]]);
            Visible = false;
        };

        Update("");
    }

    private void ItemSelect(long index)
    {
        GeneralJson generalJson = GeneralSettings.GeneralJsons[SelectedGenerals[(int)index]];
        GeneralName.Text = ItemList.GetItemText((int)index);
        Star1.SetStar(generalJson.Infantry);
        Star2.SetStar(generalJson.Artillery);
        Star3.SetStar(generalJson.Armor);
        Star4.SetStar(generalJson.Navy);
        Star5.SetStar(generalJson.AirForce);
        Star6.SetStar(generalJson.March);

        var path = $"{ImageHeadPath}/general_circle_{generalJson.Photo}.webp";
        if (!File.Exists(path)) return;
        Image image = Image.LoadFromFile(path);
        ImageTexture texture = new();
        texture.SetImage(image);
        GeneralTexture.Texture = texture;
    }

    private void Update(string text)
    {
        ItemList.Clear();
        SelectedGenerals.Clear();
        for (var index = 0; index < GeneralSettings.GeneralJsons.Length; index++)
        {
            GeneralJson generalJson = GeneralSettings.GeneralJsons[index];
            if (generalJson.Name == null || !Stringtable.GeneralName[generalJson.EName].Contains(text)) continue;
            ItemList.AddItem($"{generalJson.Name} {generalJson.Id}");
            SelectedGenerals.Add(index);
        }
    }

    public void CreateEdit(Action<GeneralJson> callback)
    {
        _callback = callback;
        Show();
    }
}