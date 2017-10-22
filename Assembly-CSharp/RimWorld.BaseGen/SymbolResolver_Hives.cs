using System;
using System.Linq;
using Verse;

namespace RimWorld.BaseGen
{
	public class SymbolResolver_Hives : SymbolResolver
	{
		private static readonly IntRange DefaultHivesCountRange = new IntRange(1, 3);

		public override bool CanResolve(ResolveParams rp)
		{
			IntVec3 intVec = default(IntVec3);
			return (byte)(base.CanResolve(rp) ? (this.TryFindFirstHivePos(rp.rect, out intVec) ? 1 : 0) : 0) != 0;
		}

		public override void Resolve(ResolveParams rp)
		{
			IntVec3 loc = default(IntVec3);
			if (this.TryFindFirstHivePos(rp.rect, out loc))
			{
				int? hivesCount = rp.hivesCount;
				int num = (!hivesCount.HasValue) ? SymbolResolver_Hives.DefaultHivesCountRange.RandomInRange : hivesCount.Value;
				Hive hive = (Hive)ThingMaker.MakeThing(ThingDefOf.Hive, null);
				hive.SetFaction(Faction.OfInsects, null);
				if (rp.disableHives.HasValue && rp.disableHives.Value)
				{
					hive.active = false;
				}
				hive = (Hive)GenSpawn.Spawn(hive, loc, BaseGen.globalSettings.map);
				for (int i = 0; i < num - 1; i++)
				{
					Hive hive2 = default(Hive);
					if (hive.GetComp<CompSpawnerHives>().TrySpawnChildHive(true, out hive2))
					{
						hive = hive2;
					}
				}
			}
		}

		private bool TryFindFirstHivePos(CellRect rect, out IntVec3 pos)
		{
			Map map = BaseGen.globalSettings.map;
			return (from mc in rect.Cells
			where mc.Standable(map)
			select mc).TryRandomElement<IntVec3>(out pos);
		}
	}
}
