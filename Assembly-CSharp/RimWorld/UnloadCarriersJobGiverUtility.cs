using Verse;
using Verse.AI;

namespace RimWorld
{
	public static class UnloadCarriersJobGiverUtility
	{
		public static bool HasJobOnThing(Pawn pawn, Thing t, bool forced)
		{
			Pawn pawn2 = t as Pawn;
			bool result;
			if (pawn2 != null && pawn2 != pawn && !pawn2.IsFreeColonist && pawn2.inventory.UnloadEverything && (pawn2.Faction == pawn.Faction || pawn2.HostFaction == pawn.Faction) && !t.IsForbidden(pawn) && !t.IsBurning())
			{
				LocalTargetInfo target = t;
				if (!pawn.CanReserve(target, 1, -1, null, forced))
					goto IL_0084;
				result = true;
				goto IL_0095;
			}
			goto IL_0084;
			IL_0084:
			result = false;
			goto IL_0095;
			IL_0095:
			return result;
		}
	}
}
