#define ENABLE_PROFILER
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using Verse;
using Verse.Noise;

namespace RimWorld
{
	public class SteadyAtmosphereEffects
	{
		private Map map;

		private ModuleBase snowNoise = null;

		private int cycleIndex = 0;

		private float outdoorMeltAmount;

		private float snowRate;

		private float rainRate;

		private float deteriorationRate;

		private const float MapFractionCheckPerTick = 0.0006f;

		private const float RainFireCheckInterval = 97f;

		private const float RainFireChanceOverall = 0.02f;

		private const float RainFireChancePerBuilding = 0.2f;

		private const float SnowFallRateFactor = 0.046f;

		private const float SnowMeltRateFactor = 0.0058f;

		private static readonly FloatRange AutoIgnitionTemperatureRange = new FloatRange(240f, 1000f);

		private const float AutoIgnitionChanceFactor = 0.7f;

		private const float FireGlowRate = 0.33f;

		public SteadyAtmosphereEffects(Map map)
		{
			this.map = map;
		}

		public void SteadyAtmosphereEffectsTick()
		{
			Profiler.BeginSample("Init");
			if ((float)Find.TickManager.TicksGame % 97.0 == 0.0 && Rand.Value < 0.019999999552965164)
			{
				this.RollForRainFire();
			}
			this.outdoorMeltAmount = this.MeltAmountAt(this.map.mapTemperature.OutdoorTemp);
			this.snowRate = this.map.weatherManager.SnowRate;
			this.rainRate = this.map.weatherManager.RainRate;
			this.deteriorationRate = Mathf.Lerp(1f, 5f, this.rainRate);
			int num = Mathf.RoundToInt((float)((float)this.map.Area * 0.00060000002849847078));
			int area = this.map.Area;
			Profiler.EndSample();
			Profiler.BeginSample("DoCells");
			for (int num2 = 0; num2 < num; num2++)
			{
				if (this.cycleIndex >= area)
				{
					this.cycleIndex = 0;
				}
				Profiler.BeginSample("Get cell");
				IntVec3 c = this.map.cellsInRandomOrder.Get(this.cycleIndex);
				Profiler.EndSample();
				Profiler.BeginSample("Affect cell");
				this.DoCellSteadyEffects(c);
				Profiler.EndSample();
				this.cycleIndex++;
			}
			Profiler.EndSample();
		}

		private void DoCellSteadyEffects(IntVec3 c)
		{
			Room room = c.GetRoom(this.map, RegionType.Set_All);
			bool flag = this.map.roofGrid.Roofed(c);
			bool flag2 = room != null && room.UsesOutdoorTemperature;
			if (room == null || flag2)
			{
				Profiler.BeginSample("Roomless or Outdoors");
				if (this.outdoorMeltAmount > 0.0)
				{
					this.map.snowGrid.AddDepth(c, (float)(0.0 - this.outdoorMeltAmount));
				}
				if (!flag && this.snowRate > 0.0010000000474974513)
				{
					this.AddFallenSnowAt(c, (float)(0.046000000089406967 * this.map.weatherManager.SnowRate));
				}
				Profiler.EndSample();
			}
			if (room != null)
			{
				if (flag2)
				{
					Profiler.BeginSample("Outdoors");
					if (!flag)
					{
						List<Thing> thingList = c.GetThingList(this.map);
						for (int i = 0; i < thingList.Count; i++)
						{
							Thing thing = thingList[i];
							Filth filth = thing as Filth;
							if (filth != null)
							{
								if (thing.def.filth.rainWashes && Rand.Value < this.rainRate)
								{
									((Filth)thing).ThinFilth();
								}
							}
							else
							{
								Corpse corpse = thing as Corpse;
								if (corpse != null && corpse.InnerPawn.apparel != null)
								{
									List<Apparel> wornApparel = corpse.InnerPawn.apparel.WornApparel;
									for (int j = 0; j < wornApparel.Count; j++)
									{
										this.TryDoDeteriorate(wornApparel[j], c, false);
									}
								}
								this.TryDoDeteriorate(thing, c, true);
							}
						}
					}
					Profiler.EndSample();
				}
				else
				{
					Profiler.BeginSample("Indoors");
					float temperature = room.Temperature;
					if (temperature > 0.0)
					{
						float num = this.MeltAmountAt(temperature);
						if (num > 0.0)
						{
							this.map.snowGrid.AddDepth(c, (float)(0.0 - num));
						}
						if (room.RegionType.Passable())
						{
							float num2 = temperature;
							FloatRange autoIgnitionTemperatureRange = SteadyAtmosphereEffects.AutoIgnitionTemperatureRange;
							if (num2 > autoIgnitionTemperatureRange.min)
							{
								float value = Rand.Value;
								if (value < SteadyAtmosphereEffects.AutoIgnitionTemperatureRange.InverseLerpThroughRange(temperature) * 0.699999988079071 && Rand.Chance(FireUtility.ChanceToStartFireIn(c, this.map)))
								{
									FireUtility.TryStartFireIn(c, this.map, 0.1f);
								}
								if (value < 0.33000001311302185)
								{
									MoteMaker.ThrowHeatGlow(c, this.map, 2.3f);
								}
							}
						}
					}
					Profiler.EndSample();
				}
			}
			Profiler.BeginSample("GameConditions");
			List<GameCondition> activeConditions = this.map.gameConditionManager.ActiveConditions;
			for (int k = 0; k < activeConditions.Count; k++)
			{
				activeConditions[k].DoCellSteadyEffects(c);
			}
			Profiler.EndSample();
		}

		public static bool InDeterioratingPosition(Thing t)
		{
			return (byte)((!t.Position.Roofed(t.Map)) ? ((!SteadyAtmosphereEffects.ProtectedByEdifice(t.Position, t.Map)) ? 1 : 0) : 0) != 0;
		}

		private static bool ProtectedByEdifice(IntVec3 c, Map map)
		{
			Building edifice = c.GetEdifice(map);
			return (byte)((edifice != null && edifice.def.building != null && edifice.def.building.preventDeteriorationOnTop) ? 1 : 0) != 0;
		}

		private float MeltAmountAt(float temperature)
		{
			return (float)((!(temperature < 0.0)) ? ((!(temperature < 10.0)) ? (temperature * 0.0057999999262392521) : (temperature * temperature * 0.0057999999262392521 * 0.10000000149011612)) : 0.0);
		}

		public void AddFallenSnowAt(IntVec3 c, float baseAmount)
		{
			if (this.snowNoise == null)
			{
				this.snowNoise = new Perlin(0.039999999105930328, 2.0, 0.5, 5, Rand.Range(0, 651431), QualityMode.Medium);
			}
			float value = this.snowNoise.GetValue(c);
			value = (float)(value + 1.0);
			value = (float)(value * 0.5);
			if (value < 0.5)
			{
				value = 0.5f;
			}
			float depthToAdd = baseAmount * value;
			this.map.snowGrid.AddDepth(c, depthToAdd);
		}

		public static float FinalDeteriorationRate(Thing t)
		{
			return (float)(t.def.CanEverDeteriorate ? t.GetStatValue(StatDefOf.DeteriorationRate, true) : 0.0);
		}

		private void TryDoDeteriorate(Thing t, IntVec3 c, bool checkEdifice)
		{
			float num = SteadyAtmosphereEffects.FinalDeteriorationRate(t);
			if (!(num < 0.0010000000474974513))
			{
				float num2 = (float)(this.deteriorationRate * num / 36.0);
				if (Rand.Value < num2)
				{
					if (checkEdifice && SteadyAtmosphereEffects.ProtectedByEdifice(c, t.Map))
						return;
					t.TakeDamage(new DamageInfo(DamageDefOf.Deterioration, 1, -1f, null, null, null, DamageInfo.SourceCategory.ThingOrUnknown));
				}
			}
		}

		private void RollForRainFire()
		{
			float chance = (float)(0.20000000298023224 * (float)this.map.listerBuildings.allBuildingsColonistElecFire.Count * this.map.weatherManager.RainRate);
			if (Rand.Chance(chance))
			{
				Building building = this.map.listerBuildings.allBuildingsColonistElecFire.RandomElement();
				if (!this.map.roofGrid.Roofed(building.Position))
				{
					ShortCircuitUtility.TryShortCircuitInRain(building);
				}
			}
		}
	}
}
