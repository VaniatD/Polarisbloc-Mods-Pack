using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI.Group;

namespace Polarisbloc
{
    public class HediffComp_Cartridge : HediffComp
    {
        public HediffCompProperties_Cartridge Props
        {
            get
            {
                return (HediffCompProperties_Cartridge)base.props;
            }
        }

        public HediffComp_RemoveIfApparelDropped HediffApparel
        {
            get
            {
                return this.parent.TryGetComp<HediffComp_RemoveIfApparelDropped>();
            }
        }

        public override void Notify_PawnDied()
        {
            base.Notify_PawnDied();
            if (this.HediffApparel != null)
            {
                Hediff combatChip = this.Pawn.health.hediffSet.hediffs.Find(x => x is Hediff_CombatChip);
                if (combatChip != null)
                {
                    this.Pawn.health.RemoveHediff(combatChip);
                    this.Pawn.health.AddHediff(PolarisblocDefOf.PolarisCombatChip_Assassin, this.Pawn.health.hediffSet.GetBrain());
                    HediffComp_ToggleHediff toggleComp = this.Pawn.health.hediffSet.GetFirstHediffOfDef(PolarisblocDefOf.PolarisCombatChip_Assassin).TryGetComp<HediffComp_ToggleHediff>();
                    if (toggleComp != null)
                    {
                        toggleComp.toggle = true;
                        toggleComp.CauseDestHediff(this.Pawn);
                    }
                }
                Apparel ap = this.HediffApparel.wornApparel;
                if (ap != null)
                {
                    
                    //base.Pawn.apparel.Remove(ap);
                    ResurrectionUtility.Resurrect(base.Pawn);
                    Messages.Message("PolarisMessageSomeoneResurrected".Translate(ap.LabelShort, base.Pawn.LabelShort), MessageTypeDefOf.PositiveEvent);
                    ap.Destroy(DestroyMode.Vanish);
                }
                if (!this.Pawn.Faction.IsPlayer)
                {
                    /*List<Lord> lords = base.Pawn.Map.lordManager.lords;
                    foreach (Lord lord in lords)
                    {
                        lord.RemovePawn(this.Pawn);
                    }*/
                    this.Pawn.GetLord()?.Notify_PawnLost(this.Pawn, PawnLostCondition.InMentalState);
                    this.Pawn.mindState.exitMapAfterTick = Find.TickManager.TicksGame + 150;
                    IntVec3 invalid = IntVec3.Invalid;
                    if (!RCellFinder.TryFindRandomCellOutsideColonyNearTheCenterOfTheMap(this.Pawn.Position, this.Pawn.Map, 10f, out invalid))
                    {
                        invalid = IntVec3.Invalid;
                    }
                    if (invalid.IsValid)
                    {
                        this.Pawn.mindState.forcedGotoPosition = CellFinder.RandomClosewalkCellNear(invalid, this.Pawn.Map, 10, null);
                    }
                }
                base.Pawn.health.RemoveHediff(base.parent);
            }
            
        }
    }

    public class HediffCompProperties_Cartridge : HediffCompProperties
    {
        public HediffCompProperties_Cartridge()
        {
            base.compClass = typeof(HediffComp_Cartridge);
        }
    }
}
