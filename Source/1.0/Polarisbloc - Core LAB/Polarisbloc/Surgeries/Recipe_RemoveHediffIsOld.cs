using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class Recipe_RemoveHediffIsOld : Recipe_Surgery
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            List<Hediff> allHediffs = pawn.health.hediffSet.hediffs;
            List<BodyPartRecord> appBodies = new List<BodyPartRecord>();
            appBodies.Clear();
            for (int i = 0; i < allHediffs.Count; i++)
            {
                if (allHediffs[i].Part != null && allHediffs[i].IsPermanent() && allHediffs[i].Visible && !appBodies.Contains(allHediffs[i].Part))
                {
                    appBodies.Add(allHediffs[i].Part);
                }
            }
            if (!appBodies.NullOrEmpty())
            {
                foreach (BodyPartRecord bodyPart in appBodies)
                {
                    yield return bodyPart;
                }
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
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
                Hediff hediff = pawn.health.hediffSet.hediffs.Find((Hediff x) => x.IsPermanent() && x.Part == part && x.Visible);
                if (hediff != null)
                {
                    pawn.health.RemoveHediff(hediff);
                    if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(billDoer))
                    {
                        Messages.Message("PolarisMessageSuccessfullyRemovedHediffIsOld".Translate(billDoer.LabelShort, pawn.LabelShort, hediff.LabelCap), pawn, MessageTypeDefOf.PositiveEvent);
                    }
                }
            }
            
        }

    }
}
