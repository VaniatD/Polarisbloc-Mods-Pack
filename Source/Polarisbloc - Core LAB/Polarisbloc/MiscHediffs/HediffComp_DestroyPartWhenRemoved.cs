using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_DestroyPartWhenRemoved : HediffComp
    {
        public int ticksToDisappear;

        private HediffCompProperties_DestroyPartWhenRemoved Props
        {
            get
            {
                return (HediffCompProperties_DestroyPartWhenRemoved)this.props;
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            Hediff_MissingPart hediff_MissingPart = (Hediff_MissingPart)HediffMaker.MakeHediff(HediffDefOf.MissingBodyPart, this.Pawn);
            hediff_MissingPart.IsFresh = false;
            hediff_MissingPart.lastInjury = HediffDefOf.SurgicalCut;
            hediff_MissingPart.Part = this.parent.Part;
            this.Pawn.health.hediffSet.AddDirect(hediff_MissingPart);
        }

        public override bool CompShouldRemove
        {
            get
            {
                return base.CompShouldRemove || ticksToDisappear <= 0;
            }
        }

        public override string CompLabelInBracketsExtra
        {
            get
            {
                if (!Props.showRemainingTime)
                {
                    return base.CompLabelInBracketsExtra;
                }
                return ticksToDisappear.ToStringTicksToPeriod(allowSeconds: true, shortForm: true);
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            ticksToDisappear = Props.disappearsAfterTicks.RandomInRange;
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            ticksToDisappear--;
            if (this.CompShouldRemove)
            {
                this.Pawn.health.RemoveHediff(this.parent);
            }
        }

        public override void CompPostMerged(Hediff other)
        {
            base.CompPostMerged(other);
            HediffComp_Disappears hediffComp_Disappears = other.TryGetComp<HediffComp_Disappears>();
            if (hediffComp_Disappears != null && hediffComp_Disappears.ticksToDisappear > ticksToDisappear)
            {
                ticksToDisappear = hediffComp_Disappears.ticksToDisappear;
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look(ref ticksToDisappear, "ticksToDisappear", 0);
        }

        public override string CompDebugString()
        {
            return "ticksToDisappear: " + ticksToDisappear;
        }

    }

    public class HediffCompProperties_DestroyPartWhenRemoved : HediffCompProperties
    {
        public HediffCompProperties_DestroyPartWhenRemoved()
        {
            this.compClass = typeof(HediffComp_DestroyPartWhenRemoved);
        }

        public IntRange disappearsAfterTicks;

        public bool showRemainingTime;
    }
}
