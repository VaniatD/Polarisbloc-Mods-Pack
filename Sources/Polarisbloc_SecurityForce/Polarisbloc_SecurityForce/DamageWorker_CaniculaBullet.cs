using RimWorld;
using Verse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Polarisbloc_SecurityForce
{
    public class DamageWorker_CaniculaBullet : DamageWorker_AddInjury
    {
        public override DamageWorker.DamageResult Apply(DamageInfo dinfo, Thing thing)
        {
            Pawn pawn = thing as Pawn;
            if (pawn == null)
            {
                return base.Apply(dinfo, thing);
            }
            if (Rand.Chance(0.25f))
            {
                BodyPartRecord bodyPart = pawn.health.hediffSet.GetBrain();
                if (bodyPart != null && dinfo.Amount >= 6f)
                {
                    dinfo.SetHitPart(bodyPart);
                }
            }
            dinfo.SetAmount(dinfo.Amount * 3);
            return base.Apply(dinfo, thing);
        }
    }
}
