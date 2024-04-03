using Godot;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.CoreScripts;

public partial class Globals : Node
{
    public static bool Win => OS.GetName() == "Windows";

    private static readonly ConfigFile Setting = new();
    private static readonly ConfigFile Work = new();
    private static readonly string SettingFile = $"{FilePath}/config.cfg";
    private static readonly string WorkFile = $"{FilePath}/work.cfg";


    public static float ShaderScale => Shader switch
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
    public static int Shader { get; private set; }
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
        Setting.Load(SettingFile);
        Scale = Setting.GetValue("Setting", "Scale", Variant.From(1.0f)).AsSingle();
        Fps = Setting.GetValue("Setting", "Fps", Variant.From(0)).AsInt32();
        Shader = Setting.GetValue("Setting", "Shader", Variant.From(1)).AsInt32();
        WindowMode = Setting.GetValue("Setting", "WindowMode", Variant.From(0L)).AsInt64();
        VSync = Setting.GetValue("Setting", "VSync", Variant.From(2L)).AsInt64();

        Work.Load(WorkFile);
        WorkPath = Work.GetValue("Work", "WorkPath", $"{FilePath}/WorkPath").AsString();
        BtlPath = Work.GetValue("Work", "BtlPath", $"{StagePath}/conquest5.btl").AsString();

        Save(Scale, Fps, Shader, WindowMode, VSync);
    }

    public static void Save(float scale, int fps, int shader, long windowMode, long vSync)
    {
        Scale = scale;
        Fps = fps;
        Shader = shader;
        WindowMode = windowMode;
        VSync = vSync;

        Setting.SetValue("Setting", "Scale", Scale);
        _root.ContentScaleFactor = Scale;
        Setting.SetValue("Setting", "Fps", Fps);
        Engine.MaxFps = FpsValue;
        Setting.SetValue("Setting", "Shader", Shader);
        Setting.SetValue("Setting", "WindowMode", WindowMode);
        DisplayServer.WindowSetMode((DisplayServer.WindowMode)WindowMode);
        Setting.SetValue("Setting", "VSync", VSync);
        DisplayServer.WindowSetVsyncMode((DisplayServer.VSyncMode)VSync);
        Setting.Save(SettingFile);
    }
}