using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc_Incidents
{
    public class IncidentWorker_ConduitFusing : IncidentWorker
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
                result = this.TryFindRandomPowerConduitInMap((Map)parms.target, out Building powerConduit);
            }
            return result;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            bool result = false;
            List<IntVec3> targets = new List<IntVec3>();
            int numOffset = Mathf.Min((int)(((Map)parms.target).wealthWatcher.WealthTotal / 100000), 7);
            int num = Rand.Range(1, 1 + numOffset);
            for (int i = 0; i < num; i++)
            {
                if (this.TryFindRandomPowerConduitInMap((Map)parms.target, out Building tempConduit))
                {
                    IntVec3 intVec = new IntVec3(tempConduit.Position.x, tempConduit.Position.y, tempConduit.Position.z);
                    Rot4 rot = new Rot4(tempConduit.Rotation.AsByte);
                    targets.Add(intVec);
                    tempConduit.Destroy(DestroyMode.Deconstruct);
                    if (Find.PlaySettings.autoRebuild  && tempConduit.def.blueprintDef != null && tempConduit.def.IsResearchFinished && ((Map)parms.target).areaManager.Home[tempConduit.Position])
                    {
                        if (GenConstruct.CanPlaceBlueprintAt(tempConduit.def, intVec, rot, (Map)parms.target, false, null).Accepted)
                        {
                            GenConstruct.PlaceBlueprintForBuild(tempConduit.def, intVec, (Map)parms.target, rot, Faction.OfPlayer, tempConduit.Stuff);
                        }
                    }
                }
            }
            if (!targets.NullOrEmpty())
            {
                result = true;
                LookTargets letterTargets = new LookTargets(this.GetLetterTargets(targets, (Map)parms.target));
                Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, this.def.letterDef, letterTargets);
            }
            return result;
        }

        private bool TryFindRandomPowerConduitInMap(Map map, out Building powerConduit)
        {
            bool result = false;
            powerConduit = null;
            List<Building> list = map.listerBuildings.allBuildingsColonist;
            if ((from x in list
                 where x.TryGetComp<CompPowerTransmitter>() != null
                 select x).TryRandomElement(out powerConduit))
            {
                result = true;
            }
            return result;
        }

        private IEnumerable<TargetInfo> GetLetterTargets(List<IntVec3> targets, Map map)
        {
            foreach (IntVec3 c in targets)
            {
                yield return new TargetInfo(c, map);
            }
        }
    }
}
