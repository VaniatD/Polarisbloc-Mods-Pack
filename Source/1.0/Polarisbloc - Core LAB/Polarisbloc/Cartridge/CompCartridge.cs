using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompCartridge : ThingComp
    {
        public CompProperties_Cartridge Props
        {
            get
            {
                return (CompProperties_Cartridge)base.props;
            }
        }

        private int checkTicks = GenDate.TicksPerHour;

        private int tick;

        public override void CompTick()
        {
            base.CompTick();
            this.tick++;
            if (this.tick >= this.checkTicks)
            {
                if (base.parent is Apparel ap && ap.Wearer != null)
                {
                    if (ap.Wearer.health.hediffSet.hediffs.Find(x => x.TryGetComp<HediffComp_Cartridge>() != null) == null)
                    {
                        Hediff hediff = HediffMaker.MakeHediff(PolarisblocDefOf.Hediff_PloarisCartridge, ap.Wearer, null);
                        ap.Wearer.health.AddHediff(hediff);
                    }
                }
                this.tick = 0;
            }
        }
    }

    public class CompProperties_Cartridge : CompProperties
    {
        public CompProperties_Cartridge()
        {
            base.compClass = typeof(CompCartridge);
        }
    }
}
