using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Polarisbloc
{
    [StaticConstructorOnStartup]
    public static class SurgeriesDefPatch
    {
        private static readonly List<RecipeDef> polarisRecipeDefs = new List<RecipeDef>
        {
            //PolarisblocDefOf.PolarisTransGenderSurgery,
            PolarisblocDefOf.PolarisCureScars,
            PolarisblocDefOf.PolarisRestoreMissingBodyPart,
            PolarisblocDefOf.PolarisRemoveImplant,
            PolarisblocDefOf.PolarisSurgeryChangeBioAge
        };

        static SurgeriesDefPatch()
        {
            LongEventHandler.ExecuteWhenFinished(SurgeriesPatch);
        }

        private static void SurgeriesPatch()
        {
            SurgeriesDefPatch.AddElementInRecipe();
            Log.Message("[Polarisbloc] SurgeriesDefPatch applied.");
        }

        private static void AddElementInRecipe()
        {
            List<ThingDef> raceDefs = DefDatabase<ThingDef>.AllDefs.Where(IsNeedAddToPolarisSurgeries).ToList();
            foreach (RecipeDef surgery in SurgeriesDefPatch.polarisRecipeDefs)
            {
                List<ThingDef> finalDefs = raceDefs.Except(surgery.AllRecipeUsers).ToList();
                surgery.recipeUsers.AddRange(finalDefs);
            }
            List<ThingDef> genderRaceDefs = raceDefs.Where(x => x.race.hasGenders).ToList();
            List<ThingDef> finGenderRaceDefs = genderRaceDefs.Except(PolarisblocDefOf.PolarisTransGenderSurgery.AllRecipeUsers).ToList();
            PolarisblocDefOf.PolarisTransGenderSurgery.recipeUsers.AddRange(finGenderRaceDefs);
        }

        private static bool IsNeedAddToPolarisSurgeries(ThingDef def)
        {
            return def.race != null;
        }
    }
}
