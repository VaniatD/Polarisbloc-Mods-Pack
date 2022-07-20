using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_TamenessKeeper : HediffComp
    {
        public HediffCompProperties_TamenessKeeper Props
        {
            get
            {
                return (HediffCompProperties_TamenessKeeper)this.props;
            }
        }

        private int ticks;

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.ticks, "ticks");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.ticks--;
            if (this.ticks <= 0 )
            {
                this.ticks = GenDate.TicksPerHour;
                if (base.Pawn.RaceProps.Animal)
                {
                    if (base.Pawn.training.CanBeTrained(TrainableDefOf.Tameness))
                    {
                        base.Pawn.training.Train(TrainableDefOf.Tameness, null, true);
                    }
                }
            }
        }

        public override string CompDebugString()
        {
            return "ticksToCheck: " + this.ticks;
        }
    }

    public class HediffCompProperties_TamenessKeeper : HediffCompProperties
    {
        public HediffCompProperties_TamenessKeeper()
        {
            this.compClass = typeof(HediffComp_TamenessKeeper);
        }
    }
}
