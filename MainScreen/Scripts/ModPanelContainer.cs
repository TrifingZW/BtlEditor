using Godot;

namespace BtlEditor.MainScreen.Scripts;

public partial class ModPanelContainer : PanelContainer
{
    private RichTextLabel _richTextLabel;
    private Label _label;

    public override void _Ready()
    {
        _richTextLabel = GetNode<RichTextLabel>("%RichTextLabel2");
        _label = GetNode<Label>("%Label");
        _richTextLabel.MetaClicked += url => { OS.ShellOpen((string)url); };
        _richTextLabel.GetVScrollBar().Scale = Vector2.Zero;
    }

    private void Close() => Visible = false;

    public void Load(string name)
    {
        _label.Text = name;
        _richTextLabel.Clear();
        _richTextLabel.ParseBbcode(FileAccess.GetFileAsString("res://Assets/Mods/" + name + ".bbcode"));
        Visible = true;
    }
}