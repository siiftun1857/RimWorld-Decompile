using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public static class WatchBuildingUtility
	{
		private static List<int> allowedDirections = new List<int>();

		public static IEnumerable<IntVec3> CalculateWatchCells(ThingDef def, IntVec3 center, Rot4 rot, Map map)
		{
			List<int> allowedDirections = WatchBuildingUtility.CalculateAllowedDirections(def, rot);
			for (int i = 0; i < allowedDirections.Count; i++)
			{
				foreach (IntVec3 item in WatchBuildingUtility.GetWatchCellRect(def, center, rot, allowedDirections[i]))
				{
					if (WatchBuildingUtility.EverPossibleToWatchFrom(item, center, map, true))
					{
						yield return item;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_0143:
			/*Error near IL_0144: Unexpected return in MoveNext()*/;
		}

		public static bool TryFindBestWatchCell(Thing toWatch, Pawn pawn, bool desireSit, out IntVec3 result, out Building chair)
		{
			List<int> list = WatchBuildingUtility.CalculateAllowedDirections(toWatch.def, toWatch.Rotation);
			IntVec3 intVec = IntVec3.Invalid;
			int num = 0;
			bool result2;
			while (true)
			{
				IntVec3 intVec2;
				Building building;
				if (num < list.Count)
				{
					CellRect watchCellRect = WatchBuildingUtility.GetWatchCellRect(toWatch.def, toWatch.Position, toWatch.Rotation, list[num]);
					IntVec3 centerCell = watchCellRect.CenterCell;
					int num2 = watchCellRect.Area * 4;
					for (int num3 = 0; num3 < num2; num3++)
					{
						intVec2 = centerCell + GenRadial.RadialPattern[num3];
						if (watchCellRect.Contains(intVec2))
						{
							bool flag = false;
							building = null;
							if (WatchBuildingUtility.EverPossibleToWatchFrom(intVec2, toWatch.Position, toWatch.Map, false) && !intVec2.IsForbidden(pawn) && pawn.CanReserve(intVec2, 1, -1, null, false) && pawn.Map.pawnDestinationReservationManager.CanReserve(intVec2, pawn))
							{
								if (desireSit)
								{
									building = intVec2.GetEdifice(pawn.Map);
									if (building != null && building.def.building.isSittable && pawn.CanReserve((Thing)building, 1, -1, null, false))
									{
										flag = true;
									}
								}
								else
								{
									flag = true;
								}
							}
							if (flag)
							{
								if (!desireSit || !(building.Rotation != new Rot4(list[num]).Opposite))
								{
									goto IL_0179;
								}
								intVec = intVec2;
							}
						}
					}
					num++;
					continue;
				}
				if (intVec.IsValid)
				{
					result = intVec;
					chair = intVec.GetEdifice(pawn.Map);
					result2 = true;
				}
				else
				{
					result = IntVec3.Invalid;
					chair = null;
					result2 = false;
				}
				break;
				IL_0179:
				result = intVec2;
				chair = building;
				result2 = true;
				break;
			}
			return result2;
		}

		public static bool CanWatchFromBed(Pawn pawn, Building_Bed bed, Thing toWatch)
		{
			bool result;
			if (!WatchBuildingUtility.EverPossibleToWatchFrom(pawn.Position, toWatch.Position, pawn.Map, true))
			{
				result = false;
			}
			else
			{
				if (toWatch.def.rotatable)
				{
					Rot4 rotation = bed.Rotation;
					CellRect cellRect = toWatch.OccupiedRect();
					if (rotation == Rot4.North)
					{
						int maxZ = cellRect.maxZ;
						IntVec3 position = pawn.Position;
						if (maxZ < position.z)
						{
							result = false;
							goto IL_017f;
						}
					}
					if (rotation == Rot4.South)
					{
						int minZ = cellRect.minZ;
						IntVec3 position2 = pawn.Position;
						if (minZ > position2.z)
						{
							result = false;
							goto IL_017f;
						}
					}
					if (rotation == Rot4.East)
					{
						int maxX = cellRect.maxX;
						IntVec3 position3 = pawn.Position;
						if (maxX < position3.x)
						{
							result = false;
							goto IL_017f;
						}
					}
					if (rotation == Rot4.West)
					{
						int minX = cellRect.minX;
						IntVec3 position4 = pawn.Position;
						if (minX > position4.x)
						{
							result = false;
							goto IL_017f;
						}
					}
				}
				List<int> list = WatchBuildingUtility.CalculateAllowedDirections(toWatch.def, toWatch.Rotation);
				for (int i = 0; i < list.Count; i++)
				{
					if (WatchBuildingUtility.GetWatchCellRect(toWatch.def, toWatch.Position, toWatch.Rotation, list[i]).Contains(pawn.Position))
						goto IL_015c;
				}
				result = false;
			}
			goto IL_017f;
			IL_017f:
			return result;
			IL_015c:
			result = true;
			goto IL_017f;
		}

		private static CellRect GetWatchCellRect(ThingDef def, IntVec3 center, Rot4 rot, int watchRot)
		{
			Rot4 a = new Rot4(watchRot);
			if (def.building == null)
			{
				def = (def.entityDefToBuild as ThingDef);
			}
			CellRect result;
			if (a.IsHorizontal)
			{
				int num = center.x + GenAdj.CardinalDirections[watchRot].x * def.building.watchBuildingStandDistanceRange.min;
				int num2 = center.x + GenAdj.CardinalDirections[watchRot].x * def.building.watchBuildingStandDistanceRange.max;
				int num3 = center.z + def.building.watchBuildingStandRectWidth / 2;
				int num4 = center.z - def.building.watchBuildingStandRectWidth / 2;
				if (def.building.watchBuildingStandRectWidth % 2 == 0)
				{
					if (a == Rot4.West)
					{
						num4++;
					}
					else
					{
						num3--;
					}
				}
				result = new CellRect(Mathf.Min(num, num2), num4, Mathf.Abs(num - num2) + 1, num3 - num4 + 1);
			}
			else
			{
				int num5 = center.z + GenAdj.CardinalDirections[watchRot].z * def.building.watchBuildingStandDistanceRange.min;
				int num6 = center.z + GenAdj.CardinalDirections[watchRot].z * def.building.watchBuildingStandDistanceRange.max;
				int num7 = center.x + def.building.watchBuildingStandRectWidth / 2;
				int num8 = center.x - def.building.watchBuildingStandRectWidth / 2;
				if (def.building.watchBuildingStandRectWidth % 2 == 0)
				{
					if (a == Rot4.North)
					{
						num8++;
					}
					else
					{
						num7--;
					}
				}
				result = new CellRect(num8, Mathf.Min(num5, num6), num7 - num8 + 1, Mathf.Abs(num5 - num6) + 1);
			}
			return result;
		}

		private static bool EverPossibleToWatchFrom(IntVec3 watchCell, IntVec3 buildingCenter, Map map, bool bedAllowed)
		{
			return (watchCell.Standable(map) || (bedAllowed && watchCell.GetEdifice(map) is Building_Bed)) && GenSight.LineOfSight(buildingCenter, watchCell, map, true, null, 0, 0);
		}

		private static List<int> CalculateAllowedDirections(ThingDef toWatchDef, Rot4 toWatchRot)
		{
			WatchBuildingUtility.allowedDirections.Clear();
			if (toWatchDef.rotatable)
			{
				WatchBuildingUtility.allowedDirections.Add(toWatchRot.AsInt);
			}
			else
			{
				WatchBuildingUtility.allowedDirections.Add(0);
				WatchBuildingUtility.allowedDirections.Add(1);
				WatchBuildingUtility.allowedDirections.Add(2);
				WatchBuildingUtility.allowedDirections.Add(3);
			}
			return WatchBuildingUtility.allowedDirections;
		}
	}
}
