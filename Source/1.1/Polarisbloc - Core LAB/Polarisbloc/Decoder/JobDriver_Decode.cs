using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;
using Verse.AI;

namespace Polarisbloc
{
    public class JobDriver_Decode : JobDriver
    {
		private Thing CodableThing
		{
			get
			{
				return this.job.GetTarget(TargetIndex.A).Thing;
			}
		}

		private Thing Item
		{
			get
			{
				return this.job.GetTarget(TargetIndex.B).Thing;
			}
		}

		private ModExtension_DecoderConfigcs Configs
		{
			get
			{
				return this.job.def.GetModExtension<ModExtension_DecoderConfigcs>();
			}
		}

		private float SuccessChance
		{
			get
			{
				float chance = this.Configs.GetChanceWithIntellectual(this.pawn);

				if (this.CodableThing.TryGetQuality(out QualityCategory qc))
				{
					chance *= this.Configs.GetFactorWithQuality(qc);
				}
				else
				{
					chance *= this.Configs.factorNormal;
				}
				return chance;
			}
		}

		public override bool TryMakePreToilReservations(bool errorOnFailed)
		{
			return this.pawn.Reserve(this.CodableThing, this.job, 1, -1, null, errorOnFailed) && this.pawn.Reserve(this.Item, this.job, 1, -1, null, errorOnFailed);
		}

		protected override IEnumerable<Toil> MakeNewToils()
		{
			yield return Toils_Goto.GotoThing(TargetIndex.B, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.B).FailOnDespawnedOrNull(TargetIndex.A);
			yield return Toils_Haul.StartCarryThing(TargetIndex.B, false, false, false);
			yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.Touch).FailOnDespawnedOrNull(TargetIndex.A);
			Toil toil = Toils_General.Wait(JobDriver_Decode.DurationTicks, TargetIndex.None);
			toil.WithProgressBarToilDelay(TargetIndex.A, false, -0.5f);
			toil.FailOnDespawnedOrNull(TargetIndex.A);
			toil.FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
			yield return toil;
			yield return Toils_General.Do(new Action(this.Recode));
			yield break;
		}

		private void Recode()
		{
			//CompBiocodableWeapon biocodableWeapon = this.CodableThing.TryGetComp<CompBiocodableWeapon>();

			CompBiocodable biocodableThing = this.CodableThing.TryGetComp<CompBiocodable>();
			CompBladelinkWeapon bladelinkWeapon = this.CodableThing.TryGetComp<CompBladelinkWeapon>();
			bool selfDestory = false;
			/*if (Rand.Value > this.SuccessChance)
			{
				Messages.Message("PolarisDecoderFailedUsedForPawn".Translate(this.pawn.NameShortColored, this.CodableThing.Label), MessageTypeDefOf.NegativeEvent, true);
				if (Rand.Value < this.Configs.selfDestoryOnFailed)
				{
					Messages.Message("PolarisDecoderSelfDestroyed".Translate(), MessageTypeDefOf.NegativeEvent, true);
					this.Item.SplitOff(1).Destroy(DestroyMode.Vanish);
				}
				return;
			}
			Messages.Message("PolarisDecoderSuccessedUsedForPawn".Translate(this.pawn.NameShortColored, this.CodableThing.Label), this.pawn, MessageTypeDefOf.PositiveEvent, true);*/
			/*if (biocodableWeapon != null)
			{
				if (biocodableWeapon.Biocoded)
				{
					biocodableWeapon.CodeFor(this.pawn);
					if (Rand.Value < this.Configs.selfDestoryOnSuccessed)
					{
						selfDestory = true;
					}
				}
				else
				{
					biocodableWeapon.CodeFor(this.pawn);
					
				}
			}*/
			if (biocodableThing != null)
			{

				if (biocodableThing.Biocoded)
				{
					if (this.CheckSuccessOnUsed())
					{
						biocodableThing.CodeFor(this.pawn);
						if (Rand.Value < this.Configs.selfDestoryOnSuccessed)
						{
							selfDestory = true;
						}
					}
				}
				else
				{
					biocodableThing.CodeFor(this.pawn);
					Messages.Message("PolarisDecoderSuccessedUsedForPawn".Translate(this.pawn.NameShortColored, this.CodableThing.Label), this.pawn, MessageTypeDefOf.PositiveEvent, true);
				}
			}

			if (bladelinkWeapon != null)
			{
				if (bladelinkWeapon.bondedPawn != this.pawn)
				{
					if (this.CheckSuccessOnUsed())
					{
						bladelinkWeapon.bondedPawn = null;
						bladelinkWeapon.Notify_Equipped(this.pawn);
						if (Rand.Value < this.Configs.selfDestoryOnSuccessed)
						{
							selfDestory = true;
						}
					}
					
				}
			}

			if (biocodableThing == null && bladelinkWeapon == null)
			{
				throw new ArgumentOutOfRangeException();
			}
			if (selfDestory)
			{
				this.Item.SplitOff(1).Destroy(DestroyMode.Vanish);
				Messages.Message("PolarisDecoderSelfDestroyed".Translate(), MessageTypeDefOf.NegativeEvent, true);
			}

		}

		private bool CheckSuccessOnUsed()
		{
			if (Rand.Value > this.SuccessChance)
			{
				Messages.Message("PolarisDecoderFailedUsedForPawn".Translate(this.pawn.NameShortColored, this.CodableThing.Label), MessageTypeDefOf.NegativeEvent, true);
				if (Rand.Value < this.Configs.selfDestoryOnFailed)
				{
					Messages.Message("PolarisDecoderSelfDestroyed".Translate(), MessageTypeDefOf.NegativeEvent, true);
					this.Item.SplitOff(1).Destroy(DestroyMode.Vanish);
				}
				return false;
			}
			Messages.Message("PolarisDecoderSuccessedUsedForPawn".Translate(this.pawn.NameShortColored, this.CodableThing.Label), this.pawn, MessageTypeDefOf.PositiveEvent, true);
			return true;
		}

		private const int DurationTicks = 1200;
	}
}
