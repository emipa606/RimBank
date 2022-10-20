using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace RimBank.Trade.Ext;

public class Trader_BankNoteExchange : VirtualTrader
{
    public override string TraderName => "Bank";

    public override bool UniqueBalanceMethod => true;

    public override IEnumerable<Thing> Goods
    {
        get
        {
            var thing = ThingMaker.MakeThing(ThingDefOf.Silver);
            thing.stackCount = 50000;
            yield return thing;

            thing = ThingMaker.MakeThing(BankDefOf.BankNote);
            thing.stackCount = 50;
            yield return thing;
        }
    }

    public override IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
    {
        foreach (var item in TradeUtility.AllLaunchableThingsForTrade(Find.CurrentMap))
        {
            if (item.def == BankDefOf.BankNote || item.def == ThingDefOf.Silver)
            {
                yield return item;
            }
        }
    }

    public override void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
    {
        var thing = toGive.SplitOff(countToGive);
        thing.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this);
        TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(Find.CurrentMap), Find.CurrentMap, thing);
    }

    public override void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
    {
        var thing = toGive.SplitOff(countToGive);
        thing.PreTraded(TradeAction.PlayerSells, playerNegotiator, this);
        var thing2 = TradeUtility.ThingFromStockToMergeWith(this, thing);
        if (thing2 != null && !thing2.TryAbsorbStack(thing, false))
        {
            thing.Destroy();
        }
    }

    public override void InvokeTradeUI()
    {
        Methods.cacheNotes = (from x in TradeSession.deal.AllTradeables
            where x.ThingDef == BankDefOf.BankNote
            orderby x.AnyThing.HitPoints descending
            select x).ToList();
        if (Methods.debug)
        {
            Utility.DebugOutputNotes();
            Utility.DebugOutputTradeables(TradeSession.deal.AllTradeables.ToList());
        }

        Utility.AskPayByBankNotes(TradeSession.deal.CurrencyTradeable, true);
    }

    public override string TipString(int index)
    {
        switch (index)
        {
            case 1:
                return "ExchangeTitle".Translate();

            case 2:
                return "ExchangeTip".Translate();

            case 3:
                return "ExchangeSilverTip".Translate();

            case 4:
                return "ExchangeBankNoteTip".Translate();

            default:
                return "BUGGED!";
        }
    }

    public override void BalanceMethod(int silver, int notes, ref int silver2, ref int notes2)
    {
        silver2 = (-notes2 * 1000) - (int)(notes2 * ExtUtil.BrokerageFactor(notes2) * 1000f);
    }

    public override bool CustomCheckViolation(Tradeable silver, Tradeable notes)
    {
        return silver.CountPostDealFor(Transactor.Trader) < 0;
    }

    public override void CustomViolationAction()
    {
        Messages.Message("NotEnoughSilverBank".Translate(), MessageTypeDefOf.RejectInput);
    }
}