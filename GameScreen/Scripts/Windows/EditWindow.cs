using System;
using Godot;

namespace BtlEditor.GameScreen.Scripts.Windows;

public partial class EditWindow : Window
{
    private SpinBox _spinBox;
    private Button _button;
    private int _value;
    private Action<int> _callback;

    public override void _Ready()
    {
        CloseRequested += () => Visible = false;
        _spinBox = GetNode<SpinBox>("MarginContainer/VBoxContainer/SpinBox");
        _button = GetNode<Button>("MarginContainer/VBoxContainer/Button");
        _spinBox.ValueChanged += v => _value = (int)v;
        _spinBox.MinValue = 0;
        _spinBox.MaxValue = int.MaxValue;
        _button.Pressed += () =>
        {
            Hide();
            _callback(_value);
        };
    }

    public void CreateEdit(string title, Action<int> callback)
    {
        Show();
        Title = title;
        _callback = callback;

        _value = 0;
        _spinBox.Value = 0;
    }
}