using BtlEditor.CoreScripts;
using BtlEditor.CoreScripts.Parser;
using Godot;

namespace BtlEditor.GameScreen.Scripts;

public static class MapHelper
{
    public static string BtlPath { get; set; }
    public static BtlParser Btl => Game.Instance.MapController.Btl;
    public static BinParser Bin => Game.Instance.MapController.Bin;
    public static int GetIndex(int x, int y) => GlobalHelper.GetIndex(x, y, Btl.Master.地图宽);

    public static int GetIndex(Vector2I vector2I) => GetIndex(vector2I.X, vector2I.Y);

    public static int GetBtlIndex(int index) => GlobalHelper.GetBtlIndex(Btl, Bin, index);
}