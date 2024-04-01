using System.IO;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.UserInterface;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.Single;

public partial class ArmySingle : BaseSingle
{
    private ArmyPanel _armyPanel;

    protected override void Update()
    {
        Clear();

        switch (LandUnit.Army)
        {
            case null:
                Button addArmy = new();
                addArmy.Pressed += () =>
                {
                    if (Btl.Version1) LandUnit.Army = new Army1 { 坐标 = LandUnit.RegionIndex };
                    if (Btl.Version2 || Btl.Version3) LandUnit.Army = new Army3 { 坐标 = LandUnit.RegionIndex };
                    Update();
                };
                addArmy.Text = "添加军队";
                EndContainer.AddChild(addArmy);
                break;
            case Army1 army1:
                ReflexStruct(army1, TreeContainer, Save);
                break;
            case Army3 army3:
                ReflexStruct(army3, TreeContainer, Save);
                break;
        }

        if (LandUnit.Army is { } army)
        {
            EditorItem editorItem = EditorItem.Instance;
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
        }
    }

    private void Save()
    {
        LandUnit.UpdateArmy();
        UpdateArmyPanel();
    }

    private void UpdateArmyPanel()
    {
        Army army = LandUnit.Army;
        if (LandUnit.ArmyJson is { } armyJson)
        {
            _armyPanel.ArmyName.Text = Stringtable.ArmyName[armyJson.Id];

            //设置将领相关
            if (LandUnit.GeneralJson is { } generalJson)
            {
                //可见
                _armyPanel.GeneralContainer.Visible = true;

                //设置勋带
                if (army is Army3 army3)
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
                    _armyPanel.GeneralRect.Texture = texture;
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
            else _armyPanel.GeneralContainer.Visible = false;
        }
    }
}