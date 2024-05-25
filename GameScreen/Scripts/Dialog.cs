using System;
using Godot;

namespace BtlEditor.GameScreen.Scripts;

public partial class Dialog : ConfirmationDialog
{
    private Action _action;

    public override void _Ready()
    {
        Position += new Vector2I(0, 34);
        GetOkButton().Pressed += () => { _action?.Invoke(); };
    }

    public void Builder(string text, Action action)
    {
        DialogText = text;
        _action = action;
        Show();
    }
}