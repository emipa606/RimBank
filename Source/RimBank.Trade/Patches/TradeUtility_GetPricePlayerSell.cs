using HarmonyLib;
using RimWorld;
using Verse;

namespace RimBank.Trade;

[HarmonyPatch(typeof(TradeUtility), nameof(TradeUtility.GetPricePlayerSell))]
public static class TradeUtility_GetPricePlayerSell
{
    public static void Postfix(ref float __result, Thing thing, TradeCurrency currency)
    {
        if (currency == TradeCurrency.Silver && thing.def == BankDefOf.BankNote)
        {
            __result = thing.MarketValue;
        }
    }
}