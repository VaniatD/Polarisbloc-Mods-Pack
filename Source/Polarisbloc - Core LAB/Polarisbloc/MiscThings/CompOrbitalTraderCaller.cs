using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Polarisbloc
{
    public class CompOrbitalTraderCaller : CompUsable
    {
        public TraderKindDef traderKindDef;

        /*protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return string.Format(base.Props.useLabel, this.traderKindDef.LabelCap);
        }*/

        private string TryFloatMenuOptionLabel(TraderKindDef traderKindDef)
        {
            if (traderKindDef.faction != null)
            {
                return string.Format(this.Props.useLabel, traderKindDef.LabelCap + "(" + traderKindDef.faction.LabelCap) + ")";
            }
            else
            {
                return string.Format(this.Props.useLabel, traderKindDef.LabelCap);
            }
        }

        private FloatMenuOption GenFloatMenuOption(Pawn pawn, TraderKindDef traderKindDef)
        {
            return new FloatMenuOption(this.TryFloatMenuOptionLabel(traderKindDef), delegate
            {
                if (pawn.CanReserveAndReach(parent, PathEndMode.Touch, Danger.Deadly))
                {
                    foreach (CompUseEffect comp in parent.GetComps<CompUseEffect>())
                    {
                        if (comp.SelectedUseOption(pawn))
                        {
                            return;
                        }
                    }
                    this.traderKindDef = traderKindDef;
                    TryStartUseJob(pawn, LocalTargetInfo.Invalid);
                }
            });
        }




        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look<TraderKindDef>(ref this.traderKindDef, "traderKindDef");
        }

        public override IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn myPawn)
        {
            if (!CanBeUsedBy(myPawn, out var failReason))
            {
                //FloatMenuOptionLabel(myPawn) + ((failReason != null) ? (" (" + failReason + ")") : ""), null
                yield return new FloatMenuOption(failReason ?? "", null);
                yield break;
            }
            if (!myPawn.CanReach(parent, PathEndMode.Touch, Danger.Deadly))
            {
                //yield return new FloatMenuOption(FloatMenuOptionLabel(myPawn) + " (" + "NoPath".Translate() + ")", null);
                yield return new FloatMenuOption("NoPath".Translate(), null);
                yield break;
            }
            if (!myPawn.CanReserve(parent))
            {
                //yield return new FloatMenuOption(FloatMenuOptionLabel(myPawn) + " (" + "Reserved".Translate() + ")", null);
                yield return new FloatMenuOption("Reserved".Translate(), null);
                yield break;
            }
            if (!myPawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
            {
                //yield return new FloatMenuOption(FloatMenuOptionLabel(myPawn) + " (" + "Incapable".Translate() + ")", null);
                yield return new FloatMenuOption("Incapable".Translate(), null);
                yield break;
            }
            foreach (TraderKindDef traderKind in (from x in DefDatabase<TraderKindDef>.AllDefs
                                                  where x.orbital
                                                  select x))
            {
                yield return this.GenFloatMenuOption(myPawn, traderKind);
            }

            /*yield return new FloatMenuOption(FloatMenuOptionLabel(myPawn), delegate
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
                    TryStartUseJob(myPawn, LocalTargetInfo.Invalid);
                }
            });*/
        }

        private bool CanBeUsedBy(Pawn p, out string failReason)
        {
            List<ThingComp> allComps = parent.AllComps;
            for (int i = 0; i < allComps.Count; i++)
            {
                if (allComps[i] is CompUseEffect compUseEffect && !compUseEffect.CanBeUsedBy(p, out failReason))
                {
                    return false;
                }
            }
            failReason = null;
            return true;
        }

        /*public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if ((from x in DefDatabase<TraderKindDef>.AllDefs
                 where x.orbital
                 select x).TryRandomElementByWeight((TraderKindDef traderDef) => traderDef.commonality, out TraderKindDef def))
            {
                this.traderKindDef = def;
            }
        }

        public override string TransformLabel(string label)
        {
            return this.traderKindDef.LabelCap + " " + label;
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
            {
                return false;
            }
            CompOrbitalTraderCaller compOrbitalTraderCaller = other.TryGetComp<CompOrbitalTraderCaller>();
            if (compOrbitalTraderCaller != null && compOrbitalTraderCaller.traderKindDef == this.traderKindDef)
            {
                return true;
            }
            return false;
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompOrbitalTraderCaller compOrbitalTraderCaller = piece.TryGetComp<CompOrbitalTraderCaller>();
            if (compOrbitalTraderCaller != null)
            {
                compOrbitalTraderCaller.traderKindDef = this.traderKindDef;
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo g in base.CompGetGizmosExtra())
            {
                yield return g;
            }
            if (Prefs.DevMode)
            {
                Command_Action choseTrader = new Command_Action
                {
                    defaultLabel = "find a trader...",
                    action = delegate
                    {
                        List<DebugMenuOption> list = new List<DebugMenuOption>();
                        foreach (TraderKindDef traderKindDef in from x in DefDatabase<TraderKindDef>.AllDefs
                                                                where x.orbital
                                                                select x)
                        {
                            list.Add(new DebugMenuOption(traderKindDef.LabelCap, DebugMenuOptionMode.Action, delegate ()
                            {
                                this.traderKindDef = traderKindDef;
                            }));
                        }
                        Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
                    }
                };
                yield return choseTrader;
            }
        }*/
    }
}
