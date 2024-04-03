using System.Collections.Generic;
using BtlEditor.CoreScripts;
using Godot;

namespace BtlEditor.GameScreen.Scripts;

public partial class CameraController : Camera2D
{
    public override void _PhysicsProcess(double delta)
    {
        if (Globals.Win)
            WindowsCameraSet();
        else
            AndroidCameraSet();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (Globals.Win)
            WindowsController(@event);
        else
            AndroidController(@event);
    }


    #region Windows视角控制器

    private bool _rightButtonPressed;
    private Vector2 _mousePosition;
    private Vector2 _position;
    private float _wheelValue = 1f;
    public float MaxZoom = 20f;
    public float MinZoom = 1f;

    private void WindowsCameraSet()
    {
        _wheelValue = Mathf.Clamp(_wheelValue, MinZoom, MaxZoom);
        Zoom = Zoom.Lerp(Vector2.One * _wheelValue / 5f, 0.2f);
    }

    private void WindowsController(InputEvent inputEvent)
    {
        if (inputEvent is InputEventMouseMotion eventMouseMotion && _rightButtonPressed)
        {
            Vector2 mousePosition = eventMouseMotion.Position;
            Vector2 delta = (_mousePosition - mousePosition) / _wheelValue * 5;
            Position = _position + delta;
        }

        if (inputEvent is not InputEventMouseButton mouseButtonEvent) return;
        if (mouseButtonEvent.ButtonIndex == MouseButton.WheelUp && mouseButtonEvent.Pressed)
            _wheelValue += mouseButtonEvent.Factor;
        if (mouseButtonEvent.ButtonIndex == MouseButton.WheelDown && mouseButtonEvent.Pressed)
            _wheelValue -= mouseButtonEvent.Factor;

        if (mouseButtonEvent.ButtonIndex == MouseButton.Right)
        {
            if (mouseButtonEvent.Pressed)
            {
                if (_rightButtonPressed == false)
                {
                    _rightButtonPressed = true;
                    _mousePosition = mouseButtonEvent.Position;
                    _position = Position;
                }
            }
            else if (!mouseButtonEvent.Pressed) _rightButtonPressed = false;
        }
    }

    #endregion

    #region Android视角控制器

    private readonly HashSet<int> _activeTouches = [];
    private bool _zoomGestures;
    private bool _gesturePanning;
    private Vector2 _touch1Position;
    private Vector2 _touch2Position;
    private Vector2 _drag1Position;
    private Vector2 _drag2Position;
    private float _zoomDelta;
    private float _zoomValue = 1f;
    private Vector2 _cameraPosition;

    private void AndroidCameraSet()
    {
        _zoomValue = Mathf.Clamp(_zoomValue, 2f, 40f);
        Zoom = Vector2.One * _zoomValue / 10f;
    }

    private void AndroidController(InputEvent inputEvent)
    {
        if (inputEvent is InputEventScreenTouch touchEvent)
        {
            if (touchEvent.Pressed)
            {
                _activeTouches.Add(touchEvent.Index);

                if (_activeTouches.Count == 1)
                {
                    _gesturePanning = true;
                    _touch1Position = touchEvent.Position;
                    _cameraPosition = Position;
                }
                else _gesturePanning = false;

                if (_activeTouches.Count == 2)
                {
                    _touch2Position = touchEvent.Position;
                    _zoomDelta = _zoomValue;
                    _drag1Position = Vector2.Zero;
                    _drag2Position = Vector2.Zero;
                    _zoomGestures = true;
                }
                else _zoomGestures = false;
            }
            else
            {
                _activeTouches.Remove(touchEvent.Index);
                _gesturePanning = false;
                _zoomGestures = false;
            }
        }

        if (inputEvent is InputEventScreenDrag dragEvent)
        {
            if (_gesturePanning)
            {
                Vector2 dragPosition = dragEvent.Position;
                Vector2 delta = (_touch1Position - dragPosition) / Zoom;
                Position = _cameraPosition + delta;
            }

            if (_zoomGestures)
            {
                if (dragEvent.Index == 0)
                    _drag1Position = dragEvent.Position;
                if (dragEvent.Index == 1)
                    _drag2Position = dragEvent.Position;

                if (_drag1Position != Vector2.Zero && _drag2Position != Vector2.Zero)
                {
                    var delta1 = (_touch1Position - _touch2Position).Length();
                    var delta2 = (_drag1Position - _drag2Position).Length();
                    var delta = delta2 - delta1;
                    _zoomValue = _zoomDelta + delta / 100f;
                }
            }
        }
    }

    #endregion
}