﻿using System;

namespace RimWorld
{
	public class Blueprint_Door : Blueprint_Build
	{
		public Blueprint_Door()
		{
		}

		public override void Draw()
		{
			base.Rotation = Building_Door.DoorRotationAt(base.Position, base.Map);
			base.Draw();
		}
	}
}
