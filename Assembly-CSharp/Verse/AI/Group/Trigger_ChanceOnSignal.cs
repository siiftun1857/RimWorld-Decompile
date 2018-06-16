﻿using System;

namespace Verse.AI.Group
{
	// Token: 0x02000A1B RID: 2587
	public class Trigger_ChanceOnSignal : Trigger
	{
		// Token: 0x060039AF RID: 14767 RVA: 0x001E80E6 File Offset: 0x001E64E6
		public Trigger_ChanceOnSignal(TriggerSignalType signalType, float chance)
		{
			this.signalType = signalType;
			this.chance = chance;
		}

		// Token: 0x060039B0 RID: 14768 RVA: 0x001E8100 File Offset: 0x001E6500
		public override bool ActivateOn(Lord lord, TriggerSignal signal)
		{
			return signal.type == this.signalType && Rand.Value < this.chance;
		}

		// Token: 0x040024B0 RID: 9392
		private TriggerSignalType signalType;

		// Token: 0x040024B1 RID: 9393
		private float chance;
	}
}