using Godot;

namespace BtlEditor.CoreScripts.Utils;

public static class Extensions
{
    public static Vector2 ClampUV(this Vector2 vector2) => vector2.Clamp(Vector2.Zero, Vector2.One);
}