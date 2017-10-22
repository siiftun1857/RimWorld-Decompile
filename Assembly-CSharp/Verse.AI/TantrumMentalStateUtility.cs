using System;
using System.Collections.Generic;

namespace Verse.AI
{
	public static class TantrumMentalStateUtility
	{
		private const int MaxRegionsToSearch = 40;

		private const int AbsoluteMinItemMarketValue = 75;

		public static bool CanSmash(Pawn pawn, Thing thing, bool skipReachabilityCheck = false, Predicate<Thing> customValidator = null, int extraMinBuildingOrItemMarketValue = 0)
		{
			bool result;
			if ((object)customValidator != null)
			{
				if (!customValidator(thing))
				{
					result = false;
					goto IL_015d;
				}
			}
			else if (!thing.def.IsBuildingArtificial && thing.def.category != ThingCategory.Item)
			{
				result = false;
				goto IL_015d;
			}
			if (!thing.Destroyed && thing.Spawned && thing != pawn && (thing.def.category == ThingCategory.Pawn || thing.def.useHitPoints) && (thing.def.category == ThingCategory.Pawn || !thing.def.CanHaveFaction || thing.Faction == pawn.Faction) && (thing.def.category != ThingCategory.Item || !(thing.MarketValue * (float)thing.stackCount < 75.0)) && (thing.def.category != ThingCategory.Pawn || !((Pawn)thing).Downed))
			{
				if (thing.def.category != ThingCategory.Item && thing.def.category != ThingCategory.Building)
				{
					goto IL_013b;
				}
				if (!(thing.MarketValue * (float)thing.stackCount < (float)extraMinBuildingOrItemMarketValue))
					goto IL_013b;
			}
			int num = 0;
			goto IL_0157;
			IL_015d:
			return result;
			IL_0157:
			result = ((byte)num != 0);
			goto IL_015d;
			IL_013b:
			num = ((skipReachabilityCheck || pawn.CanReach(thing, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn)) ? 1 : 0);
			goto IL_0157;
		}

		public static void GetSmashableThingsNear(Pawn pawn, IntVec3 near, List<Thing> outCandidates, Predicate<Thing> customValidator = null, int extraMinBuildingOrItemMarketValue = 0, int maxDistance = 40)
		{
			outCandidates.Clear();
			if (pawn.CanReach(near, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
			{
				Region region = near.GetRegion(pawn.Map, RegionType.Set_Passable);
				if (region != null)
				{
					TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
					RegionTraverser.BreadthFirstTraverse(region, (RegionEntryPredicate)((Region from, Region to) => to.Allows(traverseParams, false)), (RegionProcessor)delegate(Region r)
					{
						List<Thing> list = r.ListerThings.ThingsInGroup(ThingRequestGroup.BuildingArtificial);
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i].Position.InHorDistOf(near, (float)maxDistance) && TantrumMentalStateUtility.CanSmash(pawn, list[i], true, customValidator, extraMinBuildingOrItemMarketValue))
							{
								outCandidates.Add(list[i]);
							}
						}
						List<Thing> list2 = r.ListerThings.ThingsInGroup(ThingRequestGroup.HaulableEver);
						for (int j = 0; j < list2.Count; j++)
						{
							if (list2[j].Position.InHorDistOf(near, (float)maxDistance) && TantrumMentalStateUtility.CanSmash(pawn, list2[j], true, customValidator, extraMinBuildingOrItemMarketValue))
							{
								outCandidates.Add(list2[j]);
							}
						}
						List<Thing> list3 = r.ListerThings.ThingsInGroup(ThingRequestGroup.Pawn);
						for (int k = 0; k < list3.Count; k++)
						{
							if (list3[k].Position.InHorDistOf(near, (float)maxDistance) && TantrumMentalStateUtility.CanSmash(pawn, list3[k], true, customValidator, extraMinBuildingOrItemMarketValue))
							{
								outCandidates.Add(list3[k]);
							}
						}
						return false;
					}, 40, RegionType.Set_Passable);
				}
			}
		}

		public static void GetSmashableThingsIn(Room room, Pawn pawn, List<Thing> outCandidates, Predicate<Thing> customValidator = null, int extraMinBuildingOrItemMarketValue = 0)
		{
			outCandidates.Clear();
			List<Thing> containedAndAdjacentThings = room.ContainedAndAdjacentThings;
			for (int i = 0; i < containedAndAdjacentThings.Count; i++)
			{
				Thing thing = containedAndAdjacentThings[i];
				if (TantrumMentalStateUtility.CanSmash(pawn, thing, false, customValidator, extraMinBuildingOrItemMarketValue))
				{
					outCandidates.Add(containedAndAdjacentThings[i]);
				}
			}
		}

		public static bool CanAttackPrisoner(Pawn pawn, Thing prisoner)
		{
			Pawn pawn2 = prisoner as Pawn;
			return pawn2 != null && pawn2.IsPrisoner && !pawn2.Downed && pawn2.HostFaction == pawn.Faction;
		}
	}
}
