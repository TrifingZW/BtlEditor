using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.CoreScripts.Parser;

public class ArmySettingsParser
{
    public ArmyJson[] ArmyJsons { get; private set; }

    public ArmySettingsParser Parser()
    {
        var path = $"{JsonPath}/ArmySettings.json";
        if (!File.Exists(path)) throw new("没有导入ArmySettings.json！");

        StreamReader streamReader = new(path);
        var jsonString = streamReader.ReadToEnd();
        ArmyJsons = JsonConvert.DeserializeObject<ArmyJson[]>(jsonString);
        streamReader.Close();
        return this;
    }
}

public class ArmyJson
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Anim { get; set; }
    public int Army { get; set; }
    public int Elite { get; set; }
    public int Type { get; set; }
    public int Ranking { get; set; }
    public List<int> Feature { get; set; }
    public List<int> FeatureLevel { get; set; }
    public int MinAttack { get; set; }
    public int MaxAttack { get; set; }
    public int MinRange { get; set; }
    public int MaxRange { get; set; }
    public int HP { get; set; }
    public int Defence { get; set; }
    public int Mobility { get; set; }
    public int CostMoney { get; set; }
    public int CostGear { get; set; }
    public int CostAtomic { get; set; }
    public int CostPoints { get; set; }
    public int MaxElite { get; set; }
    public int MaxFormation { get; set; }
    public int Carrier { get; set; }
    public int BuildTime { get; set; }
    public int BuildCD { get; set; }
    public int AOE1 { get; set; }
    public int AOE2 { get; set; }
    public List<int> Country { get; set; }
    public List<int> FormationChance { get; set; }
}