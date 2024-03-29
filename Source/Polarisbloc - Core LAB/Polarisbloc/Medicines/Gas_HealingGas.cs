﻿using System;
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
                    this.TendRandomInjury(p, Rand.Value);
                }
                if (Rand.Chance(0.2f))
                {
                    this.DestroyBlight();
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

        private void TendRandomInjury(Pawn pawn, float quality)
        {
            if ((from x in pawn.health.hediffSet.GetHediffs<Hediff_Injury>()
                 where x.TendableNow()
                 select x).TryRandomElement(out Hediff_Injury hediff_Injury))
            {
                hediff_Injury.Tended(quality, 0.65f);
            }
        }

        private void DestroyBlight()
        {
            //Blight blight = this.PositionHeld.GetPlant(this.MapHeld).Blight;
            Blight blight = this.PositionHeld.GetFirstBlight(this.MapHeld);
            if (blight != null)
            {
                blight.Destroy(DestroyMode.Vanish);
            }
        }
    }
}
