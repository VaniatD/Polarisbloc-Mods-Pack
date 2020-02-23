using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc_Incidents
{
    public class IncidentWorker_TurretDisturbed : IncidentWorker
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
                result = this.TryFindRandomAutoTurretInMap((Map)parms.target, out Building turret);
            }
            return result;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool result = false;
            if (this.TryFindRandomAutoTurretInMap((Map)parms.target, out Building turret))
            {
                turret.SetFaction(Faction.OfMechanoids);
                Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, LetterDefOf.ThreatSmall, turret);
                result = true;
            }
            return result;
        }

        private bool TryFindRandomAutoTurretInMap(Map map, out Building turret)
        {
            bool result = false;
            turret = null;
            List<Building> list = map.listerBuildings.allBuildingsColonist;
                //map.listerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);
            if ((from x in list
                 where x is Building_TurretGun && x.GetComp<CompMannable>() == null && x.Faction == Faction.OfPlayer && x.GetComp<CompPowerTrader>().PowerOn
                 select x).TryRandomElement<Building>(out turret))
            {
                //turret = (Building)turretThing;
                result = true;
            }
            return result;
        }
    }
}
