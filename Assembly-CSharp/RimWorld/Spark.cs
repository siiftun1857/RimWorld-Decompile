﻿using System;
using Verse;

namespace RimWorld
{
	public class Spark : Projectile
	{
		public Spark()
		{
		}

		protected override void Impact(Thing hitThing)
		{
			Map map = base.Map;
			base.Impact(hitThing);
			FireUtility.TryStartFireIn(base.Position, map, 0.1f);
		}
	}
}
