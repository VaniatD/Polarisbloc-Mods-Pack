using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_TendPostDamage : HediffComp
    {
        private HediffCompProperties_TendPostDamage Props
        {
            get
            {
                return (HediffCompProperties_TendPostDamage)this.props;
            }
        }

        public override void Notify_PawnPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            base.Notify_PawnPostApplyDamage(dinfo, totalDamageDealt);
            IEnumerable<Hediff> injuries = (from x in this.Pawn.health.hediffSet.hediffs
                                            where x.ageTicks < 1 && x.TendableNow() && (x is Hediff_Injury || x is Hediff_MissingPart)
                                            select x);
            foreach (Hediff injury in injuries)
            {
                injury.Tended(this.Props.tendQuality, 1f);
            }
        }

    }

    public class HediffCompProperties_TendPostDamage : HediffCompProperties
    {
        public HediffCompProperties_TendPostDamage()
        {
            this.compClass = typeof(HediffComp_TendPostDamage);
        }

        public float tendQuality = 0.7f;
    }
}
