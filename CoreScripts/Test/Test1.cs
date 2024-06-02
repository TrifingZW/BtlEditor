using System;
using System.Reflection;
using BtlEditor.CoreScripts.Structures;

namespace BtlEditor.CoreScripts.Test;

public static class Test1
{
    public static void Test()
    {
        /*Tr(typeof(Master));
        Tr(typeof(Country));
        Tr(typeof(Topography));
        Tr(typeof(City));
        Tr(typeof(Army2));
        Tr(typeof(Pitfall));
        Tr(typeof(Scheme));
        Tr(typeof(Weather));
        Tr(typeof(Affair));
        Tr(typeof(Pitfall));
        Tr(typeof(Reinforcement3));
        Tr(typeof(AirRaid));
        Tr(typeof(ArmyPlacement));
        Tr(typeof(Capital));
        Tr(typeof(Strategy));
        Tr(typeof(AirSupport));*/
        Tr(typeof(Army));
        Tr(typeof(Reinforcement));
        
        void Tr(IReflect reflect)
        {
            foreach (FieldInfo fieldInfo in reflect.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                Console.WriteLine(fieldInfo.Name);
            }
        }
    }
}