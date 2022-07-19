using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompUseEffect_ReleaseTrait : CompUseEffect
    {
        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            bool result = false;
            if (p.story != null)
            {
                if (p.def == ThingDefOf.Human || p.story.traits != null)
                {
                    result = true;
                }
            }
            failReason = "PolarisTraitreleaserUsedbyHasNullTraits".Translate();
            return result;
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            Trait trait = this.parent.GetComp<CompTraitreleaser>().trait;
            List<Trait> traits = usedBy.story.traits.allTraits;
            string text = "PolarisTraitreleaserEffectDESC".Translate();
            DiaNode diaNode = new DiaNode(text);
            DiaOption diaOptionAddTrait = new DiaOption("PolarisTraitreleaserAddTraitOption".Translate(trait.LabelCap))
            {
                action = delegate
                {
                    this.AddTrait(usedBy, trait, traits);
                    base.parent.Destroy();
                },
                resolveTree = true
            };
            if (this.HasSameTrait(traits, trait))
            {
                diaOptionAddTrait.disabled = true;
                diaOptionAddTrait.disabledReason = "PolarisTraitreleaserAlreadyHadTrait".Translate();
            }
            List<Trait> conflictPawnTraits = this.ConflictTraits(traits, trait);
            if (!conflictPawnTraits.NullOrEmpty())
            {
                
                string conflictTraitsString = string.Empty;
                foreach (Trait item in conflictPawnTraits)
                {
                    conflictTraitsString += (" " + item.Label);
                }
                diaOptionAddTrait.SetText("PolarisTraitreleaserAddTraitOption".Translate(trait.LabelCap) + "PolarisTraitreleaserHasConflictTraits".Translate(conflictTraitsString));
                //diaOptionAddTrait.disabledReason = "PolarisTraitreleaserHasConflictTraits".Translate(conflictTraitsString);
            }
            diaNode.options.Add(diaOptionAddTrait);

            DiaOption diaOptionRemoveTrait = new DiaOption("PolarisTraitreleaserRemoveTraitOption".Translate(this.parent.GetComp<CompTraitreleaser>().availableTimes))
            {
                action = delegate
                {
                    List<DebugMenuOption> list = new List<DebugMenuOption>();
                    foreach (DebugMenuOption option in this.GenRemoveTraitMenuOptions(usedBy))
                    {
                        list.Add(option);
                    }
                    Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));

                },
                resolveTree = true
            };
            if (this.parent.GetComp<CompTraitreleaser>().availableTimes <= 0)
            {
                diaOptionRemoveTrait.disabled = true;
            }
            diaNode.options.Add(diaOptionRemoveTrait);

            if (Prefs.DevMode)
            {
                DiaOption diaOptionDev = new DiaOption("*(Dev mode)reset availabel times")
                {
                    action = delegate
                    {
                        this.parent.GetComp<CompTraitreleaser>().availableTimes = 3;
                    },
                    resolveTree = true
                };
                diaNode.options.Add(diaOptionDev);
            }

            DiaOption diaOptionGoBack = new DiaOption("GoBack".Translate())
            {
                resolveTree = true
            };
            diaNode.options.Add(diaOptionGoBack);
            Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, true, base.parent.LabelCap));
        }

        private void AddTrait (Pawn usedBy, Trait targetTrait, List<Trait> usersTraits)
        {
            List<Trait> ctraits = this.ConflictTraits(usersTraits, targetTrait);
            if (!ctraits.NullOrEmpty())
            {
                string textConflitTraits = string.Empty;
                /*for (int i = 0; i < usersTraits.Count; i++)
                {
                    if (ctraits.Contains(usersTraits[i]))
                    {
                        textConflitTraits += usersTraits[i].LabelCap + " ";
                        usersTraits.Remove(usersTraits[i]);
                        i--;
                    }
                }*/
                for (int i = usersTraits.Count -1; i >= 0 ; i--)
                {
                    if (ctraits.Contains(usersTraits[i]))
                    {
                        textConflitTraits += usersTraits[i].LabelCap + " ";
                        usersTraits.Remove(usersTraits[i]);
                    }
                }
                usedBy.story.traits.GainTrait(targetTrait);
                Messages.Message("PolarisTraitreleaserUsedReplaceConflitTraits".Translate(usedBy.LabelShort, targetTrait.LabelCap, textConflitTraits), usedBy, MessageTypeDefOf.PositiveEvent);
            }
            else
            {
                usedBy.story.traits.GainTrait(targetTrait);
                Messages.Message("PolarisTraitreleaserUsedAddTrait".Translate(usedBy.LabelShort, targetTrait.LabelCap), usedBy, MessageTypeDefOf.PositiveEvent);
            }
            PolarisUtility.GainSkillsExtra(usedBy, targetTrait.CurrentData.skillGains, true);
        }

        private List<Trait> ConflictTraits(List<Trait> usersTraits, Trait targetTrait)
        {
            List<Trait> clist = new List<Trait>();
            foreach (Trait temp in usersTraits)
            {
                if((temp.def.defName == targetTrait.def.defName && temp.Degree != targetTrait.Degree) || temp.def.ConflictsWith(targetTrait))
                {
                    clist.Add(temp);
                }
            }
            return clist;
        }

        private bool HasSameTrait(List<Trait> usersTraits, Trait targetTrait)
        {
            foreach(Trait temp in usersTraits)
            {
                if (temp.def.defName == targetTrait.def.defName && temp.Degree == targetTrait.Degree)
                {
                    return true;
                } 
            }
            return false;
        }

        private IEnumerable<DebugMenuOption> GenRemoveTraitMenuOptions(Pawn usedBy)
        {
            foreach (Trait trait in usedBy.story.traits.allTraits)
            {
                DebugMenuOption option = new DebugMenuOption(string.Concat(new object[]
                    {
                            trait.LabelCap,
                            " (",
                            trait.Degree,
                            ")"
                    }), DebugMenuOptionMode.Action, delegate ()
                    {
                        Trait reTrait = new Trait(trait.def, trait.Degree);
                        PolarisUtility.GainSkillsExtra(usedBy, trait.CurrentData.skillGains, false);
                        //usedBy.story.traits.allTraits.Remove(trait);
                        usedBy.story.traits.RemoveTrait(trait);
                        //PolarisUtility.RefreshPawnStat(usedBy);
                        this.parent.GetComp<CompTraitreleaser>().trait = reTrait;
                        this.parent.GetComp<CompTraitreleaser>().availableTimes--;
                        Messages.Message("PolarisTraitreleaserUsedRemovedTrait".Translate(usedBy.LabelShort, trait.LabelCap), usedBy, MessageTypeDefOf.PositiveEvent);
                    });
                yield return option;
            }

            yield break;
        }
    }
}

