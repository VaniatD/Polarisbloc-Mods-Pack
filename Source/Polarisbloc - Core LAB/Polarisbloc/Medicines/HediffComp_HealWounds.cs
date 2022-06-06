using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class HediffComp_HealWounds : HediffComp
    {
        public HediffCompProperties_HealWounds Props
        {
            get
            {
                return (HediffCompProperties_HealWounds)base.props;
            }
        }

        private HediffCompProperties_HealWounds exactProps = new HediffCompProperties_HealWounds();

        private int Healticks
        {
            get
            {
                return this.exactProps.healTicks;
            }
        }

        private float HealLimitOnce
        {
            get
            {
                return this.exactProps.healLimitOnce;
            }
        }

        private int healTick;

        private bool CanHealNow
        {
            get
            {
                if (base.Pawn.health.hediffSet.hediffs.Find((Hediff x) => x is Hediff_Injury y && (y.CanHealNaturally() || y.CanHealFromTending())) != null)
                    return true;
                else return false;
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.exactProps = this.Props;
        }

        public override string CompLabelInBracketsExtra
        {
            get
            {
                return (base.parent.Severity * 100).ToString("0.0");
            }
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<int>(ref this.healTick, "healTick");
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                this.exactProps = this.Props;
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.healTick++;
            if (this.healTick >= this.Healticks)
            {
                this.healTick = 0;
                if (this.CanHealNow)
                {
                    this.HealRandomInjury(base.Pawn, this.HealLimitOnce);
                    base.parent.Severity -= this.HealLimitOnce * 0.01f;
                }
            }
        }

        private void HealRandomInjury(Pawn pawn, float points)
        {
            if ((from x in pawn.health.hediffSet.GetHediffs<Hediff_Injury>()
                 where x.CanHealNaturally() || x.CanHealFromTending()
                 select x).TryRandomElement(out Hediff_Injury hediff_Injury))
            {
                hediff_Injury.Heal(points);
            }
        }

    }

    public class HediffCompProperties_HealWounds : HediffCompProperties
    {
        public int healTicks = 60;

        public float healLimitOnce = 1f;

        public HediffCompProperties_HealWounds()
        {
            this.compClass = typeof(HediffComp_HealWounds);
        }
    }
}
