using System.Collections.Generic;
using Godot;
using Vector2 = Godot.Vector2;

namespace BtlEditor.CoreScripts.Utils;

public static class Helpers
{
    public static bool TryGetValue<T>(this T[] array, int index, out T value)
    {
        if (array != null && index >= 0 && index < array.Length)
        {
            value = array[index];
            return true;
        }

        value = default;
        return false;
    }

    public static bool TryGetValue<T>(this List<T> array, int index, out T value)
    {
        if (array != null && index >= 0 && index < array.Count)
        {
            value = array[index];
            return true;
        }

        value = default;
        return false;
    }

    public static Vector2I ToVector2I(this Vector2 vector2) => new(Mathf.CeilToInt(vector2.X), Mathf.CeilToInt(vector2.Y));
}