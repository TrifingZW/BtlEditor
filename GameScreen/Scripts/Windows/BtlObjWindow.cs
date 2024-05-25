using System;
using System.Linq;
using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using BtlEditor.CoreScripts.Utils;
using Godot;
using EditorItem = BtlEditor.CoreScripts.Attributes.EditorItem;

namespace BtlEditor.GameScreen.Scripts.Windows;

public partial class BtlObjWindow : Window
{
    private VBoxContainer _windowContainer;
    private VBoxContainer _editorContainer;

    public override void _Ready()
    {
        CloseRequested += Hide;
        _windowContainer = GetNode<VBoxContainer>("MarginContainer/WindowContainer");
        _editorContainer = GetNode<VBoxContainer>("MarginContainer/WindowContainer/Panel/ScrollContainer/EditorContainer");
    }

    public void CreateEdit<T>(T obj, Action<T> action, Func<T, Node> head = null) where T : ICloneable
    {
        var newObj = (T)obj.Clone();

        foreach (Node child in _windowContainer.GetChildren())
            if (child is Button)
                child.QueueFree();
        foreach (Node child in _editorContainer.GetChildren())
            child.QueueFree();

        if (head != null) _editorContainer.AddChild(head(newObj));

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
        Position += new Vector2I(0, 34);

        return;

        void Parser(IReflect reflect)
        {
            var fields = reflect.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (FieldInfo field in fields)
            {
                if (field.GetCustomAttribute<EditorItem>() is { Ignore: true }) continue;
                var editorItem = UserInterface.EditorItem.Instance;
                editorItem.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

                Label label = new()
                {
                    Text = Tr(field.Name),
                    SizeFlagsHorizontal = Control.SizeFlags.ExpandFill
                };
                editorItem.Head.AddChild(label);
                if (field.GetCustomAttribute<Option>()?.Options is { } options)
                {
                    OptionButton optionButton = Helpers.ReflectionOptionButton(newObj, field, options);
                    editorItem.Content.AddChild(optionButton);
                }
                else if (field.GetCustomAttributes<Belong>().Any())
                {
                    Button countryButton = new()
                    {
                        Text = field.GetValue(newObj)!.ToString(),
                        SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
                    };
                    countryButton.Pressed += () =>
                    {
                        Game.Instance.SearchCountryWindow.CreateEdit(country =>
                        {
                            countryButton.Text = country.ToString();
                            field.SetValue(newObj, country);
                        });
                    };
                    editorItem.Content.AddChild(countryButton);
                }
                else
                {
                    SpinBox spinBox = Helpers.ReflectionSpinBox(newObj, field);
                    editorItem.Content.AddChild(spinBox);
                }

                _editorContainer.AddChild(editorItem);
            }
        }
    }
}