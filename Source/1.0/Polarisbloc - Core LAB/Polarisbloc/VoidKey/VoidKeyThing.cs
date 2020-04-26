using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using RimWorld.Planet;
using Verse.AI;
using UnityEngine;

namespace Polarisbloc
{
    public class VoidKeyThing : ThingWithComps, IThingHolder
    {
        protected ThingOwner<Pawn> innerContainer;

        public VoidKeyThing()
        {
            this.innerContainer = new ThingOwner<Pawn>(this, false, LookMode.Deep);
        }

        public bool HasAnyContents
        {
            get
            {
                return this.innerContainer.Count > 0;
            }
        }

        public Pawn InnerPawn
        {
            get
            {
                if (this.innerContainer.Count > 0)
                {
                    return this.innerContainer[0];
                }
                return null;
            }
            set
            {
                if (value == null)
                {
                    this.innerContainer.Clear();
                }
                else
                {
                    if (this.innerContainer.Count > 0)
                    {
                        this.innerContainer.Clear();
                    }
                    this.innerContainer.TryAdd(value, true);
                }
            }
        }

        public ThingOwner GetDirectlyHeldThings()
        {
            return this.innerContainer;
        }

        public void GetChildHolders(List<IThingHolder> outChildren)
        {
            ThingOwnerUtility.AppendThingHoldersFromThings(outChildren, this.GetDirectlyHeldThings());
        }

        public override void TickRare()
        {
            base.TickRare();
            this.innerContainer.ThingOwnerTickRare(true);
        }

        public override void Tick()
        {
            base.Tick();
            this.innerContainer.ThingOwnerTick(true);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look<ThingOwner<Pawn>>(ref this.innerContainer, "innerContainer", new object[]
            {
                this
            });
        }

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            this.innerContainer.ClearAndDestroyContentsOrPassToWorld(DestroyMode.Vanish);
            base.Destroy(mode);
        }

        public override string GetInspectString()
        {
            string text = base.GetInspectString();
            string str = this.innerContainer.ContentsString;
            if (!text.NullOrEmpty())
            {
                text += "\n";
            }
            return text + "PolarisVoidKeyLinkedTo".Translate() + ": " + str.CapitalizeFirst();
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            foreach (Gizmo g in base.GetGizmos())
            {
                yield return g;
            }
            yield return new Command_Action
            {
                defaultLabel = "PolarisVoidKeyTryLinkGizmoLabel".Translate(),
                defaultDesc = "PolarisVoidKeyTryLinkGizmoDesc".Translate(),
                icon = VoidKeyDataBase.VoidKeyGizmo,
                disabled = !VoidKeyUtility.TryRandomlyMissingColonist(out Pawn pawn),
                disabledReason = "PolarisVoidKeyFoundNoColonist".Translate(),
                action = delegate
                {
                    Find.WindowStack.Add(new ChooseVoidPawnWindow(this));
                },
                hotKey = KeyBindingDefOf.Designator_RotateLeft
            };
        }
    }
}
