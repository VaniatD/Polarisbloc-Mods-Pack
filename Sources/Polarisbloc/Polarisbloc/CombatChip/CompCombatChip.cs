using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompCombatChip : CompUsable
    {
        public HediffDef hediffDef;

        protected override string FloatMenuOptionLabel
        {
            get
            {
                return string.Format(base.Props.useLabel, this.hediffDef.LabelCap);
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look<HediffDef>(ref this.hediffDef, "hediffDef");
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if ((from x in DefDatabase<HediffDef>.AllDefs
                where x.hediffClass == typeof(Polarisbloc.Hediff_CombatChip)
                select x).TryRandomElement(out HediffDef tempHediffDef))
            {
                this.hediffDef = tempHediffDef;
            }
        }

        public override string TransformLabel(string label)
        {
            return this.hediffDef.LabelCap + " " + label;
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other))
            {
                return false;
            }
            CompCombatChip compCombatChip = other.TryGetComp<CompCombatChip>();
            if (compCombatChip != null && compCombatChip.hediffDef == this.hediffDef)
            {
                return true;
            }
            return false;
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompCombatChip compCombatChip = piece.TryGetComp<CompCombatChip>();
            if (compCombatChip != null)
            {
                compCombatChip.hediffDef = this.hediffDef;
            }
        }
    }
}
