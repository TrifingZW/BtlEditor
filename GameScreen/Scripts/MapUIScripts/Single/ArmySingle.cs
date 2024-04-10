using System.IO;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.UserInterface;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class ArmySingle : BaseSingle
{
    private ArmyPanel _armyPanel;

    protected override void Update()
    {
        switch (LandUnit.Army)
        {
            case null:
                Button addArmy = new();
                addArmy.Pressed += () =>
                {
                    if (Btl.Version1) LandUnit.Army = new Army1 { 坐标 = LandUnit.RegionIndex };
                    if (Btl.Version2 || Btl.Version3) LandUnit.Army = new Army2 { 坐标 = LandUnit.RegionIndex };
                    Clear();
                    Update();
                };
                addArmy.Text = "添加军队";
                EndContainer.AddChild(addArmy);
                break;
            case Army1 army1:
                ReflexStruct(army1, TreeContainer, UpdateArmy);
                break;
            case Army2 army3:
                ReflexStruct(army3, TreeContainer, UpdateArmy);
                break;
        }

        if (LandUnit.Army is not { } army) return;
        var editorItem = EditorItem.Instance;
        editorItem.Head.AddChild(new Label { Text = "方向" });
        MainTreeBar.Layout.AddChild(editorItem);
        OptionButton optionButton = new();
        optionButton.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        optionButton.AddItem("左");
        optionButton.AddItem("右");
        optionButton.Selected = army.方向;
        optionButton.ItemSelected += index =>
        {
            army.方向 = (byte)index;
            LandUnit.UpdateArmy();
        };
        editorItem.AddChild(optionButton);

        _armyPanel = ArmyPanel.Instance;
        HeadContainer.AddChild(_armyPanel);
        UpdateArmyPanel();
        //选择军队
        _armyPanel.ArmyButton.Pressed += () =>
        {
            Game.SearchArmyWindow.CreateEdit(armyJson =>
            {
                LandUnit.Army.兵种 = (byte)armyJson.Army;
                LandUnit.UpdateArmy();
                Clear();
                Update();
            });
        };
        //选择将领
        _armyPanel.GeneralButton.Pressed += () =>
        {
            Game.SearchGeneralWindow.CreateEdit(general =>
            {
                LandUnit.Army.将领 = (short)general.Id;
                LandUnit.Army.军衔 = (byte)general.Hp;

                if (general.Skills.TryGetValue(0, out var skill1))
                    LandUnit.Army.技能等级1 = (byte)(skill1 % 10);
                if (general.Skills.TryGetValue(1, out var skill2))
                    LandUnit.Army.技能等级2 = (byte)(skill2 % 10);
                if (general.Skills.TryGetValue(2, out var skill3))
                    LandUnit.Army.技能等级3 = (byte)(skill3 % 10);
                if (general.Skills.TryGetValue(3, out var skill4))
                    LandUnit.Army.技能等级4 = (byte)(skill4 % 10);
                if (general.Skills.TryGetValue(4, out var skill5))
                    LandUnit.Army.技能等级5 = (byte)(skill5 % 10);

                LandUnit.UpdateArmy();
                Clear();
                Update();
            });
        };

        Button delete = new();
        delete.Pressed += () =>
        {
            LandUnit.Army = null;
            Clear();
            Update();
        };
        delete.Text = "删除军队";
        EndContainer.AddChild(delete);
    }

    private void UpdateArmy()
    {
        LandUnit.UpdateArmy();
        UpdateArmyPanel();
    }

    private void UpdateArmyPanel()
    {
        //设置军队名称
        if (LandUnit.ArmyJson is { } armyJson)
            _armyPanel.ArmyButton.Text = Stringtable.ArmyName[armyJson.Id];
        else _armyPanel.ArmyButton.Text = "未知兵种";

        //设置将领相关
        if (LandUnit.GeneralJson is not { } generalJson) return;
        
        Army army = LandUnit.Army;

        //设置勋带
        if (army is Army2 army3)
        {
            _armyPanel.RibbonRect1.SetRibbon(army3.勋带1);
            _armyPanel.RibbonRect2.SetRibbon(army3.勋带2);
            _armyPanel.RibbonRect3.SetRibbon(army3.勋带3);
        }

        //设置勋章
        _armyPanel.MedalRect1.SetMedal(army.胸章一);
        _armyPanel.MedalRect2.SetMedal(army.胸章二);
        _armyPanel.MedalRect3.SetMedal(army.胸章三);

        //设置头像
        var path = $"{ImageHeadPath}/general_circle_{generalJson.Photo}.webp";
        if (File.Exists(path))
        {
            Image image = Image.LoadFromFile(path);
            ImageTexture texture = new();
            texture.SetImage(image);
            _armyPanel.GeneralButton.Icon = texture;
            _armyPanel.GeneralName.Text = Stringtable.GeneralName[generalJson.EName];
        }

        //设置数值
        _armyPanel.Star1.SetStar(generalJson.Infantry);
        _armyPanel.Star2.SetStar(generalJson.Artillery);
        _armyPanel.Star3.SetStar(generalJson.Armor);
        _armyPanel.Star4.SetStar(generalJson.Navy);
        _armyPanel.Star5.SetStar(generalJson.AirForce);
        _armyPanel.Star6.SetStar(generalJson.March);
    }
}