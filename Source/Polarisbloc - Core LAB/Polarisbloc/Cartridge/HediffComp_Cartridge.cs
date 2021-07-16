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

        public HediffComp_RemoveIfApparelDropped HediffApparel
        {
            get
            {
                return this.parent.TryGetComp<HediffComp_RemoveIfApparelDropped>();
            }
        }

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            if (this.HediffApparel != null)
            {
                Apparel ap = this.HediffApparel.wornApparel;
                if (ap != null)
                {
                    ap.Destroy(DestroyMode.Vanish);
                    //base.Pawn.apparel.Remove(ap);
                    ResurrectionUtility.Resurrect(base.Pawn);
                    Messages.Message("PolarisMessageSomeoneResurrected".Translate(ap.LabelShort, base.Pawn.LabelShort), MessageTypeDefOf.PositiveEvent);
                }
                base.Pawn.health.RemoveHediff(base.parent);
            }
            
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
