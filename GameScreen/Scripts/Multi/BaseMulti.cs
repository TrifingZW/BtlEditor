using System;
using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using Godot;

namespace BtlEditor.GameScreen.Scripts.Multi;

public abstract partial class BaseMulti : VBoxContainer
{
    private static GameUI GameUI => GameUI.Instance;

    public override void _Ready()
    {
        Initialize();
    }

    public virtual void Initialize()
    {
    }

    protected virtual void Update(FieldInfo field, int value, LandUnit landUnit)
    {
    }

    protected void ReflexStruct<T>()
    {
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
                if (field.GetCustomAttribute<EditorGroup>() is { Ignore: true }) continue;

                Button button = new() { Text = field.Name };
                button.FocusMode = FocusModeEnum.None;
                button.Pressed += () =>
                {
                    GameUI.EditWindow.CreateEdit(field.Name, value =>
                    {
                        foreach (LandUnit landUnit in GameUI.MultiLandUnit)
                            Update(field, value, landUnit);
                    });
                };
                AddChild(button);
            }
        }
    }
}