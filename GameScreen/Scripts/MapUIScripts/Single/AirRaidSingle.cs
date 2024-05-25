using System.Collections.Generic;
using BtlEditor.CoreScripts.Structures;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class AirRaidSingle : BaseSingle
{
    private List<AirRaid> AirRaids => GameLandUnit.AirRaids;

    protected override void UserInface()
    {
        Button add = CreateButton("添加空袭", () =>
        {
            AirRaid airRaid = new() { 坐标 = GameLandUnit.RegionIndex, };
            AirRaids.Add(airRaid);
            AddAirRaid(airRaid);
        });
        Container.AddChild(add);
        _gridContainer = new() { Columns = 2 };
        Container.AddChild(_gridContainer);

        foreach (AirRaid airRaid in AirRaids)
            AddAirRaid(airRaid);
    }

    private GridContainer _gridContainer;

    private void AddAirRaid(AirRaid airRaid)
    {
        PanelContainer panelContainer = new() { SizeFlagsHorizontal = SizeFlags.ExpandFill };
        panelContainer.MouseFilter = MouseFilterEnum.Pass;
        _gridContainer.AddChild(panelContainer);
        VBoxContainer vBoxContainer = new();
        vBoxContainer.Theme = ResourceLoader.Load<Theme>("res://editor_theme.tres");
        panelContainer.AddChild(vBoxContainer);
        Label label = new() { Text = $"空袭回合：{airRaid.回合}\n空袭兵种:{airRaid.兵种}" };
        vBoxContainer.AddChild(label);
        HBoxContainer hBoxContainer = new();
        vBoxContainer.AddChild(hBoxContainer);
        Button edit = new()
        {
            Text = "编辑",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        edit.Pressed += () =>
        {
            Game.Instance.BtlObjWindow.CreateEdit(airRaid, air =>
            {
                AirRaids[panelContainer.GetIndex()] = air;
                airRaid = air;
                GD.Print(panelContainer.GetIndex());
                label.Text = $"空袭回合：{air.回合}\n空袭兵种:{air.兵种}";
            });
        };
        hBoxContainer.AddChild(edit);
        Button delete = new()
        {
            Text = "删除",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        delete.Pressed += () =>
        {
            if (AirRaids.Remove(airRaid))
                panelContainer.QueueFree();
        };
        hBoxContainer.AddChild(delete);
    }
}