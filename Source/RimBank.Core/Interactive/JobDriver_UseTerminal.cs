﻿using System.Collections.Generic;
using Verse.AI;

namespace RimBank.Core.Interactive;

public class JobDriver_UseTerminal : JobDriver
{
    public override bool TryMakePreToilReservations(bool errorOnFailed)
    {
        return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
    }

    protected override IEnumerable<Toil> MakeNewToils()
    {
        yield return Toils_Goto.GotoCell(TargetIndex.A, PathEndMode.InteractionCell).FailOn(to =>
            !((Building_Terminal)to.actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseTerminalNow);
        yield return new Toil
        {
            initAction = delegate
            {
                var actor = CurToil.actor;
                if (((Building_Terminal)actor.jobs.curJob.GetTarget(TargetIndex.A).Thing).CanUseTerminalNow)
                {
                    FloatMenuManager.currentAction(actor);
                }
            }
        };
    }
}