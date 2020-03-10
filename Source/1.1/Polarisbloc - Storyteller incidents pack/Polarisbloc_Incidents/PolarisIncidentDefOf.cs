using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc_Incidents
{
    [DefOf]
    public static class PolarisIncidentDefOf
    {
        //public static SiteCoreDef Polaris_TradeShipPodCrashSiteCore;

        static PolarisIncidentDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PolarisIncidentDefOf));
        }

        public static JobDef Skygaze;
    }
}
