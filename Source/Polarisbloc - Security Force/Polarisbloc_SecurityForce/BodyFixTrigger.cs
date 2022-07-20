using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Polarisbloc;

namespace Polarisbloc_SecurityForce
{
    //[StaticConstructorOnStartup]
    public class BodyFixTrigger : Apparel
    {
        /*public override void Tick()
        {
            base.Tick();
            if (base.Wearer == null)
            {
                return;
            }
            if (this.Wearer.kindDef == PawnKindDefOf.WildMan)
            {
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            if (this.Wearer.Faction == null)
            {
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            if (this.Wearer.kindDef.defaultFactionType != PSFDefOf.Polaribloc_SecuirityForce)
            {
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            if (this.Wearer != null )
            {
                if (ModLister.GetActiveModWithIdentifier("Vanya.Polarisbloc.CoreLab") != null)
                {
                    if (!this.Wearer.health.hediffSet.GetHediffs<Hediff_CombatChip>().EnumerableNullOrEmpty())
                    {
                        List<Hediff_CombatChip> chips = this.Wearer.health.hediffSet.GetHediffs<Hediff_CombatChip>().ToList();
                        foreach (Hediff_CombatChip chip in chips)
                        {
                            this.Wearer.health.RemoveHediff(chip);
                        }
                    }
                    this.Wearer.health.AddHediff(HediffDef.Named("PolarisCombatChip_Currency"), this.Wearer.health.hediffSet.GetBrain(), null);
                }
                    
                BodyPartRecord eye = this.GetEye();
                if (eye != null)
                {
                    this.Wearer.health.AddHediff(HediffDef.Named("BionicEye"), eye, null);
                }
                BodyPartRecord leg = this.GetLeg();
                if (leg != null)
                {
                    this.Wearer.health.AddHediff(HediffDef.Named("BionicLeg"), leg, null);
                }
                CombatEnhancingDrugsApply(this.Wearer);
                foreach (Apparel ap in this.Wearer.apparel.WornApparel)
                {
                    CompBiocodable compBiocodable = ap.TryGetComp<CompBiocodable>();
                    if (compBiocodable != null && !compBiocodable.Biocoded)
                    {
                        compBiocodable.CodeFor(this.Wearer);
                    }
                }
                this.Wearer.apparel.Remove(this);
                this.Destroy(DestroyMode.Vanish);
                return;
            }
        }*/

        public override void Notify_Equipped(Pawn pawn)
        {
            base.Notify_Equipped(pawn);
            if (this.Wearer.kindDef == PawnKindDefOf.WildMan || this.Wearer.Faction == null || this.Wearer.kindDef.defaultFactionType != PSFDefOf.Polaribloc_SecuirityForce)
            {
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            if (this.Wearer != null)
            {
                if (ModLister.GetActiveModWithIdentifier("Vanya.Polarisbloc.CoreLab") != null)
                {
                    if (!this.Wearer.health.hediffSet.GetHediffs<Hediff_CombatChip>().EnumerableNullOrEmpty())
                    {
                        List<Hediff_CombatChip> chips = this.Wearer.health.hediffSet.GetHediffs<Hediff_CombatChip>().ToList();
                        foreach (Hediff_CombatChip chip in chips)
                        {
                            this.Wearer.health.RemoveHediff(chip);
                        }
                    }
                    this.Wearer.health.AddHediff(HediffDef.Named("PolarisCombatChip_Currency"), this.Wearer.health.hediffSet.GetBrain(), null);
                }

                /*BodyPartRecord eye = this.GetEye();
                if (eye != null)
                {
                    this.Wearer.health.AddHediff(HediffDef.Named("BionicEye"), eye, null);
                }
                BodyPartRecord leg = this.GetLeg();
                if (leg != null)
                {
                    this.Wearer.health.AddHediff(HediffDef.Named("BionicLeg"), leg, null);
                }*/
                this.CombatEnhancingDrugsApply(this.Wearer);
                foreach (Apparel ap in this.Wearer.apparel.WornApparel)
                {
                    CompBiocodable compBiocodable = ap.TryGetComp<CompBiocodable>();
                    if (compBiocodable != null && !compBiocodable.Biocoded)
                    {
                        compBiocodable.CodeFor(this.Wearer);
                    }
                }
                this.Wearer.apparel.Remove(this);
                this.Destroy(DestroyMode.Vanish);
            }
        }

        private void CombatEnhancingDrugsApply(Pawn pawn)
        {
            //pawn.health.AddHediff(HediffDef.Named("PolarisHealingPotion"));
            if (ModLister.GetActiveModWithIdentifier("Vanya.Polarisbloc.CoreLab") != null)
            {
                Hediff hpHediff = HediffMaker.MakeHediff(HediffDef.Named("PolarisHealingPotion"), pawn);
                hpHediff.Severity = 2f;
                pawn.health.AddHediff(hpHediff);
            }
            if (Rand.Chance(0.5f))
            {
                pawn.health.AddHediff(HediffDef.Named("GoJuiceHigh"));
            }
            else
            {
                pawn.health.AddHediff(HediffDef.Named("YayoHigh"));
            }
        }

        /*private BodyPartRecord GetLeg()
        {
            foreach (BodyPartRecord notMissingPart in this.Wearer.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined))
            {
                if (notMissingPart.def.tags.Contains(BodyPartTagDefOf.MovingLimbCore))
                {
                    return notMissingPart;
                }
            }
            return null;
        }

        private BodyPartRecord GetEye()
        {
            foreach (BodyPartRecord notMissingPart in this.Wearer.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined))
            {
                if (notMissingPart.def.tags.Contains(BodyPartTagDefOf.SightSource))
                {
                    return notMissingPart;
                }
            }
            return null;
        }*/
    }
}
