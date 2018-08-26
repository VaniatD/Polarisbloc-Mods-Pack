using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Polarisbloc
{
    public class ThoughtWorker_ChipCharm : ThoughtWorker
    {
        protected override ThoughtState CurrentSocialStateInternal(Pawn pawn, Pawn other)
        {
            ThoughtState result;
            if (!other.RaceProps.Humanlike || !RelationsUtility.PawnsKnowEachOther(pawn, other))
            {
                result = false;
            }
            else if (RelationsUtility.IsDisfigured(other))
            {
                result = false;
            }
            else
            {
                if (other.health.hediffSet.HasHediff(PolarisblocDefOf.PolarisCombatChip_Charm))
                {
                    result = ThoughtState.ActiveAtStage(0);
                }
                /*int num = other.story.traits.DegreeOfTrait(TraitDefOf.Beauty);
                if (num == 1)
                {
                    result = ThoughtState.ActiveAtStage(0);
                }
                else if (num == 2)
                {
                    result = ThoughtState.ActiveAtStage(1);
                }*/
                else
                {
                    result = false;
                }
            }
            return result;
        }
    }
}
