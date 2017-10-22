using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public abstract class PawnColumnWorker_Checkbox : PawnColumnWorker
	{
		private const int HorizontalPadding = 2;

		public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
		{
			if (this.HasCheckbox(pawn))
			{
				int num = (int)((rect.width - 24.0) / 2.0);
				int num2 = Mathf.Max(3, 0);
				Vector2 topLeft = new Vector2(rect.x + (float)num, rect.y + (float)num2);
				Rect rect2 = new Rect(topLeft.x, topLeft.y, 24f, 24f);
				bool value;
				bool flag = value = this.GetValue(pawn);
				Widgets.Checkbox(topLeft, ref flag, 24f, false);
				if (Mouse.IsOver(rect2))
				{
					string tip = this.GetTip(pawn);
					if (!tip.NullOrEmpty())
					{
						TooltipHandler.TipRegion(rect2, tip);
					}
				}
				if (flag != value)
				{
					this.SetValue(pawn, flag);
				}
			}
		}

		public override int GetMinWidth(PawnTable table)
		{
			return Mathf.Max(base.GetMinWidth(table), 28);
		}

		public override int GetMaxWidth(PawnTable table)
		{
			return Mathf.Min(base.GetMaxWidth(table), this.GetMinWidth(table));
		}

		public override int GetMinCellHeight(Pawn pawn)
		{
			return Mathf.Max(base.GetMinCellHeight(pawn), 24);
		}

		public override int Compare(Pawn a, Pawn b)
		{
			return this.GetValueToCompare(a).CompareTo(this.GetValueToCompare(b));
		}

		private int GetValueToCompare(Pawn pawn)
		{
			return this.HasCheckbox(pawn) ? ((!this.GetValue(pawn)) ? 1 : 2) : 0;
		}

		protected virtual string GetTip(Pawn pawn)
		{
			return (string)null;
		}

		protected virtual bool HasCheckbox(Pawn pawn)
		{
			return true;
		}

		protected abstract bool GetValue(Pawn pawn);

		protected abstract void SetValue(Pawn pawn, bool value);

		protected override void HeaderClicked(Rect headerRect, PawnTable table)
		{
			base.HeaderClicked(headerRect, table);
			if (Event.current.shift)
			{
				List<Pawn> pawnsListForReading = table.PawnsListForReading;
				for (int i = 0; i < pawnsListForReading.Count; i++)
				{
					if (this.HasCheckbox(pawnsListForReading[i]))
					{
						if (Event.current.button == 0)
						{
							if (!this.GetValue(pawnsListForReading[i]))
							{
								this.SetValue(pawnsListForReading[i], true);
							}
						}
						else if (Event.current.button == 1 && this.GetValue(pawnsListForReading[i]))
						{
							this.SetValue(pawnsListForReading[i], false);
						}
					}
				}
				if (Event.current.button == 0)
				{
					SoundDefOf.CheckboxTurnedOn.PlayOneShotOnCamera(null);
				}
				else if (Event.current.button == 1)
				{
					SoundDefOf.CheckboxTurnedOff.PlayOneShotOnCamera(null);
				}
			}
		}

		protected override string GetHeaderTip(PawnTable table)
		{
			return base.GetHeaderTip(table) + "\n" + "CheckboxShiftClickTip".Translate();
		}
	}
}
