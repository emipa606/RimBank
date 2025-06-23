using System.Reflection;
using RimBank.Trade;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace RimBank.Core;

internal class ModContentRemover
{
    private static readonly string copyrightStr = "RimBank A17,user19990313,Baidu Tieba&Ludeon forum";

    internal static int RemoveAllModContentsFromWorldObjects()
    {
        var num = 0;
        var field = typeof(Settlement_TraderTracker).GetField("stock",
            BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var settlement in Find.WorldObjects.Settlements)
        {
            if (settlement.trader == null || field?.GetValue(settlement.trader) == null)
            {
                continue;
            }

            for (var num2 = settlement.trader.StockListForReading.Count - 1; num2 > -1; num2--)
            {
                if (settlement.trader.StockListForReading[num2].def != BankDefOf.BankNote &&
                    settlement.trader.StockListForReading[num2].GetInnerIfMinified().def !=
                    CoreDefOf.RimBankBuildingTerminal)
                {
                    continue;
                }

                num += settlement.trader.StockListForReading[num2].stackCount;
                settlement.trader.StockListForReading.RemoveAt(num2);
            }
        }

        foreach (var caravan in Find.WorldObjects.Caravans)
        {
            foreach (var item in caravan.PawnsListForReading)
            {
                if (item.inventory == null || item.inventory.innerContainer.Count <= 0)
                {
                    continue;
                }

                for (var num3 = item.inventory.innerContainer.Count - 1; num3 > -1; num3--)
                {
                    if (item.inventory.innerContainer[num3].def != BankDefOf.BankNote &&
                        item.inventory.innerContainer[num3].GetInnerIfMinified().def !=
                        CoreDefOf.RimBankBuildingTerminal)
                    {
                        continue;
                    }

                    num += item.inventory.innerContainer[num3].stackCount;
                    item.inventory.innerContainer.RemoveAt(num3);
                }
            }
        }

        return num;
    }

    internal static int RemoveAllModContentsFromPassingShips()
    {
        var num = 0;
        var field = typeof(TradeShip).GetField("things", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (var map in Find.Maps)
        {
            foreach (var passingShip in map.passingShipManager.passingShips)
            {
                if (passingShip is not TradeShip tradeShip)
                {
                    continue;
                }

                var thingOwner = (ThingOwner<Thing>)field?.GetValue(tradeShip);
                if (thingOwner == null)
                {
                    continue;
                }

                for (var num2 = thingOwner.Count - 1; num2 > -1; num2--)
                {
                    if (thingOwner[num2].def != BankDefOf.BankNote && thingOwner[num2].GetInnerIfMinified().def !=
                        CoreDefOf.RimBankBuildingTerminal)
                    {
                        continue;
                    }

                    num += thingOwner[num2].stackCount;
                    thingOwner.RemoveAt(num2);
                }

                field.SetValue(tradeShip, thingOwner);
            }
        }

        return num;
    }

    internal static int RemoveAllModContentsFromMaps()
    {
        var num = 0;
        foreach (var map in Find.Maps)
        {
            foreach (var zone in map.zoneManager.AllZones)
            {
                if (zone is not Zone_Stockpile stockpile)
                {
                    continue;
                }

                stockpile.settings?.filter?.SetAllow(BankDefOf.BankNote, false);
                stockpile.settings?.filter?.SetAllow(CoreDefOf.RimBankBuildingTerminal, false);
            }

            for (var num2 = map.spawnedThings.Count - 1; num2 > -1; num2--)
            {
                if (map.spawnedThings[num2].def != BankDefOf.BankNote &&
                    map.spawnedThings[num2].GetInnerIfMinified().def != CoreDefOf.RimBankBuildingTerminal)
                {
                    continue;
                }

                var thing = map.spawnedThings[num2];
                num += thing.stackCount;
                thing.DeSpawn();
                if (!thing.Destroyed)
                {
                    thing.Destroy();
                }

                if (!thing.Discarded)
                {
                    thing.Discard();
                }
            }
        }

        return num;
    }

    internal static int RemoveAllModContentsFromAllPawns()
    {
        var num = 0;
        foreach (var item in Find.WorldPawns.AllPawnsAliveOrDead)
        {
            if (item.inventory == null || !item.inventory.innerContainer.Any)
            {
                continue;
            }

            for (var num2 = item.inventory.innerContainer.Count - 1; num2 > -1; num2--)
            {
                if (item.inventory.innerContainer[num2].def != BankDefOf.BankNote &&
                    item.inventory.innerContainer[num2].GetInnerIfMinified().def !=
                    CoreDefOf.RimBankBuildingTerminal)
                {
                    continue;
                }

                num += item.inventory.innerContainer[num2].stackCount;
                item.inventory.innerContainer.RemoveAt(num2);
            }
        }

        foreach (var map in Find.Maps)
        {
            foreach (var allPawn in map.mapPawns.AllPawns)
            {
                if (allPawn.inventory == null || !allPawn.inventory.innerContainer.Any)
                {
                    continue;
                }

                for (var num3 = allPawn.inventory.innerContainer.Count - 1; num3 > -1; num3--)
                {
                    if (allPawn.inventory.innerContainer[num3].def != BankDefOf.BankNote &&
                        allPawn.inventory.innerContainer[num3].GetInnerIfMinified().def !=
                        CoreDefOf.RimBankBuildingTerminal)
                    {
                        continue;
                    }

                    num += allPawn.inventory.innerContainer[num3].stackCount;
                    allPawn.inventory.innerContainer.RemoveAt(num3);
                }
            }
        }

        return num;
    }
}