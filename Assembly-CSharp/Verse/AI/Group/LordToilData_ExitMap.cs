﻿using System;

namespace Verse.AI.Group
{
	// Token: 0x020009F8 RID: 2552
	public class LordToilData_ExitMap : LordToilData
	{
		// Token: 0x06003946 RID: 14662 RVA: 0x001E6A91 File Offset: 0x001E4E91
		public override void ExposeData()
		{
			Scribe_Values.Look<LocomotionUrgency>(ref this.locomotion, "locomotion", LocomotionUrgency.None, false);
			Scribe_Values.Look<bool>(ref this.canDig, "canDig", false, false);
		}

		// Token: 0x0400247A RID: 9338
		public LocomotionUrgency locomotion = LocomotionUrgency.None;

		// Token: 0x0400247B RID: 9339
		public bool canDig = false;
	}
}