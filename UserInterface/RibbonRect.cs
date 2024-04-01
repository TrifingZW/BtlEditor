using BtlEditor.CoreScripts.Parser;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.UserInterface;

public partial class RibbonRect : TextureRect
{
    public void SetRibbon(int medalId)
    {
        Visible = false;
        foreach (Wc4ResourceElement element in ImageRibbonHd.Images.ImageList)
            if (element.Name == $"ribbon_{medalId}.png")
            {
                Visible = true;
                AtlasTexture texture = new();
                texture.Atlas = ImageRibbonHd.Texture2D;
                texture.Region = new()
                {
                    Position = new(element.X, element.Y),
                    Size = new(element.Width, element.Height)
                };
                Texture = texture;
            }
    }
}