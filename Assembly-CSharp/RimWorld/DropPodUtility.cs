using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	public static class DropPodUtility
	{
		private static List<List<Thing>> tempList = new List<List<Thing>>();

		public static void MakeDropPodAt(IntVec3 c, Map map, ActiveDropPodInfo info, bool explode = false)
		{
			ActiveDropPod activeDropPod = (ActiveDropPod)ThingMaker.MakeThing(ThingDefOf.ActiveDropPod, null);
			activeDropPod.Contents = info;
			ThingDef skyfaller = (!explode) ? ThingDefOf.DropPodIncoming : ThingDefOf.ExplosiveDropPodIncoming;
			SkyfallerMaker.SpawnSkyfaller(skyfaller, activeDropPod, c, map);
		}

		public static void DropThingsNear(IntVec3 dropCenter, Map map, IEnumerable<Thing> things, int openDelay = 110, bool canInstaDropDuringInit = false, bool leaveSlag = false, bool canRoofPunch = true, bool explode = false)
		{
			foreach (Thing item in things)
			{
				List<Thing> list = new List<Thing>();
				list.Add(item);
				DropPodUtility.tempList.Add(list);
			}
			DropPodUtility.DropThingGroupsNear(dropCenter, map, DropPodUtility.tempList, openDelay, canInstaDropDuringInit, leaveSlag, canRoofPunch, explode);
			DropPodUtility.tempList.Clear();
		}

		public static void DropThingGroupsNear(IntVec3 dropCenter, Map map, List<List<Thing>> thingsGroups, int openDelay = 110, bool instaDrop = false, bool leaveSlag = false, bool canRoofPunch = true, bool explode = false)
		{
			foreach (List<Thing> item in thingsGroups)
			{
				IntVec3 intVec = default(IntVec3);
				if (!DropCellFinder.TryFindDropSpotNear(dropCenter, map, out intVec, true, canRoofPunch))
				{
					Log.Warning("DropThingsNear failed to find a place to drop " + item.FirstOrDefault() + " near " + dropCenter + ". Dropping on random square instead.");
					intVec = CellFinderLoose.RandomCellWith((Predicate<IntVec3>)((IntVec3 c) => c.Walkable(map)), map, 1000);
				}
				for (int i = 0; i < item.Count; i++)
				{
					item[i].SetForbidden(true, false);
				}
				if (instaDrop)
				{
					foreach (Thing item2 in item)
					{
						GenPlace.TryPlaceThing(item2, intVec, map, ThingPlaceMode.Near, null);
					}
				}
				else
				{
					ActiveDropPodInfo activeDropPodInfo = new ActiveDropPodInfo();
					foreach (Thing item3 in item)
					{
						activeDropPodInfo.innerContainer.TryAdd(item3, true);
					}
					activeDropPodInfo.openDelay = openDelay;
					activeDropPodInfo.leaveSlag = leaveSlag;
					DropPodUtility.MakeDropPodAt(intVec, map, activeDropPodInfo, explode);
				}
			}
		}
	}
}
