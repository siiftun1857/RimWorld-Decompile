using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public abstract class ScenPart_PawnModifier : ScenPart
	{
		protected float chance = 1f;

		protected PawnGenerationContext context;

		private bool hideOffMap = false;

		private string chanceBuf;

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<float>(ref this.chance, "chance", 0f, false);
			Scribe_Values.Look<PawnGenerationContext>(ref this.context, "context", PawnGenerationContext.All, false);
			Scribe_Values.Look<bool>(ref this.hideOffMap, "hideOffMap", false, false);
		}

		protected void DoPawnModifierEditInterface(Rect rect)
		{
			Rect rect2 = rect.TopHalf();
			Rect rect3 = rect2.LeftPart(0.333f).Rounded();
			Rect rect4 = rect2.RightPart(0.666f).Rounded();
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rect3, "chance".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
			Widgets.TextFieldPercent(rect4, ref this.chance, ref this.chanceBuf, 0f, 1f);
			Rect rect5 = rect.BottomHalf();
			Rect rect6 = rect5.LeftPart(0.333f).Rounded();
			Rect rect7 = rect5.RightPart(0.666f).Rounded();
			Text.Anchor = TextAnchor.MiddleRight;
			Widgets.Label(rect6, "context".Translate());
			Text.Anchor = TextAnchor.UpperLeft;
			if (Widgets.ButtonText(rect7, this.context.ToStringHuman(), true, false, true))
			{
				List<FloatMenuOption> list = new List<FloatMenuOption>();
				IEnumerator enumerator = Enum.GetValues(typeof(PawnGenerationContext)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						PawnGenerationContext pawnGenerationContext = (PawnGenerationContext)enumerator.Current;
						PawnGenerationContext localCont = pawnGenerationContext;
						list.Add(new FloatMenuOption(localCont.ToStringHuman(), (Action)delegate
						{
							this.context = localCont;
						}, MenuOptionPriority.Default, null, null, 0f, null, null));
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				Find.WindowStack.Add(new FloatMenu(list));
			}
		}

		public override void Randomize()
		{
			this.chance = GenMath.RoundedHundredth(Rand.Range(0.05f, 1f));
			this.context = PawnGenerationContextUtility.GetRandom();
			this.hideOffMap = false;
		}

		public override void Notify_PawnGenerated(Pawn pawn, PawnGenerationContext context)
		{
			if (this.hideOffMap && PawnGenerationContext.PlayerStarter.Includes(context))
				return;
			if (pawn.RaceProps.Humanlike && this.context.Includes(context))
			{
				this.ModifyPawn(pawn);
			}
		}

		public override void PostMapGenerate(Map map)
		{
			if (Find.GameInitData != null && this.hideOffMap)
			{
				if (((this.context != PawnGenerationContext.PlayerStarter) ? this.context : PawnGenerationContext.All) != 0)
					return;
				foreach (Pawn startingPawn in Find.GameInitData.startingPawns)
				{
					if (startingPawn.RaceProps.Humanlike)
					{
						this.ModifyPawn(startingPawn);
					}
				}
			}
		}

		protected abstract void ModifyPawn(Pawn p);
	}
}
