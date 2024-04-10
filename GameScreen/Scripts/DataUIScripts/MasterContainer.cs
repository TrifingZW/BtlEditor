using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.UserInterface;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.GameScreen.Scripts.DataUIScripts;

public partial class MasterContainer : VBoxContainer
{
    public override void _Ready()
    {
        var fields = typeof(Master).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (FieldInfo field in fields)
        {
            var editorItem = EditorItem.Instance;
            editorItem.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            Label label = new()
            {
                Text = Tr(field.Name),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };
            editorItem.Head.AddChild(label);
            SpinBox spinBox = Helpers.ReflectionSpinBox(Btl.Master, field);
            if (field.GetCustomAttribute<EditorGroup>() is { Ignore: true })
                spinBox.Editable = false;
            editorItem.Content.AddChild(spinBox);
            AddChild(editorItem);
        }
    }
}