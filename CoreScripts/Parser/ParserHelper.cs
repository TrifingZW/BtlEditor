using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Godot;

namespace BtlEditor.CoreScripts.Parser;

public static class ParserHelper
{
    
    public static T[] ReadToStructArray<T>(BinaryReader reader, int count, int byteLength) where T : struct
    {
        var array = new T[count];
        for (var i = 0; i < count; i++)
        {
            var clanBytes = reader.ReadBytes(byteLength);
            if (MemoryMarshal.TryRead(clanBytes, out T clan))
                array[i] = clan;
            else GD.Print(typeof(T).Name);
        }

        return array;
    }

    public static T[] ReadToClassArray<T>(BinaryReader reader, int count) where T : class, new()
    {
        var array = new T[count];
        try
        {
            for (var index = 0; index < array.Length; index++)
                array[index] = Deserialize<T>(reader);
        }
        catch (Exception e)
        {
            throw new($"({typeof(T)}){e.Message}");
        }

        return array;
    }

    public static T Deserialize<T>(BinaryReader reader) where T : new()
    {
        T instance = new();

        Type t = typeof(T);
        if (t.BaseType is { } baseType)
            if (baseType != typeof(object))
                Parser(baseType);
        Parser(t);

        return instance;

        void Parser(IReflect reflect)
        {
            var fields = reflect.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (FieldInfo field in fields)
            {
                switch (field.FieldType)
                {
                    case { } type when type == typeof(byte):
                        field.SetValue(instance, reader.ReadByte());
                        break;
                    case { } type when type == typeof(bool):
                        field.SetValue(instance, reader.ReadBoolean());
                        break;
                    case { } type when type == typeof(short):
                        field.SetValue(instance, reader.ReadInt16());
                        break;
                    case { } type when type == typeof(int):
                        field.SetValue(instance, reader.ReadInt32());
                        break;
                    case { } type when type == typeof(long):
                        field.SetValue(instance, reader.ReadInt64());
                        break;
                    case { } type when type == typeof(float):
                        field.SetValue(instance, reader.ReadSingle());
                        break;
                    case { } type when type == typeof(double):
                        field.SetValue(instance, reader.ReadDouble());
                        break;
                }
            }
        }
    }

    public static void Serializable<T>(this T obj, BinaryWriter binaryWriter)
    {
        Type t = typeof(T);
        if (t.BaseType is { } baseType)
            if (baseType != typeof(object))
                Parser(baseType);

        Parser(t);
        return;

        void Parser(IReflect reflect)
        {
            var fields = reflect.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            foreach (FieldInfo field in fields)
                switch (field.FieldType)
                {
                    case { } type when type == typeof(byte):
                        binaryWriter.Write((byte)field.GetValue(obj)!);
                        break;
                    case { } type when type == typeof(bool):
                        binaryWriter.Write((bool)field.GetValue(obj)!);
                        break;
                    case { } type when type == typeof(short):
                        binaryWriter.Write((short)field.GetValue(obj)!);
                        break;
                    case { } type when type == typeof(int):
                        binaryWriter.Write((int)field.GetValue(obj)!);
                        break;
                    case { } type when type == typeof(long):
                        binaryWriter.Write((long)field.GetValue(obj)!);
                        break;
                    case { } type when type == typeof(float):
                        binaryWriter.Write((float)field.GetValue(obj)!);
                        break;
                    case { } type when type == typeof(double):
                        binaryWriter.Write((double)field.GetValue(obj)!);
                        break;
                }
        }
    }

  
}