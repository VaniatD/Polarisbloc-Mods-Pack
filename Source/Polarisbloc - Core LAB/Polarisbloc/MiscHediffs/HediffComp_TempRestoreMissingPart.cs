using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_TempRestoreMissingPart : HediffComp
    {
        private HediffCompProperties_TempRestoreMissingPart Props
        {
            get
            {
                return (HediffCompProperties_TempRestoreMissingPart)this.props;
            }
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            /*IEnumerable<Hediff> addedPartHediffs = (from x in this.Pawn.health.hediffSet.hediffs
                                                   where x is Hediff_AddedPart
                                                   select x);*/


            /*Predicate<Hediff> parentPartHasAddedPart = delegate (Hediff missingPartHediff)
            {
                foreach (Hediff addedPartHediff in addedPartHediffs)
                {
                    if (addedPartHediff.Part.parts.Contains(missingPartHediff.Part))
                    {
                        return true;
                    }
                }
                return false;
            };*/
            if (this.Pawn.Dead) return;
            IEnumerable<Hediff> missingPartHediffs = (from x in this.Pawn.health.hediffSet.hediffs
                                                      where x.ageTicks < 1 && x is Hediff_MissingPart && !this.Pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(x.Part.parent) && this.Pawn.health.hediffSet.GetNotMissingParts().Contains(x.Part.parent)
                                                      select x);
            List<BodyPartRecord> missingParts = new List<BodyPartRecord>();
            foreach (Hediff missingPartHediff in missingPartHediffs)
            {
                missingParts.AddDistinct(missingPartHediff.Part);
            }
            foreach (BodyPartRecord missingPart in missingParts)
            {
                this.Pawn.health.RestorePart(missingPart);
                Hediff addedPartHediff = HediffMaker.MakeHediff(this.Props.addedPartDef, this.Pawn, missingPart);

                this.Pawn.health.AddHediff(addedPartHediff);
            }

        }
    }

    public class HediffCompProperties_TempRestoreMissingPart : HediffCompProperties
    {
        public HediffCompProperties_TempRestoreMissingPart()
        {
            this.compClass = typeof(HediffComp_TempRestoreMissingPart);
        }

        public HediffDef addedPartDef;
    }
}
