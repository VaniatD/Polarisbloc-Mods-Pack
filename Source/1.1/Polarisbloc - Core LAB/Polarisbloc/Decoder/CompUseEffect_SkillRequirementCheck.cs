using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Polarisbloc
{
    public class CompUseEffect_SkillRequirementCheck : CompUseEffect
    {
        private CompProperties_UseEffectSkillRequirementCheck Props
        {
            get
            {
                return (CompProperties_UseEffectSkillRequirementCheck)this.props;
            }
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            bool result = base.CanBeUsedBy(p, out failReason);
            if (result)
            {
                foreach (SkillRequirement requirement in this.Props.skillRequirements)
                {
                    result = requirement.PawnSatisfies(p);
                    if (!result)
                    {
                        failReason = "PolarisDecoderSkillRequirementNoSatisfies".Translate(requirement.skill.LabelCap, requirement.minLevel);
                        break;
                    }
                }
            }
            return result;
        }

    }

    public class CompProperties_UseEffectSkillRequirementCheck : CompProperties_UseEffect
    {
        public CompProperties_UseEffectSkillRequirementCheck()
        {
            this.compClass = typeof(CompUseEffect_SkillRequirementCheck);
        }

        public List<SkillRequirement> skillRequirements = new List<SkillRequirement>();



    }
}
