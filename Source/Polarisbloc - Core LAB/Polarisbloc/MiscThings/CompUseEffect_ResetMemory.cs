﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

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
                    PolarisUtility.RefreshPawnStat(usedBy);
                }
                else
                {
                    Messages.Message("PolarisMomeryResterHaveNoAvaliableStory".Translate(usedBy.LabelShort, childhoodStory.title), usedBy, MessageTypeDefOf.NegativeEvent);
                    PolarisUtility.GainSkillsExtra(usedBy, childhoodStory.skillGainsResolved, true);
                    usedBy.story.childhood = childhoodStory;
                    PolarisUtility.RefreshPawnStat(usedBy);
                }
                    
            }
            else if (memResetMode == MemResetMode.adulthood)
            {
                if (usedBy.story.adulthood != null)
                {
                    Messages.Message("PolarisMomeryResterStorySuccessfullyChanged".Translate(usedBy.LabelShort, usedBy.story.adulthood.title, adulthoodStory.title), usedBy, MessageTypeDefOf.PositiveEvent);
                    PolarisUtility.GainSkillsExtra(usedBy, usedBy.story.adulthood.skillGainsResolved, false);
                    PolarisUtility.GainSkillsExtra(usedBy, adulthoodStory.skillGainsResolved, true);
                    usedBy.story.adulthood = adulthoodStory;
                    PolarisUtility.RefreshPawnStat(usedBy);
                }
                else
                {
                    Messages.Message("PolarisMomeryResterHaveNoAvaliableStory".Translate(usedBy.LabelShort, adulthoodStory.title), usedBy, MessageTypeDefOf.NegativeEvent);
                    PolarisUtility.GainSkillsExtra(usedBy, adulthoodStory.skillGainsResolved, true);
                    usedBy.story.adulthood = adulthoodStory;
                    PolarisUtility.RefreshPawnStat(usedBy);
                }
            }
            else Messages.Message("PolarisMomeryResterUnknownError".Translate(), MessageTypeDefOf.NegativeEvent);
        }
    }
}
