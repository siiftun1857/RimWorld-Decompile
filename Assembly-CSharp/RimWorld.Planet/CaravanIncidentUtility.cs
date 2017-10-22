using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	public static class CaravanIncidentUtility
	{
		public static readonly FloatRange IncidentPointsRandomFactorRange = new FloatRange(0.25f, 1.3f);

		private const int MapCellsPerPawn = 900;

		private const int MinMapSize = 75;

		private const int MaxMapSize = 110;

		private static readonly SimpleCurve StealthFactorCurve = new SimpleCurve
		{
			{
				new CurvePoint(1f, 5f),
				true
			},
			{
				new CurvePoint(6f, 1f),
				true
			},
			{
				new CurvePoint(12f, 0.9f),
				true
			}
		};

		public static int CalculateIncidentMapSize(List<Pawn> caravanPawns, List<Pawn> enemies)
		{
			int num = Mathf.RoundToInt((float)((caravanPawns.Count + enemies.Count) * 900));
			return Mathf.Clamp(Mathf.RoundToInt(Mathf.Sqrt((float)num)), 75, 110);
		}

		public static bool CanFireIncidentWhichWantsToGenerateMapAt(int tile)
		{
			bool result;
			if (Current.Game.FindMap(tile) != null)
			{
				result = false;
			}
			else if (!Find.WorldGrid[tile].biome.implemented)
			{
				result = false;
			}
			else
			{
				List<WorldObject> allWorldObjects = Find.WorldObjects.AllWorldObjects;
				for (int i = 0; i < allWorldObjects.Count; i++)
				{
					if (allWorldObjects[i].Tile == tile && !allWorldObjects[i].def.allowCaravanIncidentsWhichGenerateMap)
						goto IL_0074;
				}
				result = true;
			}
			goto IL_0094;
			IL_0074:
			result = false;
			goto IL_0094;
			IL_0094:
			return result;
		}

		public static Map SetupCaravanAttackMap(Caravan caravan, List<Pawn> enemies)
		{
			int num = CaravanIncidentUtility.CalculateIncidentMapSize(caravan.PawnsListForReading, enemies);
			Map map = CaravanIncidentUtility.GetOrGenerateMapForIncident(caravan, new IntVec3(num, 1, num), WorldObjectDefOf.Ambush);
			IntVec3 playerStartingSpot;
			IntVec3 root = default(IntVec3);
			MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out playerStartingSpot, out root);
			CaravanEnterMapUtility.Enter(caravan, map, (Func<Pawn, IntVec3>)((Pawn x) => CellFinder.RandomSpawnCellForPawnNear(playerStartingSpot, map, 4)), CaravanDropInventoryMode.DoNotDrop, true);
			for (int i = 0; i < enemies.Count; i++)
			{
				IntVec3 loc = CellFinder.RandomSpawnCellForPawnNear(root, map, 4);
				GenSpawn.Spawn(enemies[i], loc, map, Rot4.Random, false);
			}
			return map;
		}

		public static Map GetOrGenerateMapForIncident(Caravan caravan, IntVec3 size, WorldObjectDef suggestedMapParentDef)
		{
			int tile = caravan.Tile;
			bool flag = Current.Game.FindMap(tile) == null;
			Map orGenerateMap = GetOrGenerateMapUtility.GetOrGenerateMap(tile, size, suggestedMapParentDef);
			if (flag && orGenerateMap != null)
			{
				caravan.StoryState.CopyTo(orGenerateMap.StoryState);
			}
			return orGenerateMap;
		}

		public static float CalculateCaravanStealthFactor(int pawnCount)
		{
			return CaravanIncidentUtility.StealthFactorCurve.Evaluate((float)pawnCount);
		}
	}
}
