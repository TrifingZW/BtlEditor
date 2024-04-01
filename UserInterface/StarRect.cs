using Godot;

namespace BtlEditor.UserInterface;

public partial class StarRect : TextureRect
{
    public const int Width = 50;
    public const int Height = 48;

    public void SetStar(int starCount)
    {
        var starName = starCount switch
        {
            1 => "general_star_1.png",
            2 => "general_star_1.png",
            3 => "general_star_2.png",
            4 => "general_star_2.png",
            5 => "general_star_3.png",
            6 => "general_star_4.png",
            _ => "general_star_1.png"
        };
        AtlasTexture atlasTexture = new();
        atlasTexture.Atlas = ResourceLoader.Load<Texture2D>($"res://Assets/Textures/{starName}");
        atlasTexture.Region = new()
        {
            Position = new(0, 0),
            Size = new(Width * starCount, Height)
        };
        Texture = atlasTexture;
    }
}