using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RimBank.Trade;

[HarmonyPatch(typeof(Settlement_TraderTracker), "RegenerateStock")]
public static class Settlement_TraderTracker_RegenerateStock
{
    public static void Postfix(Settlement_TraderTracker __instance, ref ThingOwner<Thing> ___stock)
    {
        var thing = ThingMaker.MakeThing(BankDefOf.BankNote);
        var min = 0;
        var max = 0;
        switch (__instance.settlement.Faction.def.techLevel)
        {
            case TechLevel.Neolithic:
                min = 0;
                max = 5;
                break;

            case TechLevel.Medieval:
                min = 3;
                max = 7;
                break;

            case TechLevel.Industrial:
                min = 4;
                max = 11;
                break;

            case TechLevel.Spacer:
            case TechLevel.Ultra:
                min = 7;
                max = 14;
                break;

            case TechLevel.Archotech:
                min = 9;
                max = 16;
                break;
        }

        thing.stackCount = Rand.Range(min, max);
        ___stock.TryAdd(thing);
    }
}