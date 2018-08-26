using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompTraitreleaser : CompUsable
    {
        public Trait trait;

        public int availableTimes = 3;

        protected override string FloatMenuOptionLabel
        {
            get
            {
                return string.Format(base.Props.useLabel, this.trait.LabelCap);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Deep.Look<Trait>(ref this.trait, "trait");
            Scribe_Values.Look<int>(ref this.availableTimes, "availableTimes");
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            DefDatabase<TraitDef>.AllDefs.TryRandomElementByWeight((TraitDef xtrait) => GetTraitSpecificCommonality(xtrait), out TraitDef tdef);
            int d = this.GetRandomTraitDegree(tdef);
            this.GetExactTraitDegree(tdef, d, out int ed);
            this.trait = new Trait(tdef, ed, false);
        }

        public override string TransformLabel(string label)
        {
            return this.trait.LabelCap + " " + label;
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
            {
                return false;
            }
            CompTraitreleaser compTraitreleaser = other.TryGetComp<CompTraitreleaser>();
            if (compTraitreleaser != null && compTraitreleaser.trait == this.trait)
            {
                return true;
            }
            return false;
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompTraitreleaser compTraitreleaser = piece.TryGetComp<CompTraitreleaser>();
            if (compTraitreleaser != null)
            {
                compTraitreleaser.trait = this.trait;
            }
        }

        public override string GetDescriptionPart()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(this.trait.LabelCap);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(this.trait.CurrentData.description);
            stringBuilder.AppendLine();
            if (this.trait.GetDisabledWorkTypes().Count() > 0)
            {
                string disabledWorkTypes = string.Empty;
                foreach (WorkTypeDef tempWorkTypeDef in this.trait.GetDisabledWorkTypes())
                {
                    disabledWorkTypes += tempWorkTypeDef.LabelCap + " ";
                }
                stringBuilder.AppendLine("PolarisTraitreleaserTraitDisabledWorkTypes".Translate(disabledWorkTypes));
                stringBuilder.AppendLine();
            }
            if (this.trait.CurrentData.skillGains.Count > 0)
            {
                foreach (KeyValuePair<SkillDef, int> skillGain in this.trait.CurrentData.skillGains)
                {
                    if (skillGain.Value > 0)
                    {
                        stringBuilder.Append(skillGain.Key.LabelCap);
                        stringBuilder.Append("  +");
                        stringBuilder.AppendLine(skillGain.Value.ToString());
                    }
                    else
                    {
                        stringBuilder.Append(skillGain.Key.LabelCap);
                        stringBuilder.Append("  ");
                        stringBuilder.AppendLine(skillGain.Value.ToString());
                    }
                }
                stringBuilder.AppendLine();
            }
            if (!this.trait.CurrentData.statOffsets.NullOrEmpty())
            {
                foreach (StatModifier statOffset in this.trait.CurrentData.statOffsets)
                {
                    stringBuilder.Append(statOffset.stat.LabelCap);
                    stringBuilder.AppendLine(statOffset.ValueToStringAsOffset);
                }
                stringBuilder.AppendLine();
            }
            if (!this.trait.CurrentData.statFactors.NullOrEmpty())
            {
                foreach (StatModifier statFactor in this.trait.CurrentData.statFactors)
                {
                    stringBuilder.Append(statFactor.stat.LabelCap);
                    stringBuilder.AppendLine(statFactor.ToStringAsFactor);
                }
                stringBuilder.AppendLine();
            }
            if (this.ConflictingTraitsWith(trait, out List<Trait> ctlist))
            {
                string conflictingTraits = string.Empty;
                foreach (Trait tempTrait in ctlist)
                {
                    conflictingTraits += tempTrait.LabelCap + " ";
                }
                stringBuilder.AppendLine("PolarisTraitreleaserTraitConflictingTraits".Translate(conflictingTraits));
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();


            /*string desc = string.Empty;
            desc += this.trait.LabelCap + "\n\n";
            desc += this.trait.CurrentData.description + "\n\n";
            if (this.ConflictingTraitsWith(trait, out List<Trait> ctlist))
            {
                string conflictingTraits = string.Empty;
                foreach (Trait tempTrait in ctlist)
                {
                    conflictingTraits += tempTrait.LabelCap + " ";
                }
                desc += "PolarisTraitreleaserTraitConflictingTraits".Translate(conflictingTraits) + "\n";
                desc += "\n";
            }
            if (this.trait.GetDisabledWorkTypes().Count() > 0)
            {
                string disabledWorkTypes = string.Empty;
                foreach (WorkTypeDef tempWorkTypeDef in this.trait.GetDisabledWorkTypes())
                {
                    disabledWorkTypes += tempWorkTypeDef.LabelCap + " ";
                }
                desc += "PolarisTraitreleaserTraitDisabledWorkTypes".Translate(disabledWorkTypes) + "\n";
                desc += "\n";
            }
            if (this.trait.CurrentData.skillGains.Count > 0)
            {
                foreach (KeyValuePair<SkillDef, int> skillGain in this.trait.CurrentData.skillGains)
                {
                    if (skillGain.Value > 0)
                    {
                        desc += skillGain.Key.LabelCap + "  +" + skillGain.Value.ToString() + "\n";
                    }
                    else desc += skillGain.Key.LabelCap + "  " + skillGain.Value.ToString() + "\n";
                }
                desc += "\n";
            }
            if (!this.trait.CurrentData.statOffsets.NullOrEmpty())
            {
                foreach (StatModifier statOffset in this.trait.CurrentData.statOffsets)
                {
                    desc += statOffset.stat.LabelCap + statOffset.ValueToStringAsOffset + "\n";
                }
                desc += "\n";
            }
            if (!this.trait.CurrentData.statFactors.NullOrEmpty())
            {
                foreach (StatModifier statFactor in this.trait.CurrentData.statFactors)
                {
                    desc += statFactor.stat.LabelCap + statFactor.ToStringAsFactor + "\n";
                }
                desc += "\n";
            }
            return desc;*/
        }

        private bool ConflictingTraitsWith(Trait trait, out List<Trait> ctlist)
        {
            ctlist = new List<Trait>();
            if (trait.def.conflictingTraits.Count > 0 || trait.def.degreeDatas.Count > 1)
            {
                if (trait.def.conflictingTraits.Count > 0)
                {
                    foreach (TraitDef tdef in trait.def.conflictingTraits)
                    {
                        foreach (TraitDegreeData ddata in tdef.degreeDatas)
                        {
                            ctlist.Add(new Trait(tdef, ddata.degree, false));
                        }
                    }
                }
                if (trait.def.degreeDatas.Count > 1)
                {
                    foreach (TraitDegreeData sdata in trait.def.degreeDatas)
                    {
                        if (sdata.degree != trait.Degree)
                        {
                            ctlist.Add(new Trait(trait.def, sdata.degree, false));
                        }
                    }
                }
                return true;
            }
            return false;
        }

        private float GetTraitSpecificCommonality(TraitDef traitDef)
        {
            float commonality = (float)typeof(TraitDef).GetField("commonality", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(traitDef);
            float commonalityFemale = (float)typeof(TraitDef).GetField("commonalityFemale", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(traitDef);
            if (commonalityFemale > 0) commonality = (commonalityFemale + commonality) / 2;
            return commonality;
        }

        private int GetRandomTraitDegree(TraitDef tdef)
        {
            if (tdef.degreeDatas.Count == 1)
            {
                return tdef.degreeDatas[0].degree;
            }
            return tdef.degreeDatas.RandomElement().degree;
        }

        private int GetExactTraitDegree(TraitDef tdef, int tempDegree, out int exactDegree)
        {
            for (int i = 0; i < tdef.degreeDatas.Count; i++)
            {
                if (tdef.degreeDatas[i].degree == tempDegree)
                {
                    return exactDegree = tempDegree;
                }
            }
            return exactDegree = tdef.degreeDatas[0].degree;
        }
    }
}
