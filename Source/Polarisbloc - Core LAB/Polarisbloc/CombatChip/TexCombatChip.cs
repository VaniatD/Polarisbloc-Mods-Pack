using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using UnityEngine;

namespace Polarisbloc
{
    [StaticConstructorOnStartup]
    public static class TexCombatChip
    {
        public static Texture2D CombatChipActive = ContentFinder<Texture2D>.Get("Polarisbloc/UI/CombatChipActive", true);

        public static Texture2D CombatChipReset = ContentFinder<Texture2D>.Get("Polarisbloc/UI/CombatChipReset", true);

        public static Texture2D CombatChipDrawPsyfocusOn = ContentFinder<Texture2D>.Get("Polarisbloc/UI/CombatChipDrawPsyfocusOn", true);

        public static Texture2D CombatChipDrawPsyfocusOff = ContentFinder<Texture2D>.Get("Polarisbloc/UI/CombatChipDrawPsyfocusOff", true);
    }
}
