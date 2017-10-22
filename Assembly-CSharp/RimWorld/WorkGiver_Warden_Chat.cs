using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_Warden_Chat : WorkGiver_Warden
	{
		public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
		{
			Job result;
			if (!base.ShouldTakeCareOfPrisoner(pawn, t))
			{
				result = null;
			}
			else
			{
				Pawn pawn2 = (Pawn)t;
				if ((pawn2.guest.interactionMode == PrisonerInteractionModeDefOf.Chat || pawn2.guest.interactionMode == PrisonerInteractionModeDefOf.AttemptRecruit) && pawn2.guest.ScheduledForInteraction && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Talking) && (!pawn2.Downed || pawn2.InBed()) && pawn.CanReserve(t, 1, -1, null, false) && pawn2.Awake())
				{
					if (pawn2.guest.interactionMode == PrisonerInteractionModeDefOf.Chat)
					{
						result = new Job(JobDefOf.PrisonerFriendlyChat, t);
						goto IL_0105;
					}
					if (pawn2.guest.interactionMode == PrisonerInteractionModeDefOf.AttemptRecruit)
					{
						result = new Job(JobDefOf.PrisonerAttemptRecruit, t);
						goto IL_0105;
					}
				}
				result = null;
			}
			goto IL_0105;
			IL_0105:
			return result;
		}
	}
}
