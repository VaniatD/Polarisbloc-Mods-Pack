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
    }
}
