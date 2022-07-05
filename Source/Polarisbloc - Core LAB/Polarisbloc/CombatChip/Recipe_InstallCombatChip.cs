using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class Recipe_InstallCombatChip : Recipe_InstallImplant
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            foreach (BodyPartDef part in recipe.appliedOnFixedBodyParts)
            {
                foreach (BodyPartRecord record in pawn.RaceProps.body.AllParts)
                {
                    if(record.def == part && pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined).Contains(record) && !pawn.health.hediffSet.PartOrAnyAncestorHasDirectlyAddedParts(record) && !pawn.health.hediffSet.hediffs.Any((Hediff x) => x.Part == record && x.def == recipe.addsHediff) && !pawn.health.hediffSet.hediffs.Any((Hediff x) => x.Part == record && x is Hediff_CombatChip)) //x.def.hediffClass == typeof(Polarisbloc.Hediff_CombatChip)
                    {
                        yield return record;
                    }
                }
            }
            yield break;
        }
    }
}
