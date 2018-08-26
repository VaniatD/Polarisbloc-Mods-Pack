using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class CompMendingShelf : ThingComp
    {
        public CompProperties_CompMendingShelf Props
        {
            get
            {
                return (CompProperties_CompMendingShelf)base.props;
            }
        }

        protected CompPowerTrader powerComp;

        protected CompRefuelable refuelableComp;

        protected CompBreakdownable breakdownableComp;

        private bool CanMendNow
        {
            get
            {
                return FlickUtility.WantsToBeOn(this.parent) && (this.powerComp == null || this.powerComp.PowerOn) && (this.refuelableComp == null || this.refuelableComp.HasFuel) && (this.breakdownableComp == null || !this.breakdownableComp.BrokenDown);
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            this.powerComp = this.parent.GetComp<CompPowerTrader>();
            this.refuelableComp = this.parent.GetComp<CompRefuelable>();
            this.breakdownableComp = this.parent.GetComp<CompBreakdownable>();
        }

        public override void CompTickRare()
        {
            base.CompTickRare();
            if (base.parent is Building_Storage storage)
            {
                if (this.CanMendNow)
                {
                    if (this.MendRandomItem(storage.GetSlotGroup().HeldThings))
                    {
                        if (this.refuelableComp != null && this.refuelableComp.Fuel >= 1f)
                        {
                            this.refuelableComp.ConsumeFuel(1f);
                        }
                    }
                }
            }
        }

        private bool MendRandomItem(IEnumerable<Thing> HeldThings)
        {
            if ((from x in HeldThings
                 where x.def.useHitPoints && (x.HitPoints < x.MaxHitPoints)
                 select x).TryRandomElement(out Thing tempThing))
            {
                tempThing.HitPoints++;
                return true;
            }
            else return false;
        }
    }

    public class CompProperties_CompMendingShelf : CompProperties
    {
        public CompProperties_CompMendingShelf()
        {
            this.compClass = typeof(CompMendingShelf);
        }
    }
}
