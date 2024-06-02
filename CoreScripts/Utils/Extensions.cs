using System;
using Godot;

namespace BtlEditor.CoreScripts.Utils;

public static class Extensions
{
    public static Vector2 ClampUV(this Vector2 vector2) => vector2.Clamp(Vector2.Zero, Vector2.One);
    
    public static void Ergodic(this TreeItem source,Action<TreeItem> operation)
    {
        foreach (TreeItem treeItem in source.GetChildren())
        {
            operation(treeItem);
            treeItem.Ergodic(operation);
        }
    }
    
    public static void Screen(this TreeItem source,string value)
    {
        source.Ergodic(t =>
        {
            t.Visible = t.GetText(0).Contains(value, StringComparison.CurrentCultureIgnoreCase);
        });

        source.Ergodic(t =>
        {
            if (!t.Visible) return;
            TreeItem parent = t.GetParent();
            while (parent != null)
            {
                parent.Visible = true;
                parent = parent.GetParent();
            }
        });
    }
}