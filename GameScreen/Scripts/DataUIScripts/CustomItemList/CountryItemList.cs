using System.Linq;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;
using static BtlEditor.GameScreen.Scripts.MapHelper;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts.CustomItemList;

public partial class CountryItemList : ItemList, IDataOperation
{
    private static MapController MapController => Game.Instance.MapController;

    public override void _Ready()
    {
        base._Ready();
        ItemSelected += ItemSelect;
        ItemActivated += _ => Set();
        foreach (Country country in Btl.Countries)
            SetItem(AddItem("无数据"), country);
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
        foreach (GameLandUnit landUnit in MapController.LandUnits)
            if (landUnit.Capital != null)
                if (landUnit.Belong == index)
                {
                    Game.Instance.CameraController.TargetPosition = landUnit.Position;
                    break;
                }
    }

    public void Delete()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Countries.TryGetValue(index, out Country country)) return;

        Game.Instance.Dialog.Builder($"确定要删除这个国家?\n(归属:{index} {Stringtable.CountryName[country.国家]})", () =>
        {
            MapController.Countries.RemoveAt(index);
            RemoveItem(index);
            for (var i = 0; i < MapController.Countries.Count; i++)
            {
                Country c = MapController.Countries[i];
                c.序号 = i;
                SetItem(i, c);
            }

            foreach (GameLandUnit landUnit in MapController.LandUnits)
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

            MapController.UpdateColorUV();
        });
    }

    public void Add()
    {
        Game.Instance.Dialog.Builder("确定要新建国家？", () =>
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

    public void Set()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Countries.TryGetValue(index, out Country country)) return;

        Game.Instance.BtlObjWindow.CreateEdit(country, c =>
        {
            if (country.国家 != c.国家)
            {
                foreach (GameLandUnit landUnit in MapController.LandUnits)
                    if (landUnit.Belong == index)
                        if (landUnit.FlagSprite is { } flagSprite)
                            flagSprite.Flag = c.国家;
                SetItem(index, c);
            }

            if (country.R8 != c.R8 || country.G8 != c.G8 || country.B8 != c.B8)
                MapController.UpdateCountryColor(index, Color.Color8(c.R8, c.G8, c.B8));

            MapController.Countries[index] = c;
        }, newCountry =>
        {
            ColorPickerButton colorPickerButton = new();
            colorPickerButton.FocusMode = FocusModeEnum.None;
            colorPickerButton.CustomMinimumSize = new(0, 60);
            colorPickerButton.Color = Color.Color8(country.R8, country.G8, country.B8);
            colorPickerButton.ColorChanged += color =>
            {
                newCountry.R8 = (byte)color.R8;
                newCountry.G8 = (byte)color.G8;
                newCountry.B8 = (byte)color.B8;
            };
            return colorPickerButton;
        });
    }

    public void Copy()
    {
        if (!GetSelectedItems().TryGetValue(0, out var index)) return;
        if (!MapController.Countries.TryGetValue(index, out Country country)) return;

        Game.Instance.Dialog.Builder($"确定要复制这个国家？\n(归属:{index} {Stringtable.CountryName[country.国家]})", () =>
        {
            var newCountry = (Country)country.Clone();
            newCountry.序号 = ItemCount;
            MapController.Countries.Add(newCountry);
            SetItem(AddItem(""), newCountry);
        });
    }
}