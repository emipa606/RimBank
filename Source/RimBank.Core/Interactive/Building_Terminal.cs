using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.AI;

namespace RimBank.Core.Interactive;

public class Building_Terminal : Building
{
    private CompPowerTrader powerComp;

    public bool CanUseTerminalNow
    {
        get
        {
            if (!Spawned || !Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare))
            {
                return powerComp.PowerOn;
            }

            return false;
        }
    }

    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        powerComp = GetComp<CompPowerTrader>();
    }

    public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(Pawn selPawn)
    {
        if (!selPawn.CanReach(this, PathEndMode.InteractionCell, Danger.Some))
        {
            var item = new FloatMenuOption("CannotUseNoPath".Translate(), null);
            return new List<FloatMenuOption>
            {
                item
            };
        }

        if (Spawned && Map.gameConditionManager.ConditionIsActive(GameConditionDefOf.SolarFlare))
        {
            var item2 = new FloatMenuOption("CannotUseSolarFlare".Translate(), null);
            return new List<FloatMenuOption>
            {
                item2
            };
        }

        if (!powerComp.PowerOn)
        {
            var item3 = new FloatMenuOption("CannotUseNoPower".Translate(), null);
            return new List<FloatMenuOption>
            {
                item3
            };
        }

        if (selPawn.health.capacities.CapableOf(PawnCapacityDefOf.Sight))
        {
            return FloatMenuManager.RequestBuild(this, selPawn);
        }

        var item4 = new FloatMenuOption(
            "CannotUseReason".Translate("IncapableOfCapacity".Translate(PawnCapacityDefOf.Sight.label)), null);
        return new List<FloatMenuOption>
        {
            item4
        };
    }
}