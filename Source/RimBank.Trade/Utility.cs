using RimBank.Trade.Ext;
using RimWorld;
using System.Collections.Generic;
using Verse;

namespace RimBank.Trade
{
    public static class Utility
    {
        internal static void ResetCacheNotes()
        {
            Methods.cacheNotes = new List<Tradeable>();
        }

        internal static int GetNotesBalanceAvaliable(Transactor trans)
        {
            return GetNotesCountAvaliable(trans) * 1000;
        }

        internal static int GetNotesCountAvaliable(Transactor trans)
        {
            int num = 0;
            foreach (Tradeable cacheNote in Methods.cacheNotes)
            {
                num += cacheNote.CountHeldBy(trans);
            }
            return num;
        }

        public static void DebugOutputNotes()
        {
            foreach (Tradeable cacheNote in Methods.cacheNotes)
            {
                Log.Message(cacheNote.ThingDef.defName + ",colony=" + cacheNote.CountHeldBy(Transactor.Colony) + ",trader=" + cacheNote.CountHeldBy(Transactor.Trader) + ",dura=" + cacheNote.AnyThing.HitPoints.ToString());
            }
        }

        public static void DebugOutputTradeables(List<Tradeable> cache)
        {
            foreach (Tradeable item in cache)
            {
                Log.Message(item.ThingDef.defName + ",x" + item.CountHeldBy(Transactor.Colony).ToString() + ",dura=" + item.AnyThing.HitPoints.ToString() + ",cnt=" + item.CountToTransfer);
            }
        }

        public static void DebugprintfUpdateCurrencyCount(int num, int j, Transactor target)
        {
            Log.Message("num=" + num + ",target=" + target + ",j=" + j);
        }

        public static void AskPayByBankNotes(Tradeable currency, bool isVirtual = false)
        {
            if (!isVirtual && !Methods.CanColonyAffordTrade(TradeSession.deal))
            {
                Find.WindowStack.WindowOfType<Dialog_Trade>().FlashSilver();
                Messages.Message("MessageColonyCannotAfford".Translate(), MessageTypeDefOf.RejectInput);
                return;
            }
            int playersilver = currency.CountHeldBy(Transactor.Colony);
            int notesCountAvaliable = GetNotesCountAvaliable(Transactor.Colony);
            int countToTransfer = currency.CountToTransfer;
            int num = 0;
            for (int num2 = Methods.cacheNotes.Count - 1; num2 > -1; num2--)
            {
                num += Methods.cacheNotes[num2].CountHeldBy(Transactor.Trader);
            }
            Find.WindowStack.Add(new Dialog_PayByBankNotes(countToTransfer, playersilver, notesCountAvaliable, currency.CountHeldBy(Transactor.Trader), num, isVirtual));
        }

        internal static Pair<int, int> GetCurrencyFmt()
        {
            return Find.WindowStack.WindowOfType<Dialog_PayByBankNotes>()?.CurrencyFmt ?? ((VirtualTrader)TradeSession.trader).GetCurrencyFmt();
        }
    }
}