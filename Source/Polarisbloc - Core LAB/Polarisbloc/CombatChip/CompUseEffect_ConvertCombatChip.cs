using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompUseEffect_ConvertCombatChip : CompUseEffect
    {
        private List<HediffDef> hediffDefs = (from x in DefDatabase<HediffDef>.AllDefs
                                              where x.hediffClass == typeof(Polarisbloc.Hediff_CombatChip)
                                              select x).ToList<HediffDef>();

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            if (p.health.hediffSet.hediffs.Find(x => x.def.hediffClass == typeof(Polarisbloc.Hediff_CombatChip)) != null || p.health.hediffSet.hediffs.Find(x => x.def == PolarisblocDefOf.PolarisCombatChip_NotActive) != null)
            {
                return base.CanBeUsedBy(p, out failReason);
            }
            failReason = "PolarisConvertCombatHasNoChip".Translate();
            return false;
        }


        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            string text = "PolarisConvertCombatChipDESC".Translate();
            DiaNode diaNode = new DiaNode(text);
            Hediff reHediff = usedBy.health.hediffSet.hediffs.Find(x => x.def.hediffClass == typeof(Polarisbloc.Hediff_CombatChip));
            Hediff bHediff = usedBy.health.hediffSet.hediffs.Find(x => x.def == PolarisblocDefOf.PolarisCombatChip_NotActive);
            if (reHediff == null && bHediff == null)
            {
                Messages.Message("PolarisMessageFailedConvertCombatChip".Translate(usedBy.LabelShort), usedBy, MessageTypeDefOf.NegativeEvent);
                return;
            }
            if (bHediff != null)
            {
                foreach (DiaOption diaOption in this.GetDiaOptions(usedBy, bHediff))
                {
                    diaNode.options.Add(diaOption);
                }
            }
            else if (reHediff != null)
            {
                foreach (DiaOption diaOption in this.GetDiaOptions(usedBy, reHediff))
                {
                    diaNode.options.Add(diaOption);
                }
            }
            DiaOption diaOptionBack = new DiaOption("GoBack".Translate())
            {
                resolveTree = true
            };
            diaNode.options.Add(diaOptionBack);
            Find.WindowStack.Add(new Dialog_NodeTree(diaNode, true, true, "PolarisConvertCombatChipTitle".Translate()));
        }


        private IEnumerable<DiaOption> GetDiaOptions(Pawn pawn, Hediff reHediff)
        {
            foreach (HediffDef hediffDef in this.hediffDefs)
            {
                if (hediffDef != reHediff.def)
                {
                    DiaOption diaOption = new DiaOption(hediffDef.LabelCap)
                    {
                        action = delegate
                        {
                            Hediff appHediff = HediffMaker.MakeHediff(hediffDef, pawn, pawn.health.hediffSet.GetBrain());
                            pawn.health.RemoveHediff(reHediff);
                            pawn.health.AddHediff(appHediff);
                        },
                        resolveTree = true
                    };
                    yield return diaOption;
                }
            }
            yield break;
        }
    }
}
