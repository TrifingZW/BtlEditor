using System.Linq;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public partial class CountryItemList : BaseItemList
{
    public override void _Ready()
    {
        ItemSelected += ItemSelect;
        ItemActivated += _ => Set();
        for (var index = 0; index < Btl.Countries.Length; index++)
        {
            Country country = Btl.Countries[index];
            country.序号 = index;
            SetItem(AddItem("无数据"), country);
        }
    }

    private void SetItem(int index, Country country)
    {
        SetItemText(index, $"序号:{country.序号}  {Stringtable.CountryName[country.国家]}");
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
            SetItemIcon(index, Tacticalmap.Texture2D);
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

    public override void Delete()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Countries.TryGetValue(index, out Country country)) return;

        Game.Dialog.Builder($"确定要删除这个国家?\n(归属:{index} {Stringtable.CountryName[country.国家]})", () =>
        {
            MapController.Countries.RemoveAt(index);
            RemoveItem(index);
            for (var i = 0; i < MapController.Countries.Count; i++)
            {
                Country c = MapController.Countries[i];
                c.序号 = i;
                SetItem(i, c);
            }

            foreach (LandUnit landUnit in MapController.LandUnits)
            {
                if (landUnit.Belong == 0xff) continue;

                if (landUnit.Belong == index)
                {
                    landUnit.Belong = 0xff;
                    landUnit.UpdateBelongColor();
                }

                if (landUnit.Belong > index)
                    landUnit.Belong--;
            }

            MapController.UpdateShader();
        });
    }

    public override void Add()
    {
        Game.Dialog.Builder("确定要新建国家？", () =>
        {
            Country country = new()
            {
                序号 = ItemCount,
                empty4 = 903,
                科技等级 = 1
            };
            MapController.Countries.Add(country);
            SetItem(AddItem(""), country);
        });
    }

    public override void Set()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Countries.TryGetValue(index, out Country country)) return;

        Game.BtlObjWindow.CreateEdit(country, c =>
        {
            if (country.国家 != c.国家)
            {
                foreach (LandUnit landUnit in MapController.LandUnits)
                    if (landUnit.Belong == index)
                        if (landUnit.FlagSprite is { } flagSprite)
                            flagSprite.Flag = c.国家;
                SetItem(index, c);
            }

            MapController.Countries[index] = c;
        });
    }

    public override void Copy()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Countries.TryGetValue(index, out Country country)) return;

        Game.Dialog.Builder($"确定要复制这个国家？\n(归属:{index} {Stringtable.CountryName[country.国家]})", () =>
        {
            var newCountry = (Country)country.Clone();
            newCountry.序号 = ItemCount;
            MapController.Countries.Add(newCountry);
            SetItem(AddItem(""), newCountry);
        });
    }
}