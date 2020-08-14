using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_RemoveHediff : HediffComp
    {
        private int ticks;

        public HediffCompProperties_RemoveHediff Props
        {
            get
            {
                return this.props as HediffCompProperties_RemoveHediff;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.ticks, "ticks", 0, false);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.ticks--;
            if (this.ticks <= 0)
            {
                this.ticks = this.Props.intervalTicks;
                List<Hediff> hediffs = this.Pawn.health.hediffSet.hediffs;
                for (int i = hediffs.Count - 1; i >= 0; i--)
                {
                    if (this.Props.appliedHediffs.Contains(hediffs[i].def))
                    {
                        this.Pawn.health.RemoveHediff(hediffs[i]);
                    }
                }
            }
        }

        public override string CompDebugString()
        {
            return "ticksToRemove: " + this.ticks;
        }
    }

    public class HediffCompProperties_RemoveHediff : HediffCompProperties
    {
        public HediffCompProperties_RemoveHediff()
        {
            this.compClass = typeof(HediffComp_RemoveHediff);
        }

        public List<HediffDef> appliedHediffs;

        public int intervalTicks = 2500;
    }
}
