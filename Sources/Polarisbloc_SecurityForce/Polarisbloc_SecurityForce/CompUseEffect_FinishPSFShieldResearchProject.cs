using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Verse;
using Polarisbloc_SecurityForce;

namespace RimWorld 
{
    public class CompUseEffect_FinishPSFShieldResearchProject : CompUseEffect
    {
        
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            if (Rand.Chance(0.12f))
            {
                ResearchProjectDef shieldProj = DefDatabase<ResearchProjectDef>.GetNamed("PolarisShield", true);
                if (shieldProj != null && !shieldProj.IsFinished)
                {
                    this.FinishInstantly(shieldProj);
                }
                else if (this.TryRandomlyUnfinishedResearch(out ResearchProjectDef researchProj))
                {
                    this.FinishInstantly(researchProj);
                }
            }
            else
            {
                if (Rand.Chance(0.3f))
                {
                    Map map = usedBy.MapHeld;
                    List<Thing> things = ThingSetMakerDefOf.ResourcePod.root.Generate();
                    IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
                    DropPodUtility.DropThingsNear(intVec, map, things, 110, false, true, true);
                    Find.LetterStack.ReceiveLetter("LetterLabelPolarisblocConsolationPrize".Translate(), "PolarisblocConsolationPrize".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null);
                    return;
                }
                if (Rand.Chance(0.01f))
                {
                    Map map = usedBy.MapHeld;
                    List<Thing> things = new List<Thing>();
                    Thing thing = ThingMaker.MakeThing(PSFDefOf.PolarisBunnyGundamSculpture , ThingDefOf.Gold);
                    things.Add(thing);
                    IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
                    DropPodUtility.DropThingsNear(intVec, map, things, 110, false, true, true);
                    Find.LetterStack.ReceiveLetter("LetterLabelPolarisblocSurprised".Translate(), "PolarisblocSurprised".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null);
                    return;
                }
                else
                {
                    Messages.Message("MessageFailToUnscrambleMemoryStick".Translate(new object[]
                    {
                        usedBy.LabelShort
                    }), MessageTypeDefOf.NegativeEvent);
                }
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
            Messages.Message("MessageResearchProjectFinishedByItem".Translate(new object[]
            {
                proj.label
            }), MessageTypeDefOf.PositiveEvent);
        }
    }
}
