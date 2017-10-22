using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class Designator_Claim : Designator
	{
		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}

		public Designator_Claim()
		{
			base.defaultLabel = "DesignatorClaim".Translate();
			base.defaultDesc = "DesignatorClaimDesc".Translate();
			base.icon = ContentFinder<Texture2D>.Get("UI/Designators/Claim", true);
			base.soundDragSustain = SoundDefOf.DesignateDragStandard;
			base.soundDragChanged = SoundDefOf.DesignateDragStandardChanged;
			base.useMouseIcon = true;
			base.soundSucceeded = SoundDefOf.DesignateClaim;
			base.hotKey = KeyBindingDefOf.Misc4;
		}

		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			return c.InBounds(base.Map) ? ((!c.Fogged(base.Map)) ? ((from t in c.GetThingList(base.Map)
			where this.CanDesignateThing(t).Accepted
			select t).Any() ? true : "MessageMustDesignateClaimable".Translate()) : false) : false;
		}

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

		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			Building building = t as Building;
			return building != null && building.Faction != Faction.OfPlayer && building.ClaimableBy(Faction.OfPlayer);
		}

		public override void DesignateThing(Thing t)
		{
			t.SetFaction(Faction.OfPlayer, null);
			CellRect.CellRectIterator iterator = t.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				MoteMaker.ThrowMetaPuffs(new TargetInfo(iterator.Current, base.Map, false));
				iterator.MoveNext();
			}
		}
	}
}
