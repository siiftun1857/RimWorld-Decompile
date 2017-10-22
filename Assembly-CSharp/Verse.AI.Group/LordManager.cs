using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Verse.AI.Group
{
	public sealed class LordManager : IExposable
	{
		public Map map;

		public List<Lord> lords = new List<Lord>();

		public LordManager(Map map)
		{
			this.map = map;
		}

		public void ExposeData()
		{
			Scribe_Collections.Look<Lord>(ref this.lords, "lords", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.LoadingVars)
			{
				for (int i = 0; i < this.lords.Count; i++)
				{
					this.lords[i].lordManager = this;
				}
			}
		}

		public void LordManagerTick()
		{
			for (int i = 0; i < this.lords.Count; i++)
			{
				this.lords[i].LordTick();
			}
			for (int num = this.lords.Count - 1; num >= 0; num--)
			{
				LordToil curLordToil = this.lords[num].CurLordToil;
				if (curLordToil.ShouldFail)
				{
					this.RemoveLord(this.lords[num]);
				}
			}
		}

		public void LordManagerUpdate()
		{
			if (DebugViewSettings.drawLords)
			{
				for (int i = 0; i < this.lords.Count; i++)
				{
					this.lords[i].DebugDraw();
				}
			}
		}

		public void LordManagerOnGUI()
		{
			if (DebugViewSettings.drawLords)
			{
				for (int i = 0; i < this.lords.Count; i++)
				{
					this.lords[i].DebugOnGUI();
				}
			}
			if (DebugViewSettings.drawDuties)
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				Text.Font = GameFont.Tiny;
				foreach (Pawn allPawn in this.map.mapPawns.AllPawns)
				{
					if (allPawn.Spawned)
					{
						string text = "";
						if (!allPawn.Dead && allPawn.mindState.duty != null)
						{
							text = allPawn.mindState.duty.ToString();
						}
						if (allPawn.InMentalState)
						{
							text = text + "\nMentalState=" + allPawn.MentalState.ToString();
						}
						Vector2 vector = allPawn.DrawPos.MapToUIPosition();
						Widgets.Label(new Rect((float)(vector.x - 100.0), (float)(vector.y - 100.0), 200f, 200f), text);
					}
				}
				Text.Anchor = TextAnchor.UpperLeft;
			}
		}

		public void AddLord(Lord newLord)
		{
			this.lords.Add(newLord);
			newLord.lordManager = this;
		}

		public void RemoveLord(Lord oldLord)
		{
			this.lords.Remove(oldLord);
			oldLord.Cleanup();
		}

		public Lord LordOf(Pawn p)
		{
			int num = 0;
			Lord result;
			while (true)
			{
				Lord lord;
				if (num < this.lords.Count)
				{
					lord = this.lords[num];
					for (int i = 0; i < lord.ownedPawns.Count; i++)
					{
						if (lord.ownedPawns[i] == p)
							goto IL_0030;
					}
					num++;
					continue;
				}
				result = null;
				break;
				IL_0030:
				result = lord;
				break;
			}
			return result;
		}

		public void LogLords()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("======= Lords =======");
			stringBuilder.AppendLine("Count: " + this.lords.Count);
			for (int i = 0; i < this.lords.Count; i++)
			{
				Lord lord = this.lords[i];
				stringBuilder.AppendLine();
				stringBuilder.Append("#" + (i + 1) + ": ");
				if (lord.LordJob == null)
				{
					stringBuilder.AppendLine("no-job");
				}
				else
				{
					stringBuilder.AppendLine(lord.LordJob.GetType().Name);
				}
				stringBuilder.Append("Current toil: ");
				if (lord.CurLordToil == null)
				{
					stringBuilder.AppendLine("null");
				}
				else
				{
					stringBuilder.AppendLine(lord.CurLordToil.GetType().Name);
				}
				stringBuilder.AppendLine("Members (count: " + lord.ownedPawns.Count + "):");
				for (int j = 0; j < lord.ownedPawns.Count; j++)
				{
					stringBuilder.AppendLine("  " + lord.ownedPawns[j].LabelShort + " (" + lord.ownedPawns[j].Faction + ")");
				}
			}
			Log.Message(stringBuilder.ToString());
		}
	}
}
