using HarmonyLib;
using Verse;

namespace RimBank.Trade;

[StaticConstructorOnStartup]
internal static class DetourInjectorCompact
{
    static DetourInjectorCompact()
    {
        var harmony = new Harmony("user19990313.RimBank-Unofficial");
        harmony.PatchAll();
    }
}