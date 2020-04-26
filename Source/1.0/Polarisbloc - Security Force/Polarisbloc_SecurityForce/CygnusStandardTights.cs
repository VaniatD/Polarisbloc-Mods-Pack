using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace Polarisbloc_SecurityForce
{
    public class CygnusStandardTights : Apparel
    {
        private int TicksBetweenTend = 300;

        private int tendTicks;

        public override void Tick()
        {
            base.Tick();
            if (base.Wearer == null) return;
            tendTicks++;
            if (CheckAutoTendAvaliable())
            {
                tendTicks = 0;
                AutoTendWearer(base.Wearer);
            }
        }

        private void AutoTendWearer(Pawn pawn)
        {
            if((from x in pawn.health.hediffSet.hediffs
                where x.TendableNow() && (x is Hediff_Injury || x is Hediff_MissingPart)
                select x).TryRandomElement(out Hediff result))
            {
                result.Tended(Rand.Range(0.6f, 1f), 0);
            }
        }

        private bool CheckAutoTendAvaliable()
        {
            if (this.tendTicks >= this.TicksBetweenTend) return true;
            else return false;
        }
    }
}
