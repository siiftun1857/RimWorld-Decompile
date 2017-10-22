using RimWorld;

namespace Verse.AI
{
	public abstract class JobGiver_ExitMap : ThinkNode_JobGiver
	{
		protected LocomotionUrgency defaultLocomotion = LocomotionUrgency.None;

		protected int jobMaxDuration = 999999;

		protected bool canBash = false;

		protected bool forceCanDig = false;

		protected bool forceCanDigIfAnyHostileActiveThreat = false;

		protected bool forceCanDigIfCantReachMapEdge = false;

		protected bool failIfCantJoinOrCreateCaravan = false;

		public override ThinkNode DeepCopy(bool resolve = true)
		{
			JobGiver_ExitMap jobGiver_ExitMap = (JobGiver_ExitMap)base.DeepCopy(resolve);
			jobGiver_ExitMap.defaultLocomotion = this.defaultLocomotion;
			jobGiver_ExitMap.jobMaxDuration = this.jobMaxDuration;
			jobGiver_ExitMap.canBash = this.canBash;
			jobGiver_ExitMap.forceCanDig = this.forceCanDig;
			jobGiver_ExitMap.forceCanDigIfAnyHostileActiveThreat = this.forceCanDigIfAnyHostileActiveThreat;
			jobGiver_ExitMap.forceCanDigIfCantReachMapEdge = this.forceCanDigIfCantReachMapEdge;
			jobGiver_ExitMap.failIfCantJoinOrCreateCaravan = this.failIfCantJoinOrCreateCaravan;
			return jobGiver_ExitMap;
		}

		protected override Job TryGiveJob(Pawn pawn)
		{
			bool flag = false;
			if (this.forceCanDig || (pawn.mindState.duty != null && pawn.mindState.duty.canDig) || (this.forceCanDigIfCantReachMapEdge && !pawn.CanReachMapEdge()) || (this.forceCanDigIfAnyHostileActiveThreat && pawn.Faction != null && GenHostility.AnyHostileActiveThreatTo(pawn.Map, pawn.Faction)))
			{
				flag = true;
			}
			IntVec3 c = default(IntVec3);
			Job result;
			if (!this.TryFindGoodExitDest(pawn, flag, out c))
			{
				result = null;
			}
			else
			{
				if (flag)
				{
					using (PawnPath path = pawn.Map.pathFinder.FindPath(pawn.Position, c, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.PassAllDestroyableThings, false), PathEndMode.OnCell))
					{
						IntVec3 cellBeforeBlocker = default(IntVec3);
						Thing thing = path.FirstBlockingBuilding(out cellBeforeBlocker, pawn);
						if (thing != null)
						{
							Job job = DigUtility.PassBlockerJob(pawn, thing, cellBeforeBlocker, true, true);
							if (job != null)
							{
								return job;
							}
						}
					}
				}
				Job job2 = new Job(JobDefOf.Goto, c);
				job2.exitMapOnArrival = true;
				job2.failIfCantJoinOrCreateCaravan = this.failIfCantJoinOrCreateCaravan;
				job2.locomotionUrgency = PawnUtility.ResolveLocomotion(pawn, this.defaultLocomotion, LocomotionUrgency.Jog);
				job2.expiryInterval = this.jobMaxDuration;
				job2.canBash = this.canBash;
				result = job2;
			}
			return result;
		}

		protected abstract bool TryFindGoodExitDest(Pawn pawn, bool canDig, out IntVec3 dest);
	}
}
