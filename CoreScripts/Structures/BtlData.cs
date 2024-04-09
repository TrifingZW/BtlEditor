using System;
using BtlEditor.CoreScripts.Attributes;

// ReSharper disable InconsistentNaming
// ReSharper disable UnassignedField.Global

namespace BtlEditor.CoreScripts.Structures;

public class Master
{
    [EditorGroup(ignore: true)] public int Btl版本;
    [EditorGroup(ignore: true)] public int 地图序号;
    [EditorGroup(ignore: true)] public int 地图截取x;
    [EditorGroup(ignore: true)] public int 地图截取y;
    [EditorGroup(ignore: true)] public int 地图宽;
    [EditorGroup(ignore: true)] public int 地图高;
    [EditorGroup(ignore: true)] public int 军团总数;
    [EditorGroup(ignore: true)] public int 建筑总数;
    [EditorGroup(ignore: true)] public int 军队总数;
    [EditorGroup(ignore: true)] public int 方案总数;
    [EditorGroup(ignore: true)] public int 事件总数;
    [EditorGroup(ignore: true)] public int 天气总数;
    public int 胜利条件;
    public int 最小回合;
    public int 最大回合;
    [EditorGroup(ignore: true)] public int 援军总数;
    [EditorGroup(ignore: true)] public int 空袭总数;
    [EditorGroup(ignore: true)] public int 放置单位A;
    [EditorGroup(ignore: true)] public int 放置单位B;
    [EditorGroup(ignore: true)] public int 国家首都;
    public int 战役时代;
    public int empty4;
    [EditorGroup(ignore: true)] public int 地块总数;
    public int 积攒金钱;
    public int 积攒齿轮;
    public int 积攒原子;
    [EditorGroup(ignore: true)] public int 陷阱总数;
    public int empty5;
    [EditorGroup(ignore: true)] public int 战略总数;
    public int empty6;
    public int empty7;
    [EditorGroup(ignore: true)] public int 空中支援;
}

public class Country : ICloneable
{
    [EditorGroup(ignore: true)] public int 序号;
    public int 国家;
    public int 金钱;
    public int 齿轮;
    public int 原子;
    public int 控制;
    public int 阵营;
    public int 战败条件;
    public float 兵种加成;
    public float 税率加成;
    [EditorGroup(ignore: true)] public byte R;
    [EditorGroup(ignore: true)] public byte G;
    [EditorGroup(ignore: true)] public byte B;
    [EditorGroup(ignore: true)] public byte A;
    public int 原子弹数量;
    public int 氢弹数量;
    public int 三相弹数量;
    public int 反物质弹数量;
    public int 机动等级;
    public int 步枪等级;
    public int 迷彩等级;
    public int 工兵等级;
    public int 手雷等级;
    public int 迫击炮等级;
    public int 行军等级;
    public int 防弹衣等级;
    public int 装甲等级;
    public int 主炮等级;
    public int 车体等级;
    public int 引擎等级;
    public int 机枪等级;
    public int 突袭等级;
    public int 坦克防空等级;
    public int 强化车体等级;
    public int 火炮炮击等级;
    public int 火箭弹等级;
    public int 火炮牵引等级;
    public int 火炮装甲等级;
    public int 火炮火力等级;
    public int 火炮火箭等级;
    public int 伪装等级;
    public int 舰艇船体等级;
    public int 推进器等级;
    public int 舰艇装甲等级;
    public int 武器等级;
    public int 舰艇舰炮等级;
    public int 鱼雷等级;
    public int 舰艇扫雷;
    public int 防空武器等级;
    public int 现代舰艇等级;
    public int 航空燃油等级;
    public int 航空发动机等级;
    public int 航空炸弹等级;
    public int 空袭等级;
    public int 轰炸等级;
    public int 战略轰炸等级;
    public int 空降兵等级;
    public int 喷气发动机等级;
    public int 机枪堡等级;
    public int 要塞炮等级;
    public int 海岸炮等级;
    public int 火箭发射器等级;
    public int 工事等级;
    public int 高射机枪等级;
    public int 防空炮等级;
    public int 对空导弹等级;
    public int 雷达等级;
    public int 弹头;
    public int 固体火箭发动机等级;
    public int 破防等级;
    public int 核聚变等级;
    public int empty1;
    public int empty2;
    public int empty3;
    public int empty4;
    public int 科技等级;
    public int empty5;
    public int empty6;

    public object Clone() => MemberwiseClone();
}

public class Topography
{
    public byte 地块类型;
    public byte 地块ID;
    public byte 地块X;
    public byte 地块Y;
    public byte 装饰类型A;
    public byte 装饰AID;
    public byte 装饰AX;
    public byte 装饰AY;
    public byte 装饰类型B;
    public byte 装饰BID;
    public byte 装饰BX;
    public byte 装饰BY;
    public byte 水边缘;
    public byte 路边缘;
    public byte 海岸;
    public byte empty4;
}

public class City
{
    [EditorGroup(ignore: true)] public short 坐标;
    public short 名称;
    public byte 等级;
    public byte 外观;
    public byte 地标;
    public byte 奇观;
    public short 奖励类型;
    public short 奖励数量;
    public byte 仇恨值;
    public byte 据点;
    public byte 触发事件;
    public byte empty1;
    public byte empty2;
    public byte empty3;
    public byte empty4;
    public byte 运输船;
    public byte 火焰类型;
    public byte 持续回合;
    public byte 防空武器;
    public byte 防空武器范围;
    [EditorGroup("设施")] public byte 工厂;
    [EditorGroup("设施")] public byte 科技;
    [EditorGroup("设施")] public byte 补给站;
    [EditorGroup("设施")] public byte 航空;
    [EditorGroup("设施")] public byte 导弹;
    [EditorGroup("设施")] public byte 核弹;
    public byte empty5;
    public byte empty6;

    public City()
    {
        等级 = 11;
    }
}

public class Army
{
    [EditorGroup(ignore: true)] public short 坐标;
    public byte 兵种;
    public byte 等级;
    public byte 编制;
    [EditorGroup(ignore: true)] public byte 方向;
    public byte 移动力;
    public byte 建造回合;
    public short 兵种经验;
    public short 血量加成;
    public short 当前血量;
    public short 血量上限;
    [EditorGroup("将领")] public short 将领;
    [EditorGroup("将领")] public byte 军衔;
    [EditorGroup("将领")] public byte 爵位;
    [EditorGroup("将领")] public byte 胸章一;
    [EditorGroup("将领")] public byte 胸章二;
    [EditorGroup("将领")] public byte 胸章三;
    [EditorGroup("将领")] public byte 技能等级1;
    [EditorGroup("将领")] public byte 技能等级2;
    [EditorGroup("将领")] public byte 技能等级3;
    [EditorGroup("将领")] public byte 技能等级4;
    [EditorGroup("将领")] public byte 技能等级5;
    public byte 据点;
    public byte 方针;
    public byte 运输船;
    public byte 仇恨值;
    public short 移动目标;
    public byte empty1;
    public byte empty2;
    public short 行为方案;
    public short 改变回合;
    public byte 士气;
    public byte 士气持续回合;
    public byte 触发事件;
    public byte 盾牌标志;
    public int 固守最短距离;

    protected Army()
    {
        兵种 = 1;
        等级 = 1;
        编制 = 1;
        血量加成 = 100;
        当前血量 = 100;
        血量上限 = 100;
    }
}

public class Army1 : Army
{
    public static explicit operator Army1(Army2 a3)
    {
        return new()
        {
            坐标 = a3.坐标,
            兵种 = a3.兵种,
            等级 = a3.等级,
            编制 = a3.编制,
            方向 = a3.方向,
            移动力 = a3.移动力,
            建造回合 = a3.建造回合,
            兵种经验 = a3.兵种经验,
            血量加成 = a3.血量加成,
            当前血量 = a3.当前血量,
            血量上限 = a3.血量上限,
            将领 = a3.将领,
            军衔 = a3.军衔,
            爵位 = a3.爵位,
            胸章一 = a3.胸章一,
            胸章二 = a3.胸章二,
            胸章三 = a3.胸章三,
            技能等级1 = a3.技能等级1,
            技能等级2 = a3.技能等级2,
            技能等级3 = a3.技能等级3,
            技能等级4 = a3.技能等级4,
            技能等级5 = a3.技能等级5,
            据点 = a3.据点,
            方针 = a3.方针,
            运输船 = a3.运输船,
            仇恨值 = a3.仇恨值,
            移动目标 = a3.移动目标,
            empty1 = a3.empty1,
            empty2 = a3.empty2,
            行为方案 = a3.行为方案,
            改变回合 = a3.改变回合,
            士气 = a3.士气,
            士气持续回合 = a3.士气持续回合,
            触发事件 = a3.触发事件,
            盾牌标志 = a3.盾牌标志,
            固守最短距离 = a3.固守最短距离
        };
    }
}

public class Army2 : Army
{
    public byte empty3;
    public byte empty4;
    public byte empty5;
    public byte empty6;
    public byte empty7;
    public byte empty8;
    [EditorGroup("将领")] public short 勋带1;
    [EditorGroup("将领")] public short 勋带2;
    [EditorGroup("将领")] public short 勋带3;
    public int empty9;

    public static explicit operator Army2(Army1 a)
    {
        return new()
        {
            坐标 = a.坐标,
            兵种 = a.兵种,
            等级 = a.等级,
            编制 = a.编制,
            方向 = a.方向,
            移动力 = a.移动力,
            建造回合 = a.建造回合,
            兵种经验 = a.兵种经验,
            血量加成 = a.血量加成,
            当前血量 = a.当前血量,
            血量上限 = a.血量上限,
            将领 = a.将领,
            军衔 = a.军衔,
            爵位 = a.爵位,
            胸章一 = a.胸章一,
            胸章二 = a.胸章二,
            胸章三 = a.胸章三,
            技能等级1 = a.技能等级1,
            技能等级2 = a.技能等级2,
            技能等级3 = a.技能等级3,
            技能等级4 = a.技能等级4,
            技能等级5 = a.技能等级5,
            据点 = a.据点,
            方针 = a.方针,
            运输船 = a.运输船,
            仇恨值 = a.仇恨值,
            移动目标 = a.移动目标,
            empty1 = a.empty1,
            empty2 = a.empty2,
            行为方案 = a.行为方案,
            改变回合 = a.改变回合,
            士气 = a.士气,
            士气持续回合 = a.士气持续回合,
            触发事件 = a.触发事件,
            盾牌标志 = a.盾牌标志,
            固守最短距离 = a.固守最短距离,
            empty3 = 0,
            empty4 = 0,
            empty5 = 0,
            empty6 = 0,
            empty7 = 0,
            empty8 = 0,
            勋带1 = 0,
            勋带2 = 0,
            勋带3 = 0,
            empty9 = 0
        };
    }
}

public class Pitfall
{
    [EditorGroup(ignore: true)] public short 坐标;
    public short 所属军团;
    public short 陷阱编制;
    public int 陷阱血量;
    public short empty;
}

public class Scheme
{
    public int 方案编号;
    public int 终结事件;
    public int 触发回合;
    public int 目标地块;
}

public class Weather : ICloneable
{
    public int 天气类型;
    public int empty;
    public int 触发回合;
    public int 持续回合;
    public object Clone() => MemberwiseClone();
}

public class Affair : ICloneable
{
    public int 事件ID;
    public int 关联事件;
    public int 触发条件;
    public int 事件类型;
    public int 触发军团;
    public int 影响军团;
    public int 目标值;
    public int Zero;
    public int 触发回合;
    public int 对话代码;
    public int 默认结束段 = -858993664;
    public object Clone() => MemberwiseClone();
}

public class Reinforcement : ICloneable
{
    [EditorGroup(ignore: true)] public int 坐标;
    public int 兵种;
    public int 等级;
    public int 编制;
    public int 运输船;
    public int 方针;
    public int empty1;
    public int 将领;
    public int 军衔;
    public int 爵位;
    public int 技能等级1;
    public int 技能等级2;
    public int 技能等级3;
    public int 技能等级4;
    public int 技能等级5;
    public object Clone() => MemberwiseClone();
}

public class Reinforcement1 : Reinforcement
{
    public int 胸章一;
    public int 胸章二;
    public int 胸章三;
    public int 所属国家;
    public int 爆兵回合;

    public static explicit operator Reinforcement1(Reinforcement3 r2)
    {
        return new()
        {
            坐标 = r2.坐标,
            兵种 = r2.兵种,
            等级 = r2.等级,
            编制 = r2.编制,
            运输船 = r2.运输船,
            方针 = r2.方针,
            empty1 = r2.empty1,
            将领 = r2.将领,
            军衔 = r2.军衔,
            爵位 = r2.爵位,
            技能等级1 = r2.技能等级1,
            技能等级2 = r2.技能等级2,
            技能等级3 = r2.技能等级3,
            技能等级4 = r2.技能等级4,
            技能等级5 = r2.技能等级5,
            胸章一 = r2.勋章1,
            胸章二 = r2.勋章2,
            胸章三 = r2.勋章3,
            所属国家 = r2.所属国家,
            爆兵回合 = r2.爆兵回合
        };
    }
}

public class Reinforcement3 : Reinforcement
{
    [EditorGroup(ignore: true)] public int 勋章1;
    [EditorGroup(ignore: true)] public int 勋章2;
    [EditorGroup(ignore: true)] public int 勋章3;
    public int 所属国家;
    public int 爆兵回合;
    public int 胸章1;
    public int 胸章2;
    public int 胸章3;
    public int 勋带1;
    public int 勋带2;
    public int 勋带3;

    public static explicit operator Reinforcement3(Reinforcement1 r)
    {
        return new()
        {
            坐标 = r.坐标,
            兵种 = r.兵种,
            等级 = r.等级,
            编制 = r.编制,
            运输船 = r.运输船,
            方针 = r.方针,
            empty1 = r.empty1,
            将领 = r.将领,
            军衔 = r.军衔,
            爵位 = r.爵位,
            技能等级1 = r.技能等级1,
            技能等级2 = r.技能等级2,
            技能等级3 = r.技能等级3,
            技能等级4 = r.技能等级4,
            技能等级5 = r.技能等级5,
            勋章1 = r.胸章一,
            勋章2 = r.胸章二,
            勋章3 = r.胸章三,
            所属国家 = r.所属国家,
            爆兵回合 = r.爆兵回合,
            胸章1 = 0,
            胸章2 = 0,
            胸章3 = 0,
            勋带1 = 0,
            勋带2 = 0,
            勋带3 = 0
        };
    }
}

public class AirRaid : ICloneable
{
    [EditorGroup(ignore: true)] public int 坐标;
    public short 兵种;
    public short 精英兵种;
    public int 弹药;
    public int 军团;
    public int 回合;
    public object Clone() => MemberwiseClone();
}

public class ArmyPlacement : ICloneable
{
    public short 坐标;
    public short empty;
    public byte 方向;
    public byte Id;
    public byte 运输船;
    public byte empty2;
    public object Clone() => MemberwiseClone();
}

public class Capital
{
    public short 坐标;
    public short empty;
}

public class Strategy : ICloneable
{
    public int 军团序号;
    public int empty;
    public int 回合;
    public int 建设代码;
    public object Clone() => MemberwiseClone();
}

public class AirSupport : ICloneable
{
    public int 兵种;
    public int 弹药;
    public int 军团;
    public int 回合;
    public object Clone() => MemberwiseClone();
}