using System;
using RimBank.Trade.Ext;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimBank.Trade;

public class Dialog_PayByBankNotes : Window
{
    private readonly bool isVirtual;
    private readonly Tradeable notesTradeable;
    private readonly Tradeable silverTradeable;

    public Dialog_PayByBankNotes(int expense, int playersilver, int playernotes, int tradersilver, int tradernotes,
        bool isVirtual)
    {
        forcePause = true;
        silverTradeable = new Tradeable();
        notesTradeable = new Tradeable();
        this.isVirtual = isVirtual;

        var item = new Thing
        {
            def = ThingDefOf.Silver,
            stackCount = playersilver
        };
        silverTradeable.thingsColony.Add(item);

        item = new Thing
        {
            def = ThingDefOf.Silver,
            stackCount = tradersilver
        };
        silverTradeable.thingsTrader.Add(item);

        item = new Thing
        {
            def = BankDefOf.BankNote,
            stackCount = playernotes
        };
        notesTradeable.thingsColony.Add(item);

        item = new Thing
        {
            def = BankDefOf.BankNote,
            stackCount = tradernotes
        };
        notesTradeable.thingsTrader.Add(item);
        silverTradeable.ForceTo(expense);
    }

    public override Vector2 InitialSize => new Vector2(479f, 270f);
    private VirtualTrader VirtualTrader => (VirtualTrader)TradeSession.trader;
    public Pair<int, int> CurrencyFmt { get; private set; } = new Pair<int, int>(0, 0);

    public override void DoWindowContents(Rect inRect)
    {
        var rect = new Rect(inRect.x, inRect.y, inRect.width, 45f);
        Text.Anchor = TextAnchor.MiddleCenter;
        Text.Font = GameFont.Medium;
        if (isVirtual)
        {
            Widgets.Label(rect, VirtualTrader.TipString(1));
            TooltipHandler.TipRegion(rect, VirtualTrader.TipString(2));
        }
        else
        {
            Widgets.Label(rect, "AdjustPayment".Translate());
        }

        var height = rect.height;
        var color = GUI.color;
        GUI.color = Color.gray;
        Widgets.DrawLineHorizontal(inRect.x, height, inRect.width);
        GUI.color = color;
        height += 2.5f;
        var rect2 = new Rect(inRect.x, height, inRect.width, 30f);
        DrawTradeableRow(rect2, silverTradeable, 1);
        var countToTransfer = notesTradeable.CountToTransfer;
        var countToTransfer2 = silverTradeable.CountToTransfer;
        var rect3 = new Rect(inRect.x, height + 30f, inRect.width, 30f);
        DrawTradeableRow(rect3, notesTradeable, 2);
        if (countToTransfer != notesTradeable.CountToTransfer)
        {
            if (!isVirtual || !VirtualTrader.UniqueBalanceMethod)
            {
                var countToTransfer3 = silverTradeable.CountToTransfer;
                silverTradeable.ForceTo(countToTransfer3 +
                                        ((countToTransfer - notesTradeable.CountToTransfer) * 1000));
            }
            else
            {
                var notes = notesTradeable.CountToTransfer;
                var silver = silverTradeable.CountToTransfer;
                VirtualTrader.BalanceMethod(countToTransfer2, countToTransfer, ref silver, ref notes);
                notesTradeable.ForceTo(notes);
                silverTradeable.ForceTo(silver);
            }
        }

        var rect4 = new Rect(rect2.x, rect2.y, 27f, 27f);
        if (Mouse.IsOver(rect4))
        {
            Widgets.DrawHighlight(rect4);
        }

        Widgets.ThingIcon(rect4, silverTradeable.AnyThing);
        var rect5 = new Rect(rect2.x, rect3.y, 27f, 27f);
        if (Mouse.IsOver(rect5))
        {
            Widgets.DrawHighlight(rect5);
        }

        Widgets.ThingIcon(rect5, notesTradeable.AnyThing);
        if (isVirtual)
        {
            TooltipHandler.TipRegion(rect4, VirtualTrader.TipString(3));
            TooltipHandler.TipRegion(rect5, VirtualTrader.TipString(4));
        }
        else
        {
            TooltipHandler.TipRegion(rect4, "SilverTip".Translate());
            TooltipHandler.TipRegion(rect5, "BankNoteTip".Translate());
        }

        var num = 120f;
        var height2 = 40f;
        if (Widgets.ButtonText(new Rect(inRect.width * 9f / 16f, inRect.height - 55f, num, height2),
                "CancelButton".Translate()))
        {
            Event.current.Use();
            Close(false);
        }

        if (!Widgets.ButtonText(new Rect((inRect.width * 7f / 16f) - num, inRect.height - 55f, num, height2),
                "AcceptButton".Translate()))
        {
            return;
        }

        void ExecuteTrade()
        {
            CurrencyFmt = new Pair<int, int>(notesTradeable.CountToTransfer, silverTradeable.CountToTransfer);
            if (TradeSession.deal.DoExecute())
            {
                SoundDefOf.ExecuteTrade.PlayOneShotOnCamera();
                if (TradeSession.trader is Pawn pawn)
                {
                    TaleRecorder.RecordTale(TaleDefOf.TradedWith, TradeSession.playerNegotiator, pawn);
                }

                if (isVirtual)
                {
                    VirtualTrader.CloseTradeUI();
                }
                else
                {
                    Find.WindowStack.WindowOfType<Dialog_Trade>().Close(false);
                }
            }

            Close(false);
        }

        void ConfirmedExecuteTrade()
        {
            silverTradeable.ForceTo(silverTradeable.CountHeldBy(Transactor.Trader));
            ExecuteTrade();
        }

        ((Action)delegate
        {
            if (!TestPlayerSilver())
            {
                Messages.Message("NotEnoughSilverColony".Translate(), MessageTypeDefOf.RejectInput);
            }
            else if (isVirtual && VirtualTrader.CustomCheckViolation(silverTradeable, notesTradeable))
            {
                VirtualTrader.CustomViolationAction();
            }
            else if (!TestTraderSilver())
            {
                SoundDefOf.ClickReject.PlayOneShotOnCamera();
                Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmTraderShortFunds".Translate(),
                    ConfirmedExecuteTrade));
            }
            else
            {
                ExecuteTrade();
            }
        })();
        Event.current.Use();
    }

    public void DrawTradeableRow(Rect rect, Tradeable trad, int index)
    {
        if (index == 1)
        {
            Widgets.DrawLightHighlight(rect);
        }

        Text.Font = GameFont.Small;
        GUI.BeginGroup(rect);
        var width = rect.width;
        var num = trad.CountHeldBy(Transactor.Trader);
        if (num != 0)
        {
            var rect2 = new Rect(width - 75f, 0f, 75f, rect.height);
            if (Mouse.IsOver(rect2))
            {
                Widgets.DrawHighlight(rect2);
            }

            Text.Anchor = TextAnchor.MiddleRight;
            var rect3 = rect2;
            rect3.xMin += 5f;
            rect3.xMax -= 5f;
            Widgets.Label(rect3, num.ToStringCached());
            TooltipHandler.TipRegion(rect2, "TraderCount".Translate());
        }

        width -= 85f;
        var rect4 = new Rect(width - 240f, 0f, 240f, rect.height);
        if (index == 2 && notesTradeable.CountHeldBy(Transactor.Colony) == 0 &&
            notesTradeable.CountHeldBy(Transactor.Trader) == 0)
        {
            Text.Anchor = TextAnchor.MiddleCenter;
            var color = GUI.color;
            GUI.color = Color.gray;
            Widgets.Label(rect4, "NoNotes".Translate());
            GUI.color = color;
        }
        else if (index == 1 && isVirtual && VirtualTrader.SilverAlsoAdjustable)
        {
            ExtUtil.DoCountAdjustInterfaceForSilver(rect4, trad, index, -trad.CountHeldBy(Transactor.Colony),
                trad.CountHeldBy(Transactor.Trader), false);
        }
        else
        {
            TransferableUIUtility.DoCountAdjustInterface(rect4, trad, index, -trad.CountHeldBy(Transactor.Colony),
                trad.CountHeldBy(Transactor.Trader));
        }

        width -= 240f;
        var num2 = trad.CountHeldBy(Transactor.Colony);
        if (num2 != 0)
        {
            var rect5 = new Rect(width - 75f - 10f, 0f, 75f, rect.height);
            if (Mouse.IsOver(rect5))
            {
                Widgets.DrawHighlight(rect5);
            }

            Text.Anchor = TextAnchor.MiddleLeft;
            var rect6 = rect5;
            rect6.xMin += 5f;
            rect6.xMax -= 5f;
            Widgets.Label(rect6, num2.ToStringCached());
            TooltipHandler.TipRegion(rect5, "ColonyCount".Translate());
        }

        GenUI.ResetLabelAlign();
        GUI.EndGroup();
    }

    private bool TestPlayerSilver()
    {
        return silverTradeable.CountPostDealFor(Transactor.Colony) >= 0;
    }

    private bool TestTraderSilver()
    {
        return silverTradeable.CountPostDealFor(Transactor.Trader) >= 0;
    }
}