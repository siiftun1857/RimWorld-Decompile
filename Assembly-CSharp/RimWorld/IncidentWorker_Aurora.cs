﻿using System;
using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public class IncidentWorker_Aurora : IncidentWorker_MakeGameCondition
	{
		private const int EnsureMinDurationTicks = 5000;

		public IncidentWorker_Aurora()
		{
		}

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			if (!base.CanFireNowSub(parms))
			{
				return false;
			}
			List<Map> maps = Find.Maps;
			for (int i = 0; i < maps.Count; i++)
			{
				if (maps[i].IsPlayerHome && !this.AuroraWillEndSoon(maps[i]))
				{
					return true;
				}
			}
			return false;
		}

		private bool AuroraWillEndSoon(Map map)
		{
			return GenCelestial.CurCelestialSunGlow(map) > 0.5f || GenCelestial.CelestialSunGlow(map, Find.TickManager.TicksAbs + 5000) > 0.5f;
		}
	}
}
