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
            if (dinfo.Instigator == base.Wearer) return true;
            if (dinfo.Def == DamageDefOf.Extinguish) return true;
            if (dinfo.Def == DamageDefOf.SurgicalCut) return false;
            if (dinfo.Def == DamageDefOf.Smoke) return true;
            if (Rand.Value * 400 < this.HitPoints)
            {
                this.TakeDamage(dinfo);
                MoteMaker.ThrowText(base.Wearer.DrawPos, base.Wearer.Map, "PlrsTextMote_Absorbed".Translate(), 1.5f);
                return true;
            }
            else return false;
        }
    }
}
