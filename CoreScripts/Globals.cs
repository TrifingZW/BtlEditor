using System.Diagnostics.CodeAnalysis;
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
    private static long _windowMode;

    public static long WindowMode
    {
        get => _windowMode;
        set
        {
            _windowMode = value;
            switch (WindowMode)
            {
                case 0:
                    DisplayServer.WindowSetMode(0);
                    break;
                case 1:
                    DisplayServer.WindowSetMode((DisplayServer.WindowMode)4L);
                    break;
            }
        }
    }

    public static string WorkPath { get; private set; }
    private static Translation _translation;

    public static Translation Translation
    {
        get => _translation;
        set
        {
            _translation = value;
            TranslationServer.SetLocale(Translation.ToString());
        }
    }

    public override void _EnterTree()
    {
        Load();
    }

    public static void Load()
    {
        Config.Load(ConfigPath);
        RenderScale = Config.GetValue("Setting", "Shader", 2).AsInt32();
        WindowMode = Config.GetValue("Setting", "WindowMode", 0L).AsInt64();
        Translation = (Translation)Config.GetValue("Setting", "Translation", 0).AsInt32();
        WorkPath = Config.GetValue("Work", "WorkPath", $"{FilePath}/WorkPath").AsString();
        Save();
    }

    public static void Save()
    {
        Config.SetValue("Setting", "Shader", RenderScale);
        Config.SetValue("Setting", "WindowMode", WindowMode);
        Config.SetValue("Setting", "Translation", (int)Translation);
        Config.SetValue("Work", "WorkPath", WorkPath);
        Config.Save(ConfigPath);
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
public enum Translation
{
    cn = 0,
    en = 1
}