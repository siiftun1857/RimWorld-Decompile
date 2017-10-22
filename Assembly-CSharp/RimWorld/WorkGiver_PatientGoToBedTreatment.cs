using System.Collections.Generic;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_PatientGoToBedTreatment : WorkGiver_PatientGoToBedRecuperate
	{
		public override Job NonScanJob(Pawn pawn)
		{
			return HealthAIUtility.ShouldSeekMedicalRestUrgent(pawn) ? (this.AnyAvailableDoctorFor(pawn) ? base.NonScanJob(pawn) : null) : null;
		}

		private bool AnyAvailableDoctorFor(Pawn pawn)
		{
			Map mapHeld = pawn.MapHeld;
			bool result;
			if (mapHeld == null || pawn.Faction == null)
			{
				result = false;
			}
			else
			{
				List<Pawn> list = mapHeld.mapPawns.SpawnedPawnsInFaction(pawn.Faction);
				for (int i = 0; i < list.Count; i++)
				{
					Pawn pawn2 = list[i];
					if (pawn2 != pawn && pawn2.RaceProps.Humanlike && !pawn2.Downed && pawn2.Awake() && !pawn2.InBed() && !pawn2.InMentalState && !pawn2.IsPrisoner && pawn2.workSettings != null && pawn2.workSettings.WorkIsActive(WorkTypeDefOf.Doctor) && pawn2.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) && pawn2.CanReach((Thing)pawn, PathEndMode.Touch, Danger.Deadly, false, TraverseMode.ByPawn))
						goto IL_00f1;
				}
				result = false;
			}
			goto IL_010f;
			IL_00f1:
			result = true;
			goto IL_010f;
			IL_010f:
			return result;
		}
	}
}
