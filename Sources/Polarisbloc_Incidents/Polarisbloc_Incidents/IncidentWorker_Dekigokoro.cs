using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;

namespace Polarisbloc_Incidents
{
    public class IncidentWorker_Dekigokoro : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            bool result;
            if (!base.CanFireNowSub(parms))
            {
                result = false;
            }
            else
            {
                result = this.TryFindAnyWorkingPersonInMap((Map)parms.target, out Pawn worker);
            }
            return result;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool result = false;
            if (this.TryFindAnyWorkingPersonInMap((Map)parms.target, out Pawn worker))
            {
                if (RCellFinder.TryFindSkygazeCell(worker.Position, worker, out IntVec3 c))
                {
                    Job job = new Job(PolarisIncidentDefOf.Skygaze, c);
                    worker.jobs.StartJob(job, JobCondition.InterruptForced);
                    string text = string.Format(this.def.letterText, new object[]
                    {
                        worker.LabelShort
                    }).CapitalizeFirst();
                    Find.LetterStack.ReceiveLetter(this.def.letterLabel, text, LetterDefOf.NeutralEvent, worker);
                }
            }
            return result;
        }

            private bool TryFindAnyWorkingPersonInMap(Map map, out Pawn worker)
        {
            worker = null;
            bool result = false;
            List<Pawn> list = map.mapPawns.FreeColonists.ToList();
            if ((from x in list
                where (x.CurJobDef == JobDefOf.DoBill || x.CurJobDef == JobDefOf.Research ) && JoyUtility.EnjoyableOutsideNow(x)
                 select x).TryRandomElement(out worker))
            {
                //worker.jobs.EndCurrentJob(JobCondition.InterruptForced);
                result = true;
            }
            return result;
        }

    }
}
