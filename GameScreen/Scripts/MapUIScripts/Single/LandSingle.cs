using BtlEditor.UserInterface;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class LandSingle : BaseSingle
{
    protected override void UserInface()
    {
        var mainTreeBar = TreeBar.Instance;
        mainTreeBar.Title = "地块数据";
        Container.AddChild(mainTreeBar);
        var coordsItem = EditorItem.Instance;
        mainTreeBar.Layout.AddChild(coordsItem);
        Label coordsLabel = new()
        {
            Text = Tr("地块坐标"),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        coordsItem.Head.AddChild(coordsLabel);
        Label coords = new()
        {
            Text = GameLandUnit.RegionIndex.ToString(),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        coordsItem.Content.AddChild(coords);

        var provinceBar = TreeBar.Instance;
        provinceBar.Title = "省规划";
        Container.AddChild(provinceBar);
        var provinceItem = EditorItem.Instance;
        provinceBar.Layout.AddChild(provinceItem);
        Label provinceLabel = new()
        {
            Text = Tr("目标地块的坐标"),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        provinceItem.Head.AddChild(provinceLabel);
        SpinBox provinceSpinBox = new();
        provinceSpinBox.UpdateOnTextChanged = true;
        provinceSpinBox.MinValue = short.MinValue;
        provinceSpinBox.MaxValue = short.MaxValue;
        provinceSpinBox.Value = GameLandUnit.Province;
        provinceSpinBox.ValueChanged += value =>
        {
            GameLandUnit.Province = (short)value;
            GameLandUnit.UpdateProvinceColor();
            Game.Instance.MapController.UpdateColorUV();
        };
        provinceItem.Content.AddChild(provinceSpinBox);

        var belongBar = TreeBar.Instance;
        belongBar.Title = "地块归属";
        Container.AddChild(belongBar);
        var belongItem = EditorItem.Instance;
        belongBar.Layout.AddChild(belongItem);
        Label belongLabel = new()
        {
            Text = Tr("目标国家的序号"),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        belongItem.Head.AddChild(belongLabel);
        Button button = new()
        {
            Text = GameLandUnit.Belong.ToString(),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        button.Pressed += () =>
        {
            Game.Instance.SearchCountryWindow.CreateEdit(country =>
            {
                button.Text = country.ToString();
                GameLandUnit.Belong = country;
                GameLandUnit.UpdateBelongColor();
                Game.Instance.MapController.UpdateColorUV();
            });
        };
        belongItem.Content.AddChild(button);

        Button provinceModeButton = CreateButton("省规划模式", () => { Game.Instance.ProvinceMode = true; });
        Container.AddChild(provinceModeButton);
    }
}