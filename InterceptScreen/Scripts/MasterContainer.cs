using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using BtlEditor.CoreScripts.Structures;
using BtlEditor.CoreScripts.Utils;
using Godot;

namespace BtlEditor.InterceptScreen.Scripts;

public partial class MasterContainer : VBoxContainer
{
    public void Initialize(Master master)
    {
        var fields = typeof(Master).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (FieldInfo field in fields)
        {
            var editorItem = UserInterface.EditorItem.Instance;
            editorItem.SizeFlagsHorizontal = SizeFlags.ExpandFill;
            Label label = new()
            {
                Text = Tr(field.Name),
                SizeFlagsHorizontal = SizeFlags.ExpandFill
            };
            editorItem.Head.AddChild(label);
            SpinBox spinBox = Helpers.ReflectionSpinBox(master, field);
            if (field.GetCustomAttribute<EditorItem>() is { Ignore: true })
                spinBox.Editable = false;
            editorItem.Content.AddChild(spinBox);
            AddChild(editorItem);
        }
    }
}