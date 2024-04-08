using System;
using System.Collections.Generic;
using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using BtlEditor.GameScreen.Scripts.LandScripts;
using BtlEditor.UserInterface;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public abstract partial class BaseSingle : ScrollContainer
{
    private LandUnit _landUnit;

    public LandUnit LandUnit
    {
        get => _landUnit;
        set
        {
            _landUnit = value;
            Update();
        }
    }

    protected VBoxContainer Container { get; }
    protected VBoxContainer HeadContainer { get; }
    protected VBoxContainer TreeContainer { get; }
    protected VBoxContainer EndContainer { get; }

    protected TreeBar MainTreeBar { get;  set; }
    protected readonly Dictionary<string, TreeBar> TreeDirectory = [];

    public BaseSingle()
    {
        Container = new();
        Container.SizeFlagsVertical = SizeFlags.ExpandFill;
        Container.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        AddChild(Container);
        HeadContainer = new();
        HeadContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        Container.AddChild(HeadContainer);
        TreeContainer = new();
        TreeContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        Container.AddChild(TreeContainer);
        EndContainer = new();
        EndContainer.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        Container.AddChild(EndContainer);
    }

    protected abstract void Update();

    public virtual void Clear()
    {
        foreach (Node child in HeadContainer.GetChildren())
            child.QueueFree();
        foreach (Node child in TreeContainer.GetChildren())
            child.QueueFree();
        foreach (Node child in EndContainer.GetChildren())
            child.QueueFree();

        TreeDirectory.Clear();
    }

    protected void ReflexStruct<T>(T obj, Node node, Action save)
    {
        MainTreeBar = TreeBar.Instance;
        MainTreeBar.Title = "主数据";
        node.AddChild(MainTreeBar);

        Type t = typeof(T);
        if (t.BaseType is { } baseType)
            Parser(baseType);
        Parser(t);

        return;

        void Parser(IReflect reflect)
        {
            var fields = reflect.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (FieldInfo field in fields)
            {
                var editorItem = EditorItem.Instance;
                if (field.GetCustomAttribute<EditorGroup>() is { } editorGroup)
                {
                    if (editorGroup.Ignore) continue;
                    if (TreeDirectory.TryGetValue(editorGroup.Group, out TreeBar treeBar))
                        treeBar.Layout.AddChild(editorItem);
                    else
                    {
                        var bar = TreeBar.Instance;
                        bar.Title = editorGroup.Group;
                        node.AddChild(bar);
                        TreeDirectory.Add(editorGroup.Group, bar);
                        bar.Layout.AddChild(editorItem);
                    }
                }
                else MainTreeBar.Layout.AddChild(editorItem);

                Label label = new()
                {
                    Text = Tr(field.Name),
                    SizeFlagsHorizontal = SizeFlags.ExpandFill
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
                }

                spinBox.ValueChanged += _ => save();
                editorItem.Content.AddChild(spinBox);
            }
        }
    }
}