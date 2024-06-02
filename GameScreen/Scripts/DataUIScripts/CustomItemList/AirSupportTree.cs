using System.Collections.Generic;
using System.Linq;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public partial class AirSupportTree : Tree, IDataOperation
{
    private static MapController MapController => Game.Instance.MapController;
    public TreeItem Root { get; set; }
    private readonly List<TreeItem> _countries = [];

    public override void _Ready()
    {
        Root = CreateItem();

        foreach (AirSupport airSupport in MapController.AirSupports)
            CreateItem(airSupport);
    }

    private TreeItem CreateCountry(int countryId)
    {
        TreeItem country = Root.CreateChild();
        country.Collapsed = true;
        country.SetSelectable(0, false);
        country.SetText(0, Stringtable.CountryName[countryId]);

        if (Tacticalmap.Images.ImageList.FirstOrDefault(i => i.Name == $"f_{countryId:D2}.png") is { } element)
        {
            country.SetIcon(0, Tacticalmap.Texture2D);
            country.SetIconRegion(0, element.GetRect2());
        }

        return country;
    }

    private void CreateItem(AirSupport airSupport)
    {
        if (!MapController.Countries.TryGetValue(airSupport.军团, out Country country)) return;

        TreeItem countryTreeItem;
        if (_countries.All(c => c.GetText(0) != Stringtable.CountryName[country.国家]))
        {
            countryTreeItem = CreateCountry(country.国家);
            _countries.Add(countryTreeItem);
        }
        else countryTreeItem = _countries.First(c => c.GetText(0) == Stringtable.CountryName[country.国家]);

        TreeItem airSupportTreeItem = countryTreeItem.CreateChild();
        airSupportTreeItem.SetText(0, GetText(airSupport));
        airSupportTreeItem.SetMetadata(0, airSupport);
    }

    public void Delete()
    {
        Game.Instance.Dialog.Builder("确定要删除空中支援？", () =>
        {
            if (GetSelected() is not { } treeItem) return;
            MapController.AirSupports.Remove((AirSupport)treeItem.GetMetadata(0));
            TreeItem root = treeItem.GetParent();
            treeItem.Free();
            if (root.GetChildren().Count < 1)
                root.Free();
        });
    }

    public void Add()
    {
        Game.Instance.Dialog.Builder("确定要新建空中支援？", () =>
        {
            AirSupport airSupport = new();
            CreateItem(airSupport);
        });
    }

    private static string GetText(AirSupport airSupport) => $"兵种:{airSupport.兵种}  弹药:{airSupport.弹药}  回合:{airSupport.回合}";

    public void Set()
    {
        TreeItem treeItem = GetSelected();
        var airSupport = (AirSupport)treeItem.GetMetadata(0);
        Game.Instance.BtlObjWindow.CreateEdit(airSupport, a =>
        {
            CreateItem(a);
            TreeItem root = treeItem.GetParent();
            treeItem.Free();
            if (root.GetChildren().Count < 1)
                root.Free();
        });
    }

    public void Copy()
    {
        TreeItem treeItem = GetSelected();
        var airSupport = (AirSupport)treeItem.GetMetadata(0);
        Game.Instance.Dialog.Builder($"确定要复制这个空中支援？\n({GetText(airSupport)})", () =>
        {
            var newAirSupport = (AirSupport)airSupport.Clone();
            MapController.AirSupports.Add(newAirSupport);
            CreateItem(newAirSupport);
        });
    }
}