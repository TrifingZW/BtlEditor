using System.Linq;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public partial class CountryItemList : BaseItemList
{
    private static MapController MapController => Game.Instance.MapController;

    public override void _Ready()
    {
        ItemSelected += ItemSelect;
        ItemActivated += ItemClick;
    }

    public void AddItem(int index, Country country)
    {
        AddItem($"{Stringtable.CountryName[country.国家]}", Tacticalmap.Texture2D);

        foreach (Rect2 rect in from element in Tacticalmap.Images.ImageList
                 where element.Name == $"f_{country.国家:D2}.png"
                 select new Rect2
                 {
                     Position = new(element.X, element.Y),
                     Size = new(element.Width, element.Height)
                 })
        {
            SetItemIconRegion(index, rect);
            break;
        }
    }

    private static void ItemSelect(long index)
    {
        foreach (LandUnit landUnit in MapController.LandUnits)
            if (landUnit.Capital != null)
                if (landUnit.Belong == index)
                {
                    Game.Instance.CameraController.TargetPosition = landUnit.Position;
                    break;
                }
    }

    private void ItemClick(long index) => Set();

    public override void Delete()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (MapController.Countries.TryGetValue(index, out Country country))
            Game.Dialog.Builder($"确定要删除这个国家(归属:{index} {Stringtable.CountryName[country.国家]})",
                () =>
                {
                    MapController.RemoveCountry(index);
                    RemoveItem(index);
                });
    }

    public override void Add()
    {
    }

    public override void Set()
    {
        if (GetSelectedItems().TryGetValue(0, out var index))
            Game.Instance.DataUI.BtlObjWindow.CreateEdit(MapController.Countries[index]);
    }
}