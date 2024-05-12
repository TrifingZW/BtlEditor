using BtlEditor.CoreScripts;
using BtlEditor.CoreScripts.Parser;
using Godot;

namespace BtlEditor.InterceptScreen.Scripts;

public static class InterceptHelper
{
    public static string BtlPath { get; set; }
    public static BtlParser Btl => Intercept.Instance.Btl;
    public static BinParser Bin => Intercept.Instance.Bin;
    public static int GetIndex(int x, int y, int width) => GlobalHelper.GetIndex(x, y, width);

    public static int GetIndex(Vector2I vector2I, int width) => GetIndex(vector2I.X, vector2I.Y, width);

    public static int GetBtlIndex(int index) => GlobalHelper.GetBtlIndex(Btl, Bin, index);
}