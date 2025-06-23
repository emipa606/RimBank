using System;
using System.Collections.Generic;
using RimBank.Trade.Ext;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimBank.Core.Interactive;

public static class FloatMenuManager
{
    private static readonly Dictionary<string, Action<Pawn>> rawItems;

    private static readonly Dictionary<string, bool> usesDefaultJobDriver;

    internal static Action<Pawn> CurrentAction;

    private static readonly Dictionary<string, Action> shiftKeyItems;

    static FloatMenuManager()
    {
        rawItems = new Dictionary<string, Action<Pawn>>();
        usesDefaultJobDriver = new Dictionary<string, bool>();
        CurrentAction = null;
        shiftKeyItems = new Dictionary<string, Action>();
        add("FloatMenuCaptionExchange".Translate(),
            delegate(Pawn p) { ExtUtil.PrepareVirtualTrade(p, new Trader_BankNoteExchange()); }, true);
        addShiftKeyItem("FloatMenuCaptionRemoveAll".Translate(), delegate
        {
            Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("DlgRemoveModContents".Translate(), delegate
            {
                var num = 0;
                num += ModContentRemover.RemoveAllModContentsFromWorldObjects();
                num += ModContentRemover.RemoveAllModContentsFromPassingShips();
                num += ModContentRemover.RemoveAllModContentsFromMaps();
                num += ModContentRemover.RemoveAllModContentsFromAllPawns();
                Messages.Message("MsgModContentsRemoved".Translate(num), MessageTypeDefOf.PositiveEvent);
            }, true, "DlgTitleRemoveModContents".Translate()));
        });
    }

    private static void add(string str, Action<Pawn> action, bool defaultJobDriver = false)
    {
        rawItems.Add(str, action);
        usesDefaultJobDriver.Add(str, defaultJobDriver);
    }

    private static void addShiftKeyItem(string str, Action action)
    {
        shiftKeyItems.Add(str, action);
    }

    public static IEnumerable<FloatMenuOption> RequestBuild(Building target, Pawn pawn)
    {
        var list = new List<FloatMenuOption>();
        foreach (var pair in rawItems)
        {
            list.Add(new FloatMenuOption(pair.Key, delegate
            {
                if (usesDefaultJobDriver[pair.Key])
                {
                    CurrentAction = pair.Value;
                    var job = new Job(CoreDefOf.UseBankTerminal, target);
                    pawn.jobs.TryTakeOrderedJob(job);
                }
                else
                {
                    pair.Value(pawn);
                }
            }));
        }

        if (!Input.GetKey(KeyCode.LeftShift) && !Input.GetKey(KeyCode.RightShift))
        {
            return list;
        }

        foreach (var shiftKeyItem in shiftKeyItems)
        {
            list.Add(new FloatMenuOption(shiftKeyItem.Key, shiftKeyItem.Value));
        }

        return list;
    }
}