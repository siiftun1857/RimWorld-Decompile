﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Verse
{
	// Token: 0x02000CFA RID: 3322
	public class DamageWorker_Scratch : DamageWorker_AddInjury
	{
		// Token: 0x0600491A RID: 18714 RVA: 0x002660AC File Offset: 0x002644AC
		protected override BodyPartRecord ChooseHitPart(DamageInfo dinfo, Pawn pawn)
		{
			return pawn.health.hediffSet.GetRandomNotMissingPart(dinfo.Def, dinfo.Height, BodyPartDepth.Outside);
		}

		// Token: 0x0600491B RID: 18715 RVA: 0x002660E0 File Offset: 0x002644E0
		protected override void ApplySpecialEffectsToPart(Pawn pawn, float totalDamage, DamageInfo dinfo, DamageWorker.DamageResult result)
		{
			if (dinfo.HitPart.depth == BodyPartDepth.Inside)
			{
				List<BodyPartRecord> list = new List<BodyPartRecord>();
				for (BodyPartRecord bodyPartRecord = dinfo.HitPart; bodyPartRecord != null; bodyPartRecord = bodyPartRecord.parent)
				{
					list.Add(bodyPartRecord);
					if (bodyPartRecord.depth == BodyPartDepth.Outside)
					{
						break;
					}
				}
				float num = (float)list.Count;
				for (int i = 0; i < list.Count; i++)
				{
					DamageInfo dinfo2 = dinfo;
					dinfo2.SetHitPart(list[i]);
					base.FinalizeAndAddInjury(pawn, totalDamage / num, dinfo2, result);
				}
			}
			else
			{
				IEnumerable<BodyPartRecord> enumerable = dinfo.HitPart.GetDirectChildParts();
				if (dinfo.HitPart.parent != null)
				{
					enumerable = enumerable.Concat(dinfo.HitPart.parent);
					if (dinfo.HitPart.parent.parent != null)
					{
						enumerable = enumerable.Concat(dinfo.HitPart.parent.GetDirectChildParts());
					}
				}
				enumerable = from target in enumerable
				where target != dinfo.HitPart && !target.def.conceptual && target.depth == BodyPartDepth.Outside && !pawn.health.hediffSet.PartIsMissing(target)
				select target;
				BodyPartRecord bodyPartRecord2 = enumerable.RandomElementWithFallback(null);
				if (bodyPartRecord2 == null)
				{
					base.FinalizeAndAddInjury(pawn, base.ReduceDamageToPreserveOutsideParts(totalDamage, dinfo, pawn), dinfo, result);
				}
				else
				{
					base.FinalizeAndAddInjury(pawn, base.ReduceDamageToPreserveOutsideParts(totalDamage * this.def.scratchSplitPercentage, dinfo, pawn), dinfo, result);
					DamageInfo dinfo3 = dinfo;
					dinfo3.SetHitPart(bodyPartRecord2);
					base.FinalizeAndAddInjury(pawn, base.ReduceDamageToPreserveOutsideParts(totalDamage * this.def.scratchSplitPercentage, dinfo3, pawn), dinfo3, result);
				}
			}
		}
	}
}