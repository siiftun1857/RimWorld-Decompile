﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld.Planet
{
	// Token: 0x020008E8 RID: 2280
	public class WITab_Caravan_Social : WITab
	{
		// Token: 0x06003461 RID: 13409 RVA: 0x001BFEFE File Offset: 0x001BE2FE
		public WITab_Caravan_Social()
		{
			this.labelKey = "TabCaravanSocial";
		}

		// Token: 0x17000863 RID: 2147
		// (get) Token: 0x06003462 RID: 13410 RVA: 0x001BFF14 File Offset: 0x001BE314
		private List<Pawn> Pawns
		{
			get
			{
				return base.SelCaravan.PawnsListForReading;
			}
		}

		// Token: 0x17000864 RID: 2148
		// (get) Token: 0x06003463 RID: 13411 RVA: 0x001BFF34 File Offset: 0x001BE334
		private float SpecificSocialTabWidth
		{
			get
			{
				float result;
				if (this.specificSocialTabForPawn == null)
				{
					result = 0f;
				}
				else
				{
					result = 540f;
				}
				return result;
			}
		}

		// Token: 0x06003464 RID: 13412 RVA: 0x001BFF64 File Offset: 0x001BE364
		protected override void FillTab()
		{
			Text.Font = GameFont.Small;
			Rect rect = new Rect(0f, 0f, this.size.x, this.size.y).ContractedBy(10f);
			Rect rect2 = new Rect(0f, 0f, rect.width - 16f, this.scrollViewHeight);
			float num = 0f;
			Widgets.BeginScrollView(rect, ref this.scrollPosition, rect2, true);
			this.DoRows(ref num, rect2, rect);
			if (Event.current.type == EventType.Layout)
			{
				this.scrollViewHeight = num + 30f;
			}
			Widgets.EndScrollView();
		}

		// Token: 0x06003465 RID: 13413 RVA: 0x001C000E File Offset: 0x001BE40E
		protected override void UpdateSize()
		{
			base.UpdateSize();
			this.size.x = 243f;
			this.size.y = Mathf.Min(550f, this.PaneTopY - 30f);
		}

		// Token: 0x06003466 RID: 13414 RVA: 0x001C0048 File Offset: 0x001BE448
		protected override void ExtraOnGUI()
		{
			base.ExtraOnGUI();
			Pawn localSpecificSocialTabForPawn = this.specificSocialTabForPawn;
			if (localSpecificSocialTabForPawn != null)
			{
				Rect tabRect = base.TabRect;
				float specificSocialTabWidth = this.SpecificSocialTabWidth;
				Rect rect = new Rect(tabRect.xMax - 1f, tabRect.yMin, specificSocialTabWidth, tabRect.height);
				Find.WindowStack.ImmediateWindow(1439870015, rect, WindowLayer.GameUI, delegate
				{
					if (!localSpecificSocialTabForPawn.DestroyedOrNull())
					{
						SocialCardUtility.DrawSocialCard(rect.AtZero(), localSpecificSocialTabForPawn);
						if (Widgets.CloseButtonFor(rect.AtZero()))
						{
							this.specificSocialTabForPawn = null;
							SoundDefOf.TabClose.PlayOneShotOnCamera(null);
						}
					}
				}, true, false, 1f);
			}
		}

		// Token: 0x06003467 RID: 13415 RVA: 0x001C00F0 File Offset: 0x001BE4F0
		public override void OnOpen()
		{
			base.OnOpen();
			if ((this.specificSocialTabForPawn == null || !this.Pawns.Contains(this.specificSocialTabForPawn)) && this.Pawns.Any<Pawn>())
			{
				this.specificSocialTabForPawn = this.Pawns[0];
			}
		}

		// Token: 0x06003468 RID: 13416 RVA: 0x001C0148 File Offset: 0x001BE548
		private void DoRows(ref float curY, Rect scrollViewRect, Rect scrollOutRect)
		{
			List<Pawn> pawns = this.Pawns;
			Pawn pawn = BestCaravanPawnUtility.FindBestNegotiator(base.SelCaravan);
			GUI.color = new Color(0.8f, 0.8f, 0.8f, 1f);
			Widgets.Label(new Rect(0f, curY, scrollViewRect.width, 24f), string.Format("{0}: {1}", "Negotiator".Translate(), (pawn == null) ? "NoneCapable".Translate() : pawn.LabelShort));
			curY += 24f;
			if (this.specificSocialTabForPawn != null && !pawns.Contains(this.specificSocialTabForPawn))
			{
				this.specificSocialTabForPawn = null;
			}
			bool flag = false;
			for (int i = 0; i < pawns.Count; i++)
			{
				Pawn pawn2 = pawns[i];
				if (pawn2.RaceProps.IsFlesh)
				{
					if (pawn2.IsColonist)
					{
						if (!flag)
						{
							Widgets.ListSeparator(ref curY, scrollViewRect.width, "CaravanColonists".Translate());
							flag = true;
						}
						this.DoRow(ref curY, scrollViewRect, scrollOutRect, pawn2);
					}
				}
			}
			bool flag2 = false;
			for (int j = 0; j < pawns.Count; j++)
			{
				Pawn pawn3 = pawns[j];
				if (pawn3.RaceProps.IsFlesh)
				{
					if (!pawn3.IsColonist)
					{
						if (!flag2)
						{
							Widgets.ListSeparator(ref curY, scrollViewRect.width, "CaravanPrisonersAndAnimals".Translate());
							flag2 = true;
						}
						this.DoRow(ref curY, scrollViewRect, scrollOutRect, pawn3);
					}
				}
			}
		}

		// Token: 0x06003469 RID: 13417 RVA: 0x001C02F4 File Offset: 0x001BE6F4
		private void DoRow(ref float curY, Rect viewRect, Rect scrollOutRect, Pawn p)
		{
			float num = this.scrollPosition.y - 50f;
			float num2 = this.scrollPosition.y + scrollOutRect.height;
			if (curY > num && curY < num2)
			{
				this.DoRow(new Rect(0f, curY, viewRect.width, 50f), p);
			}
			curY += 50f;
		}

		// Token: 0x0600346A RID: 13418 RVA: 0x001C0364 File Offset: 0x001BE764
		private void DoRow(Rect rect, Pawn p)
		{
			GUI.BeginGroup(rect);
			Rect rect2 = rect.AtZero();
			CaravanThingsTabUtility.DoAbandonButton(rect2, p, base.SelCaravan);
			rect2.width -= 24f;
			Widgets.InfoCardButton(rect2.width - 24f, (rect.height - 24f) / 2f, p);
			rect2.width -= 24f;
			CaravanThingsTabUtility.DoOpenSpecificTabButton(rect2, p, ref this.specificSocialTabForPawn);
			rect2.width -= 24f;
			if (Mouse.IsOver(rect2))
			{
				Widgets.DrawHighlight(rect2);
			}
			Rect rect3 = new Rect(4f, (rect.height - 27f) / 2f, 27f, 27f);
			Widgets.ThingIcon(rect3, p, 1f);
			Rect bgRect = new Rect(rect3.xMax + 4f, 16f, 100f, 18f);
			GenMapUI.DrawPawnLabel(p, bgRect, 1f, 100f, null, GameFont.Small, false, false);
			if (p.Downed)
			{
				GUI.color = new Color(1f, 0f, 0f, 0.5f);
				Widgets.DrawLineHorizontal(0f, rect.height / 2f, rect.width);
				GUI.color = Color.white;
			}
			GUI.EndGroup();
		}

		// Token: 0x04001C58 RID: 7256
		private Vector2 scrollPosition;

		// Token: 0x04001C59 RID: 7257
		private float scrollViewHeight;

		// Token: 0x04001C5A RID: 7258
		private Pawn specificSocialTabForPawn;

		// Token: 0x04001C5B RID: 7259
		private const float RowHeight = 50f;

		// Token: 0x04001C5C RID: 7260
		private const float PawnLabelHeight = 18f;

		// Token: 0x04001C5D RID: 7261
		private const float PawnLabelColumnWidth = 100f;

		// Token: 0x04001C5E RID: 7262
		private const float SpaceAroundIcon = 4f;
	}
}