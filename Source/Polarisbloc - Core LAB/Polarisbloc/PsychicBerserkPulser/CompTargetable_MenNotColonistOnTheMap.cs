using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompTargetable_MenNotColonistOnTheMap : CompTargetable_AllPawnsOnTheMap
    {
        protected override TargetingParameters GetTargetingParameters()
        {
            TargetingParameters targetingParameters = base.GetTargetingParameters();
            targetingParameters.validator = delegate (TargetInfo targ)
            {
                bool result;
                if (!base.BaseTargetValidator(targ.Thing))
                {
                    result = false;
                }
                else
                {
                    Pawn pawn = targ.Thing as Pawn;
                    result = (pawn != null && !pawn.RaceProps.Animal && pawn.Faction != Faction.OfPlayer);
                    if (Rand.Chance(0.7f))
                    {
                        result = false;
                    }
                }
                return result;
            };
            return targetingParameters;
        }
    }
}
