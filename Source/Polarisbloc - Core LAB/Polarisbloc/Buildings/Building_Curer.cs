using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using UnityEngine;

namespace Polarisbloc
{
    class Building_Curer : Building
    {
        private const float CureRadius = 5f;

        private static List<IntVec3> curableCells = new List<IntVec3>();

        private bool applyToHuman = false;

        private List<Pawn> curablePawns = new List<Pawn>();

        private const int ticksCureInterval = 250;

        private int ticks;

        private IEnumerable<IntVec3> CurableCells
        {
            get
            {
                return Building_Curer.CurableCellsAround(base.Position, base.Map);
            }
        }

        private bool ShouldCureNow
        {
            get
            {
                if (!this.Spawned)
                {
                    return false;
                }
                if (!FlickUtility.WantsToBeOn(this))
                {
                    return false;
                }
                CompPowerTrader compPowerTrader = this.TryGetComp<CompPowerTrader>();
                if (compPowerTrader != null && !compPowerTrader.PowerOn)
                {
                    return false;
                }
                CompRefuelable compRefuelable = this.TryGetComp<CompRefuelable>();
                if (compRefuelable != null && !compRefuelable.HasFuel)
                {
                    return false;
                }
                return true;
            }
        }

        public override void Tick()
        {
            base.Tick();
            this.ticks++;
            if (this.ticks >= Building_Curer.ticksCureInterval)
            {
                if (this.ShouldCureNow)
                {
                    this.curablePawns = this.CurablePawns(this.CurableCells);
                    if (this.curablePawns.Count() > 0)
                    {
                        foreach (Pawn tempPawn in this.curablePawns)
                        {
                            this.TendPawn(tempPawn);
                        }
                    }
                }
                this.ticks = 0;
            }
        }

        /*public override void TickRare()
        {
            base.TickRare();
            if (this.ShouldCureNow)
            {
                this.curablePawns = this.CurablePawns(this.CurableCells);
                if (this.curablePawns.Count() > 0)
                {
                    foreach (Pawn tempPawn in this.curablePawns)
                    {
                        this.TendPawn(tempPawn);
                    }
                }
            }
        }*/

        private List<Pawn> CurablePawns(IEnumerable<IntVec3> intVecs)
        {
            List<Pawn> pawns = new List<Pawn>();
            pawns.Clear();
            // && x.Faction == this.Faction
            foreach (IntVec3 tempIntVec in intVecs)
            {
                foreach (Pawn pawn in from x in base.Map.thingGrid.ThingsListAt(tempIntVec)
                                        where x is Pawn
                                        select x)
                {
                    if (this.applyToHuman && pawn.RaceProps.Humanlike)
                    {
                        pawns.Add(pawn);
                    }
                    else if (!pawn.RaceProps.Humanlike)
                    {
                        pawns.Add(pawn);
                    }
                }
            }
            return pawns;
        }

        public static List<IntVec3> CurableCellsAround(IntVec3 pos, Map map)
        {
            Building_Curer.curableCells.Clear();
            if (!pos.InBounds(map))
            {
                return Building_Curer.curableCells;
            }
            Region region = pos.GetRegion(map, RegionType.Set_Passable);
            if (region == null)
            {
                return Building_Curer.curableCells;
            }
            RegionTraverser.BreadthFirstTraverse(region, (Region from, Region r) => r.door == null, delegate (Region r)
            {
                foreach (IntVec3 cell in r.Cells)
                {
                    if (cell.InHorDistOf(pos, Building_Curer.CureRadius))
                    {
                        Building_Curer.curableCells.Add(cell);
                    }
                }
                return false;
            }, 13, RegionType.Set_Passable);
            return Building_Curer.curableCells;
        }

        private void TendPawn(Pawn pawn)
        {
            if ((from x in pawn.health.hediffSet.hediffs
                 where x.TendableNow() && (x is Hediff_Injury || x is Hediff_MissingPart)
                 select x).TryRandomElement(out Hediff result))
            {
                //result.Tended(Rand.Range(0.1f, 0.4f), 0);
                result.Tended(Rand.Range(0.1f, 0.4f), 1f);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.applyToHuman, "applyToHuman");
            Scribe_Values.Look<int>(ref this.ticks, "ticks");
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            Command_Toggle appToHuman = new Command_Toggle
            {
                hotKey = KeyBindingDefOf.Command_ColonistDraft,
                icon = TexCommand.ToggleVent,
                defaultLabel = "PolarisCurerApplyToHumanLabel".Translate(),
                defaultDesc = "PolarisCurerApplyToHumanDESC".Translate(),
                isActive = () => this.applyToHuman,
                toggleAction = () => { this.applyToHuman = !this.applyToHuman; },
            };
            yield return (Gizmo)appToHuman;
            foreach (var g in base.GetGizmos())
            {
                yield return g;
            }
        }
    }
}
