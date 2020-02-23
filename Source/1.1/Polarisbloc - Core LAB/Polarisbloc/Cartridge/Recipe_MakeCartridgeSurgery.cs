using RimWorld;
using System.Collections.Generic;
using Verse;
using RimWorld.Planet;

namespace Polarisbloc
{
    public class Recipe_MakeCartridgeSurgery : Recipe_Surgery
    {
        /*public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                DamageDef surgicalCut = DamageDefOf.SurgicalCut;
                int amount = 99999;
                pawn.TakeDamage(new DamageInfo(surgicalCut, amount, -1f, null, pawn.health.hediffSet.GetBrain(), null, DamageInfo.SourceCategory.ThingOrUnknown));
                ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, PawnExecutionKind.OrganHarvesting);
            }
        }*/

        public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
        {
            if (billDoer != null)
            {
                if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
                {
                    return;
                }
                if (pawn.relations.OpinionOf(billDoer) < 60)
                {
                    Messages.Message("PolarisMessageFailedMakeCartridge".Translate(pawn.LabelShort, billDoer.LabelShort), pawn, MessageTypeDefOf.NegativeEvent);
                    return;
                }
                TaleRecorder.RecordTale(TaleDefOf.DidSurgery, billDoer, pawn);
                ThoughtUtility.GiveThoughtsForPawnExecuted(pawn, PawnExecutionKind.OrganHarvesting);
                GenSpawn.Spawn(PolarisblocDefOf.PolarisCartridge, billDoer.Position, billDoer.Map);
                pawn.apparel.DropAll(billDoer.Position);
                pawn.DeSpawn();
                pawn.Kill(null);
                //Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                
                //pawn.Destroy();
            }
        }
    }
}
