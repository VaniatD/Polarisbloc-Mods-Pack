﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.BaseGen;
using RimWorld.Planet;

namespace Polarisbloc_Incidents
{
    public class GenStep_TradeShipPodCrash : GenStep_Scatterer
    {
        public override int SeedPart
        {
            get
            {
                return 913432591;
            }
        }

        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            bool result;
            if (!base.CanScatterAt(c, map))
            {
                result = false;
            }
            else if (!c.SupportsStructureType(map, TerrainAffordanceDefOf.Heavy))
            {
                result = false;
            }
            else if (!map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)))
            {
                result = false;
            }
            else
            {
                CellRect.CellRectIterator iterator = CellRect.CenteredOn(c, 7, 7).GetIterator();
                while (!iterator.Done())
                {
                    if (!iterator.Current.InBounds(map) || iterator.Current.GetEdifice(map) != null)
                    {
                        return false;
                    }
                    iterator.MoveNext();
                }
                result = true;
            }
            return result;
        }

        protected override void ScatterAt(IntVec3 loc, Map map, int count = 1)
        {
            CellRect cellRect = CellRect.CenteredOn(loc, GenStep_TradeShipPodCrash.Size, GenStep_TradeShipPodCrash.Size).ClipInsideMap(map);
            ResolveParams resolveParams = default(ResolveParams);
            resolveParams.rect = cellRect;
            resolveParams.faction = map.ParentFaction;
            ItemStashContentsComp component = map.Parent.GetComponent<ItemStashContentsComp>();
            if (component != null && component.contents.Any)
            {
                resolveParams.stockpileConcreteContents = component.contents;
            }
            else
            {
                resolveParams.thingSetMakerDef = (this.thingSetMakerDef ?? ThingSetMakerDefOf.MapGen_DefaultStockpile);
            }
            BaseGen.globalSettings.map = map;
            BaseGen.symbolStack.Push("storage", resolveParams);
            BaseGen.Generate();
            MapGenerator.SetVar<CellRect>("RectOfInterest", cellRect);
        }

        public ThingSetMakerDef thingSetMakerDef;

        private const int Size = 16;
    }
}
