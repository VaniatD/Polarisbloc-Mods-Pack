using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using Verse.AI;
using UnityEngine;

namespace Polarisbloc
{
    public class CompIdeoCode : CompUsable
    {
        public new CompProperties_IdeoCode Props
        {
            get
            {
                return (CompProperties_IdeoCode)base.props;
            }
        }

        public CompColorable CompColorable
        {
            get
            {
                return this.parent.GetComp<CompColorable>();
            }
        }

        public Ideo targetIdeo;

        public bool injectMode = false;

        public override void PostPostMake()
        {
            base.PostPostMake();
            this.ColorCached();
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.ColorCached();
        }

        public void ColorCached()
        {
            if (this.CompColorable != null)
            {
                if (this.targetIdeo != null)
                {
                    this.parent.SetColor(this.targetIdeo.Color);
                }
                else
                {
                    this.parent.SetColor(Color.white);
                }
            }
            //Log.Message(this.targetIdeo.Color.ToString());
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
            {
                return false;
            }
            CompIdeoCode compIdeoCode = other.TryGetComp<CompIdeoCode>();
            if (compIdeoCode != null && compIdeoCode.targetIdeo == this.targetIdeo)
            {
                return true;
            }
            return false;
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompIdeoCode compIdeoCode = piece.TryGetComp<CompIdeoCode>();
            if (compIdeoCode != null)
            {
                compIdeoCode.targetIdeo = this.targetIdeo;
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_References.Look(ref targetIdeo, "targetIdeo");
            Scribe_Values.Look<bool>(ref this.injectMode, "injectMode", false, false);
        }

        public override string TransformLabel(string label)
        {
            return ((this.targetIdeo != null)? this.targetIdeo.name : this.Props.nullIdeoLabel) + " " + base.TransformLabel(label);
        }

        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return string.Format(this.Props.useLabel, this.parent.LabelCap);
        }


        public override void PostPostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
        {
            base.PostPostGeneratedForTrader(trader, forTile, forFaction);
            if (forFaction != null)
            {
                forFaction.ideos.AllIdeos.TryRandomElement<Ideo>(out this.targetIdeo);
            }
            else
            {
                Find.IdeoManager.IdeosListForReading.TryRandomElement<Ideo>(out this.targetIdeo);
            }
            this.ColorCached();
            /*if (this.parent.stackCount > 1)
            {
                ThingOwner owner = this.parent.holdingOwner;
                owner.Remove(this.parent);
                for (int i = this.parent.stackCount - 1; i >= 0; i--)
                {
                    Thing item = this.parent.SplitOff(1);
                    CompIdeoCode comeIdeoCode = item.TryGetComp<CompIdeoCode>();
                    if (comeIdeoCode != null)
                    {
                        if (forFaction != null)
                        {
                            forFaction.ideos.AllIdeos.TryRandomElement<Ideo>(out comeIdeoCode.targetIdeo);
                        }
                        else
                        {
                            Find.IdeoManager.IdeosListForReading.TryRandomElement<Ideo>(out comeIdeoCode.targetIdeo);
                        }
                    }
                    owner.TryAdd(item);
                }
            }
            else
            {
                if (forFaction != null)
                {
                    forFaction.ideos.AllIdeos.TryRandomElement<Ideo>(out this.targetIdeo);
                }
                else
                {
                    Find.IdeoManager.IdeosListForReading.TryRandomElement<Ideo>(out this.targetIdeo);
                }
            }*/

        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Prefs.DevMode)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "Select ideo...",
                    action = delegate
                    {
                        List<Ideo> ideosListForReading = Find.IdeoManager.IdeosListForReading;
                        List<FloatMenuOption> list = new List<FloatMenuOption>();
                        FloatMenuOption resetIdeo = new FloatMenuOption("Reset ideo", delegate ()
                        {
                            this.targetIdeo = null;
                            this.ColorCached();
                        }, MenuOptionPriority.Default, null, null, 0f, null, null);
                        list.Add(resetIdeo);
                        foreach (Ideo ideo in ideosListForReading)
                        {
                            FloatMenuOption item = new FloatMenuOption(ideo.name, delegate ()
                            {
                                this.targetIdeo = ideo;
                                this.ColorCached();
                            }, MenuOptionPriority.Default, null, null, 0f, null, null);
                            list.Add(item);
                        }
                        Find.WindowStack.Add(new FloatMenu(list));
                    }
                };
            }
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            foreach (FloatMenuOption option in base.CompFloatMenuOptions(myPawn))
            {
                yield return option;
            }
            if (Find.IdeoManager.classicMode)
            {
                yield break;
            }
            yield return new FloatMenuOption(String.Format(this.Props.injectIdeoOptionText, myPawn.Ideo.name?? ""), delegate
            {
                if (myPawn.CanReserveAndReach(parent, PathEndMode.Touch, Danger.Deadly))
                {
                    foreach (CompUseEffect comp in parent.GetComps<CompUseEffect>())
                    {
                        if (comp.SelectedUseOption(myPawn))
                        {
                            return;
                        }
                    }
                    this.injectMode = true;
                    TryStartUseJob(myPawn, LocalTargetInfo.Invalid);
                }
            });

        }
    }

    public class CompProperties_IdeoCode : CompProperties_Usable
    {
        public CompProperties_IdeoCode()
        {
            this.compClass = typeof(CompIdeoCode);
        }

        [MustTranslate]
        public string nullIdeoLabel;

        [MustTranslate]
        public string injectIdeoOptionText;

        [MustTranslate]
        public string injectCompletedMessage;

        public float certaintyOffset = 0.5f;
    }
}
