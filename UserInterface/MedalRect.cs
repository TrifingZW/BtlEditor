using BtlEditor.CoreScripts.Parser;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.UserInterface;

public partial class MedalRect : TextureRect
{
    public void SetMedal(int medalId)
    {
        Visible = false;
        foreach (Wc4ResourceElement element in ImageGeneralMedalHd.Images.ImageList)
            if (element.Name == $"general_medal_{medalId}.png")
            {
                Visible = true;
                AtlasTexture texture = new();
                texture.Atlas = ImageGeneralMedalHd.Texture2D;
                texture.Region = new()
                {
                    Position = new(element.X, element.Y),
                    Size = new(element.Width, element.Height)
                };
                Texture = texture;
            }
    }
}