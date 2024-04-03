using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts;

public partial class GeneralSprite : Sprite2D
{
    private GeneralJson _generalJson;

    public GeneralJson GeneralJson
    {
        get => _generalJson;
        set
        {
            _generalJson = value;
            Update();
        }
    }

    private readonly Vector2 _positionOffset = new(0f, -75f);

    public new Vector2 Position
    {
        get => base.Position - _positionOffset;
        set => base.Position = value + _positionOffset;
    }

    private void Update()
    {
        foreach (Wc4ResourceElement element in Tacticalmap.Images.ImageList)
            if (element.Name == $"head_{GeneralJson.EName}.png")
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
        Vector2 zoom = MapController.Instance.Camera2D.Zoom;
        Scale = Vector2.One / zoom.ClampUV();
    }
}