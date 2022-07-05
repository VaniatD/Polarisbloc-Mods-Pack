using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompUseEffect_UseIdeoCode : CompUseEffect
    {
        private CompIdeoCode IdeoCode
        {
            get
            {
                return this.parent.GetComp<CompIdeoCode>();
            }
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            CompUseEffect_UseIdeoCode oneEffect = this.parent.SplitOff(1).TryGetComp<CompUseEffect_UseIdeoCode>();
            if (oneEffect.IdeoCode.injectMode || oneEffect.IdeoCode.targetIdeo == null)
            {
                oneEffect.IdeoCode.targetIdeo = usedBy.Ideo;
                oneEffect.IdeoCode.ColorCached();
                Messages.Message(string.Format(oneEffect.IdeoCode.Props.injectCompletedMessage, usedBy.Ideo.name?? ""), oneEffect.parent, MessageTypeDefOf.TaskCompletion, false);
                oneEffect.IdeoCode.injectMode = false;
                GenPlace.TryPlaceThing(oneEffect.parent, usedBy.Position, usedBy.Map, ThingPlaceMode.Near);
            }
            else
            {
                if (usedBy.Ideo == oneEffect.IdeoCode.targetIdeo)
                {
                    usedBy.ideo.OffsetCertainty(oneEffect.IdeoCode.Props.certaintyOffset);
                }
                else
                {
                    usedBy.ideo.IdeoConversionAttempt(oneEffect.IdeoCode.Props.certaintyOffset, oneEffect.IdeoCode.targetIdeo);
                }
                oneEffect.parent.SplitOff(1).Destroy();
            }
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            failReason = "PlayClassic".Translate();
            return !Find.IdeoManager.classicMode;
        }
    }
}
