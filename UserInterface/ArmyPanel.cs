using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.UserInterface;

public partial class ArmyPanel : VBoxContainer
{
    public static ArmyPanel Instance => ResourceLoader.Load<PackedScene>("res://UserInterface/ArmyPanel.tscn").Instantiate<ArmyPanel>();
    public Button GeneralButton => GetNode<Button>("%GeneralButton");
    public Button ArmyButton => GetNode<Button>("%ArmyButton");
    public Label GeneralName => GetNode<Label>("%GeneralName");
    public MedalRect MedalRect1 => GetNode<MedalRect>("%MedalRect1");
    public MedalRect MedalRect2 => GetNode<MedalRect>("%MedalRect2");
    public MedalRect MedalRect3 => GetNode<MedalRect>("%MedalRect3");
    public RibbonRect RibbonRect1 => GetNode<RibbonRect>("%RibbonRect1");
    public RibbonRect RibbonRect2 => GetNode<RibbonRect>("%RibbonRect2");
    public RibbonRect RibbonRect3 => GetNode<RibbonRect>("%RibbonRect3");
    public StarRect Star1 => GetNode<StarRect>("%Star1");
    public StarRect Star2 => GetNode<StarRect>("%Star2");
    public StarRect Star3 => GetNode<StarRect>("%Star3");
    public StarRect Star4 => GetNode<StarRect>("%Star4");
    public StarRect Star5 => GetNode<StarRect>("%Star5");
    public StarRect Star6 => GetNode<StarRect>("%Star6");
}