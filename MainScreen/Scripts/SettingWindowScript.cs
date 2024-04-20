using BtlEditor.CoreScripts;
using Godot;

namespace BtlEditor.MainScreen.Scripts;

public partial class SettingWindowScript : Window
{
    private float _sc;
    private int _fp;
    private int _sh;
    private long _wi;
    private long _vs;
    private SpinBox _scale;
    private OptionButton _fps;
    private OptionButton _shader;
    private OptionButton _windowMode;
    private OptionButton _vSync;

    public override void _Ready()
    {
        //Close按钮
        CloseRequested += () => { Visible = false; };

        _sc = Globals.Scale;
        _fp = Globals.Fps;
        _sh = Globals.RenderScale;
        _wi = Globals.WindowMode;
        _vs = Globals.VSync;

        //缩放设置
        _scale = GetNode<SpinBox>("%Scale");
        _scale.ValueChanged += value => { _sc = (float)value; };

        //FPS设置
        _fps = GetNode<OptionButton>("%Fps");
        _fps.ItemSelected += select => { _fp = (int)select; };

        //着色细节倍率设置
        _shader = GetNode<OptionButton>("%Shader");
        _shader.ItemSelected += index => { _sh = (int)index; };

        //窗口设置
        _windowMode = GetNode<OptionButton>("%WindowMode");
        _windowMode.ItemSelected += select =>
        {
            _wi = select switch
            {
                0 => 0L,
                1 => 4L,
                _ => 0L
            };
        };

        //垂直同步设置
        _vSync = GetNode<OptionButton>("%VSync");
        _vSync.ItemSelected += select =>
        {
            _vs = select switch
            {
                0 => 2L,
                1 => 0L,
                _ => 2L
            };
        };
    }

    public void Load()
    {
        _scale.Value = Globals.Scale;
        _fps.Selected = Globals.Fps;
        _shader.Selected = Globals.RenderScale;
        _windowMode.Selected = Globals.WindowMode == 0 ? 0 : 1;
        _vSync.Selected = Globals.VSync == 2 ? 0 : 1;
    }

    private void Save()
    {
        Visible = false;
        Globals.Save(_sc, _fp, _sh, _wi, _vs);
    }
}