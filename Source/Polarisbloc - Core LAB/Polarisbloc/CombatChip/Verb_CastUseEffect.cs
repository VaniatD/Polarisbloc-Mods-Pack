using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;


namespace Polarisbloc
{
    public class Verb_CastUseEffect : Verb_CastBase
    {
        protected override bool TryCastShot()
        {
            Pawn casterPawn = this.CasterPawn;
            if (casterPawn == null)
            {
                return false;
            }
            foreach (CompUseEffect compUseEffect in base.EquipmentSource.GetComps<CompUseEffect>())
            {
                if (compUseEffect.CanBeUsedBy(casterPawn, out string failReason))
                {
                    compUseEffect.DoEffect(casterPawn);
                }
                else
                {
                    Messages.Message(failReason, casterPawn, MessageTypeDefOf.NegativeEvent, false);
                }
            }
            CompReloadable reloadableCompSource = base.ReloadableCompSource;
            if (reloadableCompSource != null)
            {
                reloadableCompSource.UsedOnce();
            }
            return true;
        }

        public override bool Available()
        {
            Pawn casterPawn = this.CasterPawn;
            if (casterPawn == null)
            {
                return false;
            }
            if (base.Available())
            {
                foreach (CompUseEffect compUseEffect in base.EquipmentSource.GetComps<CompUseEffect>())
                {
                    if (!compUseEffect.CanBeUsedBy(casterPawn, out string failReason))
                    {
                        return false;
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
            
        }
    }
}
