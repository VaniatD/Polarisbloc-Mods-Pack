using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;
using UnityEngine;

namespace Polarisbloc
{
    public class Recipe_ExtractAbility : Recipe_Surgery
    {
		private bool Operable(Hediff target, RecipeDef recipe)
		{
			Hediff_ImplantWithLevel hediff_ImplantWithLevel = target as Hediff_ImplantWithLevel;
			if (hediff_ImplantWithLevel == null)
			{
				return false;
			}
			int level = hediff_ImplantWithLevel.level;
			if (hediff_ImplantWithLevel.def != recipe.changesHediffLevel)
			{
				return false;
			}
			else
			{
				return level > 0;
			}
			//return (float)level < hediff_ImplantWithLevel.def.maxSeverity;
		}

		public override IEnumerable<BodyPartRecord> GetPartsToApplyOn(Pawn pawn, RecipeDef recipe)
		{
			return MedicalRecipesUtility.GetFixedPartsToApplyOn(recipe, pawn, (BodyPartRecord record) => pawn.health.hediffSet.hediffs.Any((Hediff x) => x.Part == record && this.Operable(x, recipe)));
		}

		/*public override void ConsumeIngredient(Thing ingredient, RecipeDef recipe, Map map)
		{
		}*/

		public override void ApplyOnPawn(Pawn pawn, BodyPartRecord part, Pawn billDoer, List<Thing> ingredients, Bill bill)
		{
			bool flag = this.IsViolationOnPawn(pawn, part, Faction.OfPlayer);
			if (billDoer != null)
			{
				if (base.CheckSurgeryFail(billDoer, pawn, ingredients, part, bill))
				{
					return;
				}
				TaleRecorder.RecordTale(TaleDefOf.DidSurgery, new object[]
				{
					billDoer,
					pawn
				});
				
			}
			CompNeurotrainer compNeurotrainer = ingredients[0].TryGetComp<CompNeurotrainer>();
			if (compNeurotrainer != null && !pawn.abilities.abilities.NullOrEmpty())
			{
				List<DebugMenuOption> list = new List<DebugMenuOption>();
				foreach (Ability ability in pawn.abilities.abilities)
				{
					list.Add(new DebugMenuOption(ability.def.LabelCap, DebugMenuOptionMode.Action, delegate ()
					{
						compNeurotrainer.ability = ability.def;
						string thingDefName = NeurotrainerDefGenerator.PsytrainerDefPrefix + "_" + ability.def.defName;
						ThingDef thingDef = DefDatabase<ThingDef>.AllDefsListForReading.Find(x => x.defName.Equals(thingDefName));
						//Thing thing = ThingMaker.MakeThing(thingDef, null);
						//GenPlace.TryPlaceThing(thing, pawn.Position, billDoer.Map, ThingPlaceMode.Near);
						GenSpawn.Spawn(thingDef, pawn.Position, pawn.Map, WipeMode.Vanish);
						string abilityName = ability.def.LabelCap;
						pawn.abilities.abilities.Remove(ability);
						Messages.Message("PolarisExtractAbilitySuccessfully".Translate(pawn.NameShortColored, abilityName), pawn, MessageTypeDefOf.NeutralEvent, true);
						if (flag)
						{
							base.ReportViolation(pawn, billDoer, pawn.FactionOrExtraMiniOrHomeFaction, -70, "PolarisGoodwillChangedReason_ExtractedAbility".Translate(pawn.NameShortColored));
						}
					}));
				}
				Find.WindowStack.Add(new Dialog_DebugOptionListLister(list));
			}
			else
			{
				Messages.Message("PolarisExtractAbilityFailed".Translate(pawn.NameShortColored), pawn, MessageTypeDefOf.NeutralEvent, true);
			}
			
		}



	}
}
