using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	public static class FilthMaker
	{
		private static List<Filth> toBeRemoved = new List<Filth>();

		public static void MakeFilth(IntVec3 c, Map map, ThingDef filthDef, int count = 1)
		{
			for (int num = 0; num < count; num++)
			{
				FilthMaker.MakeFilth(c, map, filthDef, null, true);
			}
		}

		public static bool MakeFilth(IntVec3 c, Map map, ThingDef filthDef, string source, int count = 1)
		{
			bool flag = false;
			for (int num = 0; num < count; num++)
			{
				flag |= FilthMaker.MakeFilth(c, map, filthDef, Gen.YieldSingle(source), true);
			}
			return flag;
		}

		public static void MakeFilth(IntVec3 c, Map map, ThingDef filthDef, IEnumerable<string> sources)
		{
			FilthMaker.MakeFilth(c, map, filthDef, sources, true);
		}

		private static bool MakeFilth(IntVec3 c, Map map, ThingDef filthDef, IEnumerable<string> sources, bool shouldPropagate)
		{
			Filth filth = (Filth)(from t in c.GetThingList(map)
			where t.def == filthDef
			select t).FirstOrDefault();
			bool result;
			if (!c.Walkable(map) || (filth != null && !filth.CanBeThickened))
			{
				if (shouldPropagate)
				{
					List<IntVec3> list = GenAdj.AdjacentCells8WayRandomized();
					for (int i = 0; i < 8; i++)
					{
						IntVec3 c2 = c + list[i];
						if (c2.InBounds(map) && FilthMaker.MakeFilth(c2, map, filthDef, sources, false))
							goto IL_009b;
					}
				}
				if (filth != null)
				{
					filth.AddSources(sources);
				}
				result = false;
			}
			else
			{
				if (filth != null)
				{
					filth.ThickenFilth();
					filth.AddSources(sources);
				}
				else
				{
					Filth filth2 = (Filth)ThingMaker.MakeThing(filthDef, null);
					filth2.AddSources(sources);
					GenSpawn.Spawn(filth2, c, map);
				}
				result = true;
			}
			goto IL_010e;
			IL_009b:
			result = true;
			goto IL_010e;
			IL_010e:
			return result;
		}

		public static void RemoveAllFilth(IntVec3 c, Map map)
		{
			FilthMaker.toBeRemoved.Clear();
			List<Thing> thingList = c.GetThingList(map);
			for (int i = 0; i < thingList.Count; i++)
			{
				Filth filth = thingList[i] as Filth;
				if (filth != null)
				{
					FilthMaker.toBeRemoved.Add(filth);
				}
			}
			for (int j = 0; j < FilthMaker.toBeRemoved.Count; j++)
			{
				FilthMaker.toBeRemoved[j].Destroy(DestroyMode.Vanish);
			}
			FilthMaker.toBeRemoved.Clear();
		}
	}
}
