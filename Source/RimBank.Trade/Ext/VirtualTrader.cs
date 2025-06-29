﻿using System;
using System.Collections.Generic;
using RimWorld;
using Verse;

namespace RimBank.Trade.Ext;

public class VirtualTrader : ITrader
{
    public virtual bool UniqueBalanceMethod => false;
    public virtual bool SilverAlsoAdjustable => false;
    public virtual bool CanTradeNow => true;
    public Faction Faction => null;

    public virtual IEnumerable<Thing> Goods => new List<Thing>
    {
        new()
        {
            def = BankDefOf.BankNote,
            stackCount = 50
        },
        new()
        {
            def = ThingDefOf.Silver,
            stackCount = 50000
        }
    };

    public int RandomPriceFactorSeed => 1;

    public float TradePriceImprovementOffsetForPlayer => 0f;

    public virtual TraderKindDef TraderKind =>
        DefDatabase<TraderKindDef>.GetNamed("Orbital_BulkGoods"); // Dummy trader def

    public virtual string TraderName => "Virtual Trader";

    public TradeCurrency TradeCurrency => TradeCurrency.Silver;

    public virtual IEnumerable<Thing> ColonyThingsWillingToBuy(Pawn playerNegotiator)
    {
        foreach (var item in TradeUtility.AllLaunchableThingsForTrade(Find.CurrentMap))
        {
            if (item.def == BankDefOf.BankNote || item.def == ThingDefOf.Silver)
            {
                yield return item;
            }
        }
    }

    public virtual void GiveSoldThingToPlayer(Thing toGive, int countToGive, Pawn playerNegotiator)
    {
        var thing = toGive.SplitOff(countToGive);
        thing.PreTraded(TradeAction.PlayerBuys, playerNegotiator, this);
        TradeUtility.SpawnDropPod(DropCellFinder.TradeDropSpot(Find.CurrentMap), Find.CurrentMap, thing);
    }

    public virtual void GiveSoldThingToTrader(Thing toGive, int countToGive, Pawn playerNegotiator)
    {
        var thing = toGive.SplitOff(countToGive);
        thing.PreTraded(TradeAction.PlayerSells, playerNegotiator, this);
        var thing2 = TradeUtility.ThingFromStockToMergeWith(this, thing);
        if (thing2 != null && !thing2.TryAbsorbStack(thing, false))
        {
            thing.Destroy();
        }
    }

    public virtual void InvokeTradeUI()
    {
        throw new NotImplementedException("Direct call of VirtualTrader.InvokeTradeUI()");
    }

    public virtual void CloseTradeUI()
    {
    }

    public virtual string TipString(int index)
    {
        return "";
    }

    public virtual void BalanceMethod(int silver, int notes, ref int silver2, ref int notes2)
    {
    }

    public virtual bool CustomCheckViolation(Tradeable silver, Tradeable notes)
    {
        return false;
    }

    public virtual void CustomViolationAction()
    {
    }

    public virtual Pair<int, int> GetCurrencyFmt()
    {
        throw new InvalidOperationException(
            "Direct call of VirtualTrader.GetCurrencyFmt(),System lost ctrl because therere no Dialog_PayByBankNotes and derived type doesnt override this function.");
    }
}