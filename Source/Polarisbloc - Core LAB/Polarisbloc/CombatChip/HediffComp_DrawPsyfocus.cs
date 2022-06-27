using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace Polarisbloc
{
    public class HediffComp_DrawPsyfocus : HediffComp
    {
        public HediffCompProperties_DrawPsyfocus Props
        {
            get
            {
                return this.props as HediffCompProperties_DrawPsyfocus;
            }
        }

        /*public override void Notify_EntropyGained(float baseAmount, float finalAmount, Thing source = null)
        {
            base.Notify_EntropyGained(baseAmount, finalAmount, source);
            float amount = Mathf.Max(0f, baseAmount - finalAmount);
            float psyfocusOffset = amount * 0.01f * this.Props.drawFactor;
            if (this.Props.psyfocusGainMulti)
            {
                psyfocusOffset *= this.Pawn.GetStatValue(StatDefOf.MeditationFocusGain);
            }
            else
            {
                psyfocusOffset *= 0.5f;
            }
            this.Pawn.psychicEntropy.OffsetPsyfocusDirectly(psyfocusOffset);
        }*/

        private float Capacity
        {
            get
            {
                return this.Props.psyfocusCapacity;
            }
        }

        private float psyfocusInit;

        private int tick = 0;

        private bool isActive = false;

        public override string CompLabelInBracketsExtra
        {
            get
            {
                return this.PsyfocusPool.ToString("P1") + "/" + this.Capacity.ToString("P0");
            }
        }

        private float PsyfocusPool
        {
            get
            {
                return this.psyfocusInit;
            }
        }

        private float CalculateSavingPsyfocus(Pawn pawn)
        {
            float savingFocus = -1f;
            float num1 = this.Capacity - this.PsyfocusPool;
            float num2 = Mathf.Max(0.5f, pawn.psychicEntropy.TargetPsyfocus) + 0.02f;

            if (pawn.psychicEntropy.CurrentPsyfocus > num2)
            {
                savingFocus = pawn.psychicEntropy.CurrentPsyfocus - num2;
                if (savingFocus < 0.05f && pawn.psychicEntropy.CurrentPsyfocus > 0.25f)
                {
                    savingFocus = 0.05f;
                }
                if (savingFocus > num1)
                {
                    savingFocus = num1;
                }
                
            }
            return savingFocus;
        }

        private float CalculateOutPsyfocus(Pawn pawn)
        {
            float outFocus = 1f - pawn.psychicEntropy.CurrentPsyfocus;
            if (outFocus > this.PsyfocusPool)
            {
                outFocus = this.PsyfocusPool;
            }
            return outFocus;
        }

        private bool TrySavePsyfocus(Pawn pawn)
        {
            if (pawn.psychicEntropy == null)
            {
                return false;
            }
            float savingFocus = this.CalculateSavingPsyfocus(pawn);
            if (savingFocus > 0f)
            {
                pawn.psychicEntropy.OffsetPsyfocusDirectly(-savingFocus);
                this.OffsetPsyfocusPool(savingFocus);
                return true;
            }
            return false;
        }

        private void TryOutPsyfocus(Pawn pawn)
        {
            float outFocus = this.CalculateOutPsyfocus(pawn);
            if (outFocus > 0f)
            {
                pawn.psychicEntropy.OffsetPsyfocusDirectly(outFocus);
                this.OffsetPsyfocusPool(-outFocus);
            }


        }

        private void OffsetPsyfocusPool(float offset)
        {
            this.psyfocusInit = Mathf.Clamp(this.psyfocusInit + offset, 0, this.Capacity);
        }

        public override void CompExposeData()
        {
            base.CompExposeData();
            Scribe_Values.Look<float>(ref this.psyfocusInit, "psyfocusInit", 0f, false);
            Scribe_Values.Look<int>(ref this.tick, "tick", 0, false);
            Scribe_Values.Look<bool>(ref this.isActive, "isActive", false, false);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            this.tick--;
            this.OffsetPsyfocusPool(this.Props.collectPsyfocusPerTick);
            if (!this.isActive && this.tick <= 0)
            {
                if (this.TrySavePsyfocus(this.Pawn))
                {
                    this.tick = GenDate.TicksPerHour;
                }
                else
                {
                    this.tick = GenDate.TicksPerHour * 3;
                }
            }
        }


        public override void Notify_EntropyGained(float baseAmount, float finalAmount, Thing source = null)
        {
            base.Notify_EntropyGained(baseAmount, finalAmount, source);
            float amount = Mathf.Max(0f, baseAmount - finalAmount);
            float psyfocusOffset = amount * 0.01f * this.Props.drawFactor;
            if (this.Props.psyfocusGainMulti)
            {
                psyfocusOffset *= this.Pawn.GetStatValue(StatDefOf.MeditationFocusGain);
            }
            else
            {
                psyfocusOffset *= 0.5f;
            }
            //this.Pawn.psychicEntropy.OffsetPsyfocusDirectly(psyfocusOffset);
            this.OffsetPsyfocusPool(psyfocusOffset);
            if (this.isActive)
            {
                this.TryOutPsyfocus(this.Pawn);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmos()
        {
            if (Find.Selector.SingleSelectedThing == this.Pawn && this.Pawn.Drafted)
            {
                yield return new Command_Toggle
                {
                    defaultDesc = "PolarisPsyChipDesc".Translate(),
                    defaultDescPostfix = this.isActive? "PolarisPsyChipActived".Translate() : "PolarisPsyChipNotActived".Translate(),
                    defaultLabel = "PolarisPsyChipLabel".Translate(),
                    //icon = this.parent.def.spawnThingOnRemoved.uiIcon,
                    icon = this.isActive? TexCombatChip.CombatChipDrawPsyfocusOn : TexCombatChip.CombatChipDrawPsyfocusOff,
                    isActive = () => this.isActive,
                    toggleAction = delegate
                    {
                        this.isActive = !this.isActive;
                    }
                };
            }
            //yield break;
            //return base.CompGetGizmos();
        }



    }

    public class HediffCompProperties_DrawPsyfocus : HediffCompProperties
    {
        public HediffCompProperties_DrawPsyfocus()
        {
            this.compClass = typeof(HediffComp_DrawPsyfocus);
        }

        public float drawFactor = 0.05f;

        public bool psyfocusGainMulti = false;

        public float psyfocusCapacity = 3f;

        public float collectPsyfocusPerTick = 0.000003f;
    }
}
