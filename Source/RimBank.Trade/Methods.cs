using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace RimBank.Trade
{
    public static class Methods
    {
        private static readonly string CopyrightStr = "RimBank A17,user19990313,Baidu Tieba&Ludeon forum";
        internal static bool debug = false;
        internal static List<Tradeable> cacheNotes = new List<Tradeable>();
        private static readonly FieldInfo tradeables = AccessTools.Field(typeof(TradeDeal), "tradeables");

        public static bool DoExecute(this TradeDeal This)
        {
            Pair<int, int> currencyFmt = Utility.GetCurrencyFmt();
            if (debug)
            {
                Log.Message(currencyFmt.First + "," + currencyFmt.Second);
            }
            UpdateCurrencyCount(This, currencyFmt);
            bool flag = false;
            foreach (Tradeable item in (List<Tradeable>)tradeables.GetValue(This))
            {
                if (item.ActionToDo != 0)
                {
                    flag = true;
                }
                item.ResolveTrade();
            }
            This.Reset();
            if (flag)
            {
                Utility.ResetCacheNotes();
            }
            return flag;
        }

        public static bool CanColonyAffordTrade(TradeDeal This)
        {
            int num = This.CurrencyTradeable.CountPostDealFor(Transactor.Colony);
            int notesBalanceAvaliable = Utility.GetNotesBalanceAvaliable(Transactor.Colony);
            if (num + notesBalanceAvaliable > 0)
            {
                return true;
            }
            return false;
        }

        public static void UpdateCurrencyCount(TradeDeal This, Pair<int, int> currencyfmt)
        {
            This.CurrencyTradeable.ForceTo(currencyfmt.Second);
            int num = Math.Abs(currencyfmt.First);
            Transactor transactor = (currencyfmt.First >= 0) ? Transactor.Trader : Transactor.Colony;
            for (int num2 = cacheNotes.Count - 1; num2 > -1; num2--)
            {
                int num3 = cacheNotes[num2].CountHeldBy(transactor);
                if (debug)
                {
                    Utility.DebugprintfUpdateCurrencyCount(num, num3, transactor);
                }
                if (num3 != 0)
                {
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
                    if (debug)
                    {
                        Utility.DebugprintfUpdateCurrencyCount(num, num3, transactor);
                    }
                    cacheNotes[num2].ForceTo(num3);
                    if (num == 0)
                    {
                        break;
                    }
                }
            }
            if (debug)
            {
                Utility.DebugOutputTradeables((List<Tradeable>)tradeables.GetValue(This));
            }
        }
    }
}