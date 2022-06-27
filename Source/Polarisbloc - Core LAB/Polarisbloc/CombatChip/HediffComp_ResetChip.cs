using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_ResetChip : HediffComp
    {
        public HediffCompProperties_ResetChip Props
        {
            get
            {
                return (HediffCompProperties_ResetChip)this.props;
            }
        }

        private int processingTicks = 600;

        private bool processing = false;

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.processingTicks = this.Props.processingTicks; ;
            this.processing = false;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.processingTicks, "ticks", 600, false);
            Scribe_Values.Look<bool>(ref this.processing, "processing", false, false);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (this.processing)
            {
                this.processingTicks--;
                if (this.processingTicks <= 0)
                {
                    this.ResetChip();
                }
            }
            
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (Find.Selector.SingleSelectedThing == this.Pawn && this.Pawn.Drafted)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "PolarisResetCombatChipLabel".Translate();
                command_Action.defaultDesc = "PolarisResetCombatChipDesc".Translate();
                command_Action.disabled = this.processing;
                command_Action.disabledReason = "PolarisResetCombatChipPrecessing".Translate(this.processingTicks.ToStringTicksToPeriod(true, true, true, true));
                command_Action.icon = TexCombatChip.CombatChipReset;
                command_Action.action = delegate
                {
                    this.ShutDown();
                };
                yield return command_Action;
            }
            //yield break;
        }

        private void ResetChip()
        {
            this.Pawn.health.AddHediff(this.Props.destinationHediff, this.Pawn.health.hediffSet.GetBrain());
            this.Pawn.health.RemoveHediff(this.parent);
        }

        private void ShutDown()
        {
            this.processingTicks = this.Props.processingTicks;
            this.processing = true;
        }
    }

    public class HediffCompProperties_ResetChip : HediffCompProperties
    {
        public HediffCompProperties_ResetChip()
        {
            this.compClass = typeof(HediffComp_ResetChip);
        }

        public HediffDef destinationHediff;

        public int processingTicks = 600;
    }
}
