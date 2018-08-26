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
        public override void Tick()
        {
            base.Tick();
            if (base.Wearer == null)
            {
                return;
            }
            if (this.Wearer.kindDef == PawnKindDefOf.WildMan)
            {
                return;
            }
            if (this.Wearer.Faction.def != PSFDefOf.Polaribloc_SecuirityForce)
            {
                this.Destroy(DestroyMode.Vanish);
                return;
            }
            if (this.Wearer != null )
            {
                this.Wearer.health.AddHediff(HediffDef.Named("PolarisCombatChip_Currency"), this.Wearer.health.hediffSet.GetBrain(), null);
                this.Wearer.health.AddHediff(HediffDef.Named("BionicEye"), this.GetEye(), null);
                this.Wearer.health.AddHediff(HediffDef.Named("BionicLeg"), this.GetLeg(), null);
                if (this.Wearer.health.hediffSet.hediffs.Find(x => x.TryGetComp<HediffComp_Cartridge>() != null) == null)
                {
                    Hediff hediff = HediffMaker.MakeHediff(PolarisblocDefOf.Hediff_PloarisCartridge, this.Wearer, null);
                    this.Wearer.health.AddHediff(hediff);
                }
                CombatEnhancingDrugsApply(this.Wearer);
                this.Wearer.apparel.Remove(this);
                this.Destroy(DestroyMode.Vanish);
                return;
            }
        }

        private void CombatEnhancingDrugsApply(Pawn pawn)
        {
            //pawn.health.AddHediff(HediffDef.Named("PolarisHealingPotion"));
            Hediff hpHediff = HediffMaker.MakeHediff(HediffDef.Named("PolarisHealingPotion"), pawn);
            hpHediff.Severity = 2f;
            pawn.health.AddHediff(hpHediff);
            if (Rand.Chance(0.5f))
            {
                pawn.health.AddHediff(HediffDef.Named("GoJuiceHigh"));
            }
            else
            {
                pawn.health.AddHediff(HediffDef.Named("YayoHigh"));
            }
        }

        private BodyPartRecord GetLeg()
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
        }
    }
}
