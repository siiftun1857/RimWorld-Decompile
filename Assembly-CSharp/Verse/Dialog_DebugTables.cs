using System.Collections.Generic;
using UnityEngine;

namespace Verse
{
	public class Dialog_DebugTables : Window
	{
		private string[,] table;

		private Vector2 scrollPosition = Vector2.zero;

		private List<float> colWidths = new List<float>();

		private float totalWidth = 0f;

		private const float RowHeight = 23f;

		private const float ColExtraWidth = 8f;

		public override Vector2 InitialSize
		{
			get
			{
				return new Vector2((float)UI.screenWidth, (float)UI.screenHeight);
			}
		}

		public Dialog_DebugTables(string[,] tables)
		{
			this.table = tables;
			base.doCloseButton = true;
			base.doCloseX = true;
			Text.Font = GameFont.Tiny;
			for (int i = 0; i < this.table.GetLength(0); i++)
			{
				float num = 0f;
				for (int j = 0; j < this.table.GetLength(1); j++)
				{
					string text = this.table[i, j];
					Vector2 vector = Text.CalcSize(text);
					float x = vector.x;
					if (x > num)
					{
						num = x;
					}
				}
				float num2 = (float)(num + 8.0);
				this.colWidths.Add(num2);
				this.totalWidth += num2;
			}
		}

		public override void DoWindowContents(Rect inRect)
		{
			Text.Font = GameFont.Tiny;
			inRect.yMax -= 40f;
			Rect viewRect = new Rect(0f, 0f, this.totalWidth, (float)((float)this.table.GetLength(1) * 23.0));
			Widgets.BeginScrollView(inRect, ref this.scrollPosition, viewRect, true);
			float num = 0f;
			for (int i = 0; i < this.table.GetLength(0); i++)
			{
				for (int j = 0; j < this.table.GetLength(1); j++)
				{
					if (!((float)(j + 1) * 23.0 - this.scrollPosition.y < 0.0) && !((float)j * 23.0 - this.scrollPosition.y > inRect.height))
					{
						Rect rect;
						Rect rect2 = rect = new Rect(num, (float)((float)j * 23.0), this.colWidths[i], 23f);
						rect.xMin -= 999f;
						rect.xMax += 999f;
						if (Mouse.IsOver(rect) || i % 2 == 0)
						{
							Widgets.DrawHighlight(rect2);
						}
						Widgets.Label(rect2, this.table[i, j]);
					}
				}
				num += this.colWidths[i];
			}
			Widgets.EndScrollView();
		}
	}
}
