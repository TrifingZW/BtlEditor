using System.Collections.Generic;
using System.IO;
using System.Linq;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.UserInterface;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class ReinforcementSingle : BaseSingle
{
    private List<Reinforcement> Reinforcements => LandUnit.Reinforcements;

    protected override void Update()
    {
        for (var index = 0; index < LandUnit.Reinforcements.Count; index++)
            AddReinforcement(index);

        Button add = new() { Text = "添加爆兵数据" };
        add.AddThemeFontSizeOverride("font_size", 50);
        add.FocusMode = FocusModeEnum.None;
        add.Pressed += () =>
        {
            Reinforcement reinforcement = null;
            if (Btl.Version1 || Btl.Version2)
            {
                reinforcement = new Reinforcement1();
                ((Reinforcement1)reinforcement).所属国家 = LandUnit.Belong;
            }

            if (Btl.Version3)
            {
                reinforcement = new Reinforcement3();
                ((Reinforcement3)reinforcement).所属国家 = LandUnit.Belong;
            }

            if (reinforcement != null)
            {
                reinforcement.坐标 = LandUnit.RegionIndex;
                LandUnit.Reinforcements.Add(reinforcement);
            }

            AddReinforcement(Reinforcements.Count - 1);
        };
        EndContainer.AddChild(add);

        Button paste = new() { Text = "粘贴爆兵数据" };
        paste.AddThemeFontSizeOverride("font_size", 50);
        paste.FocusMode = FocusModeEnum.None;
        paste.Pressed += () =>
        {
            switch (Game.ReinforcementCopy?.Clone())
            {
                case Reinforcement1 reinforcement1:
                    reinforcement1.坐标 = LandUnit.RegionIndex;
                    reinforcement1.所属国家 = LandUnit.Belong;
                    LandUnit.Reinforcements.Add(reinforcement1);
                    AddReinforcement(Reinforcements.Count - 1);
                    break;
                case Reinforcement3 reinforcement3:
                    reinforcement3.坐标 = LandUnit.RegionIndex;
                    reinforcement3.所属国家 = LandUnit.Belong;
                    LandUnit.Reinforcements.Add(reinforcement3);
                    AddReinforcement(Reinforcements.Count - 1);
                    break;
            }
        };
        EndContainer.AddChild(paste);
    }

    private void AddReinforcement(int index)
    {
        Reinforcement reinforcement = LandUnit.Reinforcements[index];

        var treeBar = TreeBar.Instance;
        UpdateTreeBar(reinforcement, treeBar);

        HeadContainer.AddChild(treeBar);
        var armyPanel = ArmyPanel.Instance;
        treeBar.Layout.AddChild(armyPanel);
        UpdateArmyPanel(reinforcement, armyPanel);
        //选择军队
        armyPanel.ArmyButton.Pressed += () =>
        {
            Game.SearchArmyWindow.CreateEdit(armyJson =>
            {
                reinforcement.兵种 = (byte)armyJson.Army;
                UpdateArmyPanel(reinforcement, armyPanel);
            });
        };
        //选择将领
        armyPanel.GeneralButton.Pressed += () =>
        {
            Game.SearchGeneralWindow.CreateEdit(general =>
            {
                reinforcement.将领 = (short)general.Id;
                reinforcement.军衔 = (byte)general.Hp;

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
            Text = "编辑爆兵数据",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        set.FocusMode = FocusModeEnum.None;
        set.Pressed += () =>
        {
            switch (reinforcement)
            {
                case Reinforcement3 reinforcement3:
                    Game.BtlObjWindow.CreateEdit(reinforcement3, r3 =>
                    {
                        Reinforcements[index] = r3;
                        reinforcement = r3;
                        UpdateTreeBar(reinforcement, treeBar);
                        UpdateArmyPanel(reinforcement, armyPanel);
                    });
                    break;

                case Reinforcement1 reinforcement1:
                    Game.BtlObjWindow.CreateEdit(reinforcement1, r1 =>
                    {
                        Reinforcements[index] = r1;
                        reinforcement = r1;
                        UpdateTreeBar(reinforcement, treeBar);
                        UpdateArmyPanel(reinforcement, armyPanel);
                    });
                    break;
            }
        };
        buttonContainer.AddChild(set);

        Button copy = new()
        {
            Text = "复制爆兵数据",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        copy.FocusMode = FocusModeEnum.None;
        copy.Pressed += () => { Game.ReinforcementCopy = reinforcement; };
        buttonContainer.AddChild(copy);

        Button delete = new()
        {
            Text = "删除爆兵数据",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };
        delete.FocusMode = FocusModeEnum.None;
        delete.Pressed += () =>
        {
            LandUnit.Reinforcements.Remove(reinforcement);
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
                var title = Stringtable.ArmyName[armyJson.Id];
                armyPanel.ArmyButton.Text = title;
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
        var path = $"{ImageHeadPath}/general_circle_{generalJson.Photo}.webp";
        if (File.Exists(path))
        {
            Image image = Image.LoadFromFile(path);
            ImageTexture texture = new();
            texture.SetImage(image);
            armyPanel.GeneralButton.Icon = texture;
            armyPanel.GeneralName.Text = Stringtable.GeneralName[generalJson.EName];
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