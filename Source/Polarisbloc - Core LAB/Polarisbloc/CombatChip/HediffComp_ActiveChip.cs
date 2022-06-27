using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace Polarisbloc
{
    public class HediffComp_ActiveChip : HediffComp
    {
        public HediffCompProperties_ActiveChip Props
        {
            get
            {
                return (HediffCompProperties_ActiveChip)this.props;
            }
        }

        private int startingTicks = 600;

        private bool starting = false;

        private HediffDef targetChip;

        private int chargingTicks = 2500;

        private bool Activeable
        {
            get
            {
                return this.chargingTicks <= 0;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.chargingTicks = this.Props.chargingTicks;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.chargingTicks, "ticks", 2500, false);
            Scribe_Values.Look<int>(ref this.startingTicks, "startingTicks", 600, false);
            Scribe_Defs.Look<HediffDef>(ref this.targetChip, "targetChipDef");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (!this.Activeable)
            {
                this.chargingTicks--;
            }
            if (this.starting)
            {
                this.startingTicks--;
                if (this.startingTicks <= 0)
                {
                    this.ActiveChip();
                }
            }
        }


        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (Find.Selector.SingleSelectedThing == this.Pawn && this.Pawn.Drafted)
            {
                Command_Action command_Action = new Command_Action();
                command_Action.defaultLabel = "PolarisActiveCombatChipLabel".Translate();
                command_Action.defaultDesc = "PolarisActiveCombatChipDesc".Translate();
                command_Action.disabled = !this.Activeable || this.starting;
                command_Action.disabledReason = this.starting? "PolarisActiveCombatChipStartingUp".Translate(this.startingTicks.ToStringTicksToPeriod(true, true, true, true)) : "PolarisActiveCombatChipLimit".Translate(this.chargingTicks.ToStringTicksToPeriod(true, true, true, true));
                command_Action.icon = TexCombatChip.CombatChipActive;
                command_Action.action = delegate
                {
                    List<HediffDef> chipDefs = (from x in DefDatabase<HediffDef>.AllDefs
                                                  where x.hediffClass == typeof(Polarisbloc.Hediff_CombatChip)
                                                  select x).ToList<HediffDef>();
                    List<FloatMenuOption> list = new List<FloatMenuOption>();
                    foreach (HediffDef chipDef in chipDefs)
                    {
                        FloatMenuOption item = new FloatMenuOption(chipDef.LabelCap, delegate ()
                        {
                            this.StartUp(chipDef);
                            /*Hediff chipHediff = HediffMaker.MakeHediff(chipDef, this.Pawn, this.Pawn.health.hediffSet.GetBrain());
                            this.Pawn.health.AddHediff(chipHediff);
                            this.Pawn.health.RemoveHediff(this.parent);*/
                        }, MenuOptionPriority.Default, null, null, 0f, null, null);
                        list.Add(item);
                    }
                    Find.WindowStack.Add(new FloatMenu(list));
                };
                yield return command_Action;
            }
            //yield break;
        }

        private void StartUp(HediffDef chipDef)
        {
            this.targetChip = chipDef;
            this.startingTicks = this.Props.startingTicks;
            this.starting = true;
        }

        private void ActiveChip()
        {
            Hediff chipHediff = HediffMaker.MakeHediff(this.targetChip, this.Pawn, this.Pawn.health.hediffSet.GetBrain());
            this.Pawn.health.AddHediff(chipHediff);
            this.Pawn.health.RemoveHediff(this.parent);
        }
    }

    public class HediffCompProperties_ActiveChip : HediffCompProperties
    {
        public HediffCompProperties_ActiveChip()
        {
            this.compClass = typeof(HediffComp_ActiveChip);
        }

        public int startingTicks = 600;

        public int chargingTicks = 2500;
    }
}
