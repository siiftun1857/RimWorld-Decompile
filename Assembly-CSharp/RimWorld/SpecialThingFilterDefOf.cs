﻿using System;
using Verse;

namespace RimWorld
{
	[DefOf]
	public static class SpecialThingFilterDefOf
	{
		public static SpecialThingFilterDef AllowDeadmansApparel;

		public static SpecialThingFilterDef AllowNonDeadmansApparel;

		static SpecialThingFilterDefOf()
		{
			DefOfHelper.EnsureInitializedInCtor(typeof(SpecialThingFilterDefOf));
		}
	}
}
