﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class Graphic_LinkedTransmitterOverlay : Graphic_Linked
	{
		public Graphic_LinkedTransmitterOverlay()
		{
		}

		public Graphic_LinkedTransmitterOverlay(Graphic subGraphic) : base(subGraphic)
		{
		}

		public override bool ShouldLinkWith(IntVec3 c, Thing parent)
		{
			return c.InBounds(parent.Map) && parent.Map.powerNetGrid.TransmittedPowerNetAt(c) != null;
		}

		public override void Print(SectionLayer layer, Thing parent)
		{
			CellRect.CellRectIterator iterator = parent.OccupiedRect().GetIterator();
			while (!iterator.Done())
			{
				IntVec3 cell = iterator.Current;
				Vector3 center = cell.ToVector3ShiftedWithAltitude(AltitudeLayer.MapDataOverlay);
				Printer_Plane.PrintPlane(layer, center, new Vector2(1f, 1f), base.LinkedDrawMatFrom(parent, cell), 0f, false, null, null, 0.01f, 0f);
				iterator.MoveNext();
			}
		}
	}
}
