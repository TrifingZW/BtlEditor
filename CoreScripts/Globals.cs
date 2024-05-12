using System.IO;
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

    public static int RenderScale { get; set; }
    public static long WindowMode { get; set; }
    public static string WorkPath { get; private set; }
    private static Window _root;

    public override void _EnterTree()
    {
        _root = GetTree().Root;
        Load();
    }

    public static void Load()
    {
        Config.Load(ConfigPath);
        RenderScale = Config.GetValue("Setting", "Shader", Variant.From(2)).AsInt32();
        WindowMode = Config.GetValue("Setting", "WindowMode", Variant.From(0L)).AsInt64();
        WorkPath = File.ReadAllText($"{FilePath}/WorkPath.txt");
        Save();
    }

    public static void Save()
    {
        Config.SetValue("Setting", "Shader", RenderScale);
        Config.SetValue("Setting", "WindowMode", WindowMode);
        switch (WindowMode)
        {
            case 0:
                DisplayServer.WindowSetMode(0);
                break;
            case 1:
                DisplayServer.WindowSetMode((DisplayServer.WindowMode)4L);
                break;
        }

        Config.SetValue("Work", "WorkPath", WorkPath);
        Config.Save(ConfigPath);
    }
}