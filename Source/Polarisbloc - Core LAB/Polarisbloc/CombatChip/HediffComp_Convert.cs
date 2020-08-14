using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_Convert : HediffComp
    {
		public int ticksToConvert;

		public HediffCompProperties_Convert Props
        {
            get
            {
                return (HediffCompProperties_Convert)this.props;
            }
        }

		/*public override bool CompShouldRemove
		{
			get
			{
				return base.CompShouldRemove || this.ticksToConvert <= 0;
			}
		}*/

		public override string CompLabelInBracketsExtra
		{
			get
			{
				if (!this.Props.showRemainingTime)
				{
					return base.CompLabelInBracketsExtra;
				}
				return this.ticksToConvert.TicksToSeconds().ToString("0.0");
			}
		}

		public override void CompPostMake()
		{
			base.CompPostMake();
			this.ticksToConvert = this.Props.convertAfterTicks.RandomInRange;
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			this.ticksToConvert--;
			if (this.ticksToConvert <= 0)
			{
				this.Pawn.health.AddHediff(this.Props.destinationHediff, this.Pawn.health.hediffSet.GetBrain());
				this.Pawn.health.RemoveHediff(this.parent);
			}
		}

		public override void CompPostMerged(Hediff other)
		{
			base.CompPostMerged(other);
			HediffComp_Convert hediffComp_Convert = other.TryGetComp<HediffComp_Convert>();
			if (hediffComp_Convert != null && hediffComp_Convert.ticksToConvert > this.ticksToConvert)
			{
				this.ticksToConvert = hediffComp_Convert.ticksToConvert;
			}
		}

		public override void CompExposeData()
		{
			Scribe_Values.Look<int>(ref this.ticksToConvert, "ticksToConvert", 0, false);
		}

		public override string CompDebugString()
		{
			return "ticksToConvert: " + this.ticksToConvert ;
		}

		

	}

    public class HediffCompProperties_Convert : HediffCompProperties
    {
        public HediffCompProperties_Convert()
        {
            this.compClass = typeof(HediffComp_Convert);
        }

        public IntRange convertAfterTicks;

        public bool showRemainingTime;

        public HediffDef destinationHediff;
    }
}
