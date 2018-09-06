using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc
{
    public class HediffComp_ReceptorAntagonist : HediffComp
    {
        public HediffCompProperties_ReceptorAntagonist Props
        {
            get
            {
                return (HediffCompProperties_ReceptorAntagonist)base.props;
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

        private List<HediffDef> ExceptHediffs
        {
            get
            {
                return this.Props.exceptHediffs;
            }
        }

        private float MinEffect
        {
            get
            {
                return this.Props.minEffect;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            ReceptorAntagonistDatabase.BuildDrugHediffsDatabaseIfNecessary();
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.ticks, "ticks");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ReceptorAntagonistDatabase.BuildDrugHediffsDatabaseIfNecessary();
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.ticks++;
            if (this.ticks >= GenDate.TicksPerHour)
            {
                this.ticks = 0;
                this.AntagonistEffect(base.Pawn, this.ExtraHediffs, this.ExceptHediffs, this.MinEffect);
            }
        }

        private void AntagonistEffect(Pawn pawn, List<HediffDef> extraHediffs, List<HediffDef> exceptHediffs, float minEffect = 0.025f)
        {
            List<Hediff> hediffs = (from x in pawn.health.hediffSet.hediffs
                                    where !exceptHediffs.Contains(x.def) && (ReceptorAntagonistDatabase.addictionHediffs.Contains(x.def) || ReceptorAntagonistDatabase.toleranceHediffs.Contains(x.def) || extraHediffs.Contains(x.def))
                                    select x).ToList();
            for (int i = hediffs.Count - 1; i >= 0; i--)
            {
                float num = 0f;
                HediffComp_SeverityPerDay hediffComp_SeverityPerDay = hediffs[i].TryGetComp<HediffComp_SeverityPerDay>();
                if (hediffComp_SeverityPerDay != null)
                {
                    if (hediffComp_SeverityPerDay.props is HediffCompProperties_SeverityPerDay prop)
                    {
                        num += Mathf.Abs(prop.severityPerDay / GenDate.TicksPerDay * 4f * GenDate.TicksPerHour);
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
            foreach (Need n in pawn.needs.AllNeeds)
            {
                if (n is Need_Chemical)
                {
                    n.CurLevel = n.MaxLevel;
                }
            }
        }
    }

    public class HediffCompProperties_ReceptorAntagonist : HediffCompProperties
    {
        public HediffCompProperties_ReceptorAntagonist()
        {
            this.compClass = typeof(HediffComp_ReceptorAntagonist);
        }

        public List<HediffDef> extraHediffs = new List<HediffDef>();

        public List<HediffDef> exceptHediffs = new List<HediffDef>();

        public float minEffect = 0.025f;
    }
}
