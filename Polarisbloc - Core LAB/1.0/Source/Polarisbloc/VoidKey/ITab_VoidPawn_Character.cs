using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc
{
    public class ITab_VoidPawn_Character : ITab
    {
        public ITab_VoidPawn_Character()
        {
            this.size = CharacterCardUtility.PawnCardSize + new Vector2(17f, 17f) * 2f;
            this.labelKey = "TabCharacter";
            this.tutorTag = "Character";
        }

        private Pawn PawnToShowInfoAbout
        {
            get
            {
                Pawn pawn = null;
                VoidKeyThing voidKeyThing = base.SelThing as VoidKeyThing;
                if (voidKeyThing != null)
                {
                    pawn = voidKeyThing.InnerPawn;
                }
                if (pawn == null)
                {
                    //Log.Error("Character tab found no selected pawn to display.", false);
                    return null;
                }
                return pawn;
            }
        }

        public override bool IsVisible
        {
            get
            {
                return this.PawnToShowInfoAbout != null && this.PawnToShowInfoAbout.story != null;
            }
        }

        protected override void FillTab()
        {
            Rect rect = new Rect(17f, 17f, CharacterCardUtility.PawnCardSize.x, CharacterCardUtility.PawnCardSize.y);
            VoidKeyCardUtility.DrawCharacterCard(rect, this.PawnToShowInfoAbout, this.SelThing, null, default(Rect));
        }
    }
}
