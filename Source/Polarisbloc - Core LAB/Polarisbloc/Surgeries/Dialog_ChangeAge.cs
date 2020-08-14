using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Polarisbloc
{
    public class Dialog_ChangeAge : Window
    {
		private Pawn pawn;

		private float curAge;

		private float MaxAge
		{
			get
			{
				return Mathf.Max(this.pawn.RaceProps.lifeStageAges.Last().minAge * 2f, 100f, this.pawn.RaceProps.lifeExpectancy);
			}
		}

		protected virtual int MaxNameLength
		{
			get
			{
				return 28;
			}
		}

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(280f, 175f);
			}
		}

		public Dialog_ChangeAge(Pawn pawn)
		{
			this.pawn = pawn;
			this.curAge = pawn.ageTracker.AgeBiologicalYearsFloat;
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Small;
			bool flag = false;
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{
				flag = true;
				Event.current.Use();
			}
			GUI.SetNextControlName("Polarisbloc.ChangeAge");
			//string text = Widgets.TextField(new Rect(0f, 15f, inRect.width, 35f), this.curName);
			//this.curAge = Widgets.HorizontalSlider(new Rect(0f, 15f, inRect.width, 35f), this.curAge, 0f, this.MaxAge, false, "age", "0", this.MaxAge.ToString(), 0.1f);
			Dialog_ChangeAge.HorizontalSlider(new Rect(0f, 15f, inRect.width, 35f), ref this.curAge, 0f, this.MaxAge, false, "PolarisTargetBioAge".Translate(this.curAge.ToString("F1")), "0", this.MaxAge.ToString(), 0.1f);
			if (Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK", true, true, true) || flag)
			{
				this.pawn.ageTracker.AgeBiologicalTicks = (long)(this.curAge * GenDate.TicksPerYear);
				string text = "PolarisMessageSuccessfullyChangeBioAge".Translate(pawn.LabelShort);
				Messages.Message(text, pawn, MessageTypeDefOf.PositiveEvent);
				Find.WindowStack.TryRemove(this, true);
			}
		}

		/*public static float HorizontalSlider(Rect rect, ref float value, float leftValue, float rightValue, string label = null, float roundTo = -1f)
		{
			if (label != null)
			{
				TextAnchor anchor = Text.Anchor;
				GameFont font = Text.Font;
				Text.Font = GameFont.Tiny;
				Text.Anchor = 0;
				Widgets.Label(rect, label);
				Text.Anchor = anchor;
				Text.Font = font;
				rect.y += 18f;
			}
			value = GUI.HorizontalSlider(rect, value, leftValue, rightValue);
			if (roundTo > 0f)
			{
				value = (float)Mathf.RoundToInt(value / roundTo) * roundTo;
			}
			if (4f.ToString() + label == null)
			{
				return 0f;
			}
			return 18f;
		}*/

		public static float HorizontalSlider(Rect rect, ref float value, float leftValue, float rightValue, bool middleAlignment = false, string label = null, string leftAlignedLabel = null, string rightAlignedLabel = null, float roundTo = -1f)
		{
			if (middleAlignment || !label.NullOrEmpty())
			{
				rect.y += Mathf.Round((rect.height - 16f) / 2f);
			}
			if (!label.NullOrEmpty())
			{
				rect.y += 5f;
			}
			value = GUI.HorizontalSlider(rect, value, leftValue, rightValue);
			if (!label.NullOrEmpty() || !leftAlignedLabel.NullOrEmpty() || !rightAlignedLabel.NullOrEmpty())
			{
				TextAnchor anchor = Text.Anchor;
				GameFont font = Text.Font;
				Text.Font = GameFont.Tiny;
				float num2 = label.NullOrEmpty() ? 18f : Text.CalcSize(label).y;
				rect.y = rect.y - num2 + 3f;
				if (!leftAlignedLabel.NullOrEmpty())
				{
					Text.Anchor = TextAnchor.UpperLeft;
					Widgets.Label(rect, leftAlignedLabel);
				}
				if (!rightAlignedLabel.NullOrEmpty())
				{
					Text.Anchor = TextAnchor.UpperRight;
					Widgets.Label(rect, rightAlignedLabel);
				}
				if (!label.NullOrEmpty())
				{
					Text.Anchor = TextAnchor.UpperCenter;
					Widgets.Label(rect, label);
				}
				Text.Anchor = anchor;
				Text.Font = font;
			}
			if (roundTo > 0f)
			{
				value = (float)Mathf.RoundToInt(value / roundTo) * roundTo;
			}
			return value;
		}
	}
}
