using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_JoyFiller : HediffComp
    {
        public HediffCompProperties_JoyFiller Props
        {
            get
            {
                return (HediffCompProperties_JoyFiller)base.props;
            }
        }

        //private int checkTicks = GenDate.TicksPerHour;

        private int tick;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.tick--;
            if (this.tick <= 0)
            {
                this.FillUpNeed(base.Pawn.needs.joy);
                this.FillUpNeed(base.Pawn.needs.outdoors);
                this.tick = this.Props.checkTicks;
            }
        }

        private void FillUpNeed(Need need)
        {
            if (need != null)
            {
                if (need.CurLevelPercentage < 0.9f)
                {
                    need.CurLevelPercentage = 0.98f;
                }
            }
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look<int>(ref this.tick, "tick", 0, false);
        }

        public override string CompDebugString()
        {
            return "ticksToCheck: " + this.tick;
        }
    }

    public class HediffCompProperties_JoyFiller : HediffCompProperties
    {
        public HediffCompProperties_JoyFiller()
        {
            this.compClass = typeof(HediffComp_JoyFiller);
        }

        public int checkTicks = 2500;
    }
}
