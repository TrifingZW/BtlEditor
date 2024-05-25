using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.UserInterface;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;
using static BtlEditor.GameScreen.Scripts.MapHelper;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public partial class ArmySingle : BaseSingle
{
    private ArmyPanel _armyPanel;

    protected override void UserInface()
    {
        //军队面板
        if (GameLandUnit.Army is not null)
        {
            var armyBar = TreeBar.Instance;
            armyBar.Title = "军队面板";
            _armyPanel = ArmyPanel.Instance;
            armyBar.Layout.AddChild(_armyPanel);
            Container.AddChild(armyBar);
            UpdateArmyPanel();
            //选择军队
            _armyPanel.ArmyButton.Pressed += () =>
            {
                Game.Instance.SearchArmyWindow.CreateEdit(armyJson =>
                {
                    GameLandUnit.Army.兵种 = (byte)armyJson.Army;
                    GameLandUnit.UpdateArmy();
                    Update();
                });
            };
            //选择将领
            _armyPanel.GeneralButton.Pressed += () =>
            {
                Game.Instance.SearchGeneralWindow.CreateEdit(general =>
                {
                    GameLandUnit.Army.将领 = (short)general.Id;
                    GameLandUnit.Army.军衔 = (byte)general.MilitaryRank;
                    GameLandUnit.Army.爵位 = (byte)general.Hp;

                    if (general.Skills.TryGetValue(0, out var skill1))
                        GameLandUnit.Army.技能等级1 = (byte)(skill1 % 10);
                    if (general.Skills.TryGetValue(1, out var skill2))
                        GameLandUnit.Army.技能等级2 = (byte)(skill2 % 10);
                    if (general.Skills.TryGetValue(2, out var skill3))
                        GameLandUnit.Army.技能等级3 = (byte)(skill3 % 10);
                    if (general.Skills.TryGetValue(3, out var skill4))
                        GameLandUnit.Army.技能等级4 = (byte)(skill4 % 10);
                    if (general.Skills.TryGetValue(4, out var skill5))
                        GameLandUnit.Army.技能等级5 = (byte)(skill5 % 10);

                    GameLandUnit.UpdateArmy();
                    Update();
                });
            };
        }

        //编辑面板
        switch (GameLandUnit.Army)
        {
            case null:

                Button paste = CreateButton("粘贴军队", () =>
                {
                    if (Game.ArmyCopy is null) return;
                    GameLandUnit.Army = (Army)Game.ArmyCopy?.Clone();
                    if (GameLandUnit.Belong == 0xff)
                        GameLandUnit.Belong = Game.BelongCopy;
                    Update();
                });
                Container.AddChild(paste);


                Button add = CreateButton("添加军队", () =>
                {
                    if (Btl.Version1) GameLandUnit.Army = new Army1 { 坐标 = GameLandUnit.RegionIndex };
                    if (Btl.Version2 || Btl.Version3) GameLandUnit.Army = new Army2 { 坐标 = GameLandUnit.RegionIndex };
                    GameLandUnit.Belong = GameLandUnit.ProvinceBelong;
                    Update();
                });
                Container.AddChild(add);
                return;

            case Army1 army1:
                ReflexStruct(army1, Container, UpdateArmy);
                break;
            case Army2 army3:
                ReflexStruct(army3, Container, UpdateArmy);
                break;
        }

        Button copy = CreateButton("复制军队", () =>
        {
            Game.ArmyCopy = (Army)GameLandUnit.Army.Clone();
            Game.BelongCopy = GameLandUnit.Belong;
        });
        Container.AddChild(copy);

        Button delete = CreateButton("删除军队", () =>
        {
            GameLandUnit.Army = null;
            Update();
        });
        Container.AddChild(delete);
    }

    private void UpdateArmy()
    {
        GameLandUnit.UpdateArmy();
        UpdateArmyPanel();
    }

    private void UpdateArmyPanel()
    {
        //设置军队名称
        if (Stringtable.ArmyName[GameLandUnit.ArmyJson.Id] is { } armyName)
            _armyPanel.ArmyButton.Text = armyName;
        else _armyPanel.ArmyButton.Text = "未知兵种";

        //设置将领相关
        if (GameLandUnit.GeneralJson is not { } generalJson) return;

        Army army = GameLandUnit.Army;

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
        var path = Helpers.GetValidImagePath($"{ImageHeadPath}/general_circle_{generalJson.Photo}");
        if (path is not null)
        {
            Image image = Image.LoadFromFile(path);
            ImageTexture texture = new();
            texture.SetImage(image);
            _armyPanel.GeneralButton.Icon = texture;
        }

        //设置名称
        if (generalJson.EName is { } eName)
        {
            if (Stringtable.GeneralName[eName] is not { } name)
                name = eName;
            _armyPanel.GeneralName.Text = name;
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