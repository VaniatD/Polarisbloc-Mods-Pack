using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompMomeryRester : CompUsable
    {
        public MemResetMode memResetMode = MemResetMode.undifined;

        public Backstory childhoodStory;

        public Backstory adulthoodStory;

        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            if (this.memResetMode == MemResetMode.childhood)
            {
                return string.Format(base.Props.useLabel, this.childhoodStory.title);
            }
            else if (this.memResetMode == MemResetMode.adulthood)
            {
                return string.Format(base.Props.useLabel, this.adulthoodStory.title);
            }
            else return "PolarisMomeryResterHaveNoAvaliableOption".Translate();
        }
        /*protected override string FloatMenuOptionLabel
        {
            get
            {
                if (this.memResetMode == MemResetMode.childhood)
                {
                    return string.Format(base.Props.useLabel, this.childhoodStory.title);
                }
                else if (this.memResetMode == MemResetMode.adulthood)
                {
                    return string.Format(base.Props.useLabel, this.adulthoodStory.title);
                }
                else return "PolarisMomeryResterHaveNoAvaliableOption".Translate();
            }
        }*/

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<MemResetMode>(ref this.memResetMode, "memResetMode");
            string text = (this.childhoodStory == null) ? null : this.childhoodStory.identifier;
            Scribe_Values.Look(ref text, "childhoodStory", null, false);
            if (Scribe.mode == LoadSaveMode.LoadingVars && !text.NullOrEmpty() && !BackstoryDatabase.TryGetWithIdentifier(text, out this.childhoodStory))
            {
                Log.Error("Couldn't load child backstory with identifier " + text + ". Giving random.");
                this.childhoodStory = BackstoryDatabase.RandomBackstory(BackstorySlot.Childhood);
            }
            string text2 = (this.adulthoodStory == null) ? null : this.adulthoodStory.identifier;
            Scribe_Values.Look(ref text2, "adulthoodStory", null, false);
            if (Scribe.mode == LoadSaveMode.LoadingVars && !text2.NullOrEmpty() && !BackstoryDatabase.TryGetWithIdentifier(text2, out this.adulthoodStory))
            {
                Log.Error("Couldn't load adult backstory with identifier " + text2 + ". Giving random.");
                this.adulthoodStory = BackstoryDatabase.RandomBackstory(BackstorySlot.Adulthood);
            }
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.childhoodStory = BackstoryDatabase.RandomBackstory(BackstorySlot.Childhood);
            this.adulthoodStory = BackstoryDatabase.RandomBackstory(BackstorySlot.Adulthood);
            if (Rand.Chance(0.5f))
            {
                memResetMode = MemResetMode.childhood;
            }
            else
            {
                memResetMode = MemResetMode.adulthood;
            }
        }

        public override string TransformLabel(string label)
        {
            string exactLabel = string.Empty;
            if(this.memResetMode == MemResetMode.childhood)
            {
                exactLabel = this.childhoodStory.title + " " + label;
            }
            else if (this.memResetMode == MemResetMode.adulthood)
            {
                exactLabel = this.adulthoodStory.title + " " + label;
            }
            else
            {
                return label;
            }
            return exactLabel;
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
            {
                return false;
            }
            CompMomeryRester compMomeryRester = other.TryGetComp<CompMomeryRester>();
            if (compMomeryRester != null && compMomeryRester.memResetMode == this.memResetMode)
            {
                return true;
            }
            return false;
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompMomeryRester compMomeryRester = piece.TryGetComp<CompMomeryRester>();
            if (compMomeryRester != null)
            {
                compMomeryRester.memResetMode = this.memResetMode;
            }
        }

        public override string GetDescriptionPart()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("Childhood".Translate() + "  " + this.GenStoryDesc(this.childhoodStory));
            stringBuilder.AppendLine("Adulthood".Translate() + "  " + this.GenStoryDesc(this.adulthoodStory));
            return stringBuilder.ToString();

            /*string desc = string.Empty;
            desc += "Childhood".Translate() + "  " + this.GenStoryDesc(this.childhoodStory);
            desc += "Adulthood".Translate() + "  " + this.GenStoryDesc(this.adulthoodStory);
            return desc;*/
        }

        private string GenStoryDesc(Backstory story)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine(story.title);
            stringBuilder.AppendLine();
            stringBuilder.AppendLine(story.baseDesc);
            stringBuilder.AppendLine();
            if (story.DisabledWorkTypes.Count() > 0)
            {
                string disabledWorkTypes = string.Empty;
                foreach (WorkTypeDef tempWorkType in story.DisabledWorkTypes)
                {
                    disabledWorkTypes += tempWorkType.LabelCap + " ";
                }
                stringBuilder.AppendLine("PolarisMomeryResterStoryDisabledWorkTypes".Translate(disabledWorkTypes));
                stringBuilder.AppendLine();
            }
            if (story.skillGainsResolved.Count > 0)
            {
                string skillGains = string.Empty;
                foreach (KeyValuePair<SkillDef, int> skillGain in story.skillGainsResolved)
                {
                    if (skillGain.Value > 0)
                    {
                        stringBuilder.AppendLine(skillGain.Key.LabelCap + "  +" + skillGain.Value.ToString());
                    }
                    else stringBuilder.AppendLine(skillGain.Key.LabelCap + "  " + skillGain.Value.ToString());
                }
                stringBuilder.AppendLine();
            }
            return stringBuilder.ToString();

            /*string desc = string.Empty;
            desc += story.Title + "\n\n";
            desc += story.baseDesc + "\n\n";
            if (story.DisabledWorkTypes.Count() > 0)
            {
                string disabledWorkTypes = string.Empty;
                foreach (WorkTypeDef tempWorkType in story.DisabledWorkTypes)
                {
                    disabledWorkTypes += tempWorkType.LabelCap + " ";
                }
                desc += "PolarisMomeryResterStoryDisabledWorkTypes".Translate(disabledWorkTypes) + "\n\n";
            }
            if (story.skillGainsResolved.Count > 0)
            {
                string skillGains = string.Empty;
                foreach (KeyValuePair<SkillDef, int> skillGain in story.skillGainsResolved)
                {
                    if (skillGain.Value > 0)
                    {
                        desc += skillGain.Key.LabelCap + "  +" + skillGain.Value.ToString() + "\n";
                    }
                    else desc += skillGain.Key.LabelCap + "  " + skillGain.Value.ToString() + "\n";
                }
                desc += "\n";
            }
            desc += "\n";
            return desc;*/
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (this.memResetMode == MemResetMode.childhood)
            {
                yield return (Gizmo)new Command_Action
                {
                    action = delegate
                    {
                        this.memResetMode = MemResetMode.adulthood;
                    },
                    defaultLabel = "PolarisMomeryResterChildhoodModeActiveLabel".Translate(),
                    defaultDesc = "PolarisMomeryResterChildhoodModeActiveDesc".Translate(),
                    icon = base.parent.def.uiIcon,
                };
            }
            if (this.memResetMode == MemResetMode.adulthood)
            {
                yield return (Gizmo)new Command_Action
                {
                    action = delegate
                    {
                        this.memResetMode = MemResetMode.childhood;
                    },
                    defaultLabel = "PolarisMomeryResterAdulthoodModeActiveLabel".Translate(),
                    defaultDesc = "PolarisMomeryResterAdulthoodModeActiveDesc".Translate(),
                    icon = base.parent.def.uiIcon,
                };
            }
            if (Prefs.DevMode)
            {
                Command_Action choseTrait = new Command_Action
                {
                    defaultLabel = "find a backstory...",
                    action = delegate
                    {
                        List<DebugMenuOption> list = new List<DebugMenuOption>();
                        foreach (Backstory story in BackstoryDatabase.allBackstories.Values)
                        {
                            if (this.memResetMode == MemResetMode.childhood && story.slot == BackstorySlot.Childhood)
                            {
                                list.Add(new DebugMenuOption(story.title, DebugMenuOptionMode.Action, delegate ()
                                {
                                    this.childhoodStory = story;
                                }));
                            }
                            if (this.memResetMode == MemResetMode.adulthood && story.slot == BackstorySlot.Adulthood)
                            {
                                list.Add(new DebugMenuOption(story.title, DebugMenuOptionMode.Action, delegate ()
                                {
                                    this.adulthoodStory = story;
                                }));
                            }
                        }
                        Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
                    }
                };
                yield return choseTrait;
            }
        }
    }
}
