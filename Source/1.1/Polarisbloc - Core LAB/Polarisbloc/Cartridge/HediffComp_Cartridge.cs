using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_Cartridge : HediffComp
    {
        public HediffCompProperties_Cartridge Props
        {
            get
            {
                return (HediffCompProperties_Cartridge)base.props;
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
                if (base.Pawn.apparel.WornApparel.Find(x => x.TryGetComp<CompCartridge>() != null) == null)
                {
                    base.Pawn.health.RemoveHediff(base.parent);
                }
                this.tick = 0;
            }
        }

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            Apparel ap = base.Pawn.apparel.WornApparel.Find(x => x.TryGetComp<CompCartridge>() != null);
            if (ap != null)
            {
                ap.Destroy(DestroyMode.Vanish);
                base.Pawn.apparel.Remove(ap);
                ResurrectionUtility.Resurrect(base.Pawn);
                Messages.Message("PolarisMessageSomeoneResurrected".Translate(ap.LabelShort, base.Pawn.LabelShort), MessageTypeDefOf.PositiveEvent);
            }
            base.Pawn.health.RemoveHediff(base.parent);
        }
    }

    public class HediffCompProperties_Cartridge : HediffCompProperties
    {
        public HediffCompProperties_Cartridge()
        {
            base.compClass = typeof(HediffComp_Cartridge);
        }
    }
}
