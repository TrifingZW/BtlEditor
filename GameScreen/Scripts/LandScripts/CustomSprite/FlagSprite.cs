using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.LandScripts.CustomSprite;

public partial class FlagSprite : Sprite2D
{
    private int _flag;

    public int Flag
    {
        get => _flag;
        set
        {
            _flag = value;
            Update();
        }
    }
    
    private readonly Vector2 _positionOffset = new(10f, 30f);

    public new Vector2 Position
    {
        get => base.Position - _positionOffset;
        set => base.Position = value + _positionOffset;
    }

    private void Update()
    {
        foreach (Wc4ResourceElement element in Tacticalmap.Images.ImageList)
            if (element.Name == $"f_{Flag:D2}.png")
            {
                Rect2 rect = new()
                {
                    Position = new(element.X, element.Y),
                    Size = new(element.Width, element.Height)
                };
                RegionRect = rect;
                break;
            }
    }

    public override void _Ready()
    {
        Texture = Tacticalmap.Texture2D;
        RegionEnabled = true;
    }

    public override void _PhysicsProcess(double delta)
    {
        Vector2 zoom = MapController.Camera2D.Zoom * 1.35f;
        Scale = Vector2.One / zoom.ClampUV();
    }
}