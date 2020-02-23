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

        private int checkTicks = GenDate.TicksPerHour;

        private int tick;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.tick++;
            if (this.tick >= this.checkTicks)
            {
                this.FillUpNeed(base.Pawn.needs.joy);
                this.FillUpNeed(base.Pawn.needs.outdoors);
                this.tick = 0;
            }
        }

        private void FillUpNeed(Need need)
        {
            if (need != null)
            {
                if (need.CurLevelPercentage < 0.85f)
                {
                    need.CurLevelPercentage = 0.9f;
                }
            }
        }
    }

    public class HediffCompProperties_JoyFiller : HediffCompProperties
    {
        public HediffCompProperties_JoyFiller()
        {
            this.compClass = typeof(HediffComp_JoyFiller);
        }
    }
}
