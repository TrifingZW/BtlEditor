using System.Linq;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Structures;
using Godot;
using static BtlEditor.CoreScripts.Globals;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.CoreScripts;

public static class GlobalHelper
{
    public static void DrawTerrain(Node2D node2D,LandUnit[] landUnits)
    {
        Terrains terrains = TerrainConfig.Terrains;

        foreach (LandUnit landUnit in landUnits)
        {
            Topography topography = landUnit.Topography;
            foreach (Terrain terrain in terrains.TerrainList)
                if (topography.装饰类型B == terrain.TerrainG)
                    foreach (Tile tile in terrain.Tiles)
                        if (topography.装饰BID == tile.Idx)
                            SetTexture(landUnit, new((sbyte)topography.装饰BX, (sbyte)topography.装饰BY), tile);
        }

        foreach (LandUnit landUnit in landUnits)
        {
            Topography topography = landUnit.Topography;
            foreach (Terrain terrain in terrains.TerrainList)
                if (topography.装饰类型A == terrain.TerrainG)
                    foreach (Tile tile in terrain.Tiles)
                        if (topography.装饰AID == tile.Idx)
                            SetTexture(landUnit, new((sbyte)topography.装饰AX, (sbyte)topography.装饰AY), tile);
        }

        foreach (LandUnit landUnit in landUnits)
        {
            Topography topography = landUnit.Topography;
            foreach (Terrain terrain in terrains.TerrainList)
                if (topography.地块类型 == terrain.TerrainG)
                    foreach (Tile tile in terrain.Tiles)
                        if (topography.地块ID == tile.Idx)
                            SetTexture(landUnit, new((sbyte)topography.地块X, (sbyte)topography.地块X), tile);
        }

        return;

        void SetTexture(LandUnit landUnit, Vector2 offset, Tile tile)
        {
            if (TerrainHd.Images.ImageList.FirstOrDefault(e => e.Name == tile.Image) is { } element)
            {
                Rect2 scrRect = new()
                {
                    Position = new(element.X, element.Y),
                    Size = new(element.Width, element.Height)
                };
                Rect2 rect = new()
                {
                    Position = (landUnit.Position - new Vector2(element.RefX, element.RefY) + offset) * RenderScaleValue,
                    Size = new Vector2(element.Width, element.Height) * RenderScaleValue
                };
                node2D.DrawTextureRectRegion(TerrainHd.Texture2D, rect, scrRect);
            }

            if (PlantHd.Images.ImageList.FirstOrDefault(e => e.Name == tile.Image) is { } element2)
            {
                Rect2 scrRect = new()
                {
                    Position = new(element2.X, element2.Y),
                    Size = new(element2.Width, element2.Height)
                };
                Rect2 rect = new()
                {
                    Position = (landUnit.Position - new Vector2(element2.RefX, element2.RefY) + offset) * RenderScaleValue,
                    Size = new Vector2(element2.Width, element2.Height) * RenderScaleValue
                };
                node2D.DrawTextureRectRegion(PlantHd.Texture2D, rect, scrRect);
            }
        }
    }
    public static int GetIndex(int x, int y, int width)
    {
        if (x >= 0 && x < width && y >= 0)
            return x + y * width;
        return -1;
    }

    public static int GetIndex(Vector2I vector2I, int width) => GetIndex(vector2I.X, vector2I.Y, width);

    public static int GetBtlIndex(BtlParser btl,BinParser bin,int index)
    {
        if (btl.IndependentTerrain) return index;
        var i = 0;
        for (var y = 0; y < bin.Height; y++)
        for (var x = 0; x < bin.Width; x++)
        {
            if (i == index) return GetIndex(new(x - btl.Master.地图截取x, y - btl.Master.地图截取y), btl.Master.地图宽);
            i++;
        }

        return -1;
    }
}