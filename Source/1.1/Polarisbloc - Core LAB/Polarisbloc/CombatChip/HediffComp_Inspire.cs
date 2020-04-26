using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_Inspire : HediffComp
    {
        public HediffCompProperties_Inspire Props
        {
            get
            {
                return (HediffCompProperties_Inspire)base.props;
            }
        }

        private int inspireTick;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.inspireTick++;
            if (this.inspireTick >= GenDate.TicksPerHour)
            {
                this.inspireTick = 0;
                if (!base.Pawn.mindState.inspirationHandler.Inspired)
                {
                    this.Inspire(base.Pawn);
                }
            }
        }

        private void Inspire(Pawn pawn)
        {
            InspirationDef randomInsDef = (from x in DefDatabase<InspirationDef>.AllDefsListForReading
                                           where x.Worker.InspirationCanOccur(pawn)
                                           select x).RandomElementByWeightWithFallback((InspirationDef x) => x.Worker.CommonalityFor(pawn), null);
            if (randomInsDef != null)
            {
                if (randomInsDef.Worker.InspirationCanOccur(pawn))
                {
                    pawn.mindState.inspirationHandler.TryStartInspiration(randomInsDef);
                }
            }

        }
    }

    public class HediffCompProperties_Inspire : HediffCompProperties
    {

        public HediffCompProperties_Inspire()
        {
            this.compClass = typeof(HediffComp_Inspire);
        }
    }
}
