﻿using System;

namespace Verse
{
	// Token: 0x02000B51 RID: 2897
	public class ManeuverDef : Def
	{
		// Token: 0x040029EA RID: 10730
		public ToolCapacityDef requiredCapacity = null;

		// Token: 0x040029EB RID: 10731
		public VerbProperties verb;

		// Token: 0x040029EC RID: 10732
		public RulePackDef combatLogRulesHit;

		// Token: 0x040029ED RID: 10733
		public RulePackDef combatLogRulesDeflect;

		// Token: 0x040029EE RID: 10734
		public RulePackDef combatLogRulesMiss;

		// Token: 0x040029EF RID: 10735
		public RulePackDef combatLogRulesDodge;

		// Token: 0x040029F0 RID: 10736
		public LogEntryDef logEntryDef;
	}
}