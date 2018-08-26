using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc
{
    public class HediffComp_Antibiotic : HediffComp
    {
        public HediffCompProperties_Antibiotic Props
        {
            get
            {
                return (HediffCompProperties_Antibiotic)base.props;
            }
        }

        private int ticks;

        private List<HediffDef> ExtraHediffs
        {
            get
            {
                return this.Props.extraHediffs;
            }
        }

        private float MinEffect
        {
            get
            {
                return this.Props.minEffect;
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.ticks, "ticks");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.ticks++;
            if (this.ticks >= GenDate.TicksPerHour)
            {
                this.AntibioticEffect(base.Pawn, this.ExtraHediffs, this.MinEffect);
                this.ticks = 0;
            }
        }

        private void AntibioticEffect(Pawn pawn, List<HediffDef> extraHediffs, float minEffect = 0.025f)
        {
            List<Hediff> hediffs = (from x in pawn.health.hediffSet.hediffs
                                   where x.def.makesSickThought || extraHediffs.Contains(x.def)
                                   select x).ToList();
            for (int i = hediffs.Count - 1; i >=0; i--)
            {
                if (hediffs[i].TendableNow())
                {
                    hediffs[i].Tended(0.8f);
                }
                float num = 0f;
                HediffComp_Immunizable hediffComp_Immunizable = hediffs[i].TryGetComp<HediffComp_Immunizable>();
                if (hediffComp_Immunizable != null)
                {
                    num += Mathf.Abs(hediffComp_Immunizable.Props.severityPerDayNotImmune / GenDate.TicksPerDay * 1.2f * GenDate.TicksPerHour);
                }
                HediffComp_GrowthMode hediffComp_GrowthMode = hediffs[i].TryGetComp<HediffComp_GrowthMode>();
                if (hediffComp_GrowthMode != null)
                {
                    num += Mathf.Abs(hediffComp_GrowthMode.Props.severityPerDayGrowing / GenDate.TicksPerDay * 1.5f * GenDate.TicksPerHour);
                }
                HediffComp_SeverityPerDay hediffComp_SeverityPerDay = hediffs[i].TryGetComp<HediffComp_SeverityPerDay>();
                if (hediffComp_SeverityPerDay != null)
                {
                    //num += Mathf.Abs(((HediffCompProperties_SeverityPerDay)hediffComp_SeverityPerDay.props).severityPerDay / GenDate.TicksPerDay * 2f * GenDate.TicksPerHour);
                    if (hediffComp_SeverityPerDay.props is HediffCompProperties_SeverityPerDay prop)
                    {
                        num += Mathf.Abs(prop.severityPerDay / GenDate.TicksPerDay * 2f * GenDate.TicksPerHour);
                    }
                }
                if (num < minEffect)
                {
                    num = minEffect;
                }
                if (hediffs[i].Severity < num)
                {
                    pawn.health.RemoveHediff(hediffs[i]);
                    continue;
                }
                hediffs[i].Severity -= num;
                if (!hediffs[i].Visible)
                {
                    pawn.health.RemoveHediff(hediffs[i]);
                }
            }


        }
    }


    public class HediffCompProperties_Antibiotic : HediffCompProperties
    {
        public List<HediffDef> extraHediffs = new List<HediffDef>();

        public float minEffect = 0.025f;

        public HediffCompProperties_Antibiotic()
        {
            this.compClass = typeof(HediffComp_Antibiotic);
        }
    }
}
