﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class BuildingProperties
	{
		public bool isEdifice = true;

		[NoTranslate]
		public List<string> buildingTags = new List<string>();

		public bool isInert;

		private bool deconstructible = true;

		public bool alwaysDeconstructible;

		public bool claimable = true;

		public bool isSittable;

		public SoundDef soundAmbient;

		public ConceptDef spawnedConceptLearnOpportunity;

		public ConceptDef boughtConceptLearnOpportunity;

		public bool expandHomeArea = true;

		public Type blueprintClass = typeof(Blueprint_Build);

		public GraphicData blueprintGraphicData;

		public float uninstallWork = 200f;

		public bool wantsHopperAdjacent;

		public bool allowWireConnection = true;

		public bool shipPart;

		public bool canPlaceOverImpassablePlant = true;

		public float heatPerTickWhileWorking;

		public bool canBuildNonEdificesUnder = true;

		public bool canPlaceOverWall;

		public bool allowAutoroof = true;

		public bool preventDeteriorationOnTop;

		public bool preventDeteriorationInside;

		public bool isMealSource;

		public bool isNaturalRock;

		public bool isResourceRock;

		public bool repairable = true;

		public float roofCollapseDamageMultiplier = 1f;

		public bool hasFuelingPort;

		public ThingDef smoothedThing;

		[Unsaved]
		public ThingDef unsmoothedThing;

		public TerrainDef naturalTerrain;

		public TerrainDef leaveTerrain;

		public bool isPlayerEjectable;

		public GraphicData fullGraveGraphicData;

		public float bed_healPerDay;

		public bool bed_defaultMedical;

		public bool bed_showSleeperBody;

		public bool bed_humanlike = true;

		public float bed_maxBodySize = 9999f;

		public bool bed_caravansCanUse;

		public float nutritionCostPerDispense;

		public SoundDef soundDispense;

		public ThingDef turretGunDef;

		public float turretBurstWarmupTime;

		public float turretBurstCooldownTime = -1f;

		[NoTranslate]
		public string turretTopGraphicPath;

		[Unsaved]
		public Material turretTopMat;

		public float turretTopDrawSize = 2f;

		public Vector2 turretTopOffset;

		public bool ai_combatDangerous;

		public bool ai_chillDestination = true;

		public SoundDef soundDoorOpenPowered;

		public SoundDef soundDoorClosePowered;

		public SoundDef soundDoorOpenManual;

		public SoundDef soundDoorCloseManual;

		[NoTranslate]
		public string sowTag;

		public ThingDef defaultPlantToGrow;

		public ThingDef mineableThing;

		public int mineableYield = 1;

		public float mineableNonMinedEfficiency = 0.7f;

		public float mineableDropChance = 1f;

		public bool mineableYieldWasteable = true;

		public float mineableScatterCommonality;

		public IntRange mineableScatterLumpSizeRange = new IntRange(20, 40);

		public StorageSettings fixedStorageSettings;

		public StorageSettings defaultStorageSettings;

		public bool ignoreStoredThingsBeauty;

		public bool isTrap;

		public bool trapDestroyOnSpring;

		public float trapPeacefulWildAnimalsSpringChanceFactor = 1f;

		public DamageArmorCategoryDef trapDamageCategory;

		public GraphicData trapUnarmedGraphicData;

		[Unsaved]
		public Graphic trapUnarmedGraphic;

		public float unpoweredWorkTableWorkSpeedFactor;

		public bool workSpeedPenaltyOutdoors;

		public bool workSpeedPenaltyTemperature;

		public IntRange watchBuildingStandDistanceRange = IntRange.one;

		public int watchBuildingStandRectWidth = 3;

		public JoyKindDef joyKind;

		public int haulToContainerDuration;

		public BuildingProperties()
		{
		}

		public bool SupportsPlants
		{
			get
			{
				return this.sowTag != null;
			}
		}

		public bool IsTurret
		{
			get
			{
				return this.turretGunDef != null;
			}
		}

		public bool IsDeconstructible
		{
			get
			{
				return this.alwaysDeconstructible || (!this.isNaturalRock && this.deconstructible);
			}
		}

		public bool IsMortar
		{
			get
			{
				if (!this.IsTurret)
				{
					return false;
				}
				List<VerbProperties> verbs = this.turretGunDef.Verbs;
				for (int i = 0; i < verbs.Count; i++)
				{
					if (verbs[i].isPrimary && verbs[i].defaultProjectile != null && verbs[i].defaultProjectile.projectile.flyOverhead)
					{
						return true;
					}
				}
				if (this.turretGunDef.HasComp(typeof(CompChangeableProjectile)))
				{
					if (this.turretGunDef.building.fixedStorageSettings.filter.Allows(ThingDefOf.Shell_HighExplosive))
					{
						return true;
					}
					foreach (ThingDef thingDef in this.turretGunDef.building.fixedStorageSettings.filter.AllowedThingDefs)
					{
						if (thingDef.projectileWhenLoaded != null && thingDef.projectileWhenLoaded.projectile.flyOverhead)
						{
							return true;
						}
					}
					return false;
				}
				return false;
			}
		}

		public IEnumerable<string> ConfigErrors(ThingDef parent)
		{
			if (this.isTrap && !this.isEdifice)
			{
				yield return "isTrap but is not edifice. Code will break.";
			}
			if (this.alwaysDeconstructible && !this.deconstructible)
			{
				yield return "alwaysDeconstructible=true but deconstructible=false";
			}
			if (parent.holdsRoof && !this.isEdifice)
			{
				yield return "holds roof but is not an edifice.";
			}
			yield break;
		}

		public void PostLoadSpecial(ThingDef parent)
		{
		}

		public void ResolveReferencesSpecial()
		{
			if (this.soundDoorOpenPowered == null)
			{
				this.soundDoorOpenPowered = SoundDefOf.Door_OpenPowered;
			}
			if (this.soundDoorClosePowered == null)
			{
				this.soundDoorClosePowered = SoundDefOf.Door_ClosePowered;
			}
			if (this.soundDoorOpenManual == null)
			{
				this.soundDoorOpenManual = SoundDefOf.Door_OpenManual;
			}
			if (this.soundDoorCloseManual == null)
			{
				this.soundDoorCloseManual = SoundDefOf.Door_CloseManual;
			}
			if (!this.turretTopGraphicPath.NullOrEmpty())
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					this.turretTopMat = MaterialPool.MatFrom(this.turretTopGraphicPath);
				});
			}
			if (this.fixedStorageSettings != null)
			{
				this.fixedStorageSettings.filter.ResolveReferences();
			}
			if (this.defaultStorageSettings == null && this.fixedStorageSettings != null)
			{
				this.defaultStorageSettings = new StorageSettings();
				this.defaultStorageSettings.CopyFrom(this.fixedStorageSettings);
			}
			if (this.defaultStorageSettings != null)
			{
				this.defaultStorageSettings.filter.ResolveReferences();
			}
		}

		public static void FinalizeInit()
		{
			List<ThingDef> allDefsListForReading = DefDatabase<ThingDef>.AllDefsListForReading;
			for (int i = 0; i < allDefsListForReading.Count; i++)
			{
				ThingDef thingDef = allDefsListForReading[i];
				if (thingDef.building != null)
				{
					if (thingDef.building.smoothedThing != null)
					{
						ThingDef thingDef2 = thingDef.building.smoothedThing;
						if (thingDef2.building == null)
						{
							Log.Error(string.Format("{0} is smoothable to non-building {1}", thingDef, thingDef2), false);
						}
						else if (thingDef2.building.unsmoothedThing == null || thingDef2.building.unsmoothedThing == thingDef)
						{
							thingDef2.building.unsmoothedThing = thingDef;
						}
						else
						{
							Log.Error(string.Format("{0} and {1} both smooth to {2}", thingDef, thingDef2.building.unsmoothedThing, thingDef2), false);
						}
					}
				}
			}
		}

		public IEnumerable<StatDrawEntry> SpecialDisplayStats(ThingDef parentDef)
		{
			if (this.joyKind != null)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Building, "StatsReport_JoyKind".Translate(), this.joyKind.LabelCap, 0, string.Empty);
			}
			if (parentDef.Minifiable)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Building, "StatsReport_WorkToUninstall".Translate(), this.uninstallWork.ToStringWorkAmount(), 0, string.Empty);
			}
			yield break;
		}

		[CompilerGenerated]
		private void <ResolveReferencesSpecial>m__0()
		{
			this.turretTopMat = MaterialPool.MatFrom(this.turretTopGraphicPath);
		}

		[CompilerGenerated]
		private sealed class <ConfigErrors>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
		{
			internal ThingDef parent;

			internal BuildingProperties $this;

			internal string $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <ConfigErrors>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					if (this.isTrap && !this.isEdifice)
					{
						this.$current = "isTrap but is not edifice. Code will break.";
						if (!this.$disposing)
						{
							this.$PC = 1;
						}
						return true;
					}
					break;
				case 1u:
					break;
				case 2u:
					goto IL_A7;
				case 3u:
					goto IL_E6;
				default:
					return false;
				}
				if (this.alwaysDeconstructible && !this.deconstructible)
				{
					this.$current = "alwaysDeconstructible=true but deconstructible=false";
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				}
				IL_A7:
				if (parent.holdsRoof && !this.isEdifice)
				{
					this.$current = "holds roof but is not an edifice.";
					if (!this.$disposing)
					{
						this.$PC = 3;
					}
					return true;
				}
				IL_E6:
				this.$PC = -1;
				return false;
			}

			string IEnumerator<string>.Current
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
				this.$disposing = true;
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				BuildingProperties.<ConfigErrors>c__Iterator0 <ConfigErrors>c__Iterator = new BuildingProperties.<ConfigErrors>c__Iterator0();
				<ConfigErrors>c__Iterator.$this = this;
				<ConfigErrors>c__Iterator.parent = parent;
				return <ConfigErrors>c__Iterator;
			}
		}

		[CompilerGenerated]
		private sealed class <SpecialDisplayStats>c__Iterator1 : IEnumerable, IEnumerable<StatDrawEntry>, IEnumerator, IDisposable, IEnumerator<StatDrawEntry>
		{
			internal ThingDef parentDef;

			internal BuildingProperties $this;

			internal StatDrawEntry $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <SpecialDisplayStats>c__Iterator1()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					if (this.joyKind != null)
					{
						this.$current = new StatDrawEntry(StatCategoryDefOf.Building, "StatsReport_JoyKind".Translate(), this.joyKind.LabelCap, 0, string.Empty);
						if (!this.$disposing)
						{
							this.$PC = 1;
						}
						return true;
					}
					break;
				case 1u:
					break;
				case 2u:
					goto IL_CD;
				default:
					return false;
				}
				if (parentDef.Minifiable)
				{
					this.$current = new StatDrawEntry(StatCategoryDefOf.Building, "StatsReport_WorkToUninstall".Translate(), this.uninstallWork.ToStringWorkAmount(), 0, string.Empty);
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				}
				IL_CD:
				this.$PC = -1;
				return false;
			}

			StatDrawEntry IEnumerator<StatDrawEntry>.Current
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
				this.$disposing = true;
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<RimWorld.StatDrawEntry>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<StatDrawEntry> IEnumerable<StatDrawEntry>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				BuildingProperties.<SpecialDisplayStats>c__Iterator1 <SpecialDisplayStats>c__Iterator = new BuildingProperties.<SpecialDisplayStats>c__Iterator1();
				<SpecialDisplayStats>c__Iterator.$this = this;
				<SpecialDisplayStats>c__Iterator.parentDef = parentDef;
				return <SpecialDisplayStats>c__Iterator;
			}
		}
	}
}
