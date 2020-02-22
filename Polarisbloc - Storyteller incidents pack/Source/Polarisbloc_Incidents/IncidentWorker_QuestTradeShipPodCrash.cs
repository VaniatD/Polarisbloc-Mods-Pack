using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;
using UnityEngine;

namespace Polarisbloc_Incidents
{
    public class IncidentWorker_QuestTradeShipPodCrash : IncidentWorker
    {
        private const float ChanceToRevealSitePart = 0.5f;

        private static readonly IntRange TimeoutDaysRange = new IntRange(15, 45);

        private const float NoSitePartChance = 0.15f;

        private static readonly string TradeShipPodCrashQuestThreatTag = "MineralScannerPreciousLumpThreat";

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            int num;
            Faction faction;
            return base.CanFireNowSub(parms) && (Find.FactionManager.RandomNonHostileFaction(false, false, false, TechLevel.Undefined) != null && TileFinder.TryFindNewSiteTile(out num, 7, 27, false, true, -1)) && SiteMakerHelper.TryFindRandomFactionFor(PolarisIncidentDefOf.Polaris_TradeShipPodCrashSiteCore, null, out faction, true, null);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Faction faction = parms.faction;
            if (faction == null)
            {
                faction = Find.FactionManager.RandomNonHostileFaction(false, false, false, TechLevel.Undefined);
            }
            bool result;
            int tile;
            SitePartDef sitePart;
            Faction siteFaction;
            if (faction == null)
            {
                result = false;
            }
            else if (!TileFinder.TryFindNewSiteTile(out tile, 7, 27, false, true, -1))
            {
                result = false;
            }
            else if (!SiteMakerHelper.TryFindSiteParams_SingleSitePart(PolarisIncidentDefOf.Polaris_TradeShipPodCrashSiteCore, (!Rand.Chance(IncidentWorker_QuestTradeShipPodCrash.NoSitePartChance)) ? IncidentWorker_QuestTradeShipPodCrash.TradeShipPodCrashQuestThreatTag : null, out sitePart, out siteFaction, null, true, null))
            {
                result = false;
            }
            else
            {
                int randomInRange = IncidentWorker_QuestTradeShipPodCrash.TimeoutDaysRange.RandomInRange;
                List<Thing> items = this.GenThingList(out string tradeShipKind);
                Site site = IncidentWorker_QuestTradeShipPodCrash.CreateSite(tile, sitePart, randomInRange, siteFaction, items);
                string letterText = this.GetLetterText(faction, tradeShipKind, randomInRange, site, site.parts.FirstOrDefault<SitePart>());
                Find.LetterStack.ReceiveLetter(this.def.letterLabel, letterText, this.def.letterDef, site, faction, null);
                result = true;
            }
            return result;
        }

        private List<Thing> GenThingList(out string tradeShipKind)
        {
            //Thing tmp;
            //int i;
            tradeShipKind = string.Empty;
            List<Thing> list = new List<Thing>();
            if ((from x in DefDatabase<TraderKindDef>.AllDefs
                 where x.orbital
                 select x).TryRandomElement(out TraderKindDef def))
            {
                TradeShip tradeShip = new TradeShip(def);
                tradeShipKind = def.LabelCap;
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
                /*foreach (Thing thing in list)
                {
                    if (thing is Pawn pawn)
                        HealthUtility.DamageUntilDowned(pawn);
                    else
                    {
                        thing.stackCount = Mathf.FloorToInt(thing.stackCount * 0.3f);
                        if (thing.stackCount <= 0)
                            thing.Destroy();
                    }
                    for (int i = thing.stackCount; i > thing.def.stackLimit; i -= thing.def.stackLimit)
                    {
                        thing.stackCount -= thing.def.stackLimit;
                        Thing tempThing = ThingMaker.MakeThing(thing.def);
                        tempThing.stackCount = thing.def.stackLimit;
                        if (tempThing is MinifiedThing && (tempThing as MinifiedThing).InnerThing == null) continue;
                        tempList.Add(tempThing);
                        
                    }
                }*/
                list.AddRange(tempList);
            }
            return list;
        }

        public static Site CreateSite(int tile, SitePartDef sitePart, int days, Faction siteFaction, IList<Thing> items)
        {
            Site site = SiteMaker.MakeSite(PolarisIncidentDefOf.Polaris_TradeShipPodCrashSiteCore, sitePart, tile, siteFaction, true);
            site.sitePartsKnown = true;
            site.GetComponent<TimeoutComp>().StartTimeout(days * 60000);
            site.GetComponent<ItemStashContentsComp>().contents.TryAddRangeOrTransfer(items, false, false);
            Find.WorldObjects.Add(site);
            return site;
        }

        private string GetLetterText(Faction alliedFaction, string tradeShipKind, int days, Site site, SitePart sitePart)
        {
            string text = string.Format(this.def.letterText, new object[]
            {
                alliedFaction.leader.LabelShort,
                alliedFaction.def.leaderTitle,
                alliedFaction.Name,
                tradeShipKind,
                days.ToString(),
                SitePartUtility.GetDescriptionDialogue(site, sitePart)
            }).CapitalizeFirst();
            return text;
        }

        // Token: 0x040008F0 RID: 2288
        


    }
}
