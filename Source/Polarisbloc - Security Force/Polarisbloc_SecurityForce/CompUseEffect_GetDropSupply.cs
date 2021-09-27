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
        private bool captain = false;

        private static readonly List<ThingDef> currencySuits = new List<ThingDef>
        {
            PSFDefOf.Polarisbloc_CygnusStandardTights,
            PSFDefOf.Polarisbloc_AlkaidStrategyAssistant,
            PSFDefOf.Polarisbloc_ThubanTacticalGoggles,
        };

        private static readonly List<ThingDef> standardSuits = new List<ThingDef>
        {
            PSFDefOf.Apparel_PolarisShieldBelt_II,
            PSFDefOf.Polarisbloc_CygnusStandardArmor,
            PSFDefOf.Polarisbloc_CaniculaRifle
        };

        private static readonly List<ThingDef> captainSuits = new List<ThingDef>
        {
            PSFDefOf.Apparel_PolarisShieldBelt_IV,
            PSFDefOf.Polarisbloc_CygnusStandardArmorC,
            PSFDefOf.Polarisbloc_CaniculaSniper
        };

        private List<Thing> MakeSuits()
        {
            QualityCategory quality = this.parent.GetComp<CompQuality>().Quality;
            List<Thing> things = new List<Thing>();
            things.Clear();
            foreach (ThingDef cuSuitDef in CompUseEffect_GetDropSupply.currencySuits)
            {
                Thing cuSuit = ThingMaker.MakeThing(cuSuitDef);
                cuSuit.TryGetComp<CompQuality>()?.SetQuality(quality, ArtGenerationContext.Outsider);
                things.Add(cuSuit);
            }
            if (this.captain)
            {
                foreach (ThingDef caSuitDef in CompUseEffect_GetDropSupply.captainSuits)
                {
                    Thing caSuit = ThingMaker.MakeThing(caSuitDef);
                    caSuit.TryGetComp<CompQuality>()?.SetQuality(quality, ArtGenerationContext.Outsider);
                    things.Add(caSuit);
                }
            }
            else
            {
                foreach (ThingDef stSuitDef in CompUseEffect_GetDropSupply.standardSuits)
                {
                    Thing stSuit = ThingMaker.MakeThing(stSuitDef);
                    stSuit.TryGetComp<CompQuality>()?.SetQuality(quality, ArtGenerationContext.Outsider);
                    things.Add(stSuit);
                }
            }
            Thing thing7 = ThingMaker.MakeThing(PSFDefOf.Polarisbloc_EmergencyFood);
            thing7.stackCount = 5;
            things.Add(thing7);
            return things;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<bool>(ref this.captain, "captain", false, false);
        }

        public override void PostPostMake()
        {
            base.PostPostMake();
            if (Rand.Chance(0.1f))
            {
                this.captain = true;
            }
        }

        public override string TransformLabel(string label)
        {
            return this.captain? "C'" + label : label;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "captain",
                    isActive = () => this.captain,
                    toggleAction = delegate
                    {
                        this.captain = !this.captain;
                    }
                };
            }
            
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            Map map = usedBy.MapHeld;
            //DropCellFinder.TryFindDropSpotNear(usedBy.PositionHeld, map, out IntVec3 intVec3, false, false);
            /*Thing thing1 = ThingMaker.MakeThing(PSFDefOf.Apparel_PolarisShieldBelt_II);
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
            if (this.parent.GetComp<CompQuality>() != null)
            {
                QualityCategory quality = this.parent.GetComp<CompQuality>().Quality;
                foreach (Thing qThing in from x in things
                                         where x.TryGetComp<CompQuality>() != null
                                         select x)
                {
                    qThing.TryGetComp<CompQuality>().SetQuality(quality, ArtGenerationContext.Outsider);
                }
            }*/
            //DropPodUtility.DropThingsNear(intVec3, map, things, 110, false, false, true, false);
            //Find.LetterStack.ReceiveLetter("LetterLabelPolarisblocGetDropSupply".Translate(), "PolarisblocGetDropSupply".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec3, map, false), null);
            List<Thing> things = this.MakeSuits();
            foreach (Thing thing in things)
            {
                GenPlace.TryPlaceThing(thing, usedBy.PositionHeld, usedBy.MapHeld, ThingPlaceMode.Near);
            }
            return;
        }
    }
}
