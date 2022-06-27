using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_ToggleHediff : HediffComp
    {
        public HediffCompProperties_ToggleHediff Props
        {
            get
            {
                return (HediffCompProperties_ToggleHediff)this.props;
            }
        }

        public bool toggle = false;

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<bool>(ref this.toggle, "toggle", false, false);
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (Find.Selector.SingleSelectedThing == this.Pawn && this.Pawn.Drafted)
            {
                Command_Toggle command_Toggle = new Command_Toggle();
                command_Toggle.defaultLabel = this.Props.gizmoLabel;
                command_Toggle.defaultDesc = this.Props.gizmoDesc;
                command_Toggle.icon = this.parent.def.spawnThingOnRemoved.uiIcon;
                //command_Toggle.hotKey = KeyBindingDefOf.Designator_Cancel;
                command_Toggle.isActive = (() => this.toggle);
                command_Toggle.toggleAction = delegate
                {
                    this.toggle = !this.toggle;
                    if (this.toggle)
                    {
                        this.CauseDestHediff(this.Pawn);
                    }
                    else
                    {
                        this.RemoveDestHediff(this.Pawn);
                    }
                };
                yield return command_Toggle;
            }
                //yield break;
        }


        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            this.RemoveDestHediff(this.Pawn);
        }

        private void RemoveDestHediff(Pawn pawn)
        {
            Hediff hediff = pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff, false);
            if (hediff != null)
            {
                pawn.health.RemoveHediff(hediff);
            }
        }

        private void CauseDestHediff(Pawn pawn)
        {
            if (pawn.health.hediffSet.GetFirstHediffOfDef(this.Props.hediff, false) == null)
            {
                
                pawn.health.AddHediff(this.Props.hediff, pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, null, null).FirstOrFallback((BodyPartRecord p) => p.def == this.Props.part, null), null, null);
            }
        }
    }

    public class HediffCompProperties_ToggleHediff : HediffCompProperties
    {
        public HediffCompProperties_ToggleHediff()
        {
            this.compClass = typeof(HediffComp_ToggleHediff);
        }

        public HediffDef hediff;

        public BodyPartDef part;

        [MustTranslate]
        public string gizmoLabel;

        [MustTranslate]
        public string gizmoDesc;
    }
}
