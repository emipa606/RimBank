using System.Reflection;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimBank.Trade.Ext;

[StaticConstructorOnStartup]
public static class ExtUtil
{
    private static readonly Texture tradeArrow = (Texture)typeof(TransferableUIUtility)
        .GetField("TradeArrow", BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic)
        ?.GetValue(null);

    public static void PrepareVirtualTrade(Pawn pawn, VirtualTrader trader)
    {
        TradeSession.SetupWith(trader, pawn, false);
        trader.InvokeTradeUI();
    }

    public static float BrokerageFactor(int a)
    {
        if (a <= 0)
        {
            return -0.035f;
        }

        return 0.015f;
    }

    public static void DoCountAdjustInterfaceForSilver(Rect rect, Transferable trad, int _, int min, int max,
        bool flash)
    {
        rect = rect.Rounded();
        var rect2 = new Rect(rect.center.x - 45f, rect.center.y - 12.5f, 90f, 25f).Rounded();
        if (flash)
        {
            GUI.DrawTexture(rect2, TransferableUIUtility.FlashTex);
        }

        var num = trad is TransferableOneWay { HasAnyThing: true, AnyThing: Pawn, MaxCount: 1 };
        if (num)
        {
            var checkOn = trad.CountToTransfer != 0;
            Widgets.Checkbox(rect2.position, ref checkOn);
            if (checkOn != (trad.CountToTransfer != 0))
            {
                trad.AdjustTo(checkOn ? trad.GetMaximumToTransfer() : trad.GetMinimumToTransfer());
            }
        }
        else
        {
            var rect3 = rect2.ContractedBy(2f);
            rect3.xMax -= 15f;
            rect3.xMin += 16f;
            var val = trad.CountToTransfer;
            var buffer = trad.EditBuffer;
            Widgets.TextFieldNumeric(rect3, ref val, ref buffer, min, max);
            trad.AdjustTo(val);
            trad.EditBuffer = buffer;
        }

        Text.Anchor = TextAnchor.UpperLeft;
        GUI.color = Color.white;
        if (!num)
        {
            var num2 = trad.PositiveCountDirection == TransferablePositiveCountDirection.Source ? 1 : -1;
            var num3 = GenUI.CurrentAdjustmentMultiplier();
            if (trad.CanAdjustBy(num2 * num3).Accepted)
            {
                var rect4 = new Rect(rect2.x - 30f, rect.y, 30f, rect.height);
                if (trad.GetRange() == 1)
                {
                    rect4.x -= rect4.width;
                    rect4.width += rect4.width;
                }

                if (Widgets.ButtonText(rect4, "<"))
                {
                    trad.AdjustBy(num2 * num3);
                    SoundDefOf.Tick_High.PlayOneShotOnCamera();
                }

                if (trad.GetRange() != 1)
                {
                    var label = "<<";
                    rect4.x -= rect4.width;
                    if (Widgets.ButtonText(rect4, label))
                    {
                        trad.AdjustTo(num2 == 1 ? trad.GetMaximumToTransfer() : trad.GetMinimumToTransfer());

                        SoundDefOf.Tick_High.PlayOneShotOnCamera();
                    }
                }
            }

            if (trad.CanAdjustBy(-num2 * num3).Accepted)
            {
                var rect5 = new Rect(rect2.xMax, rect.y, 30f, rect.height);
                if (trad.GetRange() == 1)
                {
                    rect5.width += rect5.width;
                }

                if (Widgets.ButtonText(rect5, ">"))
                {
                    trad.AdjustBy(-num2 * num3);
                    SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                }

                if (trad.GetRange() != 1)
                {
                    var label2 = ">>";
                    rect5.x += rect5.width;
                    if (Widgets.ButtonText(rect5, label2))
                    {
                        trad.AdjustTo(num2 == 1 ? trad.GetMinimumToTransfer() : trad.GetMaximumToTransfer());

                        SoundDefOf.Tick_Low.PlayOneShotOnCamera();
                    }
                }
            }
        }

        if (trad.CountToTransfer == 0)
        {
            return;
        }

        var position = new Rect(rect2.x + (rect2.width / 2f) - (tradeArrow.width / 2f),
            rect2.y + (rect2.height / 2f) - (tradeArrow.height / 2f), tradeArrow.width, tradeArrow.height);
        var positiveCountDirection = trad.PositiveCountDirection;
        if (positiveCountDirection == TransferablePositiveCountDirection.Source && trad.CountToTransfer > 0 ||
            positiveCountDirection == TransferablePositiveCountDirection.Destination &&
            trad.CountToTransfer < 0)
        {
            position.x += position.width;
            position.width *= -1f;
        }

        GUI.DrawTexture(position, tradeArrow);
    }
}