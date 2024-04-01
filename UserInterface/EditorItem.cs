using Godot;

namespace BtlEditor.UserInterface;

public partial class EditorItem : HBoxContainer
{
    private static readonly PackedScene TreeItemScene = (PackedScene)ResourceLoader.Load("res://UserInterface/EditorItem.tscn");
    public static EditorItem Instance => TreeItemScene.Instantiate<EditorItem>();

    public BoxContainer Head => GetNode<BoxContainer>("%Head");
    public BoxContainer Content => GetNode<BoxContainer>("%Content");
}   