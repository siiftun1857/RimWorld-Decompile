﻿using System;
using System.Collections.Generic;

namespace Verse.AI.Group
{
	// Token: 0x02000A03 RID: 2563
	public class TransitionAction_EndAllJobs : TransitionAction
	{
		// Token: 0x0600397A RID: 14714 RVA: 0x001E76D4 File Offset: 0x001E5AD4
		public override void DoAction(Transition trans)
		{
			List<Pawn> ownedPawns = trans.target.lord.ownedPawns;
			for (int i = 0; i < ownedPawns.Count; i++)
			{
				Pawn pawn = ownedPawns[i];
				if (pawn.jobs != null && pawn.jobs.curJob != null)
				{
					pawn.jobs.EndCurrentJob(JobCondition.InterruptForced, true);
				}
			}
		}
	}
}