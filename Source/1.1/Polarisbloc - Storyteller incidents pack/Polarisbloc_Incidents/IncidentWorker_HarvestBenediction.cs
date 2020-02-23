using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc_Incidents
{
    public class IncidentWorker_HarvestBenediction : IncidentWorker
    {
        private const float MaxDaysToGrown = 6f ;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            bool result;
            if (!base.CanFireNowSub(parms))
            {
                result = false;
            }
            else
            {
                result = this.TryFindRandomGrowthablePlant((Map)parms.target);
            }
            return result;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map target = (Map)parms.target;
            List<Thing> list = target.listerThings.ThingsInGroup(ThingRequestGroup.Plant);
            bool flag = false;
            foreach (Thing plantThing in from x in list
                                         where x is Plant
                                         select x)
            {
                Plant plant = plantThing as Plant;
                if ((target.Biome.CommonalityOfPlant(plant.def) == 0f) && plant.LifeStage != PlantLifeStage.Sowing)
                {
                    /*if (plant.def.plant.growDays <= IncidentWorker_HarvestBenediction.MaxDaysToGrown)
                    {
                        plant.Growth = 1;
                    }
                    else
                    {
                        plant.Growth += IncidentWorker_HarvestBenediction.MaxDaysToGrown / plant.def.plant.growDays;
                    }*/
                    plant.Growth += IncidentWorker_HarvestBenediction.MaxDaysToGrown / plant.def.plant.growDays;
                    flag = true;
                }
            }
            /*for (int i = list.Count - 1; i >= 0; i--)
            {
                Plant plant = (Plant)list[i];
                if (((target.Biome.CommonalityOfPlant(plant.def) == 0f) && (plant.def.plant.growDays <= IncidentWorker_HarvestBenediction.MaxDaysToGrown)) && ((plant.LifeStage == PlantLifeStage.Growing) || (plant.LifeStage == PlantLifeStage.Mature)))
                {
                    plant.Growth = 1;
                    flag = true;
                }
            }*/
            if (!flag)
            {
                return false;
            }
            Find.LetterStack.ReceiveLetter(this.def.letterLabel, this.def.letterText, LetterDefOf.PositiveEvent, null);
            return true;
        }


        private bool TryFindRandomGrowthablePlant(Map map)
        {
            bool result = (from x in map.listerThings.ThingsInGroup(ThingRequestGroup.Plant)
                           where ((Plant)x).Growth < 1f && map.Biome.CommonalityOfPlant(x.def) == 0f
                           select x).Any();

            return result;
        }
    }
}
