﻿using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;

namespace Verse.AI
{
	public static class ToilFailConditions
	{
		public static Toil FailOn(this Toil toil, Func<Toil, bool> condition)
		{
			toil.AddEndCondition(delegate
			{
				if (condition(toil))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return toil;
		}

		public static T FailOn<T>(this T f, Func<bool> condition) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				if (condition())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnDestroyedOrNull<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				if (f.GetActor().jobs.curJob.GetTarget(ind).Thing.DestroyedOrNull())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnDespawnedOrNull<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				LocalTargetInfo target = f.GetActor().jobs.curJob.GetTarget(ind);
				Thing thing = target.Thing;
				if (thing == null && target.IsValid)
				{
					return JobCondition.Ongoing;
				}
				if (thing == null || !thing.Spawned || thing.Map != f.GetActor().Map)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T EndOnDespawnedOrNull<T>(this T f, TargetIndex ind, JobCondition endCondition = JobCondition.Incompletable) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				LocalTargetInfo target = f.GetActor().jobs.curJob.GetTarget(ind);
				Thing thing = target.Thing;
				if (thing == null && target.IsValid)
				{
					return JobCondition.Ongoing;
				}
				if (thing == null || !thing.Spawned || thing.Map != f.GetActor().Map)
				{
					return endCondition;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T EndOnNoTargetInQueue<T>(this T f, TargetIndex ind, JobCondition endCondition = JobCondition.Incompletable) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn actor = f.GetActor();
				Job curJob = actor.jobs.curJob;
				List<LocalTargetInfo> targetQueue = curJob.GetTargetQueue(ind);
				if (targetQueue.NullOrEmpty<LocalTargetInfo>())
				{
					return endCondition;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnDowned<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (((Pawn)thing).Downed)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnMobile<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (((Pawn)thing).health.State == PawnHealthState.Mobile)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnNotDowned<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (!((Pawn)thing).Downed)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnNotAwake<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (!((Pawn)thing).Awake())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnNotCasualInterruptible<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Thing thing = f.GetActor().jobs.curJob.GetTarget(ind).Thing;
				if (!((Pawn)thing).CanCasuallyInteractNow(false))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnMentalState<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn pawn = f.GetActor().jobs.curJob.GetTarget(ind).Thing as Pawn;
				if (pawn != null && pawn.InMentalState)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnAggroMentalState<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn pawn = f.GetActor().jobs.curJob.GetTarget(ind).Thing as Pawn;
				if (pawn != null && pawn.InAggroMentalState)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnAggroMentalStateAndHostile<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn pawn = f.GetActor().jobs.curJob.GetTarget(ind).Thing as Pawn;
				if (pawn != null && pawn.InAggroMentalState && pawn.HostileTo(f.GetActor()))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnSomeonePhysicallyInteracting<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn actor = f.GetActor();
				Thing thing = actor.jobs.curJob.GetTarget(ind).Thing;
				if (thing != null && actor.Map.physicalInteractionReservationManager.IsReserved(thing) && !actor.Map.physicalInteractionReservationManager.IsReservedBy(actor, thing))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnForbidden<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn actor = f.GetActor();
				if (actor.Faction != Faction.OfPlayer)
				{
					return JobCondition.Ongoing;
				}
				if (actor.jobs.curJob.ignoreForbidden)
				{
					return JobCondition.Ongoing;
				}
				Thing thing = actor.jobs.curJob.GetTarget(ind).Thing;
				if (thing == null)
				{
					return JobCondition.Ongoing;
				}
				if (thing.IsForbidden(actor))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnDespawnedNullOrForbidden<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.FailOnDespawnedOrNull(ind);
			f.FailOnForbidden(ind);
			return f;
		}

		public static T FailOnDestroyedNullOrForbidden<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.FailOnDestroyedOrNull(ind);
			f.FailOnForbidden(ind);
			return f;
		}

		public static T FailOnThingMissingDesignation<T>(this T f, TargetIndex ind, DesignationDef desDef) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn actor = f.GetActor();
				Job curJob = actor.jobs.curJob;
				if (curJob.ignoreDesignations)
				{
					return JobCondition.Ongoing;
				}
				Thing thing = curJob.GetTarget(ind).Thing;
				if (thing == null || actor.Map.designationManager.DesignationOn(thing, desDef) == null)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnCellMissingDesignation<T>(this T f, TargetIndex ind, DesignationDef desDef) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				Pawn actor = f.GetActor();
				Job curJob = actor.jobs.curJob;
				if (curJob.ignoreDesignations)
				{
					return JobCondition.Ongoing;
				}
				if (actor.Map.designationManager.DesignationAt(curJob.GetTarget(ind).Cell, desDef) == null)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnBurningImmobile<T>(this T f, TargetIndex ind) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				if (f.GetActor().jobs.curJob.GetTarget(ind).ToTargetInfo(f.GetActor().Map).IsBurning())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnCannotTouch<T>(this T f, TargetIndex ind, PathEndMode peMode) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				if (!f.GetActor().CanReachImmediate(f.GetActor().jobs.curJob.GetTarget(ind), peMode))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static T FailOnIncapable<T>(this T f, PawnCapacityDef pawnCapacity) where T : IJobEndable
		{
			f.AddEndCondition(delegate
			{
				if (!f.GetActor().health.capacities.CapableOf(pawnCapacity))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			});
			return f;
		}

		public static Toil FailOnDespawnedNullOrForbiddenPlacedThings(this Toil toil)
		{
			toil.AddFailCondition(delegate
			{
				if (toil.actor.jobs.curJob.placedThings == null)
				{
					return false;
				}
				for (int i = 0; i < toil.actor.jobs.curJob.placedThings.Count; i++)
				{
					ThingCountClass thingCountClass = toil.actor.jobs.curJob.placedThings[i];
					if (thingCountClass.thing == null || !thingCountClass.thing.Spawned || thingCountClass.thing.Map != toil.actor.Map || (!toil.actor.CurJob.ignoreForbidden && thingCountClass.thing.IsForbidden(toil.actor)))
					{
						return true;
					}
				}
				return false;
			});
			return toil;
		}

		[CompilerGenerated]
		private sealed class <FailOn>c__AnonStorey0
		{
			internal Func<Toil, bool> condition;

			internal Toil toil;

			public <FailOn>c__AnonStorey0()
			{
			}

			internal JobCondition <>m__0()
			{
				if (this.condition(this.toil))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOn>c__AnonStorey1<T> where T : IJobEndable
		{
			internal Func<bool> condition;

			public <FailOn>c__AnonStorey1()
			{
			}

			internal JobCondition <>m__0()
			{
				if (this.condition())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnDestroyedOrNull>c__AnonStorey2<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnDestroyedOrNull>c__AnonStorey2()
			{
			}

			internal JobCondition <>m__0()
			{
				if (this.f.GetActor().jobs.curJob.GetTarget(this.ind).Thing.DestroyedOrNull())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnDespawnedOrNull>c__AnonStorey3<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnDespawnedOrNull>c__AnonStorey3()
			{
			}

			internal JobCondition <>m__0()
			{
				LocalTargetInfo target = this.f.GetActor().jobs.curJob.GetTarget(this.ind);
				Thing thing = target.Thing;
				if (thing == null && target.IsValid)
				{
					return JobCondition.Ongoing;
				}
				if (thing == null || !thing.Spawned || thing.Map != this.f.GetActor().Map)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <EndOnDespawnedOrNull>c__AnonStorey4<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			internal JobCondition endCondition;

			public <EndOnDespawnedOrNull>c__AnonStorey4()
			{
			}

			internal JobCondition <>m__0()
			{
				LocalTargetInfo target = this.f.GetActor().jobs.curJob.GetTarget(this.ind);
				Thing thing = target.Thing;
				if (thing == null && target.IsValid)
				{
					return JobCondition.Ongoing;
				}
				if (thing == null || !thing.Spawned || thing.Map != this.f.GetActor().Map)
				{
					return this.endCondition;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <EndOnNoTargetInQueue>c__AnonStorey5<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			internal JobCondition endCondition;

			public <EndOnNoTargetInQueue>c__AnonStorey5()
			{
			}

			internal JobCondition <>m__0()
			{
				Pawn actor = this.f.GetActor();
				Job curJob = actor.jobs.curJob;
				List<LocalTargetInfo> targetQueue = curJob.GetTargetQueue(this.ind);
				if (targetQueue.NullOrEmpty<LocalTargetInfo>())
				{
					return this.endCondition;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnDowned>c__AnonStorey6<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnDowned>c__AnonStorey6()
			{
			}

			internal JobCondition <>m__0()
			{
				Thing thing = this.f.GetActor().jobs.curJob.GetTarget(this.ind).Thing;
				if (((Pawn)thing).Downed)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnMobile>c__AnonStorey7<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnMobile>c__AnonStorey7()
			{
			}

			internal JobCondition <>m__0()
			{
				Thing thing = this.f.GetActor().jobs.curJob.GetTarget(this.ind).Thing;
				if (((Pawn)thing).health.State == PawnHealthState.Mobile)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnNotDowned>c__AnonStorey8<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnNotDowned>c__AnonStorey8()
			{
			}

			internal JobCondition <>m__0()
			{
				Thing thing = this.f.GetActor().jobs.curJob.GetTarget(this.ind).Thing;
				if (!((Pawn)thing).Downed)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnNotAwake>c__AnonStorey9<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnNotAwake>c__AnonStorey9()
			{
			}

			internal JobCondition <>m__0()
			{
				Thing thing = this.f.GetActor().jobs.curJob.GetTarget(this.ind).Thing;
				if (!((Pawn)thing).Awake())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnNotCasualInterruptible>c__AnonStoreyA<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnNotCasualInterruptible>c__AnonStoreyA()
			{
			}

			internal JobCondition <>m__0()
			{
				Thing thing = this.f.GetActor().jobs.curJob.GetTarget(this.ind).Thing;
				if (!((Pawn)thing).CanCasuallyInteractNow(false))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnMentalState>c__AnonStoreyB<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnMentalState>c__AnonStoreyB()
			{
			}

			internal JobCondition <>m__0()
			{
				Pawn pawn = this.f.GetActor().jobs.curJob.GetTarget(this.ind).Thing as Pawn;
				if (pawn != null && pawn.InMentalState)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnAggroMentalState>c__AnonStoreyC<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnAggroMentalState>c__AnonStoreyC()
			{
			}

			internal JobCondition <>m__0()
			{
				Pawn pawn = this.f.GetActor().jobs.curJob.GetTarget(this.ind).Thing as Pawn;
				if (pawn != null && pawn.InAggroMentalState)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnAggroMentalStateAndHostile>c__AnonStoreyD<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnAggroMentalStateAndHostile>c__AnonStoreyD()
			{
			}

			internal JobCondition <>m__0()
			{
				Pawn pawn = this.f.GetActor().jobs.curJob.GetTarget(this.ind).Thing as Pawn;
				if (pawn != null && pawn.InAggroMentalState && pawn.HostileTo(this.f.GetActor()))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnSomeonePhysicallyInteracting>c__AnonStoreyE<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnSomeonePhysicallyInteracting>c__AnonStoreyE()
			{
			}

			internal JobCondition <>m__0()
			{
				Pawn actor = this.f.GetActor();
				Thing thing = actor.jobs.curJob.GetTarget(this.ind).Thing;
				if (thing != null && actor.Map.physicalInteractionReservationManager.IsReserved(thing) && !actor.Map.physicalInteractionReservationManager.IsReservedBy(actor, thing))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnForbidden>c__AnonStoreyF<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnForbidden>c__AnonStoreyF()
			{
			}

			internal JobCondition <>m__0()
			{
				Pawn actor = this.f.GetActor();
				if (actor.Faction != Faction.OfPlayer)
				{
					return JobCondition.Ongoing;
				}
				if (actor.jobs.curJob.ignoreForbidden)
				{
					return JobCondition.Ongoing;
				}
				Thing thing = actor.jobs.curJob.GetTarget(this.ind).Thing;
				if (thing == null)
				{
					return JobCondition.Ongoing;
				}
				if (thing.IsForbidden(actor))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnThingMissingDesignation>c__AnonStorey10<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			internal DesignationDef desDef;

			public <FailOnThingMissingDesignation>c__AnonStorey10()
			{
			}

			internal JobCondition <>m__0()
			{
				Pawn actor = this.f.GetActor();
				Job curJob = actor.jobs.curJob;
				if (curJob.ignoreDesignations)
				{
					return JobCondition.Ongoing;
				}
				Thing thing = curJob.GetTarget(this.ind).Thing;
				if (thing == null || actor.Map.designationManager.DesignationOn(thing, this.desDef) == null)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnCellMissingDesignation>c__AnonStorey11<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			internal DesignationDef desDef;

			public <FailOnCellMissingDesignation>c__AnonStorey11()
			{
			}

			internal JobCondition <>m__0()
			{
				Pawn actor = this.f.GetActor();
				Job curJob = actor.jobs.curJob;
				if (curJob.ignoreDesignations)
				{
					return JobCondition.Ongoing;
				}
				if (actor.Map.designationManager.DesignationAt(curJob.GetTarget(this.ind).Cell, this.desDef) == null)
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnBurningImmobile>c__AnonStorey12<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			public <FailOnBurningImmobile>c__AnonStorey12()
			{
			}

			internal JobCondition <>m__0()
			{
				if (this.f.GetActor().jobs.curJob.GetTarget(this.ind).ToTargetInfo(this.f.GetActor().Map).IsBurning())
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnCannotTouch>c__AnonStorey13<T> where T : IJobEndable
		{
			internal T f;

			internal TargetIndex ind;

			internal PathEndMode peMode;

			public <FailOnCannotTouch>c__AnonStorey13()
			{
			}

			internal JobCondition <>m__0()
			{
				if (!this.f.GetActor().CanReachImmediate(this.f.GetActor().jobs.curJob.GetTarget(this.ind), this.peMode))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnIncapable>c__AnonStorey14<T> where T : IJobEndable
		{
			internal T f;

			internal PawnCapacityDef pawnCapacity;

			public <FailOnIncapable>c__AnonStorey14()
			{
			}

			internal JobCondition <>m__0()
			{
				if (!this.f.GetActor().health.capacities.CapableOf(this.pawnCapacity))
				{
					return JobCondition.Incompletable;
				}
				return JobCondition.Ongoing;
			}
		}

		[CompilerGenerated]
		private sealed class <FailOnDespawnedNullOrForbiddenPlacedThings>c__AnonStorey15
		{
			internal Toil toil;

			public <FailOnDespawnedNullOrForbiddenPlacedThings>c__AnonStorey15()
			{
			}

			internal bool <>m__0()
			{
				if (this.toil.actor.jobs.curJob.placedThings == null)
				{
					return false;
				}
				for (int i = 0; i < this.toil.actor.jobs.curJob.placedThings.Count; i++)
				{
					ThingCountClass thingCountClass = this.toil.actor.jobs.curJob.placedThings[i];
					if (thingCountClass.thing == null || !thingCountClass.thing.Spawned || thingCountClass.thing.Map != this.toil.actor.Map || (!this.toil.actor.CurJob.ignoreForbidden && thingCountClass.thing.IsForbidden(this.toil.actor)))
					{
						return true;
					}
				}
				return false;
			}
		}
	}
}
