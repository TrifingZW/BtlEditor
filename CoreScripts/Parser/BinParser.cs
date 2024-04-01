using System;
using System.IO;
using System.Runtime.InteropServices;
using BtlEditor.CoreScripts.Structures;
using static BtlEditor.CoreScripts.Parser.ParserHelper;
using static BtlEditor.CoreScripts.StaticRes;

namespace BtlEditor.CoreScripts.Parser;

public class BinParser
{
    public BinInfo BinInfo;
    public Topography[] Topographies = [];

    public int Width => BinInfo.Width;
    public int Height => BinInfo.Height;
    public int Count => Width * Height;


    public BinParser Parser(string name)
    {
        try
        {
            var binPath = $"{AssetsPath}/{name}";
            if (!File.Exists(binPath)) throw new("没有导入bin");

            FileStream stream = File.OpenRead(binPath);
            using BinaryReader reader = new(stream);
            var masterBytes = reader.ReadBytes(0x10);
            BinInfo = MemoryMarshal.Read<BinInfo>(masterBytes);
            Topographies = ReadToClassArray<Topography>(reader, Count);
        }
        catch (Exception e)
        {
            throw new($"解析Bin错误:{e.Message}");
        }

        return this;
    }
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct BinInfo
{
    public byte Unknown1 { get; set; }
    public byte Unknown2 { get; set; }
    public byte Unknown3 { get; set; }
    public byte Unknown4 { get; set; }
    public byte Unknown5 { get; set; }
    public byte Unknown6 { get; set; }
    public byte Unknown7 { get; set; }
    public byte Unknown8 { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
}