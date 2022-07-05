using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace Polarisbloc
{
    public class HediffComp_ForceCalmDown : HediffComp
    {
        public HediffCompProperties_ForceCalmDown Props
        {
            get
            {
                return (HediffCompProperties_ForceCalmDown)base.props;
            }
        }

        private int tick;

        private bool InMentalStateNow
        {
            get
            {
                return this.Pawn.mindState.mentalStateHandler.InMentalState;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.tick--;
            if (this.tick <= 0)
            {
                this.tick = this.Props.checkTicks;
                if (this.Pawn.mindState.mentalStateHandler.InMentalState)
                {
                    this.ForceCalmDown();
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (Find.Selector.SingleSelectedThing == this.Pawn && this.InMentalStateNow)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "PolarisForceCalmDownLabel".Translate();
                command_Action.defaultDesc = "PolarisForceCalmDownDesc".Translate();
                command_Action.icon = this.parent.def.spawnThingOnRemoved.uiIcon;
                command_Action.action = delegate
                {
                    this.ForceCalmDown();
                };
                yield return command_Action;
            }
        }

        private void ForceCalmDown()
        {
            MentalState mentalState = this.Pawn.mindState.mentalStateHandler.CurState;
            if (mentalState != null)
            {
                mentalState.RecoverFromState();
            }
            this.Pawn.mindState.mentalStateHandler.Reset();
            Messages.Message("PolarisForceCalmDown".Translate(this.Pawn.LabelShortCap, this.parent.LabelCap), this.Pawn, MessageTypeDefOf.PositiveEvent);
        }

        public override void CompExposeData()
        {
            Scribe_Values.Look<int>(ref this.tick, "tick", 0, false);
        }

        public override string CompDebugString()
        {
            return "ticksToCheck: " + this.tick;
        }
    }

    public class HediffCompProperties_ForceCalmDown : HediffCompProperties
    {
        public HediffCompProperties_ForceCalmDown()
        {
            this.compClass = typeof(HediffComp_ForceCalmDown);
        }

        public int checkTicks = 250;
    }
}
