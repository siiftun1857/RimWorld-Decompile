﻿using System;
using System.Collections.Generic;
using RimWorld;

namespace Verse.AI
{
	// Token: 0x02000AA6 RID: 2726
	public class AttackTargetReservationManager : IExposable
	{
		// Token: 0x06003CC0 RID: 15552 RVA: 0x00202543 File Offset: 0x00200943
		public AttackTargetReservationManager(Map map)
		{
			this.map = map;
		}

		// Token: 0x06003CC1 RID: 15553 RVA: 0x00202560 File Offset: 0x00200960
		public void Reserve(Pawn claimant, Job job, IAttackTarget target)
		{
			if (target == null)
			{
				Log.Warning(claimant + " tried to reserve null attack target.", false);
			}
			else if (!this.IsReservedBy(claimant, target))
			{
				AttackTargetReservationManager.AttackTargetReservation attackTargetReservation = new AttackTargetReservationManager.AttackTargetReservation();
				attackTargetReservation.target = target;
				attackTargetReservation.claimant = claimant;
				attackTargetReservation.job = job;
				this.reservations.Add(attackTargetReservation);
			}
		}

		// Token: 0x06003CC2 RID: 15554 RVA: 0x002025C4 File Offset: 0x002009C4
		public void Release(Pawn claimant, Job job, IAttackTarget target)
		{
			if (target == null)
			{
				Log.Warning(claimant + " tried to release reservation on null attack target.", false);
			}
			else
			{
				for (int i = 0; i < this.reservations.Count; i++)
				{
					AttackTargetReservationManager.AttackTargetReservation attackTargetReservation = this.reservations[i];
					if (attackTargetReservation.target == target && attackTargetReservation.claimant == claimant && attackTargetReservation.job == job)
					{
						this.reservations.RemoveAt(i);
						return;
					}
				}
				Log.Warning(string.Concat(new object[]
				{
					claimant,
					" with job ",
					job,
					" tried to release reservation on target ",
					target,
					", but it's not reserved by him."
				}), false);
			}
		}

		// Token: 0x06003CC3 RID: 15555 RVA: 0x00202688 File Offset: 0x00200A88
		public bool CanReserve(Pawn claimant, IAttackTarget target)
		{
			bool result;
			if (this.IsReservedBy(claimant, target))
			{
				result = true;
			}
			else
			{
				int reservationsCount = this.GetReservationsCount(target, claimant.Faction);
				int maxPreferredReservationsCount = this.GetMaxPreferredReservationsCount(target);
				result = (reservationsCount < maxPreferredReservationsCount);
			}
			return result;
		}

		// Token: 0x06003CC4 RID: 15556 RVA: 0x002026CC File Offset: 0x00200ACC
		public bool IsReservedBy(Pawn claimant, IAttackTarget target)
		{
			for (int i = 0; i < this.reservations.Count; i++)
			{
				AttackTargetReservationManager.AttackTargetReservation attackTargetReservation = this.reservations[i];
				if (attackTargetReservation.target == target && attackTargetReservation.claimant == claimant)
				{
					return true;
				}
			}
			return false;
		}

		// Token: 0x06003CC5 RID: 15557 RVA: 0x0020272C File Offset: 0x00200B2C
		public void ReleaseAllForTarget(IAttackTarget target)
		{
			this.reservations.RemoveAll((AttackTargetReservationManager.AttackTargetReservation x) => x.target == target);
		}

		// Token: 0x06003CC6 RID: 15558 RVA: 0x00202760 File Offset: 0x00200B60
		public void ReleaseClaimedBy(Pawn claimant, Job job)
		{
			for (int i = this.reservations.Count - 1; i >= 0; i--)
			{
				if (this.reservations[i].claimant == claimant && this.reservations[i].job == job)
				{
					this.reservations.RemoveAt(i);
				}
			}
		}

		// Token: 0x06003CC7 RID: 15559 RVA: 0x002027C8 File Offset: 0x00200BC8
		public void ReleaseAllClaimedBy(Pawn claimant)
		{
			for (int i = this.reservations.Count - 1; i >= 0; i--)
			{
				if (this.reservations[i].claimant == claimant)
				{
					this.reservations.RemoveAt(i);
				}
			}
		}

		// Token: 0x06003CC8 RID: 15560 RVA: 0x0020281C File Offset: 0x00200C1C
		public IAttackTarget FirstReservationFor(Pawn claimant)
		{
			for (int i = this.reservations.Count - 1; i >= 0; i--)
			{
				if (this.reservations[i].claimant == claimant)
				{
					return this.reservations[i].target;
				}
			}
			return null;
		}

		// Token: 0x06003CC9 RID: 15561 RVA: 0x00202880 File Offset: 0x00200C80
		public void ExposeData()
		{
			Scribe_Collections.Look<AttackTargetReservationManager.AttackTargetReservation>(ref this.reservations, "reservations", LookMode.Deep, new object[0]);
			if (Scribe.mode == LoadSaveMode.PostLoadInit)
			{
				this.reservations.RemoveAll((AttackTargetReservationManager.AttackTargetReservation x) => x.target == null);
				if (this.reservations.RemoveAll((AttackTargetReservationManager.AttackTargetReservation x) => x.claimant.DestroyedOrNull()) != 0)
				{
					Log.Warning("Some attack target reservations had null or destroyed claimant.", false);
				}
			}
		}

		// Token: 0x06003CCA RID: 15562 RVA: 0x00202914 File Offset: 0x00200D14
		private int GetReservationsCount(IAttackTarget target, Faction faction)
		{
			int num = 0;
			for (int i = 0; i < this.reservations.Count; i++)
			{
				AttackTargetReservationManager.AttackTargetReservation attackTargetReservation = this.reservations[i];
				if (attackTargetReservation.target == target && attackTargetReservation.claimant.Faction == faction)
				{
					num++;
				}
			}
			return num;
		}

		// Token: 0x06003CCB RID: 15563 RVA: 0x00202978 File Offset: 0x00200D78
		private int GetMaxPreferredReservationsCount(IAttackTarget target)
		{
			int num = 0;
			Thing thing = target.Thing;
			CellRect cellRect = thing.OccupiedRect();
			foreach (IntVec3 c in cellRect.ExpandedBy(1))
			{
				if (!cellRect.Contains(c))
				{
					if (c.InBounds(this.map))
					{
						if (c.Standable(this.map))
						{
							num++;
						}
					}
				}
			}
			return num;
		}

		// Token: 0x04002677 RID: 9847
		private Map map;

		// Token: 0x04002678 RID: 9848
		private List<AttackTargetReservationManager.AttackTargetReservation> reservations = new List<AttackTargetReservationManager.AttackTargetReservation>();

		// Token: 0x02000AA7 RID: 2727
		public class AttackTargetReservation : IExposable
		{
			// Token: 0x06003CCF RID: 15567 RVA: 0x00202A77 File Offset: 0x00200E77
			public void ExposeData()
			{
				Scribe_References.Look<IAttackTarget>(ref this.target, "target", false);
				Scribe_References.Look<Pawn>(ref this.claimant, "claimant", false);
				Scribe_References.Look<Job>(ref this.job, "job", false);
			}

			// Token: 0x0400267B RID: 9851
			public IAttackTarget target;

			// Token: 0x0400267C RID: 9852
			public Pawn claimant;

			// Token: 0x0400267D RID: 9853
			public Job job;
		}
	}
}