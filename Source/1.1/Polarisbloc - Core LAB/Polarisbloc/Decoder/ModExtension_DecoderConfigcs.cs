using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Verse;

namespace Polarisbloc
{
    public class ModExtension_DecoderConfigcs : DefModExtension
    {
        public float successChanceBase = 0.5f;

		private float factorAwful = 1f;

		private float factorPoor = 1f;

		public float factorNormal = 1f;

		private float factorGood = 1f;

		private float factorExcellent = 1f;

		private float factorMasterwork = 1f;

		private float factorLegendary = 1f;

		public float chanceIntellectualPerLevelOffset = 0.02f;

		public float selfDestoryOnFailed = 0.7f;

		public float selfDestoryOnSuccessed = 0.2f;

		public float GetChanceWithIntellectual(Pawn pawn)
		{
			return this.successChanceBase + (pawn.skills.GetSkill(SkillDefOf.Intellectual).Level * this.chanceIntellectualPerLevelOffset);
		}

		public float GetFactorWithQuality(QualityCategory qc)
		{
			switch (qc)
			{
				case QualityCategory.Awful:
					return this.factorAwful;
				case QualityCategory.Poor:
					return this.factorPoor;
				case QualityCategory.Normal:
					return this.factorNormal;
				case QualityCategory.Good:
					return this.factorGood;
				case QualityCategory.Excellent:
					return this.factorExcellent;
				case QualityCategory.Masterwork:
					return this.factorMasterwork;
				case QualityCategory.Legendary:
					return this.factorLegendary;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

	}
}
