using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Polarisbloc_SecurityForce
{
    public class GoodwillSituationWorker_PolarisPermit : GoodwillSituationWorker
    {
		public override int GetNaturalGoodwillOffset(Faction other)
		{
			if (other.def == PSFDefOf.Polaribloc_SecuirityForce)
			{
				return 75;
			}
			return 0;
		}
	}

}
