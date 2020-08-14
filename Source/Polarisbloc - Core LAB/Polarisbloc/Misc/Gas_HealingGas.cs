using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;


namespace Polarisbloc
{
    public class Gas_HealingGas : Gas
    {
        private int ticks;

        private List<Pawn> initPawns = new List<Pawn>();

        private List<Pawn> Pawns
        {
            get
            {
                this.initPawns.Clear();
                foreach (Pawn p in this.PositionHeld.GetThingList(this.MapHeld).FindAll(x => x is Pawn))
                {
                    this.initPawns.Add(p);
                }
                return this.initPawns;
            }
        }

        public override void Tick()
        {
            
            this.ticks++;
            if (this.ticks >= 60)
            {
                this.ticks = 0;
                foreach (Pawn p in this.Pawns)
                {
                    this.HealRandomInjury(p, 1f);
                }
            }
            base.Tick();
        }

        private void HealRandomInjury(Pawn pawn, float points)
        {
            if ((from x in pawn.health.hediffSet.GetHediffs<Hediff_Injury>()
                 where x.CanHealNaturally() || x.CanHealFromTending()
                 select x).TryRandomElement(out Hediff_Injury hediff_Injury))
            {
                hediff_Injury.Heal(points);
            }
        }
    }
}
