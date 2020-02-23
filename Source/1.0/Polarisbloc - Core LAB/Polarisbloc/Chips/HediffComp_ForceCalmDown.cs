using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Polarisbloc
{
    public class HediffComp_ForceCalmDown : HediffComp
    {
        public HediffCompProperties_ForceCalmDown Props
        {
            get
            {
                return (HediffCompProperties_ForceCalmDown)base.props;
            }
        }

        private int tick;

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.tick++;
            if (this.tick >= 250)
            {
                this.tick = 0;
                if (this.Pawn.mindState.mentalStateHandler.InMentalState)
                {
                    this.Pawn.mindState.mentalStateHandler.Reset();
                    Messages.Message("PolarisForceCalmDown".Translate(this.Pawn.LabelShortCap, this.parent.LabelCap), this.Pawn, MessageTypeDefOf.PositiveEvent);
                }
            }
        }
    }

    public class HediffCompProperties_ForceCalmDown : HediffCompProperties
    {
        public HediffCompProperties_ForceCalmDown()
        {
            this.compClass = typeof(HediffComp_ForceCalmDown);
        }
    }
}
