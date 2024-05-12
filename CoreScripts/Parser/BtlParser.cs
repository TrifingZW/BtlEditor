using System;
using System.IO;
using BtlEditor.CoreScripts.Structures;
using static BtlEditor.CoreScripts.Parser.ParserHelper;

namespace BtlEditor.CoreScripts.Parser;

public class BtlParser
{
    #region 公共字段

    public Master Master;
    public Country[] Countries;
    public Topography[] Topographies;
    public short[] Provinces;
    public byte[] Belongs;
    public City[] Cities;
    public Army1[] Armies1;
    public Army2[] Armies2;
    public Pitfall[] Pitfalls;
    public Scheme[] Schemes;
    public Weather[] Weathers;
    public Affair[] Affairs;
    public Reinforcement1[] Reinforcements1;
    public Reinforcement3[] Reinforcements3;
    public AirRaid[] AirRaids = [];
    public ArmyPlacement[] ArmyPlacements;
    public Capital[] Capitals;
    public Strategy[] Strategies;
    public AirSupport[] AirSupports;

    public bool Version1 => Master.Btl版本 == 1;
    public bool Version2 => Master.Btl版本 == 2;
    public bool Version3 => Master.Btl版本 == 3;
    public bool IndependentTerrain => Master.地图序号 == 0;

    #endregion

    public BtlParser Parser(string btlPath)
    {
        if (!File.Exists(btlPath)) throw new("BTL文件不存在！");

        try
        {
            using BinaryReader reader = new(File.OpenRead(btlPath));

            Master = Deserialize<Master>(reader);

            Countries = ReadToClassArray<Country>(reader, Master.军团总数);
            if (IndependentTerrain) Topographies = ReadToClassArray<Topography>(reader, Master.地块总数);
            Provinces = ReadToStructArray<short>(reader, Master.地块总数, 0x2);
            Belongs = reader.ReadBytes(Master.地块总数);
            Cities = ReadToClassArray<City>(reader, Master.建筑总数);
            if (Version1) Armies1 = ReadToClassArray<Army1>(reader, Master.军队总数);
            if (Version2 || Version3) Armies2 = ReadToClassArray<Army2>(reader, Master.军队总数);
            Pitfalls = ReadToClassArray<Pitfall>(reader, Master.陷阱总数);
            Schemes = ReadToClassArray<Scheme>(reader, Master.方案总数);
            Weathers = ReadToClassArray<Weather>(reader, Master.天气总数);
            Affairs = ReadToClassArray<Affair>(reader, Master.事件总数);
            if (Version1 || Version2) Reinforcements1 = ReadToClassArray<Reinforcement1>(reader, Master.援军总数);
            if (Version3) Reinforcements3 = ReadToClassArray<Reinforcement3>(reader, Master.援军总数);
            AirRaids = ReadToClassArray<AirRaid>(reader, Master.空袭总数);
            ArmyPlacements = ReadToClassArray<ArmyPlacement>(reader, Master.放置单位A + Master.放置单位B);
            Capitals = ReadToClassArray<Capital>(reader, Master.国家首都);
            Strategies = ReadToClassArray<Strategy>(reader, Master.战略总数);
            AirSupports = ReadToClassArray<AirSupport>(reader, Master.空中支援);
            for (short index = 0; index < Countries.Length; index++)
            {
                Country country = Countries[index];
                foreach (Pitfall pitfall in Pitfalls)
                    if (pitfall.所属军团 == country.序号)
                        pitfall.所属军团 = index;
                foreach (Strategy strategy in Strategies)
                    if (strategy.军团序号 == country.序号)
                        strategy.军团序号 = index;
                foreach (AirSupport airSupport in AirSupports)
                    if (airSupport.军团 == country.序号)
                        airSupport.军团 = index;
                foreach (Affair affair in Affairs)
                {
                    if (affair.触发军团 == country.序号)
                        affair.触发军团 = index;
                    if (affair.影响军团 == country.序号)
                        affair.影响军团 = index;
                }

                country.序号 = index;
            }
        }
        catch (Exception e)
        {
            throw new($"解析BTL错误:{e.Message}");
        }

        return this;
    }
}