﻿using System;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000EB5 RID: 3765
	public class Dialog_NamePawn : Window
	{
		// Token: 0x060058FB RID: 22779 RVA: 0x002D9BEC File Offset: 0x002D7FEC
		public Dialog_NamePawn(Pawn pawn)
		{
			this.pawn = pawn;
			this.curName = pawn.Name.ToStringShort;
			if (pawn.story != null)
			{
				if (pawn.story.title != null)
				{
					this.curTitle = pawn.story.title;
				}
				else
				{
					this.curTitle = "";
				}
			}
			this.forcePause = true;
			this.absorbInputAroundWindow = true;
			this.closeOnClickedOutside = true;
			this.closeOnAccept = false;
		}

		// Token: 0x17000DFB RID: 3579
		// (get) Token: 0x060058FC RID: 22780 RVA: 0x002D9C74 File Offset: 0x002D8074
		private Name CurPawnName
		{
			get
			{
				NameTriple nameTriple = this.pawn.Name as NameTriple;
				Name result;
				if (nameTriple != null)
				{
					result = new NameTriple(nameTriple.First, this.curName, nameTriple.Last);
				}
				else
				{
					NameSingle nameSingle = this.pawn.Name as NameSingle;
					if (nameSingle == null)
					{
						throw new InvalidOperationException();
					}
					result = new NameSingle(this.curName, false);
				}
				return result;
			}
		}

		// Token: 0x17000DFC RID: 3580
		// (get) Token: 0x060058FD RID: 22781 RVA: 0x002D9CEC File Offset: 0x002D80EC
		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2(500f, 175f);
			}
		}

		// Token: 0x060058FE RID: 22782 RVA: 0x002D9D10 File Offset: 0x002D8110
		public override void DoWindowContents(Rect inRect)
		{
			bool flag = false;
			if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
			{
				flag = true;
				Event.current.Use();
			}
			Text.Font = GameFont.Medium;
			string text = this.CurPawnName.ToString().Replace(" '' ", " ");
			if (this.curTitle == "")
			{
				text = text + ", " + this.pawn.story.TitleDefaultCap;
			}
			else if (this.curTitle != null)
			{
				text = text + ", " + this.curTitle.CapitalizeFirst();
			}
			Widgets.Label(new Rect(15f, 15f, 500f, 50f), text);
			Text.Font = GameFont.Small;
			string text2 = Widgets.TextField(new Rect(15f, 50f, inRect.width / 2f - 20f, 35f), this.curName);
			if (text2.Length < 16)
			{
				this.curName = text2;
			}
			if (this.curTitle != null)
			{
				string text3 = Widgets.TextField(new Rect(inRect.width / 2f, 50f, inRect.width / 2f - 20f, 35f), this.curTitle);
				if (text3.Length < 25)
				{
					this.curTitle = text3;
				}
			}
			if (Widgets.ButtonText(new Rect(inRect.width / 2f + 20f, inRect.height - 35f, inRect.width / 2f - 20f, 35f), "OK", true, false, true) || flag)
			{
				if (string.IsNullOrEmpty(this.curName))
				{
					this.curName = ((NameTriple)this.pawn.Name).First;
				}
				this.pawn.Name = this.CurPawnName;
				if (this.pawn.story != null)
				{
					this.pawn.story.Title = this.curTitle;
				}
				Find.WindowStack.TryRemove(this, true);
				string text4 = (!this.pawn.def.race.Animal) ? "PawnGainsName".Translate(new object[]
				{
					this.curName,
					this.pawn.story.Title
				}).AdjustedFor(this.pawn) : "AnimalGainsName".Translate(new object[]
				{
					this.curName
				});
				Messages.Message(text4, this.pawn, MessageTypeDefOf.PositiveEvent, false);
			}
		}

		// Token: 0x04003B55 RID: 15189
		private Pawn pawn;

		// Token: 0x04003B56 RID: 15190
		private string curName;

		// Token: 0x04003B57 RID: 15191
		private string curTitle;
	}
}