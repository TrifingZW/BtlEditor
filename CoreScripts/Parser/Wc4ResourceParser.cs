using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using BtlEditor.CoreScripts.Utils;
using Godot;
using static BtlEditor.CoreScripts.StaticRes;
using FileAccess = Godot.FileAccess;

namespace BtlEditor.CoreScripts.Parser;

public class Wc4ResourceParser(string fileName, bool external = false, string path = null)
{
    public Images Images { get; private set; }
    public Texture2D Texture2D { get; private set; } = new();

    public Wc4ResourceParser Parser()
    {
        if (external)
        {
            var xmlPath = $"{TexturesPath}/{path}{fileName}.xml";
            var texPath = Helpers.GetValidImagePath($"{TexturesPath}/{path}{fileName}");

            //判断文件是否存在
            if (!File.Exists(xmlPath) || !File.Exists(texPath))
                throw new($"没有导入{fileName}资源");

            var xml = File.ReadAllText(xmlPath).Replace($"<Texture name=\"{fileName}.png\" />", string.Empty);
            //反序列化XML
            try
            {
                XmlSerializer serializer = new(typeof(Images));
                if (serializer.Deserialize(new StringReader(xml)) is Images images)
                    Images = images;
            }
            catch (Exception e)
            {
                throw new($"反序列化{fileName}.xml失败！{e.Message}");
            }

            /*//删除.png后缀
            Wc4ResourceRoot.ImageList = Wc4ResourceRoot.ImageList.Select(i =>
            {
                i.Name = CheckAndRemoveSuffix(i.Name);
                return i;
            }).ToList();*/

            //加载图片为纹理
            Image image = Image.LoadFromFile(texPath);
            Texture2D texture2D = ImageTexture.CreateFromImage(image);
            Texture2D = texture2D;
            return this;
        }
        else
        {
            var xmlPath = $"res://Assets/Textures/{fileName}.xml";
            var texPath = $"res://Assets/Textures/{fileName}.webp";
            if (!FileAccess.FileExists(texPath)) texPath = $"res://Assets/Textures/{fileName}.png";

            //反序列化XML
            XmlSerializer serializer = new(typeof(Images));
            if (serializer.Deserialize(new StringReader(FileAccess.GetFileAsString(xmlPath))) is Images images)
                Images = images;

            /*//删除.png后缀
            Wc4ResourceRoot.ImageList = Wc4ResourceRoot.ImageList.Select(i =>
            {
                i.Name = CheckAndRemoveSuffix(i.Name);
                return i;
            }).ToList();*/

            //加载图片为纹理
            Image image = new();
            if (texPath.EndsWith(".png"))
                image.LoadPngFromBuffer(FileAccess.GetFileAsBytes(texPath));
            else if (texPath.EndsWith(".webp"))
                image.LoadWebpFromBuffer(FileAccess.GetFileAsBytes(texPath));
            else
                throw new("Not supported file type");
            Texture2D texture2D = ImageTexture.CreateFromImage(image);
            Texture2D = texture2D;
            return this;
        }
    }

    //获取数据
    public Wc4ResourceElement GetWc4ResourceElement(string name) => Images.ImageList.FirstOrDefault(i => i.Name == name);

    private static string CheckAndRemoveSuffix(string input, string suffix = ".png")
    {
        if (input != null && suffix != null && input.EndsWith(suffix))
            return input[..^suffix.Length];
        return input;
    }
}

//Images
[XmlRoot(ElementName = "Images")]
public class Images
{
    [XmlElement(ElementName = "Image")] public List<Wc4ResourceElement> ImageList { get; set; }
}

[XmlRoot(ElementName = "Image")]
public class Wc4ResourceElement
{
    [XmlAttribute("name")] public string Name { get; set; }

    [XmlAttribute("x")] public int X { get; set; }

    [XmlAttribute("y")] public int Y { get; set; }

    [XmlAttribute("w")] public int Width { get; set; }

    [XmlAttribute("h")] public int Height { get; set; }

    [XmlAttribute("refx")] public int RefX { get; set; }

    [XmlAttribute("refy")] public int RefY { get; set; }
}