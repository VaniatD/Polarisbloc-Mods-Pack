using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc_SecurityForce
{
    public class CompUseEffect_GetDropSupply : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            Map map = usedBy.MapHeld;
            //DropCellFinder.TryFindDropSpotNear(usedBy.PositionHeld, map, out IntVec3 intVec3, false, false);
            Thing thing1 = ThingMaker.MakeThing(PSFDefOf.Apparel_PolarisShieldBelt_II);
            Thing thing2 = ThingMaker.MakeThing(PSFDefOf.Polarisbloc_CygnusStandardArmor);
            Thing thing3 = ThingMaker.MakeThing(PSFDefOf.Polarisbloc_CygnusStandardTights);
            Thing thing4 = ThingMaker.MakeThing(PSFDefOf.Polarisbloc_AlkaidStrategyAssistant);
            Thing thing5 = ThingMaker.MakeThing(PSFDefOf.Polarisbloc_ThubanTacticalGoggles);
            Thing thing6 = ThingMaker.MakeThing(PSFDefOf.Polarisbloc_CaniculaRifle);
            Thing thing7 = ThingMaker.MakeThing(PSFDefOf.Polarisbloc_EmergencyFood);
            thing7.stackCount = 5;
            List<Thing> things = new List<Thing>
            {
                thing1,
                thing2,
                thing3,
                thing4,
                thing5,
                thing6,
                thing7
            };
            //DropPodUtility.DropThingsNear(intVec3, map, things, 110, false, false, true, false);
            //Find.LetterStack.ReceiveLetter("LetterLabelPolarisblocGetDropSupply".Translate(), "PolarisblocGetDropSupply".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec3, map, false), null);
            foreach (Thing thing in things)
            {
                GenPlace.TryPlaceThing(thing, usedBy.PositionHeld, usedBy.MapHeld, ThingPlaceMode.Near);
            }
            return;
        }
    }
}
