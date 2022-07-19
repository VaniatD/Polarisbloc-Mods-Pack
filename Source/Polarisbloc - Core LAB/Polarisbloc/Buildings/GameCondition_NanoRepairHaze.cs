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
                    this.TryRepairThing(thing);
                    this.TryRefreshThing(thing);
                }
                
                
                /*bool flag1 = this.TryRepairThing(thing);
                bool flag2 = this.TryRefreshThing(thing);
                if (flag1 || flag2)
                {
                    Effecter effecter = EffecterDefOf.Mine.Spawn();

                    FleckMaker.Static(c, map, FleckDefOf.ShotFlash, 4);
                }*/
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

        private void TryRepairPawnHolder(Pawn pawn)
        {
            List<Apparel> apparels = pawn.apparel?.WornApparel;
            if (!apparels.NullOrEmpty())
            {
                foreach (Apparel ap in apparels)
                {
                    this.TryRepairThing(ap);
                }
            }
            List<ThingWithComps> equipments = pawn.equipment?.AllEquipmentListForReading;
            if (!equipments.NullOrEmpty())
            {
                foreach (ThingWithComps eq in equipments)
                {
                    this.TryRepairThing(eq);
                }
            }
            List<Thing> things = pawn.inventory?.GetDirectlyHeldThings().ToList();
            if (!things.NullOrEmpty())
            {
                foreach (Thing thing in things)
                {
                    this.TryRepairThing(thing);
                }
            }
        }

        private void TryRepairThing(Thing thing)
        {
            //bool result = false;
            if (thing.def.useHitPoints && thing.HitPoints < thing.MaxHitPoints)
            {
                thing.HitPoints += Mathf.CeilToInt((thing.MaxHitPoints - thing.HitPoints) * 0.01f);
                if (thing.HitPoints == thing.MaxHitPoints && thing is Apparel ap && ap.WornByCorpse)
                {
                    ap.Notify_PawnResurrected();
                }
                //result = true;
            }
            if (thing is Corpse corpse && corpse.InnerPawn.apparel != null)
            {
                
                List<Apparel> wornApparel = corpse.InnerPawn.apparel.WornApparel;
                foreach (Apparel apparel in wornApparel)
                {
                    this.TryRepairThing(apparel);
                }
            }
            //return result;
        }

        private void TryRefreshThing(Thing thing)
        {
            //bool result = false;
            CompRottable compRottable = thing.TryGetComp<CompRottable>();
            if (compRottable != null)
            {
                compRottable.RotProgress -= 2000f;
                if (compRottable.RotProgress < 0)
                {
                    compRottable.RotProgress = 0;
                }
                //result = true;
            }
            //return result;
        }
    }

    
}
