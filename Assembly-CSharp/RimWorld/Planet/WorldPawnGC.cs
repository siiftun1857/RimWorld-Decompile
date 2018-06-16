﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x0200062D RID: 1581
	public class WorldPawnGC : IExposable
	{
		// Token: 0x06002064 RID: 8292 RVA: 0x00114D6C File Offset: 0x0011316C
		public void WorldPawnGCTick()
		{
			if (this.lastSuccessfulGCTick < Find.TickManager.TicksGame / 15000 * 15000)
			{
				if (this.activeGCProcess == null)
				{
					this.activeGCProcess = this.PawnGCPass().GetEnumerator();
					if (DebugViewSettings.logWorldPawnGC)
					{
						Log.Message(string.Format("World pawn GC started at rate {0}", this.currentGCRate), false);
					}
				}
				if (this.activeGCProcess != null)
				{
					bool flag = false;
					int num = 0;
					while (num < this.currentGCRate && !flag)
					{
						flag = !this.activeGCProcess.MoveNext();
						num++;
					}
					if (flag)
					{
						this.lastSuccessfulGCTick = Find.TickManager.TicksGame;
						this.currentGCRate = 1;
						this.activeGCProcess = null;
						if (DebugViewSettings.logWorldPawnGC)
						{
							Log.Message("World pawn GC complete", false);
						}
					}
				}
			}
		}

		// Token: 0x06002065 RID: 8293 RVA: 0x00114E58 File Offset: 0x00113258
		public void CancelGCPass()
		{
			if (this.activeGCProcess != null)
			{
				this.activeGCProcess = null;
				this.currentGCRate = Mathf.Min(this.currentGCRate * 2, 16777216);
				if (DebugViewSettings.logWorldPawnGC)
				{
					Log.Message("World pawn GC cancelled", false);
				}
			}
		}

		// Token: 0x06002066 RID: 8294 RVA: 0x00114EA8 File Offset: 0x001132A8
		private IEnumerable AccumulatePawnGCData(Dictionary<Pawn, string> keptPawns)
		{
			foreach (Pawn pawn2 in PawnsFinder.AllMapsWorldAndTemporary_AliveOrDead)
			{
				string criticalPawnReason = this.GetCriticalPawnReason(pawn2);
				if (!criticalPawnReason.NullOrEmpty())
				{
					keptPawns[pawn2] = criticalPawnReason;
					if (this.logDotgraph != null)
					{
						this.logDotgraph.AppendLine(string.Format("{0} [label=<{0}<br/><font point-size=\"10\">{1}</font>> color=\"{2}\" shape=\"{3}\"];", new object[]
						{
							WorldPawnGC.DotgraphIdentifier(pawn2),
							criticalPawnReason,
							(pawn2.relations == null || !pawn2.relations.everSeenByPlayer) ? "grey" : "black",
							(!pawn2.RaceProps.Humanlike) ? "box" : "oval"
						}));
					}
				}
				else if (this.logDotgraph != null)
				{
					this.logDotgraph.AppendLine(string.Format("{0} [color=\"{1}\" shape=\"{2}\"];", WorldPawnGC.DotgraphIdentifier(pawn2), (pawn2.relations == null || !pawn2.relations.everSeenByPlayer) ? "grey" : "black", (!pawn2.RaceProps.Humanlike) ? "box" : "oval"));
				}
			}
			foreach (Pawn key in (from pawn in PawnsFinder.AllMapsWorldAndTemporary_Alive
			where this.AllowedAsStoryPawn(pawn) && !keptPawns.ContainsKey(pawn)
			orderby pawn.records.StoryRelevance descending
			select pawn).Take(20))
			{
				keptPawns[key] = "StoryRelevant";
			}
			Pawn[] criticalPawns = keptPawns.Keys.ToArray<Pawn>();
			foreach (Pawn pawn4 in criticalPawns)
			{
				this.AddAllRelationships(pawn4, keptPawns);
				yield return null;
			}
			foreach (Pawn pawn3 in criticalPawns)
			{
				this.AddAllMemories(pawn3, keptPawns);
			}
			yield break;
		}

		// Token: 0x06002067 RID: 8295 RVA: 0x00114EDC File Offset: 0x001132DC
		private Dictionary<Pawn, string> AccumulatePawnGCDataImmediate()
		{
			Dictionary<Pawn, string> dictionary = new Dictionary<Pawn, string>();
			this.AccumulatePawnGCData(dictionary).ExecuteEnumerable();
			return dictionary;
		}

		// Token: 0x06002068 RID: 8296 RVA: 0x00114F04 File Offset: 0x00113304
		public string PawnGCDebugResults()
		{
			Dictionary<Pawn, string> dictionary = this.AccumulatePawnGCDataImmediate();
			Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
			foreach (Pawn key in Find.WorldPawns.AllPawnsAlive)
			{
				string text = "Discarded";
				if (dictionary.ContainsKey(key))
				{
					text = dictionary[key];
				}
				if (!dictionary2.ContainsKey(text))
				{
					dictionary2[text] = 0;
				}
				Dictionary<string, int> dictionary3;
				string key2;
				(dictionary3 = dictionary2)[key2 = text] = dictionary3[key2] + 1;
			}
			return GenText.ToTextList(from kvp in dictionary2
			orderby kvp.Value descending
			select string.Format("{0}: {1}", kvp.Value, kvp.Key), "\n");
		}

		// Token: 0x06002069 RID: 8297 RVA: 0x00115010 File Offset: 0x00113410
		public IEnumerable PawnGCPass()
		{
			Dictionary<Pawn, string> keptPawns = new Dictionary<Pawn, string>();
			Pawn[] worldPawnsSnapshot = Find.WorldPawns.AllPawnsAliveOrDead.ToArray<Pawn>();
			IEnumerator enumerator = this.AccumulatePawnGCData(keptPawns).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object _ = enumerator.Current;
					yield return null;
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
			foreach (Pawn pawn in worldPawnsSnapshot)
			{
				if (pawn.IsWorldPawn() && !keptPawns.ContainsKey(pawn))
				{
					Find.WorldPawns.RemoveAndDiscardPawnViaGC(pawn);
				}
			}
			yield break;
		}

		// Token: 0x0600206A RID: 8298 RVA: 0x0011503C File Offset: 0x0011343C
		private string GetCriticalPawnReason(Pawn pawn)
		{
			string result;
			if (pawn.Discarded)
			{
				result = null;
			}
			else if (PawnUtility.EverBeenColonistOrTameAnimal(pawn) && pawn.RaceProps.Humanlike)
			{
				result = "Colonist";
			}
			else if (PawnGenerator.IsBeingGenerated(pawn))
			{
				result = "Generating";
			}
			else if (PawnUtility.IsFactionLeader(pawn))
			{
				result = "FactionLeader";
			}
			else if (PawnUtility.IsKidnappedPawn(pawn))
			{
				result = "Kidnapped";
			}
			else if (pawn.IsCaravanMember())
			{
				result = "CaravanMember";
			}
			else if (PawnUtility.IsTravelingInTransportPodWorldObject(pawn))
			{
				result = "TransportPod";
			}
			else if (PawnUtility.ForSaleBySettlement(pawn))
			{
				result = "ForSale";
			}
			else if (Find.WorldPawns.ForcefullyKeptPawns.Contains(pawn))
			{
				result = "ForceKept";
			}
			else if (pawn.SpawnedOrAnyParentSpawned)
			{
				result = "Spawned";
			}
			else if (!pawn.Corpse.DestroyedOrNull())
			{
				result = "CorpseExists";
			}
			else
			{
				if (pawn.RaceProps.Humanlike && Current.ProgramState == ProgramState.Playing)
				{
					if (Find.PlayLog.AnyEntryConcerns(pawn))
					{
						return "InPlayLog";
					}
					if (Find.BattleLog.AnyEntryConcerns(pawn))
					{
						return "InBattleLog";
					}
				}
				if (Current.ProgramState == ProgramState.Playing && Find.TaleManager.AnyActiveTaleConcerns(pawn))
				{
					result = "InActiveTale";
				}
				else
				{
					result = null;
				}
			}
			return result;
		}

		// Token: 0x0600206B RID: 8299 RVA: 0x001151D8 File Offset: 0x001135D8
		private bool AllowedAsStoryPawn(Pawn pawn)
		{
			return pawn.RaceProps.Humanlike;
		}

		// Token: 0x0600206C RID: 8300 RVA: 0x00115208 File Offset: 0x00113608
		public void AddAllRelationships(Pawn pawn, Dictionary<Pawn, string> keptPawns)
		{
			if (pawn.relations != null)
			{
				foreach (Pawn pawn2 in pawn.relations.RelatedPawns)
				{
					if (this.logDotgraph != null)
					{
						string text = string.Format("{0}->{1} [label=<{2}> color=\"purple\"];", WorldPawnGC.DotgraphIdentifier(pawn), WorldPawnGC.DotgraphIdentifier(pawn2), pawn.GetRelations(pawn2).FirstOrDefault<PawnRelationDef>().ToString());
						if (!this.logDotgraphUniqueLinks.Contains(text))
						{
							this.logDotgraphUniqueLinks.Add(text);
							this.logDotgraph.AppendLine(text);
						}
					}
					if (!keptPawns.ContainsKey(pawn2))
					{
						keptPawns[pawn2] = "Relationship";
					}
				}
			}
		}

		// Token: 0x0600206D RID: 8301 RVA: 0x001152F4 File Offset: 0x001136F4
		public void AddAllMemories(Pawn pawn, Dictionary<Pawn, string> keptPawns)
		{
			if (pawn.needs != null && pawn.needs.mood != null && pawn.needs.mood.thoughts != null && pawn.needs.mood.thoughts.memories != null)
			{
				foreach (Thought_Memory thought_Memory in pawn.needs.mood.thoughts.memories.Memories)
				{
					if (thought_Memory.otherPawn != null)
					{
						if (this.logDotgraph != null)
						{
							string text = string.Format("{0}->{1} [label=<{2}> color=\"orange\"];", WorldPawnGC.DotgraphIdentifier(pawn), WorldPawnGC.DotgraphIdentifier(thought_Memory.otherPawn), thought_Memory.def);
							if (!this.logDotgraphUniqueLinks.Contains(text))
							{
								this.logDotgraphUniqueLinks.Add(text);
								this.logDotgraph.AppendLine(text);
							}
						}
						if (!keptPawns.ContainsKey(thought_Memory.otherPawn))
						{
							keptPawns[thought_Memory.otherPawn] = "Memory";
						}
					}
				}
			}
		}

		// Token: 0x0600206E RID: 8302 RVA: 0x00115444 File Offset: 0x00113844
		public void ExposeData()
		{
			Scribe_Values.Look<int>(ref this.lastSuccessfulGCTick, "lastSuccessfulGCTick", 0, false);
			Scribe_Values.Look<int>(ref this.currentGCRate, "nextGCRate", 1, false);
		}

		// Token: 0x0600206F RID: 8303 RVA: 0x0011546C File Offset: 0x0011386C
		public void LogGC()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendLine("======= GC =======");
			stringBuilder.AppendLine(this.PawnGCDebugResults());
			Log.Message(stringBuilder.ToString(), false);
		}

		// Token: 0x06002070 RID: 8304 RVA: 0x001154A8 File Offset: 0x001138A8
		public void RunGC()
		{
			this.CancelGCPass();
			PerfLogger.Reset();
			IEnumerator enumerator = this.PawnGCPass().GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					object obj = enumerator.Current;
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
			float num = PerfLogger.Duration() * 1000f;
			PerfLogger.Flush();
			Log.Message(string.Format("World pawn GC run complete in {0} ms", num), false);
		}

		// Token: 0x06002071 RID: 8305 RVA: 0x00115538 File Offset: 0x00113938
		public void LogDotgraph()
		{
			this.logDotgraph = new StringBuilder();
			this.logDotgraphUniqueLinks = new HashSet<string>();
			this.logDotgraph.AppendLine("digraph { rankdir=LR;");
			this.AccumulatePawnGCDataImmediate();
			this.logDotgraph.AppendLine("}");
			GUIUtility.systemCopyBuffer = this.logDotgraph.ToString();
			Log.Message("Dotgraph copied to clipboard", false);
			this.logDotgraph = null;
			this.logDotgraphUniqueLinks = null;
		}

		// Token: 0x06002072 RID: 8306 RVA: 0x001155B0 File Offset: 0x001139B0
		public static string DotgraphIdentifier(Pawn pawn)
		{
			return new string((from ch in pawn.LabelShort
			where char.IsLetter(ch)
			select ch).ToArray<char>()) + "_" + pawn.thingIDNumber.ToString();
		}

		// Token: 0x0400128F RID: 4751
		private int lastSuccessfulGCTick = 0;

		// Token: 0x04001290 RID: 4752
		private int currentGCRate = 1;

		// Token: 0x04001291 RID: 4753
		private const float PctOfHumanlikesAlwaysKept = 0.1f;

		// Token: 0x04001292 RID: 4754
		private const float PctOfUnnamedColonyAnimalsAlwaysKept = 0.05f;

		// Token: 0x04001293 RID: 4755
		private const int AdditionalStoryRelevantPawns = 20;

		// Token: 0x04001294 RID: 4756
		private const int GCUpdateInterval = 15000;

		// Token: 0x04001295 RID: 4757
		private IEnumerator activeGCProcess = null;

		// Token: 0x04001296 RID: 4758
		private StringBuilder logDotgraph = null;

		// Token: 0x04001297 RID: 4759
		private HashSet<string> logDotgraphUniqueLinks = null;
	}
}