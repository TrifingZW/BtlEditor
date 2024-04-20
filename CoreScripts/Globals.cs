using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.CoreScripts;

public partial class Globals : Node
{
    public static bool Win => OS.GetName() == "Windows";

    private static readonly ConfigFile Config = new();
    private static readonly string ConfigPath = $"{FilePath}/config.cfg";


    public static float RenderScaleValue => RenderScale switch
    {
        0 => 0.2f,
        1 => 0.5f,
        2 => 1f,
        _ => 0.5f
    };

    public static int FpsValue => Fps switch
    {
        0 => 0,
        1 => 30,
        2 => 60,
        3 => 120,
        4 => 144,
        5 => 165,
        6 => 240,
        _ => 0
    };

    public static float Scale { get; private set; }
    public static int Fps { get; private set; }
    public static int RenderScale { get; private set; }
    public static long WindowMode { get; private set; }
    public static long VSync { get; private set; }
    public static string WorkPath { get; private set; }
    public static string BtlPath { get; private set; }

    private static Window _root;

    public override void _EnterTree()
    {
        _root = GetTree().Root;
        Load();
    }

    public static void Load()
    {
        Config.Load(ConfigPath);
        Scale = Config.GetValue("Setting", "Scale", Variant.From(1.0f)).AsSingle();
        Fps = Config.GetValue("Setting", "Fps", Variant.From(0)).AsInt32();
        RenderScale = Config.GetValue("Setting", "Shader", Variant.From(2)).AsInt32();
        WindowMode = Config.GetValue("Setting", "WindowMode", Variant.From(0L)).AsInt64();
        VSync = Config.GetValue("Setting", "VSync", Variant.From(2L)).AsInt64();

        WorkPath = Config.GetValue("Work", "WorkPath", $"{FilePath}/WorkPath").AsString();
        BtlPath = Config.GetValue("Work", "BtlPath", $"{StagePath}/conquest5.btl").AsString();

        Save(Scale, Fps, RenderScale, WindowMode, VSync);
    }

    public static void Save(float scale, int fps, int shader, long windowMode, long vSync)
    {
        Scale = scale;
        Fps = fps;
        RenderScale = shader;
        WindowMode = windowMode;
        VSync = vSync;

        Config.SetValue("Setting", "Scale", Scale);
        _root.ContentScaleFactor = Scale;
        Config.SetValue("Setting", "Fps", Fps);
        Engine.MaxFps = FpsValue;
        Config.SetValue("Setting", "Shader", RenderScale);
        Config.SetValue("Setting", "WindowMode", WindowMode);
        DisplayServer.WindowSetMode((DisplayServer.WindowMode)WindowMode);
        Config.SetValue("Setting", "VSync", VSync);
        DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)VSync);

        Config.SetValue("Work", "WorkPath", WorkPath);
        Config.SetValue("Work", "BtlPath", BtlPath);

        Config.Save(ConfigPath);
    }
}