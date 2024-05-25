using System;
using System.IO;
using IniParser;
using IniParser.Model;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.CoreScripts.Parser;

public class StringtableParser
{
    public KeyDataCollection Global { get; private set; } = new();
    public GeneralName GeneralName { get; private set; } = new(null);
    public ArmyName ArmyName { get; private set; } = new(null);
    public CityName CityName { get; private set; } = new(null);
    public CountryName CountryName { get; private set; } = new(null);


    public StringtableParser Parser(string name)
    {
        var path = $"{AssetsPath}/{name}";
        if (!File.Exists(path)) throw new($"没有导入{name}，会导致无法显示名称。");

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
            throw new($"解析{name}错误，会导致无法显示名称。报错信息：{e.Message}");
        }
    }
}

public class GeneralName(KeyDataCollection global)
{
    public string this[string name] => global?[$"{name}"];
}

public class SkillName(KeyDataCollection global)
{
    public string this[int id] => global?[$"skill_name_{id}"];
}

public class ArmyName(KeyDataCollection global)
{
    public string this[int id] => global?[$"unit_name_{id}"];
}

public class CityName(KeyDataCollection global)
{
    public string this[string id] => global?[$"battle_cityname_{id}"];
}

public class CountryName(KeyDataCollection global)
{
    public string this[int id] => global?[$"country_{id}"];
}

public class Rank(KeyDataCollection global)
{
    public string this[int id] => global?[$"general_rank_{id}"];
}