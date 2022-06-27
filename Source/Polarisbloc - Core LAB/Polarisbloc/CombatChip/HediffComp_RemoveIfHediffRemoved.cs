using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace Polarisbloc
{
    public  class HediffComp_RemoveIfHediffRemoved : HediffComp
    {
        public HediffCompProperties_RemoveIfHediffRemoved Props
        {
            get
            {
                return (HediffCompProperties_RemoveIfHediffRemoved)this.props;
            }
        }

        public Hediff hediff;

        public override bool CompShouldRemove
        {
            get
            {
                if (this.Pawn.health.hediffSet.hediffs.Contains(this.hediff))
                {
                    return true;
                }
                return false;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_References.Look(ref this.hediff, "hediff");
        }
    }

    public class HediffCompProperties_RemoveIfHediffRemoved : HediffCompProperties
    {
        public HediffCompProperties_RemoveIfHediffRemoved()
        {
            compClass = typeof(HediffComp_RemoveIfHediffRemoved);
        }
    }
}
