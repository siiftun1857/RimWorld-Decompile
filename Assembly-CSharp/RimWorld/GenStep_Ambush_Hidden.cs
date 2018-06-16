﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000408 RID: 1032
	public class GenStep_Ambush_Hidden : GenStep_Ambush
	{
		// Token: 0x1700025D RID: 605
		// (get) Token: 0x060011C0 RID: 4544 RVA: 0x0009A694 File Offset: 0x00098A94
		public override int SeedPart
		{
			get
			{
				return 921085483;
			}
		}

		// Token: 0x060011C1 RID: 4545 RVA: 0x0009A6B0 File Offset: 0x00098AB0
		protected override RectTrigger MakeRectTrigger()
		{
			RectTrigger rectTrigger = base.MakeRectTrigger();
			rectTrigger.activateOnExplosion = true;
			return rectTrigger;
		}

		// Token: 0x060011C2 RID: 4546 RVA: 0x0009A6D4 File Offset: 0x00098AD4
		protected override SignalAction_Ambush MakeAmbushSignalAction(CellRect rectToDefend, IntVec3 root)
		{
			SignalAction_Ambush signalAction_Ambush = base.MakeAmbushSignalAction(rectToDefend, root);
			if (root.IsValid)
			{
				signalAction_Ambush.spawnNear = root;
			}
			else
			{
				signalAction_Ambush.spawnAround = rectToDefend;
			}
			return signalAction_Ambush;
		}
	}
}