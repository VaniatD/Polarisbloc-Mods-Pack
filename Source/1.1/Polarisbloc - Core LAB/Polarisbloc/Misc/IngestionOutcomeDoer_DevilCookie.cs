using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc
{
    public class IngestionOutcomeDoer_DevilCookie : IngestionOutcomeDoer
    {
        private int growDays = -1;

        protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested)
        {
            Thing thing = ThingMaker.MakeThing(ingested.def, ingested.Stuff);
            thing.stackCount = thing.def.ingestible.maxNumToIngestAtOnce;
            thing.SetForbidden(ingested.IsForbidden(pawn.Faction));
            if (pawn.MapHeld != null)
            {
                GenPlace.TryPlaceThing(thing, pawn.PositionHeld, pawn.MapHeld, ThingPlaceMode.Direct);
            }
            else
            {
                pawn.inventory.TryAddItemNotForSale(thing);
            }
            if (this.growDays > 0)
            {
                if (pawn.RaceProps.Humanlike || pawn.RaceProps.baseBodySize <= 1f)
                {
                    pawn.ageTracker.AgeBiologicalTicks += this.growDays * GenDate.TicksPerDay;
                }
                else
                {
                    pawn.ageTracker.AgeBiologicalTicks += (long)(this.growDays * GenDate.TicksPerDay / pawn.RaceProps.baseBodySize);
                }
            }
        }
    }
}
