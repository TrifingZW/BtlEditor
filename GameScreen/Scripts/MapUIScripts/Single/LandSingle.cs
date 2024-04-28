using BtlEditor.UserInterface;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class LandSingle : BaseSingle
{
    protected override void Update()
    {
        var mainTreeBar = TreeBar.Instance;
        mainTreeBar.Title = "地块数据";
        TreeContainer.AddChild(mainTreeBar);
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
            Text = LandUnit.RegionIndex.ToString(),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        coordsItem.Content.AddChild(coords);

        var provinceBar = TreeBar.Instance;
        provinceBar.Title = "省规划";
        TreeContainer.AddChild(provinceBar);
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
        provinceSpinBox.Value = LandUnit.Province;
        provinceSpinBox.ValueChanged += value =>
        {
            LandUnit.Province = (short)value;
            LandUnit.UpdateProvinceColor();
            Game.Instance.MapController.UpdateColorUV();
        };
        provinceItem.Content.AddChild(provinceSpinBox);

        var belongBar = TreeBar.Instance;
        belongBar.Title = "地块归属";
        TreeContainer.AddChild(belongBar);
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
            Text = LandUnit.Belong.ToString(),
            FocusMode = FocusModeEnum.None,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        button.Pressed += () =>
        {
            Game.Instance.SearchCountryWindow.CreateEdit(country =>
            {
                button.Text = country.ToString();
                LandUnit.Belong = country;
                LandUnit.UpdateBelongColor();
                Game.Instance.MapController.UpdateColorUV();
            });
        };
        belongItem.Content.AddChild(button);

        Button provinceModeButton = CreateButton("省规划模式", () => { Game.Instance.StartProvinceMode(LandUnit.RegionIndex); });
        EndContainer.AddChild(provinceModeButton);
    }
}