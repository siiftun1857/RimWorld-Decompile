using System;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public abstract class Lesson_Instruction : Lesson
	{
		public InstructionDef def;

		private const float RectWidth = 310f;

		private const float BarHeight = 30f;

		protected Map Map
		{
			get
			{
				return Find.AnyPlayerHomeMap;
			}
		}

		protected virtual float ProgressPercent
		{
			get
			{
				return -1f;
			}
		}

		protected virtual bool ShowProgressBar
		{
			get
			{
				return this.ProgressPercent >= 0.0;
			}
		}

		public override string DefaultRejectInputMessage
		{
			get
			{
				return this.def.rejectInputMessage;
			}
		}

		public override void ExposeData()
		{
			Scribe_Defs.Look<InstructionDef>(ref this.def, "def");
			base.ExposeData();
		}

		public override void OnActivated()
		{
			base.OnActivated();
			if (this.def.giveOnActivateCount > 0)
			{
				Thing thing = ThingMaker.MakeThing(this.def.giveOnActivateDef, null);
				thing.stackCount = this.def.giveOnActivateCount;
				GenSpawn.Spawn(thing, TutorUtility.FindUsableRect(2, 2, this.Map, 0f, false).CenterCell, this.Map);
			}
			if (this.def.resetBuildDesignatorStuffs)
			{
				foreach (DesignationCategoryDef allDef in DefDatabase<DesignationCategoryDef>.AllDefs)
				{
					foreach (Designator resolvedAllowedDesignator in allDef.ResolvedAllowedDesignators)
					{
						Designator_Build designator_Build = resolvedAllowedDesignator as Designator_Build;
						if (designator_Build != null)
						{
							designator_Build.ResetStuffToDefault();
						}
					}
				}
			}
		}

		public override void LessonOnGUI()
		{
			Text.Font = GameFont.Small;
			string textAdj = this.def.text.AdjustedForKeys();
			float num = Text.CalcHeight(textAdj, 290f);
			float num2 = (float)(num + 20.0);
			if (this.ShowProgressBar)
			{
				num2 = (float)(num2 + 47.0);
			}
			Vector2 b = new Vector2((float)((float)UI.screenWidth - 17.0 - 155.0), (float)(17.0 + num2 / 2.0));
			if (!Find.TutorialState.introDone)
			{
				float screenOverlayAlpha = 0f;
				if (this.def.startCentered)
				{
					Vector2 vector = new Vector2((float)(UI.screenWidth / 2), (float)(UI.screenHeight / 2));
					if (base.AgeSeconds < 4.0)
					{
						b = vector;
						screenOverlayAlpha = 0.9f;
					}
					else if (base.AgeSeconds < 5.0)
					{
						float t = (float)((base.AgeSeconds - 4.0) / 1.0);
						b = Vector2.Lerp(vector, b, t);
						screenOverlayAlpha = Mathf.Lerp(0.9f, 0f, t);
					}
				}
				if (screenOverlayAlpha > 0.0)
				{
					Rect fullScreenRect = new Rect(0f, 0f, (float)UI.screenWidth, (float)UI.screenHeight);
					Find.WindowStack.ImmediateWindow(972651, fullScreenRect, WindowLayer.SubSuper, (Action)delegate
					{
						GUI.color = new Color(1f, 1f, 1f, screenOverlayAlpha);
						GUI.DrawTexture(fullScreenRect, BaseContent.BlackTex);
						GUI.color = Color.white;
					}, false, true, 0f);
				}
				else
				{
					Find.TutorialState.introDone = true;
				}
			}
			Rect mainRect = new Rect((float)(b.x - 155.0), (float)(b.y - num2 / 2.0 - 10.0), 310f, num2);
			Find.WindowStack.ImmediateWindow(177706, mainRect, WindowLayer.Super, (Action)delegate
			{
				Rect rect = mainRect.AtZero();
				Widgets.DrawWindowBackgroundTutor(rect);
				Rect rect2 = rect.ContractedBy(10f);
				Text.Font = GameFont.Small;
				Rect rect3 = rect2;
				if (this.ShowProgressBar)
				{
					rect3.height -= 47f;
				}
				Widgets.Label(rect3, textAdj);
				if (this.ShowProgressBar)
				{
					Rect rect4 = new Rect(rect2.x, (float)(rect2.yMax - 30.0), rect2.width, 30f);
					Widgets.FillableBar(rect4, this.ProgressPercent, LearningReadout.ProgressBarFillTex);
				}
				if (base.AgeSeconds < 0.5)
				{
					GUI.color = new Color(1f, 1f, 1f, Mathf.Lerp(1f, 0f, (float)(base.AgeSeconds / 0.5)));
					GUI.DrawTexture(rect, BaseContent.WhiteTex);
					GUI.color = Color.white;
				}
			}, false, false, 1f);
			if (this.def.highlightTags != null)
			{
				for (int i = 0; i < this.def.highlightTags.Count; i++)
				{
					UIHighlighter.HighlightTag(this.def.highlightTags[i]);
				}
			}
		}

		public override void Notify_Event(EventPack ep)
		{
			if (this.def.eventTagsEnd != null && this.def.eventTagsEnd.Contains(ep.Tag))
			{
				Find.ActiveLesson.Deactivate();
			}
		}

		public override AcceptanceReport AllowAction(EventPack ep)
		{
			return this.def.actionTagsAllowed != null && this.def.actionTagsAllowed.Contains(ep.Tag);
		}

		public override void PostDeactivated()
		{
			SoundDefOf.CommsWindow_Close.PlayOneShotOnCamera(null);
			TutorSystem.Notify_Event("InstructionDeactivated-" + this.def.defName);
			if (this.def.endTutorial)
			{
				Find.ActiveLesson.Deactivate();
				Find.TutorialState.Notify_TutorialEnding();
				LessonAutoActivator.Notify_TutorialEnding();
			}
		}
	}
}
