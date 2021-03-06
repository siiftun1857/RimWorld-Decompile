﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Verse;

namespace RimWorld
{
	public static class ThingDefGenerator_Meat
	{
		public static IEnumerable<ThingDef> ImpliedMeatDefs()
		{
			foreach (ThingDef sourceDef in DefDatabase<ThingDef>.AllDefs.ToList<ThingDef>())
			{
				if (sourceDef.category == ThingCategory.Pawn)
				{
					if (sourceDef.race.useMeatFrom == null)
					{
						if (!sourceDef.race.IsFlesh)
						{
							DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(sourceDef.race, "meatDef", "Steel");
						}
						else
						{
							ThingDef d = new ThingDef();
							d.resourceReadoutPriority = ResourceCountPriority.Middle;
							d.category = ThingCategory.Item;
							d.thingClass = typeof(ThingWithComps);
							d.graphicData = new GraphicData();
							d.graphicData.graphicClass = typeof(Graphic_StackCount);
							d.useHitPoints = true;
							d.selectable = true;
							d.SetStatBaseValue(StatDefOf.MaxHitPoints, 100f);
							d.altitudeLayer = AltitudeLayer.Item;
							d.stackLimit = 75;
							d.comps.Add(new CompProperties_Forbiddable());
							CompProperties_Rottable rotProps = new CompProperties_Rottable();
							rotProps.daysToRotStart = 2f;
							rotProps.rotDestroys = true;
							d.comps.Add(rotProps);
							d.tickerType = TickerType.Rare;
							d.SetStatBaseValue(StatDefOf.Beauty, -4f);
							d.alwaysHaulable = true;
							d.rotatable = false;
							d.pathCost = 15;
							d.drawGUIOverlay = true;
							d.socialPropernessMatters = true;
							d.modContentPack = sourceDef.modContentPack;
							d.category = ThingCategory.Item;
							if (sourceDef.race.Humanlike)
							{
								d.description = "MeatHumanDesc".Translate(new object[]
								{
									sourceDef.label
								});
							}
							else if (sourceDef.race.FleshType == FleshTypeDefOf.Insectoid)
							{
								d.description = "MeatInsectDesc".Translate(new object[]
								{
									sourceDef.label
								});
							}
							else
							{
								d.description = "MeatDesc".Translate(new object[]
								{
									sourceDef.label
								});
							}
							d.useHitPoints = true;
							d.SetStatBaseValue(StatDefOf.MaxHitPoints, 60f);
							d.SetStatBaseValue(StatDefOf.DeteriorationRate, 6f);
							d.SetStatBaseValue(StatDefOf.Mass, 0.03f);
							d.SetStatBaseValue(StatDefOf.Flammability, 0.5f);
							d.SetStatBaseValue(StatDefOf.Nutrition, 0.05f);
							d.SetStatBaseValue(StatDefOf.FoodPoisonChanceFixedHuman, 0.02f);
							d.BaseMarketValue = sourceDef.race.meatMarketValue;
							if (d.thingCategories == null)
							{
								d.thingCategories = new List<ThingCategoryDef>();
							}
							DirectXmlCrossRefLoader.RegisterListWantsCrossRef<ThingCategoryDef>(d.thingCategories, "MeatRaw", d);
							d.ingestible = new IngestibleProperties();
							d.ingestible.parent = d;
							d.ingestible.foodType = FoodTypeFlags.Meat;
							d.ingestible.preferability = FoodPreferability.RawBad;
							DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(d.ingestible, "tasteThought", ThoughtDefOf.AteRawFood.defName);
							d.ingestible.ingestEffect = EffecterDefOf.EatMeat;
							d.ingestible.ingestSound = SoundDefOf.RawMeat_Eat;
							d.ingestible.specialThoughtDirect = sourceDef.race.FleshType.ateDirect;
							d.ingestible.specialThoughtAsIngredient = sourceDef.race.FleshType.ateAsIngredient;
							d.graphicData.color = sourceDef.race.meatColor;
							if (sourceDef.race.Humanlike)
							{
								d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Human";
							}
							else if (sourceDef.race.FleshType == FleshTypeDefOf.Insectoid)
							{
								d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Insect";
							}
							else if (sourceDef.race.baseBodySize < 0.7f)
							{
								d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Small";
							}
							else
							{
								d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Big";
							}
							d.defName = "Meat_" + sourceDef.defName;
							if (sourceDef.race.meatLabel.NullOrEmpty())
							{
								d.label = "MeatLabel".Translate(new object[]
								{
									sourceDef.label
								});
							}
							else
							{
								d.label = sourceDef.race.meatLabel;
							}
							d.ingestible.sourceDef = sourceDef;
							sourceDef.race.meatDef = d;
							yield return d;
						}
					}
				}
			}
			yield break;
		}

		[CompilerGenerated]
		private sealed class <ImpliedMeatDefs>c__Iterator0 : IEnumerable, IEnumerable<ThingDef>, IEnumerator, IDisposable, IEnumerator<ThingDef>
		{
			internal List<ThingDef>.Enumerator $locvar0;

			internal ThingDef <sourceDef>__1;

			internal ThingDef <d>__2;

			internal CompProperties_Rottable <rotProps>__2;

			internal ThingDef $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <ImpliedMeatDefs>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				bool flag = false;
				switch (num)
				{
				case 0u:
					enumerator = DefDatabase<ThingDef>.AllDefs.ToList<ThingDef>().GetEnumerator();
					num = 4294967293u;
					break;
				case 1u:
					break;
				default:
					return false;
				}
				try
				{
					switch (num)
					{
					}
					while (enumerator.MoveNext())
					{
						sourceDef = enumerator.Current;
						if (sourceDef.category == ThingCategory.Pawn)
						{
							if (sourceDef.race.useMeatFrom == null)
							{
								if (sourceDef.race.IsFlesh)
								{
									d = new ThingDef();
									d.resourceReadoutPriority = ResourceCountPriority.Middle;
									d.category = ThingCategory.Item;
									d.thingClass = typeof(ThingWithComps);
									d.graphicData = new GraphicData();
									d.graphicData.graphicClass = typeof(Graphic_StackCount);
									d.useHitPoints = true;
									d.selectable = true;
									d.SetStatBaseValue(StatDefOf.MaxHitPoints, 100f);
									d.altitudeLayer = AltitudeLayer.Item;
									d.stackLimit = 75;
									d.comps.Add(new CompProperties_Forbiddable());
									rotProps = new CompProperties_Rottable();
									rotProps.daysToRotStart = 2f;
									rotProps.rotDestroys = true;
									d.comps.Add(rotProps);
									d.tickerType = TickerType.Rare;
									d.SetStatBaseValue(StatDefOf.Beauty, -4f);
									d.alwaysHaulable = true;
									d.rotatable = false;
									d.pathCost = 15;
									d.drawGUIOverlay = true;
									d.socialPropernessMatters = true;
									d.modContentPack = sourceDef.modContentPack;
									d.category = ThingCategory.Item;
									if (sourceDef.race.Humanlike)
									{
										d.description = "MeatHumanDesc".Translate(new object[]
										{
											sourceDef.label
										});
									}
									else if (sourceDef.race.FleshType == FleshTypeDefOf.Insectoid)
									{
										d.description = "MeatInsectDesc".Translate(new object[]
										{
											sourceDef.label
										});
									}
									else
									{
										d.description = "MeatDesc".Translate(new object[]
										{
											sourceDef.label
										});
									}
									d.useHitPoints = true;
									d.SetStatBaseValue(StatDefOf.MaxHitPoints, 60f);
									d.SetStatBaseValue(StatDefOf.DeteriorationRate, 6f);
									d.SetStatBaseValue(StatDefOf.Mass, 0.03f);
									d.SetStatBaseValue(StatDefOf.Flammability, 0.5f);
									d.SetStatBaseValue(StatDefOf.Nutrition, 0.05f);
									d.SetStatBaseValue(StatDefOf.FoodPoisonChanceFixedHuman, 0.02f);
									d.BaseMarketValue = sourceDef.race.meatMarketValue;
									if (d.thingCategories == null)
									{
										d.thingCategories = new List<ThingCategoryDef>();
									}
									DirectXmlCrossRefLoader.RegisterListWantsCrossRef<ThingCategoryDef>(d.thingCategories, "MeatRaw", d);
									d.ingestible = new IngestibleProperties();
									d.ingestible.parent = d;
									d.ingestible.foodType = FoodTypeFlags.Meat;
									d.ingestible.preferability = FoodPreferability.RawBad;
									DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(d.ingestible, "tasteThought", ThoughtDefOf.AteRawFood.defName);
									d.ingestible.ingestEffect = EffecterDefOf.EatMeat;
									d.ingestible.ingestSound = SoundDefOf.RawMeat_Eat;
									d.ingestible.specialThoughtDirect = sourceDef.race.FleshType.ateDirect;
									d.ingestible.specialThoughtAsIngredient = sourceDef.race.FleshType.ateAsIngredient;
									d.graphicData.color = sourceDef.race.meatColor;
									if (sourceDef.race.Humanlike)
									{
										d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Human";
									}
									else if (sourceDef.race.FleshType == FleshTypeDefOf.Insectoid)
									{
										d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Insect";
									}
									else if (sourceDef.race.baseBodySize < 0.7f)
									{
										d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Small";
									}
									else
									{
										d.graphicData.texPath = "Things/Item/Resource/MeatFoodRaw/Meat_Big";
									}
									d.defName = "Meat_" + sourceDef.defName;
									if (sourceDef.race.meatLabel.NullOrEmpty())
									{
										d.label = "MeatLabel".Translate(new object[]
										{
											sourceDef.label
										});
									}
									else
									{
										d.label = sourceDef.race.meatLabel;
									}
									d.ingestible.sourceDef = sourceDef;
									sourceDef.race.meatDef = d;
									this.$current = d;
									if (!this.$disposing)
									{
										this.$PC = 1;
									}
									flag = true;
									return true;
								}
								DirectXmlCrossRefLoader.RegisterObjectWantsCrossRef(sourceDef.race, "meatDef", "Steel");
							}
						}
					}
				}
				finally
				{
					if (!flag)
					{
						((IDisposable)enumerator).Dispose();
					}
				}
				this.$PC = -1;
				return false;
			}

			ThingDef IEnumerator<ThingDef>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			public void Dispose()
			{
				uint num = (uint)this.$PC;
				this.$disposing = true;
				this.$PC = -1;
				switch (num)
				{
				case 1u:
					try
					{
					}
					finally
					{
						((IDisposable)enumerator).Dispose();
					}
					break;
				}
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Verse.ThingDef>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<ThingDef> IEnumerable<ThingDef>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				return new ThingDefGenerator_Meat.<ImpliedMeatDefs>c__Iterator0();
			}
		}
	}
}
