using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public static class LoadTransportersJobUtility
	{
		private static HashSet<Thing> neededThings = new HashSet<Thing>();

		public static bool HasJobOnTransporter(Pawn pawn, CompTransporter transporter)
		{
			bool result;
			if (transporter.parent.IsForbidden(pawn))
			{
				result = false;
			}
			else if (!transporter.AnythingLeftToLoad)
			{
				result = false;
			}
			else if (!pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation))
			{
				result = false;
			}
			else if (!pawn.CanReserveAndReach((Thing)transporter.parent, PathEndMode.Touch, pawn.NormalMaxDanger(), 1, -1, null, false))
			{
				result = false;
			}
			else
			{
				Thing thing = LoadTransportersJobUtility.FindThingToLoad(pawn, transporter);
				result = ((byte)((thing != null) ? 1 : 0) != 0);
			}
			return result;
		}

		public static Job JobOnTransporter(Pawn p, CompTransporter transporter)
		{
			Thing thing = LoadTransportersJobUtility.FindThingToLoad(p, transporter);
			Job job = new Job(JobDefOf.HaulToContainer, thing, (Thing)transporter.parent);
			int countToTransfer = TransferableUtility.TransferableMatchingDesperate(thing, transporter.leftToLoad).CountToTransfer;
			job.count = Mathf.Min(countToTransfer, thing.stackCount);
			job.ignoreForbidden = true;
			return job;
		}

		private static Thing FindThingToLoad(Pawn p, CompTransporter transporter)
		{
			LoadTransportersJobUtility.neededThings.Clear();
			List<TransferableOneWay> leftToLoad = transporter.leftToLoad;
			if (leftToLoad != null)
			{
				for (int i = 0; i < leftToLoad.Count; i++)
				{
					TransferableOneWay transferableOneWay = leftToLoad[i];
					if (transferableOneWay.CountToTransfer > 0)
					{
						for (int j = 0; j < transferableOneWay.things.Count; j++)
						{
							LoadTransportersJobUtility.neededThings.Add(transferableOneWay.things[j]);
						}
					}
				}
			}
			Thing result;
			if (!LoadTransportersJobUtility.neededThings.Any())
			{
				result = null;
			}
			else
			{
				Thing thing = GenClosest.ClosestThingReachable(p.Position, p.Map, ThingRequest.ForGroup(ThingRequestGroup.HaulableEver), PathEndMode.Touch, TraverseParms.For(p, Danger.Deadly, TraverseMode.ByPawn, false), 9999f, (Predicate<Thing>)((Thing x) => LoadTransportersJobUtility.neededThings.Contains(x) && p.CanReserve(x, 1, -1, null, false)), null, 0, -1, false, RegionType.Set_Passable, false);
				if (thing == null)
				{
					foreach (Thing neededThing in LoadTransportersJobUtility.neededThings)
					{
						Pawn pawn = neededThing as Pawn;
						if (pawn != null && (!pawn.IsColonist || pawn.Downed) && !pawn.inventory.UnloadEverything && p.CanReserveAndReach((Thing)pawn, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false))
						{
							return pawn;
						}
					}
				}
				LoadTransportersJobUtility.neededThings.Clear();
				result = thing;
			}
			return result;
		}
	}
}
