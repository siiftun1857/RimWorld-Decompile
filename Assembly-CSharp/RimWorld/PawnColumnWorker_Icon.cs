using UnityEngine;
using Verse;

namespace RimWorld
{
	public abstract class PawnColumnWorker_Icon : PawnColumnWorker
	{
		protected virtual int Width
		{
			get
			{
				return 26;
			}
		}

		public override void DoCell(Rect rect, Pawn pawn, PawnTable table)
		{
			Texture2D iconFor = this.GetIconFor(pawn);
			if ((Object)iconFor != (Object)null)
			{
				Vector2 iconSize = this.GetIconSize(pawn);
				int num = (int)((rect.width - iconSize.x) / 2.0);
				int num2 = Mathf.Max((int)((30.0 - iconSize.y) / 2.0), 0);
				Rect rect2 = new Rect(rect.x + (float)num, rect.y + (float)num2, iconSize.x, iconSize.y);
				GUI.color = this.GetIconColor(pawn);
				GUI.DrawTexture(rect2, iconFor);
				GUI.color = Color.white;
				if (Mouse.IsOver(rect2))
				{
					string iconTip = this.GetIconTip(pawn);
					if (!iconTip.NullOrEmpty())
					{
						TooltipHandler.TipRegion(rect2, iconTip);
					}
				}
				if (Widgets.ButtonInvisible(rect2, false))
				{
					this.ClickedIcon(pawn);
				}
			}
		}

		public override int GetMinWidth(PawnTable table)
		{
			return Mathf.Max(base.GetMinWidth(table), this.Width);
		}

		public override int GetMaxWidth(PawnTable table)
		{
			return Mathf.Min(base.GetMaxWidth(table), this.GetMinWidth(table));
		}

		public override int GetMinCellHeight(Pawn pawn)
		{
			int minCellHeight = base.GetMinCellHeight(pawn);
			Vector2 iconSize = this.GetIconSize(pawn);
			return Mathf.Max(minCellHeight, Mathf.CeilToInt(iconSize.y));
		}

		public override int Compare(Pawn a, Pawn b)
		{
			return this.GetValueToCompare(a).CompareTo(this.GetValueToCompare(b));
		}

		private int GetValueToCompare(Pawn pawn)
		{
			Texture2D iconFor = this.GetIconFor(pawn);
			return (!((Object)iconFor != (Object)null)) ? (-2147483648) : iconFor.GetInstanceID();
		}

		protected abstract Texture2D GetIconFor(Pawn pawn);

		protected virtual string GetIconTip(Pawn pawn)
		{
			return (string)null;
		}

		protected virtual Color GetIconColor(Pawn pawn)
		{
			return Color.white;
		}

		protected virtual void ClickedIcon(Pawn pawn)
		{
		}

		protected virtual Vector2 GetIconSize(Pawn pawn)
		{
			Texture2D iconFor = this.GetIconFor(pawn);
			return (!((Object)iconFor == (Object)null)) ? new Vector2((float)iconFor.width, (float)iconFor.height) : Vector2.zero;
		}
	}
}
