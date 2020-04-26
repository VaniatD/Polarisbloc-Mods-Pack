using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class Recipe_TransgenderSurgery : Recipe_Surgery
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
                this.ChangeGender(pawn);
            }
        }

        private void ChangeGender(Pawn pawn)
        {
            if (pawn.gender == Gender.Male)
            {
                pawn.gender = Gender.Female;
            }
            else if (pawn.gender == Gender.Female)
            {
                pawn.gender = Gender.Male;
            }
            pawn.Drawer.renderer.graphics.ResolveAllGraphics();
        }
    }
}
