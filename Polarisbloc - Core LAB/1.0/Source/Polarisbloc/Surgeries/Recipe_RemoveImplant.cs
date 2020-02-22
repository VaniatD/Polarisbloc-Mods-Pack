using System;
using System.Collections.Generic;
using Verse;
using RimWorld;
using System.Linq;

namespace Polarisbloc
{
    public class Recipe_RemoveImplant : Recipe_Surgery
    {
        public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
        {
            List<Hediff> allHediffs = pawn.health.hediffSet.hediffs;
            List<BodyPartRecord> appBodies = new List<BodyPartRecord>();
            appBodies.Clear();
            for (int i =0; i < allHediffs.Count; i++)
            {
                if (allHediffs[i].Part != null && allHediffs[i].Visible && allHediffs[i] is Hediff_Implant && !appBodies.Contains(allHediffs[i].Part))
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
                List<Hediff> hediffs = new List<Hediff>();
                hediffs.Clear();
                string textHediffs = string.Empty;
                foreach (Hediff tempHediff in pawn.health.hediffSet.hediffs)
                {
                    if (tempHediff is Hediff_Implant && tempHediff.Part == part && tempHediff.Visible)
                    {
                        textHediffs += "*" + tempHediff.LabelCap;
                        hediffs.Add(tempHediff);
                    }
                }
                if (PawnUtility.ShouldSendNotificationAbout(pawn) || PawnUtility.ShouldSendNotificationAbout(billDoer) && !hediffs.NullOrEmpty())
                {
                    Messages.Message("MessageSuccessfullyRemovedHediff".Translate(billDoer.LabelShort, pawn.LabelShort, textHediffs), pawn, MessageTypeDefOf.PositiveEvent);
                }
                if (!hediffs.NullOrEmpty())
                {
                    for (int i = hediffs.Count - 1; i >= 0; i--)
                    {
                        pawn.health.RemoveHediff(hediffs[i]);
                        if (hediffs[i].def.spawnThingOnRemoved != null)
                        {
                            Thing thing = ThingMaker.MakeThing(hediffs[i].def.spawnThingOnRemoved);
                            GenPlace.TryPlaceThing(thing, billDoer.Position, billDoer.Map, ThingPlaceMode.Near);
                            //GenSpawn.Spawn(hediffs[i].def.spawnThingOnRemoved, billDoer.Position, billDoer.Map);
                        }
                    }
                }
            }
        }
    }
}
