using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc
{
    public class HediffComp_NeedsFiller : HediffComp
    {
        public HediffCompProperties_NeedsFiller Props
        {
            get
            {
                return (HediffCompProperties_NeedsFiller)base.props;
            }
        }

        private HediffCompProperties_NeedsFiller exactProps = new HediffCompProperties_NeedsFiller();

        //private List<NeedDef> fixNeeds;

        private int tick;

        private FillMode WorkMode
        {
            get
            {
                return this.exactProps.workMode;
            }
        }

        private int CheckPerTicks
        {
            get
            {
                return this.Props.checkPerTicks;
            }
        }

        private List<NeedDef> ExtraNeeds
        {
            get
            {
                return this.Props.extraNeeds;
                //return this.fixNeeds;
            }
        }

        private float LowerLimit
        {
            get
            {
                return Mathf.Clamp01(this.exactProps.limitFactor.min);
            }
        }

        private float UpperLimit
        {
            get
            {
                return Mathf.Clamp01(this.exactProps.limitFactor.max);
            }
        }

        public override void CompPostMake()
        {
            base.CompPostMake();
            this.exactProps = this.Props;
            //this.fixNeeds = this.Props.extraNeeds;
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<HediffCompProperties_NeedsFiller>(ref this.exactProps, "exactProps");
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.tick++;
            if(this.tick >= this.CheckPerTicks)
            {
                if(this.WorkMode == FillMode.food)
                {
                    this.FillUpNeed(base.Pawn.needs.food);
                }
                if(this.WorkMode == FillMode.joy)
                {
                    this.FillUpNeed(base.Pawn.needs.joy);
                }
                if(this.WorkMode == FillMode.rest)
                {
                    this.FillUpNeed(base.Pawn.needs.rest);
                }
                if(this.WorkMode == FillMode.allNeeds)
                {
                    this.FillAllNeeds(base.Pawn.needs.AllNeeds);
                }
                if(!this.ExtraNeeds.NullOrEmpty())
                {
                    this.FillUpExtraNeeds(base.Pawn.needs.AllNeeds, this.ExtraNeeds);
                }
                this.tick = 0;
            }
        }

        private void FillUpNeed(Need need)
        {
            if (need != null)
            {
                if (need.CurLevelPercentage < this.LowerLimit)
                {
                    need.CurLevelPercentage = this.UpperLimit;
                }
            }
        }

        private void FillAllNeeds(List<Need> allNeeds)
        {
            if(!allNeeds.NullOrEmpty())
            {
                foreach(Need need in allNeeds)
                {
                    this.FillUpNeed(need);
                }
            }
        }

        private void FillUpExtraNeeds(List<Need> allNeeds, List<NeedDef> needList)
        {
            if (!allNeeds.NullOrEmpty() && !needList.NullOrEmpty())
            {
                foreach (Need need in from x in allNeeds
                                      where needList.Contains(x.def)
                                      select x)
                {
                    this.FillUpNeed(need);
                }
            }
        }

    }

    public class HediffCompProperties_NeedsFiller : HediffCompProperties
    {
        public FillMode workMode = FillMode.undifined;

        public int checkPerTicks = 2500;

        public List<NeedDef> extraNeeds = new List<NeedDef>();

        public FloatRange limitFactor = new FloatRange() { min = 0f, max = 1f };

        public HediffCompProperties_NeedsFiller()
        {
            this.compClass = typeof(HediffComp_NeedsFiller);
        }
    }

    public enum FillMode : byte
    {
        undifined,
        food,
        rest,
        joy,
        allNeeds,
    }
}
