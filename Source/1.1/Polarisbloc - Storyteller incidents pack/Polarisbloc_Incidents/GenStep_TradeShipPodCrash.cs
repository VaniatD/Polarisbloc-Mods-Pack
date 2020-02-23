using System;
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
			if (!base.CanScatterAt(c, map))
			{
				return false;
			}
			if (!c.SupportsStructureType(map, TerrainAffordanceDefOf.Heavy))
			{
				return false;
			}
			if (!map.reachability.CanReachMapEdge(c, TraverseParms.For(TraverseMode.PassDoors, Danger.Deadly, false)))
			{
				return false;
			}
			CellRect rect = CellRect.CenteredOn(c, GenStep_TradeShipPodCrash.Size, GenStep_TradeShipPodCrash.Size);
			List<CellRect> list;
			if (MapGenerator.TryGetVar<List<CellRect>>("UsedRects", out list) && list.Any((CellRect x) => x.Overlaps(rect)))
			{
				return false;
			}
			foreach (IntVec3 c2 in rect)
			{
				if (!c2.InBounds(map) || c2.GetEdifice(map) != null)
				{
					return false;
				}
			}
			return true;
		}

		protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int count = 1)
		{
			CellRect cellRect = CellRect.CenteredOn(loc, GenStep_TradeShipPodCrash.Size, GenStep_TradeShipPodCrash.Size).ClipInsideMap(map);
			List<CellRect> list;
			if (!MapGenerator.TryGetVar<List<CellRect>>("UsedRects", out list))
			{
				list = new List<CellRect>();
				MapGenerator.SetVar<List<CellRect>>("UsedRects", list);
			}
			ResolveParams resolveParams = default(ResolveParams);
			resolveParams.rect = cellRect;
			resolveParams.faction = map.ParentFaction;
			if (parms.sitePart != null && parms.sitePart.things != null && parms.sitePart.things.Any)
			{
				resolveParams.stockpileConcreteContents = parms.sitePart.things;
			}
			else
			{
				ItemStashContentsComp component = map.Parent.GetComponent<ItemStashContentsComp>();
				if (component != null && component.contents.Any)
				{
					resolveParams.stockpileConcreteContents = component.contents;
				}
				else
				{
					resolveParams.thingSetMakerDef = (this.thingSetMakerDef ?? ThingSetMakerDefOf.MapGen_DefaultStockpile);
				}
			}
			BaseGen.globalSettings.map = map;
			BaseGen.symbolStack.Push("storage", resolveParams, null);
			BaseGen.Generate();
			MapGenerator.SetVar<CellRect>("RectOfInterest", cellRect);
			list.Add(cellRect);
		}

		public ThingSetMakerDef thingSetMakerDef;

		private const int Size = 12;
	}
}
