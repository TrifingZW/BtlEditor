using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
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

    public static SpinBox ReflectionSpinBox<T>(T obj, FieldInfo field)
    {
        SpinBox spinBox = new();
        spinBox.UpdateOnTextChanged = true;
        switch (field.FieldType)
        {
            case { } type when type == typeof(byte):
                if (field.GetValue(obj) is byte b)
                {
                    spinBox.MinValue = byte.MinValue;
                    spinBox.MaxValue = byte.MaxValue;
                    spinBox.Value = b;
                    spinBox.ValueChanged += value => field.SetValue(obj, (byte)value);
                }

                break;
            case { } type when type == typeof(short):
                if (field.GetValue(obj) is short s)
                {
                    spinBox.MinValue = short.MinValue;
                    spinBox.MaxValue = short.MaxValue;
                    spinBox.Value = s;
                    spinBox.ValueChanged += value => field.SetValue(obj, (short)value);
                }

                break;
            case { } type when type == typeof(int):
                if (field.GetValue(obj) is int i)
                {
                    spinBox.MinValue = int.MinValue;
                    spinBox.MaxValue = int.MaxValue;
                    spinBox.Value = i;
                    spinBox.ValueChanged += value => field.SetValue(obj, (int)value);
                }

                break;
            case { } type when type == typeof(float):
                if (field.GetValue(obj) is float f)
                {
                    spinBox.Step = 0.01;
                    spinBox.Value = f;
                    spinBox.ValueChanged += value => field.SetValue(obj, (float)value);
                }

                break;
        }

        return spinBox;
    }

    public static string GetValidImagePath(string basePath)
    {
        var extensions = new[] { ".webp", ".png" }; // 支持的文件扩展名列表
        return extensions.Select(ext => $"{basePath}{ext}").FirstOrDefault(File.Exists);
    }
}