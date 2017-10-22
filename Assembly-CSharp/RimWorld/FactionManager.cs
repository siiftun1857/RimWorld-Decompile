using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	public class FactionManager : IExposable
	{
		private List<Faction> allFactions = new List<Faction>();

		public List<Faction> AllFactionsListForReading
		{
			get
			{
				return this.allFactions;
			}
		}

		public IEnumerable<Faction> AllFactions
		{
			get
			{
				return this.allFactions;
			}
		}

		public IEnumerable<Faction> AllFactionsVisible
		{
			get
			{
				return from fa in this.allFactions
				where !fa.def.hidden
				select fa;
			}
		}

		public IEnumerable<Faction> AllFactionsInViewOrder
		{
			get
			{
				return (from x in this.AllFactionsVisible
				orderby x.defeated
				select x).ThenByDescending((Func<Faction, float>)((Faction fa) => fa.def.startingGoodwill.Average));
			}
		}

		public void ExposeData()
		{
			Scribe_Collections.Look<Faction>(ref this.allFactions, "allFactions", LookMode.Deep, new object[0]);
		}

		public void Add(Faction faction)
		{
			this.allFactions.Add(faction);
		}

		public void FactionManagerTick()
		{
			for (int i = 0; i < this.allFactions.Count; i++)
			{
				this.allFactions[i].FactionTick();
			}
		}

		public void FactionsDebugDrawOnMap()
		{
			if (DebugViewSettings.drawFactions)
			{
				for (int i = 0; i < this.allFactions.Count; i++)
				{
					this.allFactions[i].DebugDrawOnMap();
				}
			}
		}

		public Faction FirstFactionOfDef(FactionDef facDef)
		{
			int num = 0;
			Faction result;
			while (true)
			{
				if (num < this.allFactions.Count)
				{
					if (this.allFactions[num].def == facDef)
					{
						result = this.allFactions[num];
						break;
					}
					num++;
					continue;
				}
				result = null;
				break;
			}
			return result;
		}

		public bool TryGetRandomNonColonyHumanlikeFaction(out Faction faction, bool tryMedievalOrBetter, bool allowDefeated = false, TechLevel minTechLevel = TechLevel.Undefined)
		{
			IEnumerable<Faction> source = from x in this.AllFactions
			where !x.IsPlayer && !x.def.hidden && x.def.humanlikeFaction && (allowDefeated || !x.defeated) && (minTechLevel == TechLevel.Undefined || (int)x.def.techLevel >= (int)minTechLevel)
			select x;
			return source.TryRandomElementByWeight<Faction>((Func<Faction, float>)((Faction x) => (float)((!tryMedievalOrBetter || (int)x.def.techLevel >= 3) ? 1.0 : 0.10000000149011612)), out faction);
		}

		public Faction RandomEnemyFaction(bool allowHidden = false, bool allowDefeated = false, bool allowNonHumanlike = true, TechLevel minTechLevel = TechLevel.Undefined)
		{
			Faction faction = default(Faction);
			return (!(from x in this.AllFactions
			where !x.IsPlayer && (allowHidden || !x.def.hidden) && (allowDefeated || !x.defeated) && (allowNonHumanlike || x.def.humanlikeFaction) && (minTechLevel == TechLevel.Undefined || (int)x.def.techLevel >= (int)minTechLevel) && x.HostileTo(Faction.OfPlayer)
			select x).TryRandomElement<Faction>(out faction)) ? null : faction;
		}

		public Faction RandomAlliedFaction(bool allowHidden = false, bool allowDefeated = false, bool allowNonHumanlike = true, TechLevel minTechLevel = TechLevel.Undefined)
		{
			Faction faction = default(Faction);
			return (!(from x in this.AllFactions
			where !x.IsPlayer && (allowHidden || !x.def.hidden) && (allowDefeated || !x.defeated) && (allowNonHumanlike || x.def.humanlikeFaction) && (minTechLevel == TechLevel.Undefined || (int)x.def.techLevel >= (int)minTechLevel) && !x.HostileTo(Faction.OfPlayer)
			select x).TryRandomElement<Faction>(out faction)) ? null : faction;
		}

		public void LogKidnappedPawns()
		{
			Log.Message("Kidnapped pawns:");
			for (int i = 0; i < this.allFactions.Count; i++)
			{
				this.allFactions[i].kidnapped.LogKidnappedPawns();
			}
		}
	}
}
