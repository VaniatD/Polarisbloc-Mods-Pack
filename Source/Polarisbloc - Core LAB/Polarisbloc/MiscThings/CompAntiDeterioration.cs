using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompAntiDeterioration : ThingComp
    {
        private CompProperties_AntiDeterioration Props
        {
            get
            {
                return (CompProperties_AntiDeterioration)this.props;
            }
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            base.PostPreApplyDamage(dinfo, out absorbed);
            if (dinfo.Def == DamageDefOf.Deterioration) absorbed = true;
        }

    }

    public class CompProperties_AntiDeterioration : CompProperties
    {
        public CompProperties_AntiDeterioration()
        {
            this.compClass = typeof(CompAntiDeterioration);
        }
    }
}
