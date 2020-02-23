using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc_Incidents
{
    public class IncidentWorker_PowerOverload : IncidentWorker
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
                result = this.TryFindRandomPowerTraderBuilding((Map)parms.target, out Building building);
            }
            return result;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool result = false;
            if (this.TryFindRandomPowerTraderBuilding((Map)parms.target, out Building building))
            {
                DamageInfo dinfo = new DamageInfo(DamageDefOf.Flame, 5f);
                building.TakeDamage(dinfo);
                FireUtility.TryStartFireIn(building.Position, (Map)parms.target, 0.3f);
                Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, building);
                result = true;
            }
            return result;
        }




        private bool TryFindRandomPowerTraderBuilding(Map map, out Building building)
        {
            bool result = false;
            building = null;
            List<Building> list = map.listerBuildings.allBuildingsColonist;
            if ((from x in list
                 where x.GetComp<CompPowerTrader>() != null && x.FlammableNow && x.GetComp<CompPowerTrader>().PowerOn
                 select x).TryRandomElement(out building))
            {
                result = true;
            }
            return result;
        }
    }
}
