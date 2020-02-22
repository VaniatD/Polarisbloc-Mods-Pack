using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc
{
    public static class ReceptorAntagonistDatabase
    {
        public static List<HediffDef> addictionHediffs = new List<HediffDef>();

        public static List<HediffDef> toleranceHediffs = new List<HediffDef>();

        public static void BuildDrugHediffsDatabaseIfNecessary()
        {
            if (ReceptorAntagonistDatabase.addictionHediffs.Count > 0 && ReceptorAntagonistDatabase.toleranceHediffs.Count > 0)
            {
                return;
            }
            ReceptorAntagonistDatabase.Reset();
            foreach (ChemicalDef c in DefDatabase<ChemicalDef>.AllDefs)
            {
                ReceptorAntagonistDatabase.addictionHediffs.Add(c.addictionHediff);
                ReceptorAntagonistDatabase.toleranceHediffs.Add(c.toleranceHediff);
            }
        }

        public static void Reset()
        {
            ReceptorAntagonistDatabase.addictionHediffs.Clear();
            ReceptorAntagonistDatabase.toleranceHediffs.Clear();
        }

    }
}
