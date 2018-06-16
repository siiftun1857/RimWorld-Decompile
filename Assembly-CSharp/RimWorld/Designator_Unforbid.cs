﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld
{
	// Token: 0x020007E2 RID: 2018
	public class Designator_Unforbid : Designator
	{
		// Token: 0x06002CB4 RID: 11444 RVA: 0x001784A4 File Offset: 0x001768A4
		public Designator_Unforbid()
		{
			this.defaultLabel = "DesignatorUnforbid".Translate();
			this.defaultDesc = "DesignatorUnforbidDesc".Translate();
			this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Unforbid", true);
			this.soundDragSustain = SoundDefOf.Designate_DragStandard;
			this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.Designate_Claim;
			this.hotKey = KeyBindingDefOf.Misc6;
			this.hasDesignateAllFloatMenuOption = true;
			this.designateAllLabel = "UnforbidAllItems".Translate();
		}

		// Token: 0x17000708 RID: 1800
		// (get) Token: 0x06002CB5 RID: 11445 RVA: 0x00178534 File Offset: 0x00176934
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x06002CB6 RID: 11446 RVA: 0x0017854C File Offset: 0x0017694C
		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			AcceptanceReport result;
			if (!c.InBounds(base.Map) || c.Fogged(base.Map))
			{
				result = false;
			}
			else if (!c.GetThingList(base.Map).Any((Thing t) => this.CanDesignateThing(t).Accepted))
			{
				result = "MessageMustDesignateUnforbiddable".Translate();
			}
			else
			{
				result = true;
			}
			return result;
		}

		// Token: 0x06002CB7 RID: 11447 RVA: 0x001785CC File Offset: 0x001769CC
		public override void DesignateSingleCell(IntVec3 c)
		{
			List<Thing> thingList = c.GetThingList(base.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (this.CanDesignateThing(thingList[i]).Accepted)
				{
					this.DesignateThing(thingList[i]);
				}
			}
		}

		// Token: 0x06002CB8 RID: 11448 RVA: 0x00178628 File Offset: 0x00176A28
		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			AcceptanceReport result;
			if (t.def.category != ThingCategory.Item)
			{
				result = false;
			}
			else
			{
				CompForbiddable compForbiddable = t.TryGetComp<CompForbiddable>();
				result = (compForbiddable != null && compForbiddable.Forbidden);
			}
			return result;
		}

		// Token: 0x06002CB9 RID: 11449 RVA: 0x00178675 File Offset: 0x00176A75
		public override void DesignateThing(Thing t)
		{
			t.SetForbidden(false, false);
		}
	}
}