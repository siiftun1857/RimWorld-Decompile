﻿using System;
using RimWorld;

namespace Verse.AI
{
	// Token: 0x02000ACA RID: 2762
	public class JobGiver_IdleError : ThinkNode_JobGiver
	{
		// Token: 0x06003D57 RID: 15703 RVA: 0x00205600 File Offset: 0x00203A00
		protected override Job TryGiveJob(Pawn pawn)
		{
			Log.ErrorOnce(pawn + " issued IdleError wait job. The behavior tree should never get here.", 532983, false);
			return new Job(JobDefOf.Wait)
			{
				expiryInterval = 100
			};
		}

		// Token: 0x040026B0 RID: 9904
		private const int WaitTime = 100;
	}
}