using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using RimWorld;

namespace Polarisbloc
{
    public class ChooseVoidPawnWindow : Window
    {
        public static string instructionString = "";

        public List<Pawn> voidPawnCache = new List<Pawn>();

        public List<Pawn> voidPawnAnimalCache = new List<Pawn>();

        private Thing thing;

        public Vector2 resultsAreaScroll;

        public override Vector2 InitialSize
        {
            get
            {
                return new Vector2(640f, 480f);
            }
        }

        public ChooseVoidPawnWindow(Thing thing)
        {
            this.optionalTitle = "PolarisVoidKeyWindowTitle".Translate();
            this.preventCameraMotion = false;
            this.absorbInputAroundWindow = false;
            this.draggable = true;
            this.doCloseX = true;
            ChooseVoidPawnWindow.instructionString = Translator.Translate("PolarisVoidKeyWindowInformation");
            this.thing = thing;
            this.voidPawnCache = (from x in Find.WorldPawns.AllPawnsDead
                                  where x.Corpse == null && x.Faction == Faction.OfPlayer && x.def.race.Humanlike
                                  select x).ToList();
            this.voidPawnAnimalCache = (from x in Find.WorldPawns.AllPawnsDead
                                        where x.Corpse == null && x.Faction == Faction.OfPlayer && x.def.race.Animal
                                        select x).ToList();
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Rect rect = new Rect(inRect);
            rect.height = Text.CalcHeight(ChooseVoidPawnWindow.instructionString, rect.width) + 2f;
            Widgets.Label(rect, ChooseVoidPawnWindow.instructionString);
            Rect rect2 = new Rect(rect);
            rect2.y += rect.height + 2f;
            rect2.height = inRect.height - rect2.y - 2f;
            Rect rect3 = new Rect(rect2);
            rect3.width -= 16f;
            float num = Text.LineHeight * 2.25f + 2f;
            float num2 = this.voidPawnCache.Count > 0 ? ((float)(this.voidPawnCache.Count + 1) * num) : 0f;
            float num3 = this.voidPawnAnimalCache.Count > 0 ? ((float)(this.voidPawnAnimalCache.Count + 1) * num) : 0f;
            rect3.height = num2 + num3;
            Widgets.BeginScrollView(rect2, ref this.resultsAreaScroll, rect3, true);
            Rect rect4 = new Rect(rect3);
            rect4.height = num;
            if (this.voidPawnCache.Count > 0)
            {
                /*Text.Font = GameFont.Medium;
                Text.Anchor = TextAnchor.MiddleLeft;
                Rect rect5 = new Rect(rect4);
                rect5.x += 4f;
                rect5.width -= 4f;
                Widgets.Label(rect5, Translator.Translate("VoidKeyPawnsToChoose", new object[]
                    {
                        this.voidPawnCache.Count
                    }));
                Widgets.DrawLineHorizontal(rect4.x, rect4.y + rect4.height, rect4.width);
                Text.Font = GameFont.Small;
                rect4.y += num;*/
                foreach (Pawn pawn in this.voidPawnCache)
                {
                    Widgets.DrawHighlightIfMouseover(rect4);
                    Rect rect6 = new Rect(rect4);
                    rect6.width = rect6.height;
                    Rect rectTemp = new Rect(rect6);
                    rectTemp.width *= 0.8f;
                    rectTemp.height = rectTemp.width;
                    Widgets.ThingIcon(rectTemp, pawn, 1f);
                    Rect rect7 = new Rect(rect4);
                    rect7.width = rect7.width - rect6.width - num * 2f;
                    rect7.x += rect6.width;
                    Widgets.Label(rect7, pawn.LabelCap);
                    Rect rect8 = new Rect(rect4);
                    rect8.width = rect8.height;
                    rect8.x = rect7.x + rect7.width;
                    TooltipHandler.TipRegion(rect8, "PolarisVoidKeyPullVoidPawnData".Translate(this.thing.LabelCap));
                    if (Widgets.ButtonImage(GenUI.ContractedBy(rect8, rect8.width / 4f), VoidKeyDataBase.VoidKeyExactThisOne))
                    {
                        if (this.thing is VoidKeyThing tempThing)
                        {
                            tempThing.InnerPawn = pawn;
                            Messages.Message("PolarisVoidKeyPullDataDone".Translate(pawn.LabelShortCap), MessageTypeDefOf.PositiveEvent);
                            this.Close();
                        }
                    }
                    Rect rect9 = new Rect(rect4);
                    rect9.width = rect9.height;
                    rect9.x = rect8.x + rect8.width;
                    Widgets.InfoCardButton(rect9.x + rect9.width / 2f - 12f, rect9.y + rect9.height / 2f - 12f, pawn);
                    TooltipHandler.TipRegion(rect4, pawn.DescriptionDetailed);
                    rect4.y += num;
                }
                foreach (Pawn pawn in this.voidPawnAnimalCache)
                {
                    Widgets.DrawHighlightIfMouseover(rect4);
                    Rect rect6 = new Rect(rect4);
                    rect6.width = rect6.height;
                    Rect rectTemp = new Rect(rect6);
                    rectTemp.width *= 0.8f;
                    rectTemp.height = rectTemp.width;
                    Widgets.ThingIcon(rectTemp, pawn, 1f);
                    Rect rect7 = new Rect(rect4);
                    rect7.width = rect7.width - rect6.width - num * 2f;
                    rect7.x += rect6.width;
                    Widgets.Label(rect7, pawn.LabelCap);
                    Rect rect8 = new Rect(rect4);
                    rect8.width = rect8.height;
                    rect8.x = rect7.x + rect7.width;
                    TooltipHandler.TipRegion(rect8, "PolarisVoidKeyPullVoidPawnData".Translate(this.thing.LabelCap));
                    if (Widgets.ButtonImage(GenUI.ContractedBy(rect8, rect8.width / 4f), VoidKeyDataBase.VoidKeyExactThisOne))
                    {
                        if (this.thing is VoidKeyThing tempThing)
                        {
                            tempThing.InnerPawn = pawn;
                            Messages.Message("PolarisVoidKeyPullDataDone".Translate(pawn.LabelShortCap), MessageTypeDefOf.PositiveEvent);
                            this.Close();
                        }
                    }
                    Rect rect9 = new Rect(rect4);
                    rect9.width = rect9.height;
                    rect9.x = rect8.x + rect8.width;
                    Widgets.InfoCardButton(rect9.x + rect9.width / 2f - 12f, rect9.y + rect9.height / 2f - 12f, pawn);
                    TooltipHandler.TipRegion(rect4, pawn.DescriptionDetailed);
                    rect4.y += num;
                }
            }
            Text.Anchor = TextAnchor.UpperLeft;
            Widgets.EndScrollView();
        }
    }
}
