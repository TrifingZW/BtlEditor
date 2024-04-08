using System;
using System.IO;
using IniParser;
using IniParser.Model;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.CoreScripts.Parser;

public class StringtableParser
{
    public KeyDataCollection Global { get; set; }
    public GeneralName GeneralName { get; set; }
    public ArmyName ArmyName { get; set; }
    public CityName CityName { get; set; }
    public CountryName CountryName { get; set; }


    public StringtableParser Parser()
    {
        var path = $"{AssetsPath}/stringtable_cn.ini";
        if (!File.Exists(path)) throw new("没有导入stringtable_cn.ini！");

        try
        {
            FileIniDataParser parser = new();
            IniData data = parser.ReadData(new(File.OpenRead(path)));
            data.Configuration.AllowDuplicateKeys = true;
            Global = data.Global;
            GeneralName = new(Global);
            ArmyName = new(Global);
            CityName = new(Global);
            CountryName = new(Global);
            return this;
        }
        catch (Exception e)
        {
            throw new($"解析stringtable_cn.ini错误:{e.Message}");
        }
    }
}

public class GeneralName(KeyDataCollection global)
{
    public string this[string name] => global[$"{name}"];
}

public class SkillName(KeyDataCollection global)
{
    public string this[int id] => global[$"skill_name_{id}"];
}

public class ArmyName(KeyDataCollection global)
{
    public string this[int id] => global[$"unit_name_{id}"];
}

public class CityName(KeyDataCollection global)
{
    public string this[string id] => global[$"battle_cityname_{id}"];
}

public class CountryName(KeyDataCollection global)
{
    public string this[int id] => global[$"country_{id}"];
}

public class Rank(KeyDataCollection global)
{
    public string this[int id] => global[$"general_rank_{id}"];
}