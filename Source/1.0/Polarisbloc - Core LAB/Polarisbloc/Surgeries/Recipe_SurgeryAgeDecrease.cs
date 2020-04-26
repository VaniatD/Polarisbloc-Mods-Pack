using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class Recipe_SurgeryAgeDecrease : Recipe_Surgery
    {
        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
                int x = pawn.ageTracker.AgeBiologicalYears;
                this.AgeDecrease(pawn);
                int y = pawn.ageTracker.AgeBiologicalYears;
                int t = x - y;
                string text = "PolarisMessageSuccessfullyAgeDecrease".Translate(pawn.LabelShort, t.ToString());
                Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
            }
        }

        private void AgeDecrease(Pawn pawn)
        {
            int a = pawn.ageTracker.AgeBiologicalYears / 10;
            pawn.ageTracker.AgeBiologicalTicks -= (10800000 * (a + 1));
            if (pawn.ageTracker.AgeBiologicalTicks <= 0)
            {
                pawn.ageTracker.AgeBiologicalTicks = 1;
            }
        }
    }
}
