using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;

namespace Polarisbloc
{
    public class CompUseEffect_FinishCartridgeResearchProject : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            if (!PolarisblocDefOf.ResearchProject_PolarisCartridge.IsFinished)
            {
                this.FinishInstantly(PolarisblocDefOf.ResearchProject_PolarisCartridge);
            }
            else if (this.TryRandomlyUnfinishedResearch(out ResearchProjectDef researchProj))
            {
                this.FinishInstantly(researchProj);
            }
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            bool result = this.TryRandomlyUnfinishedResearch(out ResearchProjectDef researchProj);
            failReason = "PolarisFoundNoResearchProject".Translate();
            return result;
        }

        private bool TryRandomlyUnfinishedResearch(out ResearchProjectDef researchProj)
        {
            bool result = (from x in DefDatabase<ResearchProjectDef>.AllDefs
                           where !x.IsFinished
                           select x).TryRandomElement(out researchProj);
            return result;
        }

        private void FinishInstantly(ResearchProjectDef proj)
        {
            Find.ResearchManager.FinishProject(proj, false, null);
            Messages.Message("MessageResearchProjectFinishedByItem".Translate(proj.label), MessageTypeDefOf.PositiveEvent);
        }
    }
}
