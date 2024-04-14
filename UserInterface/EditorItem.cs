using Godot;

namespace BtlEditor.UserInterface;

public partial class EditorItem : HBoxContainer
{
    public static EditorItem Instance => ResourceLoader.Load<PackedScene>("res://UserInterface/EditorItem.tscn").Instantiate<EditorItem>();

    public BoxContainer Head => GetNode<BoxContainer>("%Head");
    public BoxContainer Content => GetNode<BoxContainer>("%Content");
}   