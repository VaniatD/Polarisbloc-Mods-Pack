using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc_Incidents
{
    public class IncidentWorker_TradeShipPodCrash : IncidentWorker
    {
        protected override bool CanFireNowSub(IncidentParms parms)
        {
            bool result;
            if (!base.CanFireNowSub(parms))
            {
                result = false;
            }
            else
            {
                Map map = (Map)parms.target;
                //Find.FactionManager.OfPlayer.);
                return ResearchProjectDef.Named("MicroelectronicsBasics").IsFinished;
            }
            return result;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map map = (Map)parms.target;
            if ((from x in DefDatabase<TraderKindDef>.AllDefs
                 where x.orbital
                 select x).TryRandomElement(out TraderKindDef def))
            {
                TradeShip tradeShip = new TradeShip(def);
                List<Thing> list = new List<Thing>();
                ThingSetMakerParams Trader = default(ThingSetMakerParams);
                Trader.traderDef = tradeShip.def;
                Trader.tile = map.Tile;
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

                IntVec3 intVec = DropCellFinder.RandomDropSpot(map);
                DropPodUtility.DropThingsNear(intVec, map, list, 110, false, true, true);
                Find.LetterStack.ReceiveLetter(tradeShip.def.LabelCap + " " + "PolarisTitleTradeShipPodCrash".Translate(), "PolarisTradeShipPodCrash".Translate(tradeShip.name,tradeShip.def.label), LetterDefOf.PositiveEvent, new TargetInfo(intVec, map, false), null);
                return true;
            }
            throw new InvalidOperationException();
        }



    }
}
