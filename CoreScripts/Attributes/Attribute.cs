using System;
using System.Collections.Generic;
using System.Linq;

namespace BtlEditor.CoreScripts.Attributes;

[AttributeUsage(AttributeTargets.Field)]
public class EditorItem(string group = null, bool ignore = false) : Attribute
{
    public string Group => group ?? string.Empty;
    public bool Ignore => ignore;
}

[AttributeUsage(AttributeTargets.Field)]
public class Option(Type options) : Attribute
{
    public enum Direction : byte
    {
        左 = 0,
        右 = 1,
    }

    public enum Weave : byte
    {
        一编 = 1,
        二编 = 2,
        三编 = 3,
        四编 = 4,
    }

    public enum Stronghold : byte
    {
        无 = 0,
        红圈 = 1,
        绿圈 = 2
    }

    public enum Ai : byte
    {
        自由行动 = 0,
        积极进攻 = 1,
        保守进攻 = 2,
        原地固守 = 3,
        撤退 = 4
    }

    public enum Transport : byte
    {
        自由行动 = 0,
        无法下海 = 2,
    }

    public enum Morale : byte
    {
        士气上升 = 1,
        士气下降 = 255,
        士气双降 = 254,
        混乱 = 253,
    }

    public enum Shield : byte
    {
        无 = 1,
        显示盾 = 9,
    }

    public enum WeatherType : byte
    {
        下雨 = 1,
        暴雨 = 2,
        下雪 = 3
    }

    public enum TriggerConditions : byte
    {
        占城触发 = 0,
        单位死亡 = 1,
        回合触发 = 2,
        连带触发 = 4,
    }

    public enum TriggerAnEvent : byte
    {
        士气上升 = 0,
        士气下降 = 1,
        士气大降 = 2,
        士气混乱 = 3,
        调用对话 = 4,
        引发火灾 = 5,
        军团部队方针转变 = 6,
        阵营变化 = 7,
        军团向某方位移动 = 8,
        加钱 = 10,
        加工业 = 11,
        加科技 = 12,
    }

    public Dictionary<string, byte> Options =>
        Enum.GetValues(options).Cast<object>().ToDictionary(value => Enum.GetName(options, value)!, value => (byte)value);
}

[AttributeUsage(AttributeTargets.Field)]
public class Target : Attribute;

[AttributeUsage(AttributeTargets.Field)]
public class Belong : Attribute;