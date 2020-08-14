using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace Polarisbloc
{
    public class CompTargetable_BiocodableWeapon : CompTargetable
    {
		protected override bool PlayerChoosesTarget
		{
			get
			{
				return true;
			}
		}

		protected override TargetingParameters GetTargetingParameters()
		{
			return new TargetingParameters
			{
				canTargetPawns = false,
				canTargetBuildings = false,
				canTargetItems = true,
				mapObjectTargetsMustBeAutoAttackable = false,
				validator = ((TargetInfo x) => this.IsBiocodableThing(x.Thing) || this.IsBladeLinkWeapon(x.Thing))
			};
		}


		public override IEnumerable<Thing> GetTargets(Thing targetChosenByPlayer = null)
		{
			yield return targetChosenByPlayer;
			yield break;
		}

		private bool IsBiocodableThing(Thing t)
		{
			if (t.TryGetComp<CompBiocodable>() != null)
			{
				return true;
			}
			return false;
		}

		/*private bool IsBiocodableWeapon(Thing t)
		{
			if (t.TryGetComp<CompBiocodableWeapon>() != null)
			{
				return true;
			}
			return false;
		}*/

		private bool IsBladeLinkWeapon(Thing t)
		{
			if (t.TryGetComp<CompBladelinkWeapon>() != null)
			{
				return true;
			}
			return false;
		}

		/*private bool IsBiocodableApparel(Thing t)
		{
			if (t.TryGetComp<CompBiocodableApparel>() != null)
			{
				return true;
			}
			return false;
		}*/

	}
}
