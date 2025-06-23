using System.Collections.Generic;
using RimBank.Trade.Ext;
using RimWorld;
using Verse;

namespace RimBank.Trade;

public static class Utility
{
    internal static void ResetCacheNotes()
    {
        Methods.CacheNotes = [];
    }

    internal static int GetNotesBalanceAvailable(Transactor trans)
    {
        return getNotesCountAvailable(trans) * 1000;
    }

    private static int getNotesCountAvailable(Transactor trans)
    {
        var num = 0;
        foreach (var cacheNote in Methods.CacheNotes)
        {
            num += cacheNote.CountHeldBy(trans);
        }

        return num;
    }

    public static void DebugOutputNotes()
    {
        foreach (var cacheNote in Methods.CacheNotes)
        {
            Log.Message(
                $"{cacheNote.ThingDef.defName},colony={cacheNote.CountHeldBy(Transactor.Colony)},trader={cacheNote.CountHeldBy(Transactor.Trader)},dura={cacheNote.AnyThing.HitPoints}");
        }
    }

    public static void DebugOutputTradeables(List<Tradeable> cache)
    {
        foreach (var item in cache)
        {
            Log.Message(
                $"{item.ThingDef.defName},x{item.CountHeldBy(Transactor.Colony)},dura={item.AnyThing.HitPoints},cnt={item.CountToTransfer}");
        }
    }

    public static void DebugprintfUpdateCurrencyCount(int num, int j, Transactor target)
    {
        Log.Message($"num={num},target={target},j={j}");
    }

    public static void AskPayByBankNotes(Tradeable currency, bool isVirtual = false)
    {
        if (!isVirtual && !Methods.CanColonyAffordTrade(TradeSession.deal))
        {
            Find.WindowStack.WindowOfType<Dialog_Trade>().FlashSilver();
            Messages.Message("MessageColonyCannotAfford".Translate(), MessageTypeDefOf.RejectInput);
            return;
        }

        var playerSilver = currency.CountHeldBy(Transactor.Colony);
        var notesCountAvailable = getNotesCountAvailable(Transactor.Colony);
        var countToTransfer = currency.CountToTransfer;
        var num = 0;
        for (var num2 = Methods.CacheNotes.Count - 1; num2 > -1; num2--)
        {
            num += Methods.CacheNotes[num2].CountHeldBy(Transactor.Trader);
        }

        Find.WindowStack.Add(new Dialog_PayByBankNotes(countToTransfer, playerSilver, notesCountAvailable,
            currency.CountHeldBy(Transactor.Trader), num, isVirtual));
    }

    internal static Pair<int, int> GetCurrencyFmt()
    {
        return Find.WindowStack.WindowOfType<Dialog_PayByBankNotes>()?.CurrencyFmt ??
               ((VirtualTrader)TradeSession.trader).GetCurrencyFmt();
    }
}