using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Reflection;

namespace Polarisbloc
{
    public static class PolarisUtility
    {
        public static void RefreshPawnStat(Pawn pawn)
        {
            if (pawn.workSettings != null)
            {
                pawn.Notify_DisabledWorkTypesChanged();
            }
            if (pawn.skills != null)
            {
                pawn.skills.Notify_SkillDisablesChanged();
            }
            if (!pawn.Dead && pawn.RaceProps.Humanlike)
            {
                pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
            }
        }

        public static void GainSkillsExtra(Pawn pawn, Dictionary<SkillDef, int> skillGains, bool getSkill)
        {
            if (getSkill)
            {
                foreach (KeyValuePair<SkillDef, int> skillGain in skillGains)
                {
                    pawn.skills.GetSkill(skillGain.Key).Level += skillGain.Value;
                }
            }
            else
            {
                foreach (KeyValuePair<SkillDef, int> skillGain in skillGains)
                {
                    pawn.skills.GetSkill(skillGain.Key).Level -= skillGain.Value;
                }
            }
        }


        /*public static void DecodeBiocoded(this Thing thing)
        {
            CompBiocodable biocodableThing = thing.TryGetComp<CompBiocodable>();
            
            if (biocodableThing != null)
            {

                if (biocodableThing.Biocoded)
                {
                    typeof(CompBiocodable).GetField("biocoded", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(biocodableThing, false);
                    typeof(CompBiocodable).GetField("codedPawn", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(biocodableThing, null);
                }
            }
            
        }

        public static void UnbondBladelink(this Thing thing)
        {
            CompBladelinkWeapon bladelinkWeapon = thing.TryGetComp<CompBladelinkWeapon>();
            if (bladelinkWeapon != null)
            {
                Pawn oldBondedPawn = (Pawn)typeof(CompBladelinkWeapon).GetField("oldBondedPawn", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(bladelinkWeapon);
                if (oldBondedPawn != null)
                {
                    typeof(CompBladelinkWeapon).GetField("bonded", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(bladelinkWeapon, false);
                    typeof(CompBladelinkWeapon).GetField("oldBondedPawn", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(bladelinkWeapon, null);
                }
            }
        }*/

        public static bool IsBiocodableThing(this Thing thing)
        {
            if (thing.TryGetComp<CompBiocodable>() != null)
            {
                return true;
            }
            return false;
        }

        public static bool IsBladelinkWeapon(this Thing weapon, out CompBladelinkWeapon compBladelink)
        {
            compBladelink = weapon.TryGetComp<CompBladelinkWeapon>();
            if (compBladelink != null)
            {
                return true;
            }
            return false;
        }

        public static bool CanAddWeaponTrait(this CompBladelinkWeapon compBladelink, WeaponTraitDef traitDef)
        {
            List<WeaponTraitDef> curTraits = compBladelink.TraitsListForReading;
            if (curTraits.NullOrEmpty<WeaponTraitDef>())
            {
                return true;
            }
            for (int i = 0; i < curTraits.Count; i++)
            {
                if (traitDef.Overlaps(curTraits[i]))
                {
                    return false;
                }
            }
            return true;
        }

        public static void AddWeaponTrait(this CompBladelinkWeapon compBladelink, WeaponTraitDef traitDef)
        {
            List<WeaponTraitDef> curTraits = compBladelink.TraitsListForReading;
            if (!curTraits.Contains(traitDef))
            {
                curTraits.Add(traitDef);
            }
            typeof(CompBladelinkWeapon).GetField("traits", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(compBladelink, curTraits);
        }

        public static void RemoveWeaponTrait(this CompBladelinkWeapon compBladelink, WeaponTraitDef traitDef)
        {
            List<WeaponTraitDef> curTraits = compBladelink.TraitsListForReading;
            curTraits.Remove(traitDef);
            typeof(CompBladelinkWeapon).GetField("traits", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(compBladelink, curTraits);
        }

        public static bool IsBondedFor(this Thing weapon, Pawn pawn)
        {
            return pawn.equipment.bondedWeapon == weapon;
        }

        public static float GetTraitSpecificCommonality(this TraitDef traitDef)
        {
            float commonality = (float)typeof(TraitDef).GetField("commonality", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(traitDef);
            float commonalityFemale = (float)typeof(TraitDef).GetField("commonalityFemale", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(traitDef);
            if (commonalityFemale > 0) commonality = (commonalityFemale + commonality) / 2;
            return commonality;
        }

        public static bool ThingAddWeaponTrait(Thing weapon)
        {
            if (weapon.IsBladelinkWeapon(out CompBladelinkWeapon compBladelink))
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
                return true;
            }
            return false;
        }

        public static bool ThingRemoveWeaponTrait(Thing thing)
        {
            if (thing.IsBladelinkWeapon(out CompBladelinkWeapon compBladelink))
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
                return true;
            }
            return false;
        }



    }
}
