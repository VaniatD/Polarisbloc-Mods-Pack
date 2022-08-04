using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;
using Verse.Grammar;
using UnityEngine;

namespace Polarisbloc
{
    public class GameCondition_NanoRepairHaze : GameCondition
    {

        public override void DoCellSteadyEffects(IntVec3 c, Map map)
        {
            base.DoCellSteadyEffects(c, map);
            foreach (Thing thing in c.GetThingList(map))
            {
                if (thing is Pawn pawn)
                {
                    this.TryRepairPawnHolder(pawn);
                }
                else
                {
                    if (this.TryRepairThing(thing))
                    {
                        continue;
                    }
                    if (this.TryRefreshThing(thing))
                    {
                        continue;
                    }
                    if (this.TryCureBlighted(thing))
                    {
                        break;
                    }
                }
            }
        }

        /*public override void GameConditionTick()
        {
            base.GameConditionTick();
            if (this.TicksPassed % 2000 == 0)
            {
                List<Pawn> allPawnsSpawned = this.SingleMap.mapPawns.AllPawnsSpawned;
                foreach (Pawn pawn in allPawnsSpawned)
                {
                    this.TryRepairPawnHolder(pawn);
                }
            }
        }*/

        private bool TryRepairPawnHolder(Pawn pawn)
        {
            bool result = false;
            List<Apparel> apparels = pawn.apparel?.WornApparel;
            if (!apparels.NullOrEmpty())
            {
                foreach (Apparel ap in apparels)
                {
                    if (this.TryRepairThing(ap))
                    {
                        result = true;
                    }
                }
            }
            List<ThingWithComps> equipments = pawn.equipment?.AllEquipmentListForReading;
            if (!equipments.NullOrEmpty())
            {
                foreach (ThingWithComps eq in equipments)
                {
                    if (this.TryRepairThing(eq))
                    {
                        result = true;
                    }
                }
            }
            ThingOwner<Thing> things = pawn.inventory?.innerContainer;
            if (!things.NullOrEmpty())
            {
                foreach (Thing thing in things)
                {
                    if (this.TryRepairThing(thing))
                    {
                        result = true;
                    }
                }
            }
            return result;
        }

        private bool TryRepairThing(Thing thing)
        {
            bool result = false;
            if (thing.def.useHitPoints)
            {
                if (thing.HitPoints < thing.MaxHitPoints)
                {
                    thing.HitPoints += Mathf.CeilToInt((thing.MaxHitPoints - thing.HitPoints) * 0.01f);
                    result = true;
                }
                else if (thing.HitPoints == thing.MaxHitPoints && thing is Apparel ap && ap.WornByCorpse)
                {
                    ap.Notify_PawnResurrected();
                    result = true;
                }
                
            }
            if (thing is Corpse corpse && corpse.InnerPawn != null)
            {
                
                if (this.TryRepairPawnHolder(corpse.InnerPawn))
                {
                    result = true;
                }
            }
            return result;
        }

        private bool TryRefreshThing(Thing thing)
        {
            bool result = false;
            CompRottable compRottable = thing.TryGetComp<CompRottable>();
            if (compRottable != null)
            {
                compRottable.RotProgress -= 2000f;
                if (compRottable.RotProgress < 0)
                {
                    compRottable.RotProgress = 0;
                }
                result = true;
            }
            return result;
        }

        private bool TryCureBlighted(Thing thing)
        {
            if (thing is Plant plant)
            {
                if (plant.Blighted)
                {
                    plant.Blight.Destroy();
                    return true;
                }
            }
            return false;
        }
    }

    
}
