using HarmonyLib;
using RimWorld;
using Verse;

namespace RimBank.Trade;

[HarmonyPatch(typeof(TradeShip), nameof(TradeShip.GenerateThings))]
public static class TradeShip_GenerateThings
{
    public static void Postfix(ref ThingOwner ___things)
    {
        var thing = ThingMaker.MakeThing(BankDefOf.BankNote);
        thing.stackCount = Rand.Range(8, 16);
        ___things.TryAdd(thing);
    }
}