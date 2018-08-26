using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc_SecurityForce
{
    public class CygnusStandardArmor : Apparel
    {
        public override bool CheckPreAbsorbDamage(DamageInfo dinfo)
        {
            if (dinfo.Def == DamageDefOf.SurgicalCut)
            {
                return false;
            }
            if (Rand.Value * 200 < this.HitPoints)
            {
                this.TakeDamage(dinfo);
                return true;
            }
            else return false;
        }
    }
}
