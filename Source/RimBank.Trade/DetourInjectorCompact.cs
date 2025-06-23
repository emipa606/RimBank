using System.Reflection;
using HarmonyLib;
using Verse;

namespace RimBank.Trade;

[StaticConstructorOnStartup]
internal static class DetourInjectorCompact
{
    static DetourInjectorCompact()
    {
        new Harmony("user19990313.RimBank-Unofficial").PatchAll(Assembly.GetExecutingAssembly());
    }
}