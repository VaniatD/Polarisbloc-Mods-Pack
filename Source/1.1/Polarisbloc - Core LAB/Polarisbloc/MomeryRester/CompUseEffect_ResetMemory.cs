using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using System.Reflection;

namespace Polarisbloc
{
    public class CompUseEffect_ResetMemory : CompUseEffect
    {
        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            MemResetMode memResetMode = this.parent.GetComp<CompMomeryRester>().memResetMode;
            Backstory childhoodStory = this.parent.GetComp<CompMomeryRester>().childhoodStory;
            Backstory adulthoodStory = this.parent.GetComp<CompMomeryRester>().adulthoodStory;
            if (memResetMode == MemResetMode.childhood)
            {
                if (usedBy.story.childhood != null)
                {
                    Messages.Message("PolarisMomeryResterStorySuccessfullyChanged".Translate(usedBy.LabelShort, usedBy.story.childhood.title, childhoodStory.title), usedBy, MessageTypeDefOf.PositiveEvent);
                    PolarisUtility.GainSkillsExtra(usedBy, usedBy.story.childhood.skillGainsResolved, false);
                    PolarisUtility.GainSkillsExtra(usedBy, childhoodStory.skillGainsResolved, true);
                    usedBy.story.childhood = childhoodStory;
                    //this.RefreshPawnStat(usedBy);
                    PolarisUtility.RefreshPawnStat(usedBy);
                }
                else Messages.Message("PolarisMomeryResterHaveNoAvaliableStory".Translate(usedBy.LabelShort), usedBy, MessageTypeDefOf.NegativeEvent);
            }
            else if (memResetMode == MemResetMode.adulthood)
            {
                if (usedBy.story.adulthood != null)
                {
                    Messages.Message("PolarisMomeryResterStorySuccessfullyChanged".Translate(usedBy.LabelShort, usedBy.story.adulthood.title, adulthoodStory.title), usedBy, MessageTypeDefOf.PositiveEvent);
                    PolarisUtility.GainSkillsExtra(usedBy, usedBy.story.adulthood.skillGainsResolved, false);
                    PolarisUtility.GainSkillsExtra(usedBy, adulthoodStory.skillGainsResolved, true);
                    usedBy.story.adulthood = adulthoodStory;
                    //this.RefreshPawnStat(usedBy);
                    PolarisUtility.RefreshPawnStat(usedBy);
                }
                else Messages.Message("PolarisMomeryResterHaveNoAvaliableStory".Translate(usedBy.LabelShort), usedBy, MessageTypeDefOf.NegativeEvent);
            }
            else Messages.Message("PolarisMomeryResterUnknownError".Translate(), MessageTypeDefOf.NegativeEvent);
        }

        /*private void RefreshPawnStat(Pawn pawn)
        {
            if (pawn.workSettings != null)
            {
                pawn.workSettings.Notify_GainedTrait();
            }
            typeof(Pawn_StoryTracker).GetField("cachedDisabledWorkTypes", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(pawn.story, null);
            if (pawn.skills != null)
            {
                pawn.skills.Notify_SkillDisablesChanged();
            }
            if (!pawn.Dead && pawn.RaceProps.Humanlike)
            {
                pawn.needs.mood.thoughts.situational.Notify_SituationalThoughtsDirty();
            }
        }*/
    }
}
