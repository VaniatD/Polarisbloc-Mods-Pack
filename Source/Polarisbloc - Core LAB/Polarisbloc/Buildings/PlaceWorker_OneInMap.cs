using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;

namespace Polarisbloc
{
    public class PlaceWorker_OneInMap : PlaceWorker
    {
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null, Thing thing = null)
		{
			if (map.listerThings.AllThings.Any(x => x.def == checkingDef || x.def == checkingDef.blueprintDef || x.def == checkingDef.frameDef || x.def == checkingDef.installBlueprintDef))
			{
				return new AcceptanceReport("PolarisPlaceOnlyOneInMap".Translate());
			}
			return true;
		}
	}
}
