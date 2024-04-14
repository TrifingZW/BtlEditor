using Godot;

namespace BtlEditor.UserInterface;

public partial class TreeBar : VBoxContainer
{
    public static TreeBar Instance => ResourceLoader.Load<PackedScene>("res://UserInterface/TreeBar.tscn").Instantiate<TreeBar>();
    public Button Bar => GetNode<Button>("Button");
    public PanelContainer Context => GetNode<PanelContainer>("Panel");
    public VBoxContainer Layout => GetNode<VBoxContainer>("Panel/MarginContainer/Layout");

    public bool Expanded
    {
        get => GetMeta("Expanded").AsBool();
        set => SetMeta("Expanded", value);
    }

    public string Title
    {
        get => Bar.Text;
        set => Bar.Text = value;
    }

    private static readonly Texture2D[] BarIcons =
    [
        ResourceLoader.Load<Texture2D>("res://Assets/Textures/UI/down.png"),
        ResourceLoader.Load<Texture2D>("res://Assets/Textures/UI/right.png")
    ];

    private Texture2D BarIcon => Expanded ? BarIcons[0] : BarIcons[1];

    public override void _Ready()
    {
        Update();
        Bar.Pressed += () =>
        {
            Expanded = !Expanded;
            Update();
        };
    }

    private void Update()
    {
        Context.Visible = Expanded;
        Bar.Icon = BarIcon;
    }

    public override void _Process(double delta)
    {
    }
}