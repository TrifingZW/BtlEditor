using System;
using System.Reflection;
using BtlEditor.CoreScripts.Attributes;
using BtlEditor.GameScreen.Scripts.LandScripts;
using Godot;

namespace BtlEditor.GameScreen.Scripts.MapUIScripts.Multi;

public abstract partial class BaseMulti : VBoxContainer
{
    private static MapUI MapUI => Game.Instance.MapUI;

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
                if (field.GetCustomAttribute<EditorItem>() is { Ignore: true }) continue;

                Button button = new() { Text = field.Name };
                button.FocusMode = FocusModeEnum.None;
                button.Pressed += () =>
                {
                    Game.Instance.EditWindow.CreateEdit(field.Name, value =>
                    {
                        foreach (LandUnit landUnit in MapUI.MultiLandUnit)
                            Update(field, value, landUnit);
                    });
                };
                AddChild(button);
            }
        }
    }
}