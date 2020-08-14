using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld.QuestGen;
using Verse;
using Verse.Grammar;
using RimWorld.Planet;
using RimWorld;
using UnityEngine;

namespace Polarisbloc_Incidents
{
    public class SitePartWorker_ShipCrashed : SitePartWorker
    {
		public override void Notify_GeneratedByQuestGen(SitePart part, Slate slate, List<Rule> outExtraDescriptionRules, Dictionary<string, string> outExtraDescriptionConstants)
		{
			base.Notify_GeneratedByQuestGen(part, slate, outExtraDescriptionRules, outExtraDescriptionConstants);
			ThingDef thingDef = slate.Get<ThingDef>("itemStashSingleThing", null, false);
			IEnumerable<ThingDef> enumerable = slate.Get<IEnumerable<ThingDef>>("itemStashThings", null, false);
			List<Thing> list = this.GenThingList();
			part.things = new ThingOwner<Thing>(part, false, LookMode.Deep);
			part.things.TryAddRangeOrTransfer(list, false, false);
			outExtraDescriptionRules.Add(new Rule_String("itemStashContents", GenLabel.ThingsLabel(list, "  - ")));
			outExtraDescriptionRules.Add(new Rule_String("itemStashContentsValue", GenThing.GetMarketValue(list).ToStringMoney(null)));
		}

		public override string GetPostProcessedThreatLabel(Site site, SitePart sitePart)
		{
			string text = base.GetPostProcessedThreatLabel(site, sitePart);
			if (site.HasWorldObjectTimeout)
			{
				text += " (" + "DurationLeft".Translate(site.WorldObjectTimeoutTicksLeft.ToStringTicksToPeriod(true, false, true, true)) + ")";

			}
			return text;
		}

        private List<Thing> GenThingList()
        {
            List<Thing> list = new List<Thing>();
            if ((from x in DefDatabase<TraderKindDef>.AllDefs
                 where x.orbital
                 select x).TryRandomElement(out TraderKindDef def))
            {
                TradeShip tradeShip = new TradeShip(def);
                ThingSetMakerParams Trader = default(ThingSetMakerParams);
                Trader.traderDef = tradeShip.def;
                list = ThingSetMakerDefOf.TraderStock.root.Generate(Trader);
                List<Thing> tempList = new List<Thing>();
                for (int i = list.Count - 1; i >= 0; i--)
                {
                    bool flag = true;
                    if (list[i] is Pawn pawn)
                    {
                        if (Rand.Chance(0.6f))
                        {
                            list.Remove(list[i]);
                            flag = false;
                        }
                        else
                        {
                            HealthUtility.DamageUntilDowned(pawn);
                        }
                    }
                    else
                    {
                        if (list[i].stackCount == 1)
                        {
                            if (Rand.Chance(0.6f))
                            {
                                list.Remove(list[i]);
                                //list[i].Destroy();
                                flag = false;
                            }
                        }
                        else
                        {
                            list[i].stackCount = Mathf.RoundToInt(list[i].stackCount * 0.3f);
                            if (list[i].stackCount < 1)
                            {
                                list.Remove(list[i]);
                                //list[i].Destroy();
                                flag = false;
                            }
                        }
                    }
                    if (flag)
                    {
                        for (int j = list[i].stackCount; j > list[i].def.stackLimit; j -= list[i].def.stackLimit)
                        {
                            list[i].stackCount -= list[i].def.stackLimit;
                            Thing tempThing = ThingMaker.MakeThing(list[i].def);
                            tempThing.stackCount = list[i].def.stackLimit;
                            if (tempThing is MinifiedThing && (tempThing as MinifiedThing).InnerThing == null) continue;
                            tempList.Add(tempThing);
                        }
                    }
                }
                list.AddRange(tempList);
            }
            return list;
        }
    }
}
