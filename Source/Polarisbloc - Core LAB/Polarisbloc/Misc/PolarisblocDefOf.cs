using RimWorld;
using Verse;

namespace Polarisbloc
{
    [DefOf]
    public static class PolarisblocDefOf
    {
        static PolarisblocDefOf()
        {
            DefOfHelper.EnsureInitializedInCtor(typeof(PolarisblocDefOf));
        }

        public static ThingDef PolarisCartridge;

        //public static HediffDef Hediff_PloarisCartridge;

        public static ResearchProjectDef ResearchProject_PolarisCartridge;

        public static HediffDef PolarisCombatChip_NotActive;

        //public static ResearchProjectDef PolarisSecretArchives;

        public static HediffDef PolarisCombatChip_Charm;

        public static HediffDef PolarisCombatChip_Assassin;

        public static JobDef PolarisDecode;

        public static RecipeDef PolarisTransGenderSurgery;

        public static RecipeDef PolarisCureScars;

        public static RecipeDef PolarisRestoreMissingBodyPart;

        public static RecipeDef PolarisRemoveImplant;

        public static RecipeDef PolarisSurgeryChangeBioAge;

    }
}
