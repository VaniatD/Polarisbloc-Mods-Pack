using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class IncidentWorker_GetPloarisblocSecretArchives : IncidentWorker
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
                result = this.CouldGetProjNow((Map)parms.target);
            }
            return result;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Find.ResearchManager.FinishProject(PolarisblocDefOf.PolarisSecretArchives, false, null);
            Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef);
            return true;
        }


        private bool CouldGetProjNow(Map map)
        {
            bool result = false;
            if (map.listerBuildings.allBuildingsColonist.Find(x => x.def == ThingDefOf.CommsConsole) != null && !PolarisblocDefOf.PolarisSecretArchives.IsFinished)
            {
                result = true;
            }
            return result;
        }

    }
}
