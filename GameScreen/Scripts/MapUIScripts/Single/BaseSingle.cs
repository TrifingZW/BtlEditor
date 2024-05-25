using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using BtlEditor.CoreScripts.Utils;
using BtlEditor.GameScreen.Scripts.LandScripts;
using BtlEditor.UserInterface;
using Godot;
using EditorItem = BtlEditor.CoreScripts.Attributes.EditorItem;


namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Single;

public abstract partial class BaseSingle : ScrollContainer
{
    private static MapController MapController => Game.Instance.MapController;

    private GameLandUnit _gameLandUnit;

    public GameLandUnit GameLandUnit
    {
        get => _gameLandUnit;
        set
        {
            _gameLandUnit = value;
            Clear();
            UserInface();
        }
    }

    protected VBoxContainer Container { get; }

    protected TreeBar MainTreeBar { get; set; }
    protected readonly Dictionary<string, TreeBar> TreeDirectory = [];

    public BaseSingle()
    {
        Container = new();
        Container.SizeFlagsVertical = SizeFlags.ExpandFill;
        Container.SizeFlagsHorizontal = SizeFlags.ExpandFill;
        AddChild(Container);
    }

    protected abstract void UserInface();

    protected void Update()
    {
        Clear();
        UserInface();
    }

    private void Clear()
    {
        foreach (Node child in Container.GetChildren())
            child.QueueFree();

        TreeDirectory.Clear();
    }

    protected void ReflexStruct<T>(T obj, Node node, bool ignore, params string[] fieldStrings) =>
        ReflexStruct(obj, node, null, ignore, fieldStrings);

    protected void ReflexStruct<T>(T obj, Node node, Action save = null, bool ignore = true, params string[] fieldStrings)
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
                if (ignore == fieldStrings.Contains(field.Name)) continue;

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
                    };
                    label.Pressed += () =>
                    {
                        if (!MapController.LandUnits.TryGetValue(MapHelper.GetBtlIndex((short)field.GetValue(obj)!), out GameLandUnit landUnit))
                            return;
                        Game.Instance.CameraController.TargetPosition = landUnit.Position;
                        MapController.TileMapLayer.Clear();
                        MapController.TileMapLayer.SetCell(landUnit.Coords, MapController.SingleTileSetAtlasId, new(), 2);
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
                    };
                    countryButton.Pressed += () =>
                    {
                        Game.Instance.SearchCountryWindow.CreateEdit(country =>
                        {
                            countryButton.Text = country.ToString();
                            field.SetValue(obj, country);
                            save?.Invoke();
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
        Button button = new() { Text = text };
        button.AddThemeFontSizeOverride("font_size", 50);
        button.Pressed += action;
        return button;
    }
}