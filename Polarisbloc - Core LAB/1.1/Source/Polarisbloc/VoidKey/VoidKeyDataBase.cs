using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;

namespace Polarisbloc
{
    [StaticConstructorOnStartup]
    public static class VoidKeyDataBase
    {
        public static Texture2D ChioosePawnButton = ContentFinder<Texture2D>.Get("Polarisbloc/UI/VoidKeyChoosePawn", true);

        public static Texture2D VoidPawnInfoButton = ContentFinder<Texture2D>.Get("Polarisbloc/UI/VoidKeyPawnInfo", true);

        public static Texture2D VoidKeyExactThisOne = ContentFinder<Texture2D>.Get("Polarisbloc/UI/VoidKeyExactThisOne", true);

        public static Texture2D VoidKeyGizmo = ContentFinder<Texture2D>.Get("Polarisbloc/UI/VoidKeyGizmo", true);
    }
}
