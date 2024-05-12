using System.Linq;
using BtlEditor.CoreScripts.Parser;
using Godot;
using Godot.Collections;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.BinScreen;

public partial class TopographyContainer : FlowContainer
{
    private readonly Dictionary<int, Button> _barDirectory = [];

    public override void _Ready()
    {
        foreach (Terrain terrain in TerrainConfig.Terrains.TerrainList)
        foreach (Tile tile in terrain.Tiles)
        {
            Button button = new();
            if (TerrainHd.Images.ImageList.FirstOrDefault(i => i.Name == tile.Image) is { } terrainHd)
            {
                Rect2 rect2 = terrainHd.GetRect2();
                AtlasTexture atlasTexture = new();
                atlasTexture.Atlas = TerrainHd.Texture2D;
                atlasTexture.Region = rect2;

                Image scaledImage = atlasTexture.GetImage();
                scaledImage.Resize((int)rect2.Size.X / 3, (int)rect2.Size.Y / 3); // 将图像宽高缩小到一半
                var scaledTexture = ImageTexture.CreateFromImage(scaledImage);
                scaledImage.Dispose();

                button.Icon = scaledTexture;
            }
            else if (PlantHd.Images.ImageList.FirstOrDefault(i => i.Name == tile.Image) is { } plantHd)
            {
                Rect2 rect2 = plantHd.GetRect2();
                AtlasTexture atlasTexture = new();
                atlasTexture.Atlas = PlantHd.Texture2D;
                atlasTexture.Region = rect2;

                Image scaledImage = atlasTexture.GetImage();
                scaledImage.Resize((int)rect2.Size.X / 2, (int)rect2.Size.Y / 2); // 将图像宽高缩小到一半
                var scaledTexture = ImageTexture.CreateFromImage(scaledImage);
                scaledImage.Dispose();

                button.Icon = scaledTexture;
            }

            AddChild(button);
            _barDirectory[tile.Idx] = button;
        }
    }
}