using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc
{
    public class IngestionOutcomeDoer_Restorer : IngestionOutcomeDoer
    {
        public bool restoreMissingBodyPart = false;

        public List<HediffDef> extraHediffs = new List<HediffDef>();

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            if (this.restoreMissingBodyPart)
            {
                List<Hediff_MissingPart> missingPartHediffs = pawn.health.hediffSet.GetMissingPartsCommonAncestors();
                for (int i = missingPartHediffs.Count - 1; i >= 0; i--)
                {
                    pawn.health.RestorePart(missingPartHediffs[i].Part, null, true);
                }
            }
            List<Hediff> hediffs = pawn.health.hediffSet.hediffs;
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                if (this.extraHediffs.Contains(hediffs[i].def) || hediffs[i].IsPermanent() || (hediffs[i].def.isBad && !(hediffs[i] is Hediff_Injury) && !(hediffs[i] is Hediff_MissingPart)))
                {
                    pawn.health.RemoveHediff(hediffs[i]);
                }
            }
            Messages.Message("PolarisRestorerIngest".Translate(pawn.LabelShortCap), pawn, MessageTypeDefOf.NeutralEvent);
        }
    }
}
