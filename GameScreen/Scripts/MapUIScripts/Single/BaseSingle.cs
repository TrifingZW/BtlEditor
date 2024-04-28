using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using BtlEditor.CoreScripts.Parser;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using BtlEditor.UserInterface;
using Godot;
using EditorItem = BtlEditor.CoreScripts.Attributes.EditorItem;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public abstract partial class BaseSingle : ScrollContainer
{
    private static MapController MapController => Game.Instance.MapController;

    private LandUnit _landUnit;

    public LandUnit LandUnit
    {
        get => _landUnit;
        set
        {
            _landUnit = value;
            Clear();
            Update();
        }
    }

    protected VBoxContainer Container { get; }
    protected VBoxContainer HeadContainer { get; }
    protected VBoxContainer TreeContainer { get; }
    protected VBoxContainer EndContainer { get; }

    protected TreeBar MainTreeBar { get; set; }
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

    public void Clear()
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
                var editorItem = UserInterface.EditorItem.Instance;
                if (field.GetCustomAttribute<EditorItem>() is { } editorGroup)
                {
                    if (editorGroup.Ignore) continue;

                    if (editorGroup.Group == string.Empty)
                        MainTreeBar.Layout.AddChild(editorItem);
                    else if (TreeDirectory.TryGetValue(editorGroup.Group, out TreeBar treeBar))
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

                if (field.GetCustomAttributes<Target>().Any())
                {
                    Button label = new()
                    {   
                        Text = Tr(field.Name),
                        SizeFlagsHorizontal = SizeFlags.ExpandFill,
                        FocusMode = FocusModeEnum.None
                    };
                    label.Pressed += () =>
                    {
                        if (!MapController.LandUnits.TryGetValue(ParserHelper.GetBtlIndex((short)field.GetValue(obj)!), out LandUnit landUnit))
                            return;
                        Game.Instance.CameraController.TargetPosition = landUnit.Position;
                        MapController.TileMap.ClearLayer(MapController.CoverLayout);
                        MapController.TileMap.SetCell(MapController.CoverLayout, landUnit.Coords, MapController.SingleTileSetAtlasId, new(), 2);
                    };
                    editorItem.Head.AddChild(label);
                }
                else
                {
                    Label label = new()
                    {
                        Text = Tr(field.Name),
                        SizeFlagsHorizontal = SizeFlags.ExpandFill
                    };
                    editorItem.Head.AddChild(label);
                }

                if (field.GetCustomAttribute<Option>()?.Options is { } options)
                {
                    OptionButton optionButton = Helpers.ReflectionOptionButton(obj, field, options, save);
                    editorItem.Content.AddChild(optionButton);
                }
                else if (field.GetCustomAttributes<Belong>().Any())
                {
                    Button countryButton = new()
                    {
                        Text = field.GetValue(obj)!.ToString(),
                        SizeFlagsHorizontal = SizeFlags.ExpandFill,
                        FocusMode = FocusModeEnum.None
                    };
                    countryButton.Pressed += () =>
                    {
                        Game.Instance.SearchCountryWindow.CreateEdit(country =>
                        {
                            countryButton.Text = country.ToString();
                            field.SetValue(obj, country);
                            save();
                        });
                    };
                    editorItem.Content.AddChild(countryButton);
                }
                else
                {
                    SpinBox spinBox = Helpers.ReflectionSpinBox(obj, field, save);
                    editorItem.Content.AddChild(spinBox);
                }
            }
        }
    }

    protected static Button CreateButton(string text, Action action)
    {
        Button button = new()
        {
            Text = text,
            FocusMode = FocusModeEnum.None
        };
        button.AddThemeFontSizeOverride("font_size", 50);
        button.Pressed += action;
        return button;
    }
}