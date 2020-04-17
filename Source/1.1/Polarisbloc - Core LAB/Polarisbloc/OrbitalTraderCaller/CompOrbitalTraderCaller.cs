using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompOrbitalTraderCaller : CompUsable
    {
        public TraderKindDef traderKindDef;

        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return string.Format(base.Props.useLabel, this.traderKindDef.LabelCap);
        }

        /*protected override string FloatMenuOptionLabel
        {
            get
            {
                return string.Format(base.Props.useLabel, this.traderKindDef.LabelCap);
            }
        }*/

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look<TraderKindDef>(ref this.traderKindDef, "traderKindDef");
        }

        public override void Initialize(CompProperties props)
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
        }
    }
}
