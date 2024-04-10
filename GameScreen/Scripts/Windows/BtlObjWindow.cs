using System;
using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.UserInterface;
using Godot;

namespace BtlEditor.GameScreen.Scripts.Windows;

public partial class BtlObjWindow : Window
{
    private VBoxContainer _windowContainer;
    private VBoxContainer _editorContainer;

    public override void _Ready()
    {
        CloseRequested += () => Visible = false;
        _windowContainer = GetNode<VBoxContainer>("MarginContainer/WindowContainer");
        _editorContainer = GetNode<VBoxContainer>("MarginContainer/WindowContainer/ScrollContainer/EditorContainer");
    }

    public void CreateEdit<T>(T obj, Action<T> action) where T : ICloneable
    {
        var newObj = (T)obj.Clone();

        foreach (Node child in _windowContainer.GetChildren())
            if (child is Button)
                child.QueueFree();
        foreach (Node child in _editorContainer.GetChildren())
            child.QueueFree();

        Type t = typeof(T);
        if (t.BaseType is { } baseType)
            Parser(baseType);
        Parser(t);

        Button button = new()
        {
            Text = Tr("确定保存"),
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
        };
        button.Pressed += () =>
        {
            action(newObj);
            Visible = false;
        };
        _windowContainer.AddChild(button);
        Show();

        return;

        void Parser(IReflect reflect)
        {
            var fields = reflect.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttribute<EditorGroup>() is { Ignore: true }) continue;
                var editorItem = EditorItem.Instance;
                editorItem.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

                Label label = new()
                {
                    Text = Tr(field.Name),
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                };
                editorItem.Head.AddChild(label);
                SpinBox spinBox = Helpers.ReflectionSpinBox(newObj, field);
                editorItem.Content.AddChild(spinBox);
                _editorContainer.AddChild(editorItem);
            }
        }
    }
}