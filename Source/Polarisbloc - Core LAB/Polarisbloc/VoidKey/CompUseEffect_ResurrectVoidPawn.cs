using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;
using RimWorld.Planet;
using Verse.AI.Group;

namespace Polarisbloc
{
    public class CompUseEffect_ResurrectVoidPawn : CompUseEffect
    {
        private VoidKeyThing VoidKeyThing
        {
            get
            {
                return base.parent as VoidKeyThing;
            }
        }

        private Pawn VoidPawn
        {
            get
            {
                return VoidKeyThing.InnerPawn;
            }
        }

        public override void DoEffect(Pawn usedBy)
        {
            base.DoEffect(usedBy);
            if (this.VoidPawn != null)
            {
                Pawn tempPawn = this.VoidPawn;
                VoidKeyUtility.ResurrectPawnFromVoid(this.parent.MapHeld, this.parent.PositionHeld, tempPawn);
                VoidKeyUtility.GiveSideEffects(tempPawn);
            }
            
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            bool result = this.VoidPawn != null;
            failReason = "PolarisVoidKeyFoundNoColonist".Translate();
            return result;
        }
    }
}
