using System.Collections.Generic;
using System.Linq;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.UserInterface;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;
using static BtlEditor.GameScreen.Scripts.MapHelper;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class ReinforcementSingle : BaseSingle
{
    private List<Reinforcement> Reinforcements => GameLandUnit.Reinforcements;

    protected override void UserInface()
    {
        Button add = new() { Text = "添加爆兵" };
        add.AddThemeFontSizeOverride("font_size", 50);
        add.Pressed += () =>
        {
            Reinforcement reinforcement = null;
            if (Btl.Version1 || Btl.Version2)
            {
                reinforcement = new Reinforcement1();
                ((Reinforcement1)reinforcement).所属国家 = GameLandUnit.ProvinceBelong;
                ((Reinforcement1)reinforcement).爆兵回合 = 1;
            }

            if (Btl.Version3)
            {
                reinforcement = new Reinforcement3();
                ((Reinforcement3)reinforcement).所属国家 = GameLandUnit.ProvinceBelong;
                ((Reinforcement3)reinforcement).爆兵回合 = 1;
            }

            if (reinforcement != null)
            {
                reinforcement.兵种 = 1;
                reinforcement.等级 = 1;
                reinforcement.编制 = 1;
                reinforcement.坐标 = GameLandUnit.RegionIndex;
                Reinforcements.Add(reinforcement);
            }

            AddReinforcement(reinforcement);
        };
        Container.AddChild(add);

        Button paste = new() { Text = "粘贴爆兵" };
        paste.AddThemeFontSizeOverride("font_size", 50);
        paste.Pressed += () =>
        {
            switch (Game.ReinforcementCopy?.Clone())
            {
                case Reinforcement1 reinforcement1:
                    reinforcement1.坐标 = GameLandUnit.RegionIndex;
                    reinforcement1.所属国家 = GameLandUnit.Belong;
                    GameLandUnit.Reinforcements.Add(reinforcement1);
                    AddReinforcement(reinforcement1);
                    break;
                case Reinforcement3 reinforcement3:
                    reinforcement3.坐标 = GameLandUnit.RegionIndex;
                    reinforcement3.所属国家 = GameLandUnit.Belong;
                    GameLandUnit.Reinforcements.Add(reinforcement3);
                    AddReinforcement(reinforcement3);
                    break;
            }
        };
        Container.AddChild(paste);
        
        _vBoxContainer = new();
        Container.AddChild(_vBoxContainer);
        foreach (Reinforcement reinforcement in GameLandUnit.Reinforcements)
            AddReinforcement(reinforcement);
    }

    private VBoxContainer _vBoxContainer;

    private void AddReinforcement(Reinforcement reinforcement)
    {
        var treeBar = TreeBar.Instance;
        UpdateTreeBar(reinforcement, treeBar);

        _vBoxContainer.AddChild(treeBar);
        var armyPanel = ArmyPanel.Instance;
        treeBar.Layout.AddChild(armyPanel);
        UpdateArmyPanel(reinforcement, armyPanel);
        //选择军队
        armyPanel.ArmyButton.Pressed += () =>
        {
            Game.Instance.SearchArmyWindow.CreateEdit(armyJson =>
            {
                reinforcement.兵种 = (byte)armyJson.Army;
                UpdateArmyPanel(reinforcement, armyPanel);
            });
        };
        //选择将领
        armyPanel.GeneralButton.Pressed += () =>
        {
            Game.Instance.SearchGeneralWindow.CreateEdit(general =>
            {
                reinforcement.将领 = (short)general.Id;
                reinforcement.军衔 = (byte)general.MilitaryRank;
                reinforcement.爵位 = (byte)general.Hp;


                if (general.Skills.TryGetValue(0, out var skill1))
                    reinforcement.技能等级1 = (byte)(skill1 % 10);
                if (general.Skills.TryGetValue(1, out var skill2))
                    reinforcement.技能等级2 = (byte)(skill2 % 10);
                if (general.Skills.TryGetValue(2, out var skill3))
                    reinforcement.技能等级3 = (byte)(skill3 % 10);
                if (general.Skills.TryGetValue(3, out var skill4))
                    reinforcement.技能等级4 = (byte)(skill4 % 10);
                if (general.Skills.TryGetValue(4, out var skill5))
                    reinforcement.技能等级5 = (byte)(skill5 % 10);

                UpdateArmyPanel(reinforcement, armyPanel);
            });
        };

        HBoxContainer buttonContainer = new();
        treeBar.Layout.AddChild(buttonContainer);

        Button set = new()
        {
            Text = "编辑爆兵",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        set.Pressed += () =>
        {
            switch (reinforcement)
            {
                case Reinforcement3 reinforcement3:
                    Game.Instance.BtlObjWindow.CreateEdit(reinforcement3, r3 =>
                    {
                        Reinforcements[treeBar.GetIndex()] = r3;
                        reinforcement = r3;
                        UpdateTreeBar(r3, treeBar);
                        UpdateArmyPanel(r3, armyPanel);
                    });
                    break;

                case Reinforcement1 reinforcement1:
                    Game.Instance.BtlObjWindow.CreateEdit(reinforcement1, r1 =>
                    {
                        Reinforcements[treeBar.GetIndex()] = r1;
                        reinforcement = r1;
                        UpdateTreeBar(r1, treeBar);
                        UpdateArmyPanel(r1, armyPanel);
                    });
                    break;
            }
        };
        buttonContainer.AddChild(set);

        Button copy = new()
        {
            Text = "复制爆兵",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        copy.Pressed += () => { Game.ReinforcementCopy = reinforcement; };
        buttonContainer.AddChild(copy);

        Button delete = new()
        {
            Text = "删除爆兵",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        delete.Pressed += () =>
        {
            if (GameLandUnit.Reinforcements.Remove(reinforcement))
                treeBar.QueueFree();
        };
        buttonContainer.AddChild(delete);
    }

    private static void UpdateTreeBar(Reinforcement reinforcement, TreeBar treeBar)
    {
        treeBar.Title = reinforcement switch
        {
            Reinforcement1 reinforcement1 => $"爆兵回合：{reinforcement1.爆兵回合}",
            Reinforcement3 reinforcement3 => $"爆兵回合：{reinforcement3.爆兵回合}",
            _ => treeBar.Title
        };
    }

    private static void UpdateArmyPanel(Reinforcement reinforcement, ArmyPanel armyPanel)
    {
        armyPanel.ArmyButton.Text = "未知兵种";
        foreach (ArmyJson armyJson in ArmySettings.ArmyJsons)
            if (armyJson.Army == reinforcement.兵种)
            {
                armyPanel.ArmyButton.Text = armyJson.Name;
                break;
            }

        if (GeneralSettings.GeneralJsons.FirstOrDefault(g => g.Id == reinforcement.将领 && g.Name != null) is not { } generalJson) return;

        //设置勋带和勋章
        switch (reinforcement)
        {
            case Reinforcement3 reinforcement3:
                armyPanel.RibbonRect1.SetRibbon(reinforcement3.勋带1);
                armyPanel.RibbonRect2.SetRibbon(reinforcement3.勋带2);
                armyPanel.RibbonRect3.SetRibbon(reinforcement3.勋带3);

                armyPanel.MedalRect1.SetMedal(reinforcement3.胸章1);
                armyPanel.MedalRect2.SetMedal(reinforcement3.胸章2);
                armyPanel.MedalRect3.SetMedal(reinforcement3.胸章3);
                break;
            case Reinforcement1 reinforcement1:
                armyPanel.MedalRect1.SetMedal(reinforcement1.胸章一);
                armyPanel.MedalRect2.SetMedal(reinforcement1.胸章二);
                armyPanel.MedalRect3.SetMedal(reinforcement1.胸章三);
                break;
        }


        //设置头像
        var path = Helpers.GetValidImagePath($"{ImageHeadPath}/general_circle_{generalJson.Photo}");
        if (path is not null)
        {
            Image image = Image.LoadFromFile(path);
            ImageTexture texture = new();
            texture.SetImage(image);
            armyPanel.GeneralButton.Icon = texture;
        }

        //设置名称
        if (generalJson.EName is { } eName)
        {
            if (Stringtable.GeneralName[eName] is not { } name)
                name = eName;
            armyPanel.GeneralName.Text = name;
        }

        //设置数值
        armyPanel.Star1.SetStar(generalJson.Infantry);
        armyPanel.Star2.SetStar(generalJson.Artillery);
        armyPanel.Star3.SetStar(generalJson.Armor);
        armyPanel.Star4.SetStar(generalJson.Navy);
        armyPanel.Star5.SetStar(generalJson.AirForce);
        armyPanel.Star6.SetStar(generalJson.March);
    }
}