using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld.BaseGen
{
	public static class BaseGenUtility
	{
		public static ThingDef RandomCheapWallStuff(Faction faction, bool notVeryFlammable = false)
		{
			TechLevel techLevel = (faction != null) ? faction.def.techLevel : TechLevel.Spacer;
			return BaseGenUtility.RandomCheapWallStuff(techLevel, notVeryFlammable);
		}

		public static ThingDef RandomCheapWallStuff(TechLevel techLevel, bool notVeryFlammable = false)
		{
			return (!techLevel.IsNeolithicOrWorse()) ? (from d in DefDatabase<ThingDef>.AllDefsListForReading
			where BaseGenUtility.IsCheapWallStuff(d) && (!notVeryFlammable || d.BaseFlammability < 0.5)
			select d).RandomElement() : ThingDefOf.WoodLog;
		}

		public static bool IsCheapWallStuff(ThingDef d)
		{
			return d.IsStuff && d.stuffProps.CanMake(ThingDefOf.Wall) && d.BaseMarketValue / d.VolumePerUnit < 5.0;
		}

		public static ThingDef RandomHightechWallStuff()
		{
			return (!(Rand.Value < 0.15000000596046448)) ? ThingDefOf.Steel : ThingDefOf.Plasteel;
		}

		public static TerrainDef RandomHightechFloorDef()
		{
			return Rand.Element(TerrainDefOf.Concrete, TerrainDefOf.Concrete, TerrainDefOf.PavedTile, TerrainDefOf.PavedTile, TerrainDefOf.MetalTile);
		}

		public static TerrainDef RandomBasicFloorDef(Faction faction, bool allowCarpet = false)
		{
			return (!allowCarpet || (faction != null && faction.def.techLevel.IsNeolithicOrWorse()) || !Rand.Chance(0.1f)) ? Rand.Element(TerrainDefOf.MetalTile, TerrainDefOf.PavedTile, TerrainDefOf.WoodPlankFloor, TerrainDefOf.TileSandstone) : (from x in DefDatabase<TerrainDef>.AllDefsListForReading
			where x.IsCarpet
			select x).RandomElement();
		}

		public static TerrainDef CorrespondingTerrainDef(ThingDef stuffDef, bool beautiful)
		{
			TerrainDef terrainDef = null;
			List<TerrainDef> allDefsListForReading = DefDatabase<TerrainDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				if (allDefsListForReading[i].costList != null)
				{
					for (int j = 0; j < allDefsListForReading[i].costList.Count; j++)
					{
						if (allDefsListForReading[i].costList[j].thingDef == stuffDef && (terrainDef == null || ((!beautiful) ? (terrainDef.statBases.GetStatOffsetFromList(StatDefOf.Beauty) > allDefsListForReading[i].statBases.GetStatOffsetFromList(StatDefOf.Beauty)) : (terrainDef.statBases.GetStatOffsetFromList(StatDefOf.Beauty) < allDefsListForReading[i].statBases.GetStatOffsetFromList(StatDefOf.Beauty)))))
						{
							terrainDef = allDefsListForReading[i];
						}
					}
				}
			}
			if (terrainDef == null)
			{
				terrainDef = TerrainDefOf.Concrete;
			}
			return terrainDef;
		}

		public static TerrainDef RegionalRockTerrainDef(int tile, bool beautiful)
		{
			ThingDef thingDef = Find.World.NaturalRockTypesIn(tile).RandomElementWithFallback(null);
			ThingDef thingDef2 = (thingDef == null) ? null : thingDef.building.mineableThing;
			ThingDef stuffDef = (thingDef2 == null || thingDef2.butcherProducts == null || thingDef2.butcherProducts.Count <= 0) ? null : thingDef2.butcherProducts[0].thingDef;
			return BaseGenUtility.CorrespondingTerrainDef(stuffDef, beautiful);
		}

		public static bool AnyDoorAdjacentCardinalTo(IntVec3 cell, Map map)
		{
			int num = 0;
			bool result;
			while (true)
			{
				if (num < 4)
				{
					IntVec3 c = cell + GenAdj.CardinalDirections[num];
					if (c.InBounds(map) && c.GetDoor(map) != null)
					{
						result = true;
						break;
					}
					num++;
					continue;
				}
				result = false;
				break;
			}
			return result;
		}

		public static bool AnyDoorAdjacentCardinalTo(CellRect rect, Map map)
		{
			foreach (IntVec3 item in rect.AdjacentCellsCardinal)
			{
				if (item.InBounds(map) && item.GetDoor(map) != null)
				{
					return true;
				}
			}
			return false;
		}

		public static ThingDef WallStuffAt(IntVec3 c, Map map)
		{
			Building edifice = c.GetEdifice(map);
			return (edifice == null || edifice.def != ThingDefOf.Wall) ? null : edifice.Stuff;
		}
	}
}
