using System;

namespace BtlEditor.CoreScripts.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EditorGroup(string group = null, bool ignore = false) : Attribute
{
    public string Group => group ?? string.Empty;
    public bool Ignore => ignore;
}