using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace RimBank.Trade;

public static class Methods
{
    internal const bool Debug = false;
    private static readonly string CopyrightStr = "RimBank A17,user19990313,Baidu Tieba&Ludeon forum";
    internal static List<Tradeable> CacheNotes = [];
    private static readonly FieldInfo tradeables = AccessTools.Field(typeof(TradeDeal), "tradeables");

    public static bool DoExecute(this TradeDeal deal)
    {
        var currencyFmt = Utility.GetCurrencyFmt();
        if (Debug)
        {
            Log.Message($"{currencyFmt.First},{currencyFmt.Second}");
        }

        UpdateCurrencyCount(deal, currencyFmt);
        var actionsToDo = false;
        foreach (var item in (List<Tradeable>)tradeables.GetValue(deal))
        {
            if (item.ActionToDo != 0)
            {
                actionsToDo = true;
            }

            item.ResolveTrade();
        }

        deal.Reset();
        if (actionsToDo)
        {
            Utility.ResetCacheNotes();
        }

        return actionsToDo;
    }

    public static bool CanColonyAffordTrade(TradeDeal deal)
    {
        var num = deal.CurrencyTradeable.CountPostDealFor(Transactor.Colony);
        var notesBalanceAvailable = Utility.GetNotesBalanceAvailable(Transactor.Colony);
        return num + notesBalanceAvailable > 0;
    }

    private static void UpdateCurrencyCount(TradeDeal deal, Pair<int, int> currencyfmt)
    {
        deal.CurrencyTradeable.ForceTo(currencyfmt.Second);
        var num = Math.Abs(currencyfmt.First);
        var transactor = currencyfmt.First >= 0 ? Transactor.Trader : Transactor.Colony;
        for (var num2 = CacheNotes.Count - 1; num2 > -1; num2--)
        {
            var num3 = CacheNotes[num2].CountHeldBy(transactor);
            if (Debug)
            {
                Utility.DebugprintfUpdateCurrencyCount(num, num3, transactor);
            }

            if (num3 == 0)
            {
                continue;
            }

            if (num < num3)
            {
                num3 = num;
                num = 0;
            }
            else
            {
                num -= num3;
            }

            if (transactor == Transactor.Colony)
            {
                num3 = -num3;
            }

            if (Debug)
            {
                Utility.DebugprintfUpdateCurrencyCount(num, num3, transactor);
            }

            CacheNotes[num2].ForceTo(num3);
            if (num == 0)
            {
                break;
            }
        }

        if (Debug)
        {
            Utility.DebugOutputTradeables((List<Tradeable>)tradeables.GetValue(deal));
        }
    }
}