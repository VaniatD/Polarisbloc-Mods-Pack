using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using RimWorld.Planet;
using Verse.AI.Group;

namespace Polarisbloc
{
    public static class VoidKeyUtility
    {
        /*public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            if (this.TryRandomlyMissingColonist(usedBy, out Pawn pawn))
            {
                this.ResurrectPawnFromVoid(this.parent.MapHeld, this.parent.PositionHeld, pawn);
                this.GiveSideEffects(pawn);
            }
            else
            {
                Messages.Message("FoundNoColonist", MessageTypeDefOf.NeutralEvent);
            }
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            bool result = this.TryRandomlyMissingColonist(p, out Pawn pawn);
            failReason = "FoundNoColonist";
            return result;
        }*/

        public static bool TryRandomlyMissingColonist(out Pawn pawn)
        {
            bool result = false;
            pawn = (from x in Find.WorldPawns.AllPawnsDead
                    where x.Faction == Faction.OfPlayer && x.Corpse == null
                    select x).RandomElement();
            if (pawn != null)
            {
                result = true;
            }
            
            return result;
        }

        public static void ResurrectPawnFromVoid(Map map, IntVec3 loc, Pawn pawn)
        {
            Corpse corpse = pawn.Corpse;
            if (corpse != null)
            {
                corpse.Destroy(DestroyMode.Vanish);
            }
            if (pawn.IsWorldPawn())
            {
                Find.WorldPawns.RemovePawn(pawn);
            }
            pawn.ForceSetStateToUnspawned();
            PawnComponentsUtility.CreateInitialComponents(pawn);
            pawn.health.Notify_Resurrected();
            if (pawn.Faction != null && pawn.Faction.IsPlayer)
            {
                if (pawn.workSettings != null)
                {
                    pawn.workSettings.EnableAndInitialize();
                }
                Find.StoryWatcher.watcherPopAdaptation.Notify_PawnEvent(pawn, PopAdaptationEvent.GainedColonist);
            }
            GenSpawn.Spawn(pawn, loc, map, WipeMode.Vanish);
            for (int i = 0; i < 10; i++)
            {
                MoteMaker.ThrowAirPuffUp(pawn.DrawPos, map);
            }
            if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer && pawn.HostileTo(Faction.OfPlayer))
            {
                LordMaker.MakeNewLord(pawn.Faction, new LordJob_AssaultColony(pawn.Faction, true, true, false, false, true), pawn.Map, Gen.YieldSingle<Pawn>(pawn));
            }
            if (pawn.apparel != null)
            {
                List<Apparel> wornApparel = pawn.apparel.WornApparel;
                for (int j = 0; j < wornApparel.Count; j++)
                {
                    wornApparel[j].Notify_PawnResurrected();
                }
            }
            PawnDiedOrDownedThoughtsUtility.RemoveDiedThoughts(pawn);
        }

        public static void GiveSideEffects(Pawn pawn)
        {
            BodyPartRecord brain = pawn.health.hediffSet.GetBrain();
            Hediff hediff = HediffMaker.MakeHediff(HediffDefOf.ResurrectionSickness, pawn, null);
            if (!pawn.health.WouldDieAfterAddingHediff(hediff))
            {
                pawn.health.AddHediff(hediff, null, null, null);
            }
            if (Rand.Chance(0.02f) && brain != null)
            {
                Hediff hediff2 = HediffMaker.MakeHediff(HediffDefOf.Dementia, pawn, brain);
                if (!pawn.health.WouldDieAfterAddingHediff(hediff2))
                {
                    pawn.health.AddHediff(hediff2, null, null, null);
                }
            }
            if (Rand.Chance(0.02f))
            {
                IEnumerable<BodyPartRecord> enumerable = from x in pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null)
                                                         where x.def == BodyPartDefOf.Eye
                                                         select x;
                foreach (BodyPartRecord partRecord in enumerable)
                {
                    Hediff hediff3 = HediffMaker.MakeHediff(HediffDefOf.Blindness, pawn, partRecord);
                    pawn.health.AddHediff(hediff3, null, null, null);
                }
            }
            if (brain != null)
            {
                if (Rand.Chance(0.2f))
                {
                    Hediff hediff4 = HediffMaker.MakeHediff(HediffDefOf.ResurrectionPsychosis, pawn, brain);
                    if (!pawn.health.WouldDieAfterAddingHediff(hediff4))
                    {
                        pawn.health.AddHediff(hediff4, null, null, null);
                    }
                }
            }
            if (pawn.Dead)
            {
                Log.Error("The pawn has died while being resurrected.", false);
                ResurrectionUtility.Resurrect(pawn);
            }
        }
    }
}
