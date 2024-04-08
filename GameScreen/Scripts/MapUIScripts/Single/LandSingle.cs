using BtlEditor.UserInterface;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class LandSingle : BaseSingle
{
    protected override void Update()
    {
        Clear();

        short province = -1;
        byte belong = 255;

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
        provinceSpinBox.ValueChanged += value => province = (short)value;
        provinceItem.Content.AddChild(provinceSpinBox);
        Button updateProvinceButton = new();
        updateProvinceButton.Text = "保存并更新";
        updateProvinceButton.Pressed += () =>
        {
            LandUnit.Province = province;
            LandUnit.UpdateProvince();
            Game.Instance.MapController.UpdateShader();
        };
        provinceBar.Layout.AddChild(updateProvinceButton);

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
        SpinBox belongSpinBox = new();
        belongSpinBox.UpdateOnTextChanged = true;
        belongSpinBox.MinValue = byte.MinValue;
        belongSpinBox.MaxValue = byte.MaxValue;
        belongSpinBox.Value = LandUnit.Belong;
        belongSpinBox.ValueChanged += value => belong = (byte)value;
        belongItem.Content.AddChild(belongSpinBox);
        Button updateBelongButton = new();
        updateBelongButton.Text = "保存并更新";
        updateBelongButton.Pressed += () =>
        {
            LandUnit.Belong = belong;
            LandUnit.UpdateBelong();
            Game.Instance.MapController.UpdateShader();
        };
        belongBar.Layout.AddChild(updateBelongButton);
    }
}