using RimWorld;
using System;

namespace Verse.AI
{
	public static class ToilJumpConditions
	{
		public static Toil JumpIf(this Toil toil, Func<bool> jumpCondition, Toil jumpToil)
		{
			toil.AddPreTickAction((Action)delegate()
			{
				if (jumpCondition())
				{
					toil.actor.jobs.curDriver.JumpToToil(jumpToil);
				}
			});
			return toil;
		}

		public static Toil JumpIfDespawnedOrNull(this Toil toil, TargetIndex ind, Toil jumpToil)
		{
			return toil.JumpIf((Func<bool>)delegate()
			{
				Thing thing = toil.actor.jobs.curJob.GetTarget(ind).Thing;
				return thing == null || !thing.Spawned;
			}, jumpToil);
		}

		public static Toil JumpIfDespawnedOrNullOrForbidden(this Toil toil, TargetIndex ind, Toil jumpToil)
		{
			return toil.JumpIf((Func<bool>)delegate()
			{
				Thing thing = toil.actor.jobs.curJob.GetTarget(ind).Thing;
				return thing == null || !thing.Spawned || thing.IsForbidden(toil.actor);
			}, jumpToil);
		}

		public static Toil JumpIfOutsideHomeArea(this Toil toil, TargetIndex ind, Toil jumpToil)
		{
			return toil.JumpIf((Func<bool>)delegate()
			{
				Thing thing = toil.actor.jobs.curJob.GetTarget(ind).Thing;
				return (byte)((!((Area)toil.actor.Map.areaManager.Home)[thing.Position]) ? 1 : 0) != 0;
			}, jumpToil);
		}
	}
}
