using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using RimWorld.QuestGen;
using UnityEngine;

namespace Polarisbloc_SecurityForce
{
    public class CompUseEffect_GenQuestInPoints : CompUseEffect
    {
        private CompProperties_GenQuestInPoints Props
        {
            get
            {
                return (CompProperties_GenQuestInPoints)this.props;
            }
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            Slate slate = new Slate();
            float points = Mathf.Max(this.Props.minPoints, this.parent.MarketValue + this.Props.pointsOffset.RandomInRange) ;
            switch (usedBy.skills?.GetSkill(SkillDefOf.Intellectual).Level)
            {
                case 15:
                    points *= 1.1f;
                    break;
                case 16:
                    points *= 1.2f;
                    break;
                case 17:
                    points *= 1.3f;
                    break;
                case 18:
                    points *= 1.5f;
                    break;
                case 19:
                    points *= 1.75f;
                    break;
                case 20:
                    points *= 2f;
                    break;
                default:
                    break;
            }
            slate.Set("points", points);
            slate.Set("map", usedBy.Map);
            //slate.PushPrefix(this.parent.LabelCap);
            if (!this.Props.quest.CanRun(slate))
            {
                Map map = usedBy.MapHeld;
                List<Thing> things = ThingSetMakerDefOf.VisitorGift.root.Generate();
                IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
                DropPodUtility.DropThingsNear(intVec, map, things, 110, false, true, true);
                Find.LetterStack.ReceiveLetter("LetterLabelPolarisblocConsolationPrize".Translate(), "PolarisblocConsolationPrize".Translate(), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null);
                return;
            }
            else
            {
                Quest quest = QuestUtility.GenerateQuestAndMakeAvailable(this.Props.quest, slate);
                QuestUtility.SendLetterQuestAvailable(quest);
            }
        }

        /*private Quest GenerateQuest()
        {
            Slate slate = new Slate();
            slate.Set("points", points);
            return QuestUtility.GenerateQuestAndMakeAvailable(this.Props.quest, slate);
        }*/
    }

    public class CompProperties_GenQuestInPoints : CompProperties_UseEffect
    {
        public CompProperties_GenQuestInPoints()
        {
            this.compClass = typeof(CompUseEffect_GenQuestInPoints);
        }

        public QuestScriptDef quest;

        public FloatRange pointsOffset = new FloatRange(-200f, 300f);

        public float minPoints = 300f;
    }
}
