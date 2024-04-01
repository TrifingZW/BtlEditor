using Godot;

namespace BtlEditor.UserInterface;

public partial class ArmyPanel : PanelContainer
{
    private static readonly PackedScene TreeItemScene = (PackedScene)ResourceLoader.Load("res://UserInterface/ArmyPanel.tscn");
    public static ArmyPanel Instance => TreeItemScene.Instantiate<ArmyPanel>();
    public TextureRect GeneralRect => GetNode<TextureRect>("%TextureRect");
    public Label ArmyName => GetNode<Label>("%ArmyName");
    public VBoxContainer GeneralContainer => GetNode<VBoxContainer>("%GeneralContainer");
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