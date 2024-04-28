using System.Collections.Generic;
using BtlEditor.CoreScripts;
using Godot;

namespace BtlEditor.GameScreen.Scripts;

public partial class CameraController : Camera2D
{
    private Vector2 _targetPosition = Vector2.Zero;

    private bool _target;

    public Vector2 TargetPosition
    {
        get => _targetPosition;
        set
        {
            if (value == Vector2.Zero) return;
            _targetPosition = value;
            _target = true;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        AndroidCameraSet();
        //目标位置插值
        if (!_target || TargetPosition == Vector2.Zero) return;
        Position = Position.Lerp(TargetPosition, 0.2f);
        if (!(Position.DistanceTo(TargetPosition) < 1f)) return;
        _target = false;
        Position = TargetPosition;
        TargetPosition = Vector2.Zero;
    }

    public override void _UnhandledInput(InputEvent @event) => AndroidController(@event);

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
    public bool AndroidCameraController { get; set; } = true;

    private void AndroidCameraSet()
    {
        _zoomValue = Mathf.Clamp(_zoomValue, 2f, 40f);
        Zoom = Vector2.One * _zoomValue / 10f;
    }

    private void AndroidController(InputEvent inputEvent)
    {
        if (_target) return;
        if (!AndroidCameraController) return;
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