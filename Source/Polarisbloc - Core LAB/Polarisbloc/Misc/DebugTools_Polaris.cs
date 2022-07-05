using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;
using Verse.AI;
using Verse.AI.Group;

namespace Polarisbloc
{
    public static class DebugTools_Polaris
    {

        [DebugAction("Polaris Tools", "Max Hit Points", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void MaxHitPoints()
        {
            foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList<Thing>())
            {
                if (thing.def.useHitPoints)
                {
                    thing.HitPoints = thing.MaxHitPoints;
                    DebugActionsUtility.DustPuffFrom(thing);
                }
            }
        }

        [DebugAction("Polaris Tools", "Set Hit Points To 1", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void MinHitPoints()
        {
            foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList<Thing>())
            {
                if (thing.def.useHitPoints)
                {
                    thing.HitPoints = 1;
                    DebugActionsUtility.DustPuffFrom(thing);
                }
            }
        }

        [DebugAction("Polaris Tools", "Uninstal Building", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void UninstalBuilding()
        {
            foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList<Thing>())
            {
                DebugActionsUtility.DustPuffFrom(thing);
                if (thing is Building building && building.def.Minifiable)
                {
                    MinifiedThing minifiedThing = building.MakeMinified();
                    GenSpawn.Spawn(minifiedThing, UI.MouseCell(), Find.CurrentMap, WipeMode.Vanish);
                }
            }
        }

        [DebugAction("Polaris Tools", "Convert to silver", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ConvertToSilver()
        {
            float money = 0f;
            foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList<Thing>())
            {
                DebugActionsUtility.DustPuffFrom(thing);
                money += thing.GetStatValue(StatDefOf.MarketValue, true) * thing.stackCount;
                thing.Destroy(DestroyMode.Vanish);
            }
            int silverCount = Mathf.RoundToInt(money);
            if (silverCount > 0)
            {
                Thing silver = ThingMaker.MakeThing(ThingDefOf.Silver);
                silver.stackCount = silverCount;
                GenPlace.TryPlaceThing(silver, UI.MouseCell(), Find.CurrentMap, ThingPlaceMode.Near);
            }
        }

        [DebugAction("Polaris Tools", "Decode Biocoded", actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void DecodeBiocoded()
        {
            foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList<Thing>())
            {
                CompBiocodable compBiocodable = thing.TryGetComp<CompBiocodable>();
                if (compBiocodable != null && compBiocodable.Biocoded)
                {
                    compBiocodable.UnCode();
                }
                DebugActionsUtility.DustPuffFrom(thing);
            }
        }

        [DebugAction("Polaris Tools", "Add Weapon Trait", true, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap, requiresRoyalty = true)]
        private static void AddWeaponTrait()
        {
            foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList<Thing>())
            {
                /*if (thing.IsBladelinkWeapon(out CompBladelinkWeapon compBladelink))
                {
                    List<DebugMenuOption> list = new List<DebugMenuOption>();
                    foreach (WeaponTraitDef traitDef in DefDatabase<WeaponTraitDef>.AllDefs)
                    {
                        if (compBladelink.CanAddWeaponTrait(traitDef))
                        {
                            list.Add(new DebugMenuOption(traitDef.label, DebugMenuOptionMode.Action, delegate ()
                            {
                                compBladelink.AddWeaponTrait(traitDef);
                            }));
                        }

                    }
                    Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
                }*/
                PolarisUtility.ThingAddWeaponTrait(thing);
                DebugActionsUtility.DustPuffFrom(thing);
            }
        }

        [DebugAction("Polaris Tools", "Remove Weapon Trait", true, false, actionType = DebugActionType.ToolMap, allowedGameStates = AllowedGameStates.PlayingOnMap, requiresRoyalty = true)]
        private static void RemoveWeaponTrait()
        {
            foreach (Thing thing in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell()).ToList<Thing>())
            {
                /*if (thing.IsBladelinkWeapon(out CompBladelinkWeapon compBladelink))
                {
                    List<WeaponTraitDef> curTraits = compBladelink.TraitsListForReading;
                    List<DebugMenuOption> list = new List<DebugMenuOption>();
                    foreach (WeaponTraitDef curTrait in curTraits)
                    {
                        list.Add(new DebugMenuOption(curTrait.label, DebugMenuOptionMode.Action, delegate ()
                        {
                            //curTraits.Remove(curTrait);
                            //typeof(CompBladelinkWeapon).GetField("traits", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(compBladelink, curTraits);
                            compBladelink.RemoveWeaponTrait(curTrait);
                        }));
                    }
                    Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
                }*/
                PolarisUtility.ThingRemoveWeaponTrait(thing);
                DebugActionsUtility.DustPuffFrom(thing);
            }
        }

        [DebugAction("Polaris Tools", "Done Abilities Cooldown", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap, requiresRoyalty = true)]
        private static void DoneAbilitiesCooldown(Pawn pawn)
        {
            foreach (Ability ability in pawn.abilities.abilities)
            {
                if (ability?.CooldownTicksRemaining > 0)
                {
                    ability.StartCooldown(0);
                }
            }
        }


        [DebugAction("Polaris Tools", "Remove Ability...", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap, requiresRoyalty = true)]
        private static void RemoveAbility(Pawn pawn)
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            foreach (Ability ability in pawn.abilities.abilities)
            {
                list.Add(new DebugMenuOption(ability.def.LabelCap, DebugMenuOptionMode.Action, delegate ()
                {
                    //pawn.abilities.abilities.Remove(ability);
                    pawn.abilities.RemoveAbility(ability.def);
                    DebugActionsUtility.DustPuffFrom(pawn);
                }));
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }

        [DebugAction("Polaris Tools", "Remove Trait...", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void RemoveTrait(Pawn pawn)
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            foreach (Trait trait in pawn.story.traits.allTraits)
            {
                list.Add(new DebugMenuOption(string.Concat(new object[]
                    {
                        trait.LabelCap,
                        " (",
                        trait.Degree,
                        ")"
                    }), DebugMenuOptionMode.Action, delegate ()
                    {
                        PolarisUtility.GainSkillsExtra(pawn, trait.CurrentData.skillGains, false);
                        pawn.story.traits.RemoveTrait(trait);
                        DebugActionsUtility.DustPuffFrom(pawn);
                    }));
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }

        [DebugAction("Polaris Tools", "Set Biological Age", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SetBioAge(Pawn pawn)
        {
            Find.WindowStack.Add(new Dialog_ChangeAge(pawn));

        }

        [DebugAction("Polaris Tools", "Trans Gender", actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void TransGender(Pawn pawn)
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
            DebugActionsUtility.DustPuffFrom(pawn);
        }

        [DebugAction("Polaris Tools", "Change Body...", actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void ChangeBodyType()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            foreach (BodyTypeDef bodyType in DefDatabase<BodyTypeDef>.AllDefs)
            {
                list.Add(new DebugMenuOption(bodyType.defName, DebugMenuOptionMode.Tool, delegate ()
                {
                    foreach (Pawn pawn in (from t in Find.CurrentMap.thingGrid.ThingsAt(UI.MouseCell())
                                           where t is Pawn
                                           select t).Cast<Pawn>())
                    {
                        pawn.story.bodyType = bodyType;
                        PortraitsCache.SetDirty(pawn);
                        PortraitsCache.PortraitsCacheUpdate();
                        pawn.Drawer.renderer.graphics.SetAllGraphicsDirty();
                        pawn.Drawer.renderer.graphics.ResolveAllGraphics();
                        DebugActionsUtility.DustPuffFrom(pawn);
                    }
                }));
            }
            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }

        [DebugAction("Polaris Tools", "Spawning Miss Pawn Of Player", allowedGameStates = AllowedGameStates.PlayingOnMap)]
        private static void SpawningMissPawn()
        {
            List<DebugMenuOption> list = new List<DebugMenuOption>();
            foreach (Pawn pawn in (from x in Find.WorldPawns.AllPawnsDead
                                   where x.Corpse == null && x.Faction == Faction.OfPlayer
                                   select x))
            {
                list.Add(new DebugMenuOption(pawn.NameFullColored + "(" + pawn.kindDef.race.LabelCap + ")", DebugMenuOptionMode.Tool, delegate ()
                {
                    if (pawn.Dead)
                    {
                        Map map = Find.CurrentMap;
                        IntVec3 loc = UI.MouseCell();
                        VoidKeyUtility.ResurrectPawnFromVoid(map, loc, pawn);
                    }
                }));
            }

            Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
        }

        [DebugAction("Polaris Tools", "Force suppress", false, false, actionType = DebugActionType.ToolMapForPawns, allowedGameStates = AllowedGameStates.PlayingOnMap, requiresIdeology = true)]
        private static void ForceSuppress(Pawn slave)
        {
            slave.GetLord().Notify_PawnLost(slave, PawnLostCondition.MadeSlave);
            slave.Notify_LordDestroyed();
            //slave.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
            //GenGuest.EnslavePrisoner(this.Pawn.Map.mapPawns.FreeColonists.RandomElement(), this.Pawn);
            slave.needs.TryGetNeed<Need_Suppression>().CurLevelPercentage = 1;
            DebugActionsUtility.DustPuffFrom(slave);
        }

    }
}
