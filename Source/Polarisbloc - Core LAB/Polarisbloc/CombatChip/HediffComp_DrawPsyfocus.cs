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

        public override void Notify_EntropyGained(float baseAmount, float finalAmount, Thing source = null)
        {
            base.Notify_EntropyGained(baseAmount, finalAmount, source);
            float amount = Mathf.Max(0f, baseAmount - finalAmount);
            float psyfocusOffset = amount * 0.01f * this.Props.drawFactor;
            if (this.Props.psyfocusGainMulti)
            {
                psyfocusOffset *= this.Pawn.GetStatValue(StatDefOf.MeditationFocusGain);
            }
            this.Pawn.psychicEntropy.OffsetPsyfocusDirectly(psyfocusOffset);
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
    }
}
