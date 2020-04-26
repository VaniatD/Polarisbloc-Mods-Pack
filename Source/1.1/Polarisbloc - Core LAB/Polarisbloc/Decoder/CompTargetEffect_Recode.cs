using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using Verse.AI;

namespace Polarisbloc
{
    public class CompTargetEffect_Recode : CompTargetEffect
    {
		public override void DoEffectOn(Pawn user, Thing target)
		{
			if (!user.IsColonistPlayerControlled)
			{
				return;
			}
			if (!user.CanReserveAndReach(target, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
			{
				return;
			}
			Job job = JobMaker.MakeJob(PolarisblocDefOf.PolarisDecode, target, this.parent);
			job.count = 1;
			user.jobs.TryTakeOrderedJob(job, JobTag.Misc);
		}


		
	}
}
