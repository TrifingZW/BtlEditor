using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts;

public partial class Indicator : Node2D
{
    private Sprite2D _select1 = new();
    private Sprite2D _select2 = new();
    private Sprite2D _select3 = new();
    private Sprite2D _select4 = new();

    private int Timer { get; set; }

    public override void _Ready()
    {
        var texture1 = ResourceLoader.Load<Texture2D>("res://Assets/Textures/select_1.png");
        var texture2 = ResourceLoader.Load<Texture2D>("res://Assets/Textures/select_2.png");

        _select1.Texture = texture2;
        _select2.Texture = texture2;
        _select2.FlipH = true;

        _select3.Texture = texture1;
        _select4.Texture = texture1;
        _select4.FlipH = true;
        
        AddChild(_select1);
        AddChild(_select2);
        AddChild(_select3);
        AddChild(_select4);
    }   

    public override void _PhysicsProcess(double delta)
    {
        Timer++;
        var distanceInterpolation = Mathf.Sin(Timer / 10.0f);
        _select1.Position = new Vector2(-50f, -50f) + new Vector2(-6f, -6f) * distanceInterpolation;
        _select2.Position = new Vector2(50f, -50f) + new Vector2(6f, -6f) * distanceInterpolation;
        _select3.Position = new Vector2(-50f, 50f) + new Vector2(-6f, 6f) * distanceInterpolation;
        _select4.Position = new Vector2(50f, 50f) + new Vector2(6f, 6f) * distanceInterpolation;
    }
}