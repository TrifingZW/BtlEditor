using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using BtlEditor.UserInterface;
using Godot;

namespace BtlEditor.GameScreen.Scripts.Windows;

public partial class BtlObjWindow : Window
{
    private VBoxContainer _vboxContainer;

    public override void _Ready()
    {
        CloseRequested += () => Visible = false;
        _vboxContainer = GetNode<VBoxContainer>("ScrollContainer/MarginContainer/VBoxContainer");
    }

    public void CreateEdit<T>(T obj)
    {
        foreach (Node child in _vboxContainer.GetChildren())
            child.QueueFree();
        var fields = typeof(T).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        foreach (FieldInfo field in fields)
        {
            if (field.GetCustomAttribute<EditorGroup>() is { Ignore: true }) continue;

            var editorItem = EditorItem.Instance;
            editorItem.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

            _vboxContainer.AddChild(editorItem);
            Label label = new()
            {
                Text = Tr(field.Name),
                SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
            };
            editorItem.Head.AddChild(label);
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
                        spinBox.MinValue = float.MinValue;
                        spinBox.MaxValue = float.MaxValue;
                        spinBox.Value = f;
                        spinBox.ValueChanged += value => field.SetValue(obj, (float)value);
                    }

                    break;
            }

            editorItem.Content.AddChild(spinBox);
        }

        Show();
    }
}