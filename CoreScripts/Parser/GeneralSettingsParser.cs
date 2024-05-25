using System;
using System.IO;
using Newtonsoft.Json;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.CoreScripts.Parser;

public class GeneralSettingsParser
{
    public GeneralJson[] GeneralJsons { get; private set; } = [];

    public GeneralSettingsParser Parser()
    {
        var path = $"{JsonPath}/GeneralSettings.json";
        if (!File.Exists(path)) throw new("没有读取到GeneralSettings.json文件，会导致部分将领内容无法显示。");

        try
        {
            using StreamReader streamReader = new(path);
            var jsonString = streamReader.ReadToEnd();
            GeneralJsons = JsonConvert.DeserializeObject<GeneralJson[]>(jsonString);
        }
        catch (Exception e)
        {
            throw new($"解析GeneralSettings.json错误，会导致部分将领内容无法显示。报错信息：({e.Message})");
        }

        return this;
    }
}

public class GeneralJson
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string EName { get; set; }
    public int InShop { get; set; }
    public int Type { get; set; }
    public int Evaluate { get; set; }
    public int Sequence { get; set; }
    public int UnlockHqLv { get; set; }
    public int CostMedal { get; set; }
    public int CostGold { get; set; }
    public int MilitaryRank { get; set; }
    public int Infantry { get; set; }
    public int Artillery { get; set; }
    public int Armor { get; set; }
    public int Navy { get; set; }
    public int AirForce { get; set; }
    public int March { get; set; }
    public int[] Skills { get; set; }
    public int Hp { get; set; }
    public int[] Medals { get; set; }
    public string Photo { get; set; }
    public int InfantryMax { get; set; }
    public int ArtilleryMax { get; set; }
    public int ArmorMax { get; set; }
    public int NavyMax { get; set; }
    public int AirForceMax { get; set; }
    public int MarchMax { get; set; }
    public int SkillsMax { get; set; }
}