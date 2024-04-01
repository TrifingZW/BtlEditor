using BtlEditor.CoreScripts.Parser;
using Godot;

namespace BtlEditor.CoreScripts;

public static class StaticRes
{
    #region Resource

    public static BtlParser Btl { get; set; }
    public static BinParser Bin { get; set; }
    public static Wc4ResourceParser TerrainHd { get; set; }
    public static Wc4ResourceParser PlantHd { get; set; }
    public static Wc4ResourceParser BuildingsHd { get; set; }
    public static Wc4ResourceParser Tacticalmap { get; set; }
    public static Wc4ResourceParser ImageGeneralMedalHd { get; set; }
    public static Wc4ResourceParser ImageRibbonHd { get; set; }
    public static GeneralSettingsParser GeneralSettings { get; set; }
    public static ArmySettingsParser ArmySettings { get; set; }
    public static TerrainConfigParser TerrainConfig { get; set; }
    public static MapConfigParser MapConfig { get; set; }
    public static StringtableParser Stringtable { get; set; }
    public static Image ColorUvImage { get; set; }

    #endregion

    #region Constant

    //Path
    public const string Package = "com.trifingzw.emodule";
    public static string FilePath => Globals.Win ? ProjectSettings.GlobalizePath("res://files") : $"/storage/emulated/0/Android/data/{Package}/files";
    public static string WorkPath => Globals.WorkPath;
    public static string AssetsPath => $"{WorkPath}/assets";
    public static string TexturesPath => AssetsPath;
    public static string JsonPath => $"{AssetsPath}/json";
    public static string StagePath => $"{AssetsPath}/stage";
    public static string ConfigPath => $"{AssetsPath}/config";
    public static string ImagePath => $"{AssetsPath}/image";
    public static string ImageHeadPath => $"{ImagePath}/heads";

    #endregion
}