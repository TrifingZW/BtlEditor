using System;

namespace BtlEditor.CoreScripts.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EditorGroup(string group = null, EditorGroupType type = EditorGroupType.Default, bool ignore = false) : Attribute
{
    public string Group => group ?? string.Empty;
    public EditorGroupType Type => type;
    public bool Ignore => ignore;
}

public enum EditorGroupType
{
    Default,
    Direction
}