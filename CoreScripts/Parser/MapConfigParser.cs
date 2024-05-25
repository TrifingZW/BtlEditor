using System;
using System.IO;
using System.Xml.Serialization;
using static BtlEditor.CoreScripts.StaticRes;
using FileAccess = Godot.FileAccess;

namespace BtlEditor.CoreScripts.Parser;

public class MapConfigParser
{
    public Maps Maps { get; private set; } = new();

    public MapConfigParser Parser()
    {
        const string fileName = "def_map";
        var xmlPath = $"{ConfigPath}/{fileName}.xml";
        if (!File.Exists(xmlPath)) throw new($"没有导入{fileName}.xml，会导致无法编辑征服文件。");

        //反序列化XML
        try
        {
            XmlSerializer serializer = new(typeof(Maps));
            if (serializer.Deserialize(new StringReader(FileAccess.GetFileAsString(xmlPath))) is Maps maps)
                Maps = maps;
        }
        catch (Exception e)
        {
            throw new($"解析{fileName}.xml错误，会导致无法编辑征服文件。报错信息：{e.Message}");
        }

        return this;
    }
}

[XmlRoot(ElementName = "maps")]
public class Maps
{
    [XmlElement("map")] public Map[] MapArray { get; set; } = [];
}

[XmlRoot(ElementName = "map")]
public class Map
{
    [XmlAttribute("id")] public int Id { get; set; }
    [XmlAttribute("name")] public string Name { get; set; }
    [XmlAttribute("file")] public string File { get; set; }
    [XmlAttribute("w")] public int W { get; set; }
    [XmlAttribute("h")] public int H { get; set; }
    [XmlAttribute("tile")] public string Tile { get; set; }
    [XmlAttribute("textpos")] public string TexPos { get; set; }
    [XmlAttribute("tilesize")] public int TileSize { get; set; }
    [XmlAttribute("pattern")] public string Pattern { get; set; }
    [XmlAttribute("patternsize")] public int PatternSize { get; set; }
}