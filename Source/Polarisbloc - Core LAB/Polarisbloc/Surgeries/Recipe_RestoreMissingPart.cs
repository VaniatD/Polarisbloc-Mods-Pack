using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class Recipe_RestoreMissingPart : Recipe_Surgery
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            foreach (Hediff_MissingPart missingPartsCommonAncestor in pawn.health.hediffSet.GetMissingPartsCommonAncestors())
            {
                yield return missingPartsCommonAncestor.Part;
                /*if (!pawn.health.hediffSet.HasDirectlyAddedPartFor(missingPartsCommonAncestor.Part))
                {
                    
                }*/
            }
            yield break;
        }

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
                {
                    billDoer,
                    pawn
                });
                pawn.health.RestorePart(part, null, true);
                float amount = part.def.GetMaxHealth(pawn) * 0.75f;
                DamageInfo dinfo = new DamageInfo(DamageDefOf.SurgicalCut, amount, 2f, -1f, null, part, null, DamageInfo.SourceCategory.ThingOrUnknown, null);
                pawn.TakeDamage(dinfo);
            }
        }
    }
}
