using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using static BtlEditor.CoreScripts.StaticRes;
using FileAccess = Godot.FileAccess;

namespace BtlEditor.CoreScripts.Parser;

public class TerrainConfigParser
{
    public Terrains Terrains { get; private set; }

    public TerrainConfigParser Parser()
    {
        const string fileName = "def_mapterrain";
        var xmlPath = $"{ConfigPath}/{fileName}.xml";
        if (!File.Exists(xmlPath)) throw new($"没有导入{fileName}.xml！");

        //反序列化XML
        try
        {
            XmlSerializer serializer = new(typeof(Terrains));
            if (serializer.Deserialize(new StringReader(FileAccess.GetFileAsString(xmlPath))) is Terrains terrains)
                Terrains = terrains;
        }
        catch (Exception e)
        {
            throw new($"反序列化{fileName}.xml失败！{e.Message}");
        }

        return this;
    }
}

//Terrains
[XmlRoot(ElementName = "terrains")]
public class Terrains
{
    [XmlElement(ElementName = "terrain")] public List<Terrain> ImageList { get; set; }
}

[XmlRoot(ElementName = "terrain")]
public class Terrain
{
    [XmlAttribute("name")] public string Name { get; set; }
    [XmlAttribute("terrain")] public int TerrainG { get; set; }
    [XmlAttribute("type")] public int Type { get; set; }
    [XmlElement(ElementName = "tile")] public List<Tile> Tiles { get; set; }
}

[XmlRoot(ElementName = "tile")]
public class Tile
{
    [XmlAttribute("idx")] public int Idx { get; set; }
    [XmlAttribute("image")] public string Image { get; set; }
}