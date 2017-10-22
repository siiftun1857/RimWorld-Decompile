using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace RimWorld
{
	public static class PawnWeaponGenerator
	{
		private static List<ThingStuffPair> allWeaponPairs;

		private static List<ThingStuffPair> workingWeapons = new List<ThingStuffPair>();

		public static void Reset()
		{
			Predicate<ThingDef> isWeapon = (Predicate<ThingDef>)((ThingDef td) => td.equipmentType == EquipmentType.Primary && td.canBeSpawningInventory && !td.weaponTags.NullOrEmpty());
			PawnWeaponGenerator.allWeaponPairs = ThingStuffPair.AllWith(isWeapon);
			foreach (ThingDef item in from td in DefDatabase<ThingDef>.AllDefs
			where isWeapon(td)
			select td)
			{
				float num = PawnWeaponGenerator.allWeaponPairs.Where((Func<ThingStuffPair, bool>)((ThingStuffPair pa) => pa.thing == item)).Sum((Func<ThingStuffPair, float>)((ThingStuffPair pa) => pa.Commonality));
				float num2 = item.generateCommonality / num;
				if (num2 != 1.0)
				{
					for (int i = 0; i < PawnWeaponGenerator.allWeaponPairs.Count; i++)
					{
						ThingStuffPair thingStuffPair = PawnWeaponGenerator.allWeaponPairs[i];
						if (thingStuffPair.thing == item)
						{
							PawnWeaponGenerator.allWeaponPairs[i] = new ThingStuffPair(thingStuffPair.thing, thingStuffPair.stuff, thingStuffPair.commonalityMultiplier * num2);
						}
					}
				}
			}
		}

		public static void TryGenerateWeaponFor(Pawn pawn)
		{
			PawnWeaponGenerator.workingWeapons.Clear();
			if (pawn.kindDef.weaponTags != null && pawn.kindDef.weaponTags.Count != 0 && pawn.RaceProps.ToolUser && pawn.health.capacities.CapableOf(PawnCapacityDefOf.Manipulation) && (pawn.story == null || !pawn.story.WorkTagIsDisabled(WorkTags.Violent)))
			{
				float randomInRange = pawn.kindDef.weaponMoney.RandomInRange;
				for (int i = 0; i < PawnWeaponGenerator.allWeaponPairs.Count; i++)
				{
					ThingStuffPair w2 = PawnWeaponGenerator.allWeaponPairs[i];
					if (!(w2.Price > randomInRange) && pawn.kindDef.weaponTags.Any((Predicate<string>)((string tag) => w2.thing.weaponTags.Contains(tag))) && (!(w2.thing.generateAllowChance < 1.0) || !(Rand.ValueSeeded(pawn.thingIDNumber ^ 28554824) > w2.thing.generateAllowChance)))
					{
						PawnWeaponGenerator.workingWeapons.Add(w2);
					}
				}
				if (PawnWeaponGenerator.workingWeapons.Count != 0)
				{
					pawn.equipment.DestroyAllEquipment(DestroyMode.Vanish);
					ThingStuffPair thingStuffPair = default(ThingStuffPair);
					if (((IEnumerable<ThingStuffPair>)PawnWeaponGenerator.workingWeapons).TryRandomElementByWeight<ThingStuffPair>((Func<ThingStuffPair, float>)((ThingStuffPair w) => w.Commonality * w.Price), out thingStuffPair))
					{
						ThingWithComps thingWithComps = (ThingWithComps)ThingMaker.MakeThing(thingStuffPair.thing, thingStuffPair.stuff);
						PawnGenerator.PostProcessGeneratedGear(thingWithComps, pawn);
						pawn.equipment.AddEquipment(thingWithComps);
					}
					PawnWeaponGenerator.workingWeapons.Clear();
				}
			}
		}

		public static bool IsDerpWeapon(ThingDef thing, ThingDef stuff)
		{
			bool result;
			if (stuff == null)
			{
				result = false;
			}
			else
			{
				if (thing.IsMeleeWeapon)
				{
					if (thing.tools.NullOrEmpty())
					{
						result = false;
						goto IL_0083;
					}
					DamageDef damageDef = ThingUtility.PrimaryMeleeWeaponDamageType(thing);
					if (damageDef == null)
					{
						result = false;
						goto IL_0083;
					}
					DamageArmorCategoryDef armorCategory = damageDef.armorCategory;
					if (armorCategory != null && armorCategory.multStat != null && stuff.GetStatValueAbstract(armorCategory.multStat, null) < 0.699999988079071)
					{
						result = true;
						goto IL_0083;
					}
				}
				result = false;
			}
			goto IL_0083;
			IL_0083:
			return result;
		}

		public static float CheapestNonDerpPriceFor(ThingDef weaponDef)
		{
			float num = 9999999f;
			for (int i = 0; i < PawnWeaponGenerator.allWeaponPairs.Count; i++)
			{
				ThingStuffPair thingStuffPair = PawnWeaponGenerator.allWeaponPairs[i];
				if (thingStuffPair.thing == weaponDef && !PawnWeaponGenerator.IsDerpWeapon(thingStuffPair.thing, thingStuffPair.stuff) && thingStuffPair.Price < num)
				{
					num = thingStuffPair.Price;
				}
			}
			return num;
		}

		internal static void MakeTableWeaponPairs()
		{
			DebugTables.MakeTablesDialog(from p in PawnWeaponGenerator.allWeaponPairs
			orderby p.thing.defName descending
			select p, new TableDataGetter<ThingStuffPair>("thing", (Func<ThingStuffPair, string>)((ThingStuffPair p) => p.thing.defName)), new TableDataGetter<ThingStuffPair>("stuff", (Func<ThingStuffPair, string>)((ThingStuffPair p) => (p.stuff == null) ? "" : p.stuff.defName)), new TableDataGetter<ThingStuffPair>("price", (Func<ThingStuffPair, string>)((ThingStuffPair p) => p.Price.ToString())), new TableDataGetter<ThingStuffPair>("commonality", (Func<ThingStuffPair, string>)((ThingStuffPair p) => p.Commonality.ToString("F5"))), new TableDataGetter<ThingStuffPair>("commMult", (Func<ThingStuffPair, string>)((ThingStuffPair p) => p.commonalityMultiplier.ToString("F5"))), new TableDataGetter<ThingStuffPair>("def-commonality", (Func<ThingStuffPair, string>)((ThingStuffPair p) => p.thing.generateCommonality.ToString("F2"))), new TableDataGetter<ThingStuffPair>("derp", (Func<ThingStuffPair, string>)((ThingStuffPair p) => (!PawnWeaponGenerator.IsDerpWeapon(p.thing, p.stuff)) ? "" : "D")));
		}

		internal static void MakeTableWeaponPairsByThing()
		{
			PawnApparelGenerator.MakeTablePairsByThing(PawnWeaponGenerator.allWeaponPairs);
		}
	}
}
