﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using RimWorld;
using UnityEngine;
using Verse.AI;

namespace Verse
{
	public class ThingDef : BuildableDef
	{
		public Type thingClass;

		public ThingCategory category;

		public TickerType tickerType;

		public int stackLimit = 1;

		public IntVec2 size = new IntVec2(1, 1);

		public bool destroyable = true;

		public bool rotatable = true;

		public bool smallVolume;

		public bool useHitPoints = true;

		public bool receivesSignals;

		public List<CompProperties> comps = new List<CompProperties>();

		public List<ThingDefCountClass> killedLeavings;

		public List<ThingDefCountClass> butcherProducts;

		public List<ThingDefCountClass> smeltProducts;

		public bool smeltable;

		public bool randomizeRotationOnSpawn;

		public List<DamageMultiplier> damageMultipliers;

		public bool isTechHediff;

		public RecipeMakerProperties recipeMaker;

		public ThingDef minifiedDef;

		public bool isUnfinishedThing;

		public bool leaveResourcesWhenKilled;

		public ThingDef slagDef;

		public bool isFrameInt;

		public IntVec3 interactionCellOffset = IntVec3.Zero;

		public bool hasInteractionCell;

		public ThingDef interactionCellIcon;

		public bool interactionCellIconReverse;

		public ThingDef filthLeaving;

		public bool forceDebugSpawnable;

		public bool intricate;

		public bool scatterableOnMapGen = true;

		public float deepCommonality;

		public int deepCountPerCell = 300;

		public int deepCountPerPortion = -1;

		public IntRange deepLumpSizeRange = IntRange.zero;

		public float generateCommonality = 1f;

		public float generateAllowChance = 1f;

		private bool canOverlapZones = true;

		public FloatRange startingHpRange = FloatRange.One;

		[NoTranslate]
		public List<string> thingSetMakerTags;

		public bool alwaysFlee;

		public List<RecipeDef> recipes;

		public GraphicData graphicData;

		public DrawerType drawerType = DrawerType.RealtimeOnly;

		public bool drawOffscreen;

		public ColorGenerator colorGenerator;

		public float hideAtSnowDepth = 99999f;

		public bool drawDamagedOverlay = true;

		public bool castEdgeShadows;

		public float staticSunShadowHeight;

		public bool selectable;

		public bool neverMultiSelect;

		public bool isAutoAttackableMapObject;

		public bool hasTooltip;

		public List<Type> inspectorTabs;

		[Unsaved]
		public List<InspectTabBase> inspectorTabsResolved;

		public bool seeThroughFog;

		public bool drawGUIOverlay;

		public ResourceCountPriority resourceReadoutPriority;

		public bool resourceReadoutAlwaysShow;

		public bool drawPlaceWorkersWhileSelected;

		public ConceptDef storedConceptLearnOpportunity;

		public float uiIconScale = 1f;

		public bool alwaysHaulable;

		public bool designateHaulable;

		public List<ThingCategoryDef> thingCategories;

		public bool mineable;

		public bool socialPropernessMatters;

		public bool stealable = true;

		public SoundDef soundDrop;

		public SoundDef soundPickup;

		public SoundDef soundInteract;

		public SoundDef soundImpactDefault;

		public bool saveCompressible;

		public bool isSaveable = true;

		public bool holdsRoof;

		public float fillPercent;

		public bool coversFloor;

		public bool neverOverlapFloors;

		public SurfaceType surfaceType;

		public bool blockPlants;

		public bool blockLight;

		public bool blockWind;

		public Tradeability tradeability = Tradeability.All;

		[NoTranslate]
		public List<string> tradeTags;

		public bool tradeNeverStack;

		public ColorGenerator colorGeneratorInTraderStock;

		private List<VerbProperties> verbs;

		public List<Tool> tools;

		public float equippedAngleOffset;

		public EquipmentType equipmentType;

		public TechLevel techLevel;

		[NoTranslate]
		public List<string> weaponTags;

		[NoTranslate]
		public List<string> techHediffsTags;

		public bool destroyOnDrop;

		public List<StatModifier> equippedStatOffsets;

		public BuildableDef entityDefToBuild;

		public ThingDef projectileWhenLoaded;

		public IngestibleProperties ingestible;

		public FilthProperties filth;

		public GasProperties gas;

		public BuildingProperties building;

		public RaceProperties race;

		public ApparelProperties apparel;

		public MoteProperties mote;

		public PlantProperties plant;

		public ProjectileProperties projectile;

		public StuffProperties stuffProps;

		public SkyfallerProperties skyfaller;

		[Unsaved]
		private string descriptionDetailedCached;

		[Unsaved]
		public Graphic interactionCellGraphic;

		public const int SmallUnitPerVolume = 10;

		public const float SmallVolumePerUnit = 0.1f;

		private List<RecipeDef> allRecipesCached;

		private static List<VerbProperties> EmptyVerbPropertiesList = new List<VerbProperties>();

		private Dictionary<ThingDef, Thing> concreteExamplesInt;

		public ThingDef()
		{
		}

		public bool EverHaulable
		{
			get
			{
				return this.alwaysHaulable || this.designateHaulable;
			}
		}

		public float VolumePerUnit
		{
			get
			{
				return this.smallVolume ? 0.1f : 1f;
			}
		}

		public override IntVec2 Size
		{
			get
			{
				return this.size;
			}
		}

		public bool DiscardOnDestroyed
		{
			get
			{
				return this.race == null;
			}
		}

		public int BaseMaxHitPoints
		{
			get
			{
				return Mathf.RoundToInt(this.GetStatValueAbstract(StatDefOf.MaxHitPoints, null));
			}
		}

		public float BaseFlammability
		{
			get
			{
				return this.GetStatValueAbstract(StatDefOf.Flammability, null);
			}
		}

		public float BaseMarketValue
		{
			get
			{
				return this.GetStatValueAbstract(StatDefOf.MarketValue, null);
			}
			set
			{
				this.SetStatBaseValue(StatDefOf.MarketValue, value);
			}
		}

		public float BaseMass
		{
			get
			{
				return this.GetStatValueAbstract(StatDefOf.Mass, null);
			}
		}

		public bool PlayerAcquirable
		{
			get
			{
				return !this.destroyOnDrop;
			}
		}

		public bool EverTransmitsPower
		{
			get
			{
				for (int i = 0; i < this.comps.Count; i++)
				{
					CompProperties_Power compProperties_Power = this.comps[i] as CompProperties_Power;
					if (compProperties_Power != null && compProperties_Power.transmitsPower)
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool Minifiable
		{
			get
			{
				return this.minifiedDef != null;
			}
		}

		public bool HasThingIDNumber
		{
			get
			{
				return this.category != ThingCategory.Mote;
			}
		}

		public List<RecipeDef> AllRecipes
		{
			get
			{
				if (this.allRecipesCached == null)
				{
					this.allRecipesCached = new List<RecipeDef>();
					if (this.recipes != null)
					{
						for (int i = 0; i < this.recipes.Count; i++)
						{
							this.allRecipesCached.Add(this.recipes[i]);
						}
					}
					List<RecipeDef> allDefsListForReading = DefDatabase<RecipeDef>.AllDefsListForReading;
					for (int j = 0; j < allDefsListForReading.Count; j++)
					{
						if (allDefsListForReading[j].recipeUsers != null && allDefsListForReading[j].recipeUsers.Contains(this))
						{
							this.allRecipesCached.Add(allDefsListForReading[j]);
						}
					}
				}
				return this.allRecipesCached;
			}
		}

		public bool ConnectToPower
		{
			get
			{
				if (this.EverTransmitsPower)
				{
					return false;
				}
				for (int i = 0; i < this.comps.Count; i++)
				{
					if (this.comps[i].compClass == typeof(CompPowerBattery))
					{
						return true;
					}
					if (this.comps[i].compClass == typeof(CompPowerTrader))
					{
						return true;
					}
				}
				return false;
			}
		}

		public bool CoexistsWithFloors
		{
			get
			{
				return !this.neverOverlapFloors && !this.coversFloor;
			}
		}

		public FillCategory Fillage
		{
			get
			{
				if (this.fillPercent < 0.01f)
				{
					return FillCategory.None;
				}
				if (this.fillPercent > 0.99f)
				{
					return FillCategory.Full;
				}
				return FillCategory.Partial;
			}
		}

		public bool MakeFog
		{
			get
			{
				return this.Fillage == FillCategory.Full;
			}
		}

		public bool CanOverlapZones
		{
			get
			{
				if (this.building != null && this.building.SupportsPlants)
				{
					return false;
				}
				if (this.passability == Traversability.Impassable && this.category != ThingCategory.Plant)
				{
					return false;
				}
				if (this.surfaceType >= SurfaceType.Item)
				{
					return false;
				}
				if (typeof(ISlotGroupParent).IsAssignableFrom(this.thingClass))
				{
					return false;
				}
				if (!this.canOverlapZones)
				{
					return false;
				}
				if (this.IsBlueprint || this.IsFrame)
				{
					ThingDef thingDef = this.entityDefToBuild as ThingDef;
					if (thingDef != null)
					{
						return thingDef.CanOverlapZones;
					}
				}
				return true;
			}
		}

		public bool CountAsResource
		{
			get
			{
				return this.resourceReadoutPriority != ResourceCountPriority.Uncounted;
			}
		}

		public bool BlockPlanting
		{
			get
			{
				return (this.building == null || !this.building.SupportsPlants) && (this.blockPlants || this.category == ThingCategory.Plant || this.Fillage > FillCategory.None || this.IsEdifice());
			}
		}

		public List<VerbProperties> Verbs
		{
			get
			{
				if (this.verbs != null)
				{
					return this.verbs;
				}
				return ThingDef.EmptyVerbPropertiesList;
			}
		}

		public bool CanHaveFaction
		{
			get
			{
				if (this.IsBlueprint || this.IsFrame)
				{
					return true;
				}
				ThingCategory thingCategory = this.category;
				return thingCategory == ThingCategory.Pawn || thingCategory == ThingCategory.Building;
			}
		}

		public bool Claimable
		{
			get
			{
				return this.building != null && this.building.claimable && !this.building.isNaturalRock;
			}
		}

		public ThingCategoryDef FirstThingCategory
		{
			get
			{
				if (this.thingCategories.NullOrEmpty<ThingCategoryDef>())
				{
					return null;
				}
				return this.thingCategories[0];
			}
		}

		public float MedicineTendXpGainFactor
		{
			get
			{
				return Mathf.Clamp(this.GetStatValueAbstract(StatDefOf.MedicalPotency, null) * 0.7f, 0.5f, 1f);
			}
		}

		public bool CanEverDeteriorate
		{
			get
			{
				return this.useHitPoints && (this.category == ThingCategory.Item || this == ThingDefOf.BurnedTree);
			}
		}

		public bool CanInteractThroughCorners
		{
			get
			{
				return this.category == ThingCategory.Building && this.holdsRoof && (this.building == null || !this.building.isNaturalRock || this.IsSmoothed);
			}
		}

		public bool AffectsRegions
		{
			get
			{
				return this.passability == Traversability.Impassable || this.IsDoor;
			}
		}

		public bool AffectsReachability
		{
			get
			{
				return this.AffectsRegions || (this.passability == Traversability.Impassable || this.IsDoor) || TouchPathEndModeUtility.MakesOccupiedCellsAlwaysReachableDiagonally(this);
			}
		}

		public string DescriptionDetailed
		{
			get
			{
				if (this.descriptionDetailedCached == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.AppendLine(this.description);
					if (this.IsApparel)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(string.Format("{0}: {1}", "Layer".Translate(), this.apparel.GetLayersString()));
						stringBuilder.AppendLine(string.Format("{0}: {1}", "Covers".Translate(), this.apparel.GetCoveredOuterPartsString(BodyDefOf.Human)));
						if (this.equippedStatOffsets != null && this.equippedStatOffsets.Count > 0)
						{
							stringBuilder.AppendLine();
							foreach (StatModifier statModifier in this.equippedStatOffsets)
							{
								stringBuilder.AppendLine(string.Format("{0}: {1}", statModifier.stat.LabelCap, statModifier.ValueToStringAsOffset));
							}
						}
					}
					this.descriptionDetailedCached = stringBuilder.ToString();
				}
				return this.descriptionDetailedCached;
			}
		}

		public bool IsApparel
		{
			get
			{
				return this.apparel != null;
			}
		}

		public bool IsBed
		{
			get
			{
				return typeof(Building_Bed).IsAssignableFrom(this.thingClass);
			}
		}

		public bool IsCorpse
		{
			get
			{
				return typeof(Corpse).IsAssignableFrom(this.thingClass);
			}
		}

		public bool IsFrame
		{
			get
			{
				return this.isFrameInt;
			}
		}

		public bool IsBlueprint
		{
			get
			{
				return this.entityDefToBuild != null && this.category == ThingCategory.Ethereal;
			}
		}

		public bool IsStuff
		{
			get
			{
				return this.stuffProps != null;
			}
		}

		public bool IsMedicine
		{
			get
			{
				return this.statBases.StatListContains(StatDefOf.MedicalPotency);
			}
		}

		public bool IsDoor
		{
			get
			{
				return typeof(Building_Door).IsAssignableFrom(this.thingClass);
			}
		}

		public bool IsFilth
		{
			get
			{
				return this.filth != null;
			}
		}

		public bool IsIngestible
		{
			get
			{
				return this.ingestible != null;
			}
		}

		public bool IsNutritionGivingIngestible
		{
			get
			{
				return this.IsIngestible && this.ingestible.CachedNutrition > 0f;
			}
		}

		public bool IsWeapon
		{
			get
			{
				return this.category == ThingCategory.Item && (!this.verbs.NullOrEmpty<VerbProperties>() || !this.tools.NullOrEmpty<Tool>());
			}
		}

		public bool IsCommsConsole
		{
			get
			{
				return typeof(Building_CommsConsole).IsAssignableFrom(this.thingClass);
			}
		}

		public bool IsOrbitalTradeBeacon
		{
			get
			{
				return typeof(Building_OrbitalTradeBeacon).IsAssignableFrom(this.thingClass);
			}
		}

		public bool IsFoodDispenser
		{
			get
			{
				return typeof(Building_NutrientPasteDispenser).IsAssignableFrom(this.thingClass);
			}
		}

		public bool IsDrug
		{
			get
			{
				return this.ingestible != null && this.ingestible.drugCategory != DrugCategory.None;
			}
		}

		public bool IsPleasureDrug
		{
			get
			{
				return this.IsDrug && this.ingestible.joy > 0f;
			}
		}

		public bool IsNonMedicalDrug
		{
			get
			{
				return this.IsDrug && this.ingestible.drugCategory != DrugCategory.Medical;
			}
		}

		public bool IsTable
		{
			get
			{
				return this.surfaceType == SurfaceType.Eat && this.HasComp(typeof(CompGatherSpot));
			}
		}

		public bool IsWorkTable
		{
			get
			{
				return typeof(Building_WorkTable).IsAssignableFrom(this.thingClass);
			}
		}

		public bool IsShell
		{
			get
			{
				return this.projectileWhenLoaded != null;
			}
		}

		public bool IsArt
		{
			get
			{
				return this.IsWithinCategory(ThingCategoryDefOf.BuildingsArt);
			}
		}

		public bool IsSmoothable
		{
			get
			{
				return this.building != null && this.building.smoothedThing != null;
			}
		}

		public bool IsSmoothed
		{
			get
			{
				return this.building != null && this.building.unsmoothedThing != null;
			}
		}

		public bool IsMetal
		{
			get
			{
				return this.stuffProps != null && this.stuffProps.categories.Contains(StuffCategoryDefOf.Metallic);
			}
		}

		public bool IsAddictiveDrug
		{
			get
			{
				CompProperties_Drug compProperties = this.GetCompProperties<CompProperties_Drug>();
				return compProperties != null && compProperties.addictiveness > 0f;
			}
		}

		public bool IsMeat
		{
			get
			{
				return this.category == ThingCategory.Item && this.thingCategories != null && this.thingCategories.Contains(ThingCategoryDefOf.MeatRaw);
			}
		}

		public bool IsLeather
		{
			get
			{
				return this.category == ThingCategory.Item && this.thingCategories != null && this.thingCategories.Contains(ThingCategoryDefOf.Leathers);
			}
		}

		public bool IsRangedWeapon
		{
			get
			{
				if (!this.IsWeapon)
				{
					return false;
				}
				if (!this.verbs.NullOrEmpty<VerbProperties>())
				{
					for (int i = 0; i < this.verbs.Count; i++)
					{
						if (!this.verbs[i].IsMeleeAttack)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool IsMeleeWeapon
		{
			get
			{
				return this.IsWeapon && !this.IsRangedWeapon;
			}
		}

		public bool IsWeaponUsingProjectiles
		{
			get
			{
				if (!this.IsWeapon)
				{
					return false;
				}
				if (!this.verbs.NullOrEmpty<VerbProperties>())
				{
					for (int i = 0; i < this.verbs.Count; i++)
					{
						if (this.verbs[i].LaunchesProjectile)
						{
							return true;
						}
					}
				}
				return false;
			}
		}

		public bool IsBuildingArtificial
		{
			get
			{
				return (this.category == ThingCategory.Building || this.IsFrame) && (this.building == null || (!this.building.isNaturalRock && !this.building.isResourceRock));
			}
		}

		public bool EverStorable(bool willMinifyIfPossible)
		{
			if (typeof(MinifiedThing).IsAssignableFrom(this.thingClass))
			{
				return true;
			}
			if (!this.thingCategories.NullOrEmpty<ThingCategoryDef>())
			{
				if (this.category == ThingCategory.Item)
				{
					return true;
				}
				if (willMinifyIfPossible && this.Minifiable)
				{
					return true;
				}
			}
			return false;
		}

		public Thing GetConcreteExample(ThingDef stuff = null)
		{
			if (this.concreteExamplesInt == null)
			{
				this.concreteExamplesInt = new Dictionary<ThingDef, Thing>();
			}
			if (stuff == null)
			{
				stuff = ThingDefOf.Steel;
			}
			if (!this.concreteExamplesInt.ContainsKey(stuff))
			{
				if (this.race == null)
				{
					this.concreteExamplesInt[stuff] = ThingMaker.MakeThing(this, (!base.MadeFromStuff) ? null : stuff);
				}
				else
				{
					this.concreteExamplesInt[stuff] = PawnGenerator.GeneratePawn((from pkd in DefDatabase<PawnKindDef>.AllDefsListForReading
					where pkd.race == this
					select pkd).FirstOrDefault<PawnKindDef>(), null);
				}
			}
			return this.concreteExamplesInt[stuff];
		}

		public CompProperties CompDefFor<T>() where T : ThingComp
		{
			return this.comps.FirstOrDefault((CompProperties c) => c.compClass == typeof(T));
		}

		public CompProperties CompDefForAssignableFrom<T>() where T : ThingComp
		{
			return this.comps.FirstOrDefault((CompProperties c) => typeof(T).IsAssignableFrom(c.compClass));
		}

		public bool HasComp(Type compType)
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				if (this.comps[i].compClass == compType)
				{
					return true;
				}
			}
			return false;
		}

		public T GetCompProperties<T>() where T : CompProperties
		{
			for (int i = 0; i < this.comps.Count; i++)
			{
				T t = this.comps[i] as T;
				if (t != null)
				{
					return t;
				}
			}
			return (T)((object)null);
		}

		public override void PostLoad()
		{
			if (this.graphicData != null)
			{
				LongEventHandler.ExecuteWhenFinished(delegate
				{
					if (this.graphicData.shaderType == null)
					{
						this.graphicData.shaderType = ShaderTypeDefOf.Cutout;
					}
					this.graphic = this.graphicData.Graphic;
				});
			}
			if (this.tools != null)
			{
				for (int i = 0; i < this.tools.Count; i++)
				{
					this.tools[i].id = i.ToString();
				}
			}
			if (this.verbs != null && this.verbs.Count == 1)
			{
				this.verbs[0].label = this.label;
			}
			base.PostLoad();
			if (this.category == ThingCategory.Building && this.building == null)
			{
				this.building = new BuildingProperties();
			}
			if (this.building != null)
			{
				this.building.PostLoadSpecial(this);
			}
			if (this.plant != null)
			{
				this.plant.PostLoadSpecial(this);
			}
		}

		protected override void ResolveIcon()
		{
			base.ResolveIcon();
			if (this.category == ThingCategory.Pawn)
			{
				if (!this.race.Humanlike)
				{
					PawnKindDef anyPawnKind = this.race.AnyPawnKind;
					if (anyPawnKind != null)
					{
						Material material = anyPawnKind.lifeStages.Last<PawnKindLifeStage>().bodyGraphicData.Graphic.MatAt(Rot4.East, null);
						this.uiIcon = (Texture2D)material.mainTexture;
						this.uiIconColor = material.color;
					}
				}
			}
			else
			{
				ThingDef thingDef = GenStuff.DefaultStuffFor(this);
				if (this.colorGenerator != null && (thingDef == null || thingDef.stuffProps.allowColorGenerators))
				{
					this.uiIconColor = this.colorGenerator.ExemplaryColor;
				}
				else if (thingDef != null)
				{
					this.uiIconColor = thingDef.stuffProps.color;
				}
				else if (this.graphicData != null)
				{
					this.uiIconColor = this.graphicData.color;
				}
				if (this.rotatable && this.graphic != null && this.graphic != BaseContent.BadGraphic && this.graphic.ShouldDrawRotated && this.defaultPlacingRot == Rot4.South)
				{
					this.uiIconAngle = 180f + this.graphic.DrawRotatedExtraAngleOffset;
				}
			}
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();
			if (this.ingestible != null)
			{
				this.ingestible.parent = this;
			}
			if (this.building != null)
			{
				this.building.ResolveReferencesSpecial();
			}
			if (this.graphicData != null)
			{
				this.graphicData.ResolveReferencesSpecial();
			}
			if (this.race != null)
			{
				this.race.ResolveReferencesSpecial();
			}
			if (this.stuffProps != null)
			{
				this.stuffProps.ResolveReferencesSpecial();
			}
			if (this.soundImpactDefault == null)
			{
				this.soundImpactDefault = SoundDefOf.BulletImpact_Ground;
			}
			if (this.soundDrop == null)
			{
				this.soundDrop = SoundDefOf.Standard_Drop;
			}
			if (this.soundPickup == null)
			{
				this.soundPickup = SoundDefOf.Standard_Pickup;
			}
			if (this.soundInteract == null)
			{
				this.soundInteract = SoundDefOf.Standard_Pickup;
			}
			if (this.inspectorTabs != null && this.inspectorTabs.Any<Type>())
			{
				this.inspectorTabsResolved = new List<InspectTabBase>();
				for (int i = 0; i < this.inspectorTabs.Count; i++)
				{
					try
					{
						this.inspectorTabsResolved.Add(InspectTabManager.GetSharedInstance(this.inspectorTabs[i]));
					}
					catch (Exception ex)
					{
						Log.Error(string.Concat(new object[]
						{
							"Could not instantiate inspector tab of type ",
							this.inspectorTabs[i],
							": ",
							ex
						}), false);
					}
				}
			}
			if (this.comps != null)
			{
				for (int j = 0; j < this.comps.Count; j++)
				{
					this.comps[j].ResolveReferences(this);
				}
			}
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string str in base.ConfigErrors())
			{
				yield return str;
			}
			if (this.label.NullOrEmpty())
			{
				yield return "no label";
			}
			if (this.graphicData != null)
			{
				foreach (string err in this.graphicData.ConfigErrors(this))
				{
					yield return err;
				}
			}
			if (this.projectile != null)
			{
				foreach (string err2 in this.projectile.ConfigErrors(this))
				{
					yield return err2;
				}
			}
			if (this.statBases != null)
			{
				using (List<StatModifier>.Enumerator enumerator4 = this.statBases.GetEnumerator())
				{
					while (enumerator4.MoveNext())
					{
						StatModifier statBase = enumerator4.Current;
						if ((from st in this.statBases
						where st.stat == statBase.stat
						select st).Count<StatModifier>() > 1)
						{
							yield return "defines the stat base " + statBase.stat + " more than once.";
						}
					}
				}
			}
			if (!BeautyUtility.BeautyRelevant(this.category) && this.StatBaseDefined(StatDefOf.Beauty))
			{
				yield return "Beauty stat base is defined, but Things of category " + this.category + " cannot have beauty.";
			}
			if (char.IsNumber(this.defName[this.defName.Length - 1]))
			{
				yield return "ends with a numerical digit, which is not allowed on ThingDefs.";
			}
			if (this.thingClass == null)
			{
				yield return "has null thingClass.";
			}
			if (this.comps.Count > 0 && !typeof(ThingWithComps).IsAssignableFrom(this.thingClass))
			{
				yield return "has components but it's thingClass is not a ThingWithComps";
			}
			if (this.ConnectToPower && this.drawerType == DrawerType.RealtimeOnly && this.IsFrame)
			{
				yield return "connects to power but does not add to map mesh. Will not create wire meshes.";
			}
			if (this.costList != null)
			{
				foreach (ThingDefCountClass cost in this.costList)
				{
					if (cost.count == 0)
					{
						yield return "cost in " + cost.thingDef + " is zero.";
					}
				}
			}
			if (this.thingCategories != null)
			{
				ThingCategoryDef doubleCat = this.thingCategories.FirstOrDefault((ThingCategoryDef cat) => this.thingCategories.Count((ThingCategoryDef c) => c == cat) > 1);
				if (doubleCat != null)
				{
					yield return "has duplicate thingCategory " + doubleCat + ".";
				}
			}
			if (this.Fillage == FillCategory.Full && this.category != ThingCategory.Building)
			{
				yield return "gives full cover but is not a building.";
			}
			if (this.comps.Any((CompProperties c) => c.compClass == typeof(CompPowerTrader)) && this.drawerType == DrawerType.MapMeshOnly)
			{
				yield return "has PowerTrader comp but does not draw real time. It won't draw a needs-power overlay.";
			}
			if (this.equipmentType != EquipmentType.None)
			{
				if (this.techLevel == TechLevel.Undefined)
				{
					yield return "is equipment but has no tech level.";
				}
				if (!this.comps.Any((CompProperties c) => c.compClass == typeof(CompEquippable)))
				{
					yield return "is equipment but has no CompEquippable";
				}
			}
			if (this.thingClass == typeof(Bullet) && this.projectile.damageDef == null)
			{
				yield return " is a bullet but has no damageDef.";
			}
			if (this.destroyOnDrop)
			{
				if (!this.menuHidden)
				{
					yield return "destroyOnDrop but not menuHidden.";
				}
				if (this.tradeability != Tradeability.None)
				{
					yield return "destroyOnDrop but tradeability is " + this.tradeability;
				}
			}
			if (this.stackLimit > 1 && !this.drawGUIOverlay)
			{
				yield return "has stackLimit > 1 but also has drawGUIOverlay = false.";
			}
			if (this.damageMultipliers != null)
			{
				using (List<DamageMultiplier>.Enumerator enumerator6 = this.damageMultipliers.GetEnumerator())
				{
					while (enumerator6.MoveNext())
					{
						DamageMultiplier mult = enumerator6.Current;
						if ((from m in this.damageMultipliers
						where m.damageDef == mult.damageDef
						select m).Count<DamageMultiplier>() > 1)
						{
							yield return "has multiple damage multipliers for damageDef " + mult.damageDef;
							break;
						}
					}
				}
			}
			if (this.Fillage == FillCategory.Full && !this.IsEdifice())
			{
				yield return "fillPercent is 1.00 but is not edifice";
			}
			if (base.MadeFromStuff && this.constructEffect != null)
			{
				yield return "madeFromStuff but has a defined constructEffect (which will always be overridden by stuff's construct animation).";
			}
			if (base.MadeFromStuff && this.stuffCategories.NullOrEmpty<StuffCategoryDef>())
			{
				yield return "madeFromStuff but has no stuffCategories.";
			}
			if (this.costList.NullOrEmpty<ThingDefCountClass>() && this.costStuffCount <= 0 && this.recipeMaker != null)
			{
				yield return "has a recipeMaker but no costList or costStuffCount.";
			}
			if (this.GetStatValueAbstract(StatDefOf.DeteriorationRate, null) > 1E-05f && !this.CanEverDeteriorate)
			{
				yield return "has >0 DeteriorationRate but can't deteriorate.";
			}
			if (this.drawerType == DrawerType.MapMeshOnly)
			{
				if (this.comps.Any((CompProperties c) => c.compClass == typeof(CompForbiddable)))
				{
					yield return "drawerType=MapMeshOnly but has a CompForbiddable, which must draw in real time.";
				}
			}
			if (this.smeltProducts != null && this.smeltable)
			{
				yield return "has smeltProducts but has smeltable=false";
			}
			if (this.equipmentType != EquipmentType.None && this.verbs.NullOrEmpty<VerbProperties>() && this.tools.NullOrEmpty<Tool>())
			{
				yield return "is equipment but has no verbs or tools";
			}
			if (this.Minifiable && this.thingCategories.NullOrEmpty<ThingCategoryDef>())
			{
				yield return "is minifiable but not in any thing category";
			}
			if (this.category == ThingCategory.Building && !this.Minifiable && !this.thingCategories.NullOrEmpty<ThingCategoryDef>())
			{
				yield return "is not minifiable yet has thing categories (could be confusing in thing filters because it can't be moved/stored anyway)";
			}
			if (this != ThingDefOf.MinifiedThing && (this.EverHaulable || this.Minifiable))
			{
				if (!this.statBases.NullOrEmpty<StatModifier>())
				{
					if (this.statBases.Any((StatModifier s) => s.stat == StatDefOf.Mass))
					{
						goto IL_D74;
					}
				}
				yield return "is haulable, but does not have an authored mass value";
			}
			IL_D74:
			if (this.ingestible == null && this.GetStatValueAbstract(StatDefOf.Nutrition, null) != 0f)
			{
				yield return "has nutrition but ingestible properties are null";
			}
			if (this.BaseFlammability != 0f && !this.useHitPoints && this.category != ThingCategory.Pawn)
			{
				yield return "flammable but has no hitpoints (will burn indefinitely)";
			}
			if (this.graphicData != null && this.graphicData.shadowData != null && this.staticSunShadowHeight > 0f)
			{
				yield return "graphicData defines a shadowInfo but staticSunShadowHeight > 0";
			}
			if (this.saveCompressible && this.Claimable)
			{
				yield return "claimable item is compressible; faction will be unset after load";
			}
			if (this.deepCommonality > 0f != this.deepLumpSizeRange.TrueMax > 0)
			{
				yield return "if deepCommonality or deepLumpSizeRange is set, the other also must be set";
			}
			if (this.deepCommonality > 0f && this.deepCountPerPortion <= 0)
			{
				yield return "deepCommonality > 0 but deepCountPerPortion is not set";
			}
			if (this.verbs != null)
			{
				for (int k = 0; k < this.verbs.Count; k++)
				{
					foreach (string err3 in this.verbs[k].ConfigErrors(this))
					{
						yield return string.Format("verb {0}: {1}", k, err3);
					}
				}
			}
			if (this.race != null && this.tools != null)
			{
				int i;
				for (i = 0; i < this.tools.Count; i++)
				{
					if (this.tools[i].linkedBodyPartsGroup != null && !this.race.body.AllParts.Any((BodyPartRecord part) => part.groups.Contains(this.tools[i].linkedBodyPartsGroup)))
					{
						yield return string.Concat(new object[]
						{
							"has tool with linkedBodyPartsGroup ",
							this.tools[i].linkedBodyPartsGroup,
							" but body ",
							this.race.body,
							" has no parts with that group."
						});
					}
				}
			}
			if (this.building != null)
			{
				foreach (string err4 in this.building.ConfigErrors(this))
				{
					yield return err4;
				}
			}
			if (this.apparel != null)
			{
				foreach (string err5 in this.apparel.ConfigErrors(this))
				{
					yield return err5;
				}
			}
			if (this.comps != null)
			{
				for (int j = 0; j < this.comps.Count; j++)
				{
					foreach (string err6 in this.comps[j].ConfigErrors(this))
					{
						yield return err6;
					}
				}
			}
			if (this.race != null)
			{
				foreach (string e in this.race.ConfigErrors())
				{
					yield return e;
				}
			}
			if (this.ingestible != null)
			{
				foreach (string e2 in this.ingestible.ConfigErrors())
				{
					yield return e2;
				}
			}
			if (this.plant != null)
			{
				foreach (string e3 in this.plant.ConfigErrors())
				{
					yield return e3;
				}
			}
			if (this.tools != null)
			{
				Tool dupeTool = this.tools.SelectMany((Tool lhs) => from rhs in this.tools
				where lhs != rhs && lhs.id == rhs.id
				select rhs).FirstOrDefault<Tool>();
				if (dupeTool != null)
				{
					yield return string.Format("duplicate thingdef tool id {0}", dupeTool.id);
				}
				foreach (Tool t in this.tools)
				{
					foreach (string e4 in t.ConfigErrors())
					{
						yield return e4;
					}
				}
			}
			yield break;
		}

		public static ThingDef Named(string defName)
		{
			return DefDatabase<ThingDef>.GetNamed(defName, true);
		}

		public string LabelAsStuff
		{
			get
			{
				if (!this.stuffProps.stuffAdjective.NullOrEmpty())
				{
					return this.stuffProps.stuffAdjective;
				}
				return this.label;
			}
		}

		public bool IsWithinCategory(ThingCategoryDef category)
		{
			if (this.thingCategories == null)
			{
				return false;
			}
			for (int i = 0; i < this.thingCategories.Count; i++)
			{
				if (this.thingCategories[i] == category || this.thingCategories[i].Parents.Contains(category))
				{
					return true;
				}
			}
			return false;
		}

		public override IEnumerable<StatDrawEntry> SpecialDisplayStats(StatRequest req)
		{
			foreach (StatDrawEntry stat in base.SpecialDisplayStats(req))
			{
				yield return stat;
			}
			if (this.apparel != null)
			{
				string coveredParts = this.apparel.GetCoveredOuterPartsString(BodyDefOf.Human);
				yield return new StatDrawEntry(StatCategoryDefOf.Apparel, "Covers".Translate(), coveredParts, 100, string.Empty);
				yield return new StatDrawEntry(StatCategoryDefOf.Apparel, "Layer".Translate(), this.apparel.GetLayersString(), 95, string.Empty);
			}
			if (this.IsMedicine && this.MedicineTendXpGainFactor != 1f)
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "MedicineXpGainFactor".Translate(), this.MedicineTendXpGainFactor.ToStringPercent(), 0, string.Empty);
			}
			if (this.fillPercent > 0f && this.fillPercent < 1f && (this.category == ThingCategory.Item || this.category == ThingCategory.Building || this.category == ThingCategory.Plant))
			{
				yield return new StatDrawEntry(StatCategoryDefOf.Basics, "CoverEffectiveness".Translate(), this.BaseBlockChance().ToStringPercent(), 0, string.Empty)
				{
					overrideReportText = "CoverEffectivenessExplanation".Translate()
				};
			}
			if (this.constructionSkillPrerequisite > 0)
			{
				StatCategoryDef statCategoryDef = StatCategoryDefOf.Basics;
				string text = "ConstructionSkillRequired".Translate();
				string valueString = this.constructionSkillPrerequisite.ToString();
				string text2 = "ConstructionSkillRequiredExplanation".Translate();
				yield return new StatDrawEntry(statCategoryDef, text, valueString, 0, text2);
			}
			if (!this.verbs.NullOrEmpty<VerbProperties>())
			{
				VerbProperties verb = this.verbs.First((VerbProperties x) => x.isPrimary);
				StatCategoryDef verbStatCategory = (this.category != ThingCategory.Pawn) ? (verbStatCategory = StatCategoryDefOf.Weapon) : (verbStatCategory = StatCategoryDefOf.PawnCombat);
				float warmup = verb.warmupTime;
				if (warmup > 0f)
				{
					string warmupLabel = (this.category != ThingCategory.Pawn) ? "WarmupTime".Translate() : "MeleeWarmupTime".Translate();
					yield return new StatDrawEntry(verbStatCategory, warmupLabel, warmup.ToString("0.##") + " s", 40, string.Empty);
				}
				if (verb.defaultProjectile != null)
				{
					StringBuilder damageAmountExplanation = new StringBuilder();
					float dam = (float)verb.defaultProjectile.projectile.GetDamageAmount(req.Thing, damageAmountExplanation);
					yield return new StatDrawEntry(verbStatCategory, "Damage".Translate(), dam.ToString(), 50, damageAmountExplanation.ToString());
					if (verb.defaultProjectile.projectile.damageDef.armorCategory != null)
					{
						StringBuilder armorPenetrationExplanation = new StringBuilder();
						float ap = verb.defaultProjectile.projectile.GetArmorPenetration(req.Thing, armorPenetrationExplanation);
						string fullExplanation = "ArmorPenetrationExplanation".Translate();
						if (armorPenetrationExplanation.Length != 0)
						{
							fullExplanation = fullExplanation + "\n\n" + armorPenetrationExplanation;
						}
						yield return new StatDrawEntry(verbStatCategory, "ArmorPenetration".Translate(), ap.ToStringPercent(), 49, fullExplanation);
					}
				}
				if (verb.LaunchesProjectile)
				{
					int burstShotCount = verb.burstShotCount;
					float burstShotFireRate = 60f / verb.ticksBetweenBurstShots.TicksToSeconds();
					float range = verb.range;
					if (burstShotCount > 1)
					{
						yield return new StatDrawEntry(verbStatCategory, "BurstShotCount".Translate(), burstShotCount.ToString(), 20, string.Empty);
						yield return new StatDrawEntry(verbStatCategory, "BurstShotFireRate".Translate(), burstShotFireRate.ToString("0.##") + " rpm", 19, string.Empty);
					}
					yield return new StatDrawEntry(verbStatCategory, "Range".Translate(), range.ToString("F0"), 10, string.Empty);
					if (verb.defaultProjectile != null && verb.defaultProjectile.projectile != null && verb.defaultProjectile.projectile.stoppingPower != 0f)
					{
						StatCategoryDef statCategoryDef = verbStatCategory;
						string text2 = "StoppingPower".Translate();
						string valueString = verb.defaultProjectile.projectile.stoppingPower.ToString("F1");
						string text = "StoppingPowerExplanation".Translate();
						yield return new StatDrawEntry(statCategoryDef, text2, valueString, 0, text);
					}
				}
				if (verb.forcedMissRadius > 0f)
				{
					yield return new StatDrawEntry(verbStatCategory, "MissRadius".Translate(), verb.forcedMissRadius.ToString("0.#"), 30, string.Empty);
					yield return new StatDrawEntry(verbStatCategory, "DirectHitChance".Translate(), (1f / (float)GenRadial.NumCellsInRadius(verb.forcedMissRadius)).ToStringPercent(), 29, string.Empty);
				}
			}
			if (this.plant != null)
			{
				foreach (StatDrawEntry s in this.plant.SpecialDisplayStats())
				{
					yield return s;
				}
			}
			if (this.ingestible != null)
			{
				foreach (StatDrawEntry s2 in this.ingestible.SpecialDisplayStats())
				{
					yield return s2;
				}
			}
			if (this.race != null)
			{
				foreach (StatDrawEntry s3 in this.race.SpecialDisplayStats(this))
				{
					yield return s3;
				}
			}
			if (this.building != null)
			{
				foreach (StatDrawEntry s4 in this.building.SpecialDisplayStats(this))
				{
					yield return s4;
				}
			}
			if (this.isTechHediff)
			{
				foreach (RecipeDef def in from x in DefDatabase<RecipeDef>.AllDefs
				where x.IsIngredient(this)
				select x)
				{
					HediffDef diff = def.addsHediff;
					if (diff != null)
					{
						if (diff.addedPartProps != null)
						{
							yield return new StatDrawEntry(StatCategoryDefOf.Basics, "BodyPartEfficiency".Translate(), diff.addedPartProps.partEfficiency.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Absolute), 0, string.Empty);
						}
						foreach (StatDrawEntry s5 in diff.SpecialDisplayStats(StatRequest.ForEmpty()))
						{
							yield return s5;
						}
						HediffCompProperties_VerbGiver vg = diff.CompProps<HediffCompProperties_VerbGiver>();
						if (vg != null)
						{
							if (!vg.verbs.NullOrEmpty<VerbProperties>())
							{
								VerbProperties verb2 = vg.verbs[0];
								if (!verb2.IsMeleeAttack)
								{
									if (verb2.defaultProjectile != null)
									{
										int projDamage = verb2.defaultProjectile.projectile.GetDamageAmount(null, null);
										yield return new StatDrawEntry(StatCategoryDefOf.Basics, "Damage".Translate(), projDamage.ToString(), 0, string.Empty);
										if (verb2.defaultProjectile.projectile.damageDef.armorCategory != null)
										{
											float projArmorPenetration = verb2.defaultProjectile.projectile.GetArmorPenetration(null, null);
											StatCategoryDef statCategoryDef = StatCategoryDefOf.Basics;
											string text = "ArmorPenetration".Translate();
											string valueString = projArmorPenetration.ToStringPercent();
											string text2 = "ArmorPenetrationExplanation".Translate();
											yield return new StatDrawEntry(statCategoryDef, text, valueString, 0, text2);
										}
									}
								}
								else
								{
									int meleeDamage = verb2.meleeDamageBaseAmount;
									yield return new StatDrawEntry(StatCategoryDefOf.Weapon, "Damage".Translate(), meleeDamage.ToString(), 0, string.Empty);
									if (verb2.meleeDamageDef.armorCategory != null)
									{
										float armorPenetration = verb2.meleeArmorPenetrationBase;
										if (armorPenetration < 0f)
										{
											armorPenetration = (float)meleeDamage * 0.015f;
										}
										StatCategoryDef statCategoryDef = StatCategoryDefOf.Weapon;
										string text2 = "ArmorPenetration".Translate();
										string valueString = armorPenetration.ToStringPercent();
										string text = "ArmorPenetrationExplanation".Translate();
										yield return new StatDrawEntry(statCategoryDef, text2, valueString, 0, text);
									}
								}
							}
							else if (!vg.tools.NullOrEmpty<Tool>())
							{
								Tool tool = vg.tools[0];
								yield return new StatDrawEntry(StatCategoryDefOf.Weapon, "Damage".Translate(), tool.power.ToString(), 0, string.Empty);
								if (ThingUtility.PrimaryMeleeWeaponDamageType(vg.tools).armorCategory != null)
								{
									float armorPenetration2 = tool.armorPenetration;
									if (armorPenetration2 < 0f)
									{
										armorPenetration2 = tool.power * 0.015f;
									}
									StatCategoryDef statCategoryDef = StatCategoryDefOf.Weapon;
									string text = "ArmorPenetration".Translate();
									string valueString = armorPenetration2.ToStringPercent();
									string text2 = "ArmorPenetrationExplanation".Translate();
									yield return new StatDrawEntry(statCategoryDef, text, valueString, 0, text2);
								}
							}
						}
						ThoughtDef thought = DefDatabase<ThoughtDef>.AllDefs.FirstOrDefault((ThoughtDef x) => x.hediff == diff);
						if (thought != null && thought.stages != null && thought.stages.Any<ThoughtStage>())
						{
							yield return new StatDrawEntry(StatCategoryDefOf.Basics, "MoodChange".Translate(), thought.stages.First<ThoughtStage>().baseMoodEffect.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Offset), 0, string.Empty);
						}
					}
				}
			}
			for (int i = 0; i < this.comps.Count; i++)
			{
				foreach (StatDrawEntry s6 in this.comps[i].SpecialDisplayStats())
				{
					yield return s6;
				}
			}
			yield break;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static ThingDef()
		{
		}

		[CompilerGenerated]
		private bool <GetConcreteExample>m__0(PawnKindDef pkd)
		{
			return pkd.race == this;
		}

		[CompilerGenerated]
		private static bool <CompDefFor<T>(CompProperties c) where T : ThingComp
		{
			return c.compClass == typeof(T);
		}

		[CompilerGenerated]
		private static bool <CompDefForAssignableFrom<T>(CompProperties c) where T : ThingComp
		{
			return typeof(T).IsAssignableFrom(c.compClass);
		}

		[CompilerGenerated]
		private void <PostLoad>m__3()
		{
			if (this.graphicData.shaderType == null)
			{
				this.graphicData.shaderType = ShaderTypeDefOf.Cutout;
			}
			this.graphic = this.graphicData.Graphic;
		}

		[DebuggerHidden]
		[CompilerGenerated]
		private IEnumerable<string> <ConfigErrors>__BaseCallProxy0()
		{
			return base.ConfigErrors();
		}

		[DebuggerHidden]
		[CompilerGenerated]
		private IEnumerable<StatDrawEntry> <SpecialDisplayStats>__BaseCallProxy1(StatRequest req)
		{
			return base.SpecialDisplayStats(req);
		}

		[CompilerGenerated]
		private sealed class <ConfigErrors>c__Iterator0 : IEnumerable, IEnumerable<string>, IEnumerator, IDisposable, IEnumerator<string>
		{
			internal IEnumerator<string> $locvar0;

			internal string <str>__1;

			internal IEnumerator<string> $locvar1;

			internal string <err>__2;

			internal IEnumerator<string> $locvar2;

			internal string <err>__3;

			internal List<StatModifier>.Enumerator $locvar3;

			internal List<ThingDefCountClass>.Enumerator $locvar4;

			internal ThingDefCountClass <cost>__5;

			internal ThingCategoryDef <doubleCat>__6;

			internal List<DamageMultiplier>.Enumerator $locvar5;

			internal int <i>__8;

			internal IEnumerator<string> $locvar6;

			internal string <err>__9;

			internal IEnumerator<string> $locvar7;

			internal string <err>__11;

			internal IEnumerator<string> $locvar8;

			internal string <err>__12;

			internal int <i>__13;

			internal IEnumerator<string> $locvar9;

			internal string <err>__14;

			internal IEnumerator<string> $locvarA;

			internal string <e>__15;

			internal IEnumerator<string> $locvarB;

			internal string <e>__16;

			internal IEnumerator<string> $locvarC;

			internal string <e>__17;

			internal Tool <dupeTool>__18;

			internal List<Tool>.Enumerator $locvarD;

			internal Tool <t>__19;

			internal IEnumerator<string> $locvarE;

			internal string <e>__20;

			internal ThingDef $this;

			internal string $current;

			internal bool $disposing;

			internal int $PC;

			private ThingDef.<ConfigErrors>c__Iterator0.<ConfigErrors>c__AnonStorey2 $locvarF;

			private static Predicate<CompProperties> <>f__am$cache0;

			private static Predicate<CompProperties> <>f__am$cache1;

			private ThingDef.<ConfigErrors>c__Iterator0.<ConfigErrors>c__AnonStorey4 $locvar10;

			private static Predicate<CompProperties> <>f__am$cache2;

			private static Predicate<StatModifier> <>f__am$cache3;

			private ThingDef.<ConfigErrors>c__Iterator0.<ConfigErrors>c__AnonStorey5 $locvar11;

			[DebuggerHidden]
			public <ConfigErrors>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				bool flag = false;
				int i;
				switch (num)
				{
				case 0u:
					enumerator = base.<ConfigErrors>__BaseCallProxy0().GetEnumerator();
					num = 4294967293u;
					break;
				case 1u:
					break;
				case 2u:
					goto IL_1A0;
				case 3u:
					goto IL_1D4;
				case 4u:
					Block_8:
					try
					{
						switch (num)
						{
						}
						if (enumerator3.MoveNext())
						{
							err2 = enumerator3.Current;
							this.$current = err2;
							if (!this.$disposing)
							{
								this.$PC = 4;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator3 != null)
							{
								enumerator3.Dispose();
							}
						}
					}
					goto IL_2F0;
				case 5u:
					Block_10:
					try
					{
						switch (num)
						{
						}
						while (enumerator4.MoveNext())
						{
							StatModifier statBase = enumerator4.Current;
							if ((from st in this.statBases
							where st.stat == statBase.stat
							select st).Count<StatModifier>() > 1)
							{
								this.$current = "defines the stat base " + statBase.stat + " more than once.";
								if (!this.$disposing)
								{
									this.$PC = 5;
								}
								flag = true;
								return true;
							}
						}
					}
					finally
					{
						if (!flag)
						{
							((IDisposable)enumerator4).Dispose();
						}
					}
					goto IL_3E9;
				case 6u:
					IL_44C:
					if (char.IsNumber(this.defName[this.defName.Length - 1]))
					{
						this.$current = "ends with a numerical digit, which is not allowed on ThingDefs.";
						if (!this.$disposing)
						{
							this.$PC = 7;
						}
						return true;
					}
					goto IL_497;
				case 7u:
					goto IL_497;
				case 8u:
					goto IL_4C6;
				case 9u:
					goto IL_51B;
				case 10u:
					goto IL_56C;
				case 11u:
					goto IL_595;
				case 12u:
					IL_697:
					if (base.Fillage == FillCategory.Full && this.category != ThingCategory.Building)
					{
						this.$current = "gives full cover but is not a building.";
						if (!this.$disposing)
						{
							this.$PC = 13;
						}
						return true;
					}
					goto IL_6D9;
				case 13u:
					goto IL_6D9;
				case 14u:
					goto IL_73C;
				case 15u:
					goto IL_77C;
				case 16u:
					goto IL_7CE;
				case 17u:
					goto IL_81D;
				case 18u:
					goto IL_85D;
				case 19u:
					goto IL_8A2;
				case 20u:
					goto IL_8E3;
				case 21u:
					goto IL_90C;
				case 22u:
					IL_A1F:
					if (base.MadeFromStuff && this.constructEffect != null)
					{
						this.$current = "madeFromStuff but has a defined constructEffect (which will always be overridden by stuff's construct animation).";
						if (!this.$disposing)
						{
							this.$PC = 23;
						}
						return true;
					}
					goto IL_A5F;
				case 23u:
					goto IL_A5F;
				case 24u:
					goto IL_AA4;
				case 25u:
					goto IL_AFA;
				case 26u:
					goto IL_B45;
				case 27u:
					goto IL_BA8;
				case 28u:
					goto IL_BE8;
				case 29u:
					goto IL_C42;
				case 30u:
					goto IL_C87;
				case 31u:
					goto IL_CDD;
				case 32u:
					goto IL_D74;
				case 33u:
					goto IL_DBF;
				case 34u:
					goto IL_E15;
				case 35u:
					goto IL_E6F;
				case 36u:
					goto IL_EAF;
				case 37u:
					goto IL_EF9;
				case 38u:
					goto IL_F3F;
				case 39u:
					Block_115:
					try
					{
						switch (num)
						{
						}
						if (enumerator7.MoveNext())
						{
							err3 = enumerator7.Current;
							this.$current = string.Format("verb {0}: {1}", i, err3);
							if (!this.$disposing)
							{
								this.$PC = 39;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator7 != null)
							{
								enumerator7.Dispose();
							}
						}
					}
					i++;
					goto IL_1023;
				case 40u:
					IL_114F:
					i++;
					goto IL_1167;
				case 41u:
					Block_122:
					try
					{
						switch (num)
						{
						}
						if (enumerator8.MoveNext())
						{
							err4 = enumerator8.Current;
							this.$current = err4;
							if (!this.$disposing)
							{
								this.$PC = 41;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator8 != null)
							{
								enumerator8.Dispose();
							}
						}
					}
					goto IL_1231;
				case 42u:
					Block_124:
					try
					{
						switch (num)
						{
						}
						if (enumerator9.MoveNext())
						{
							err5 = enumerator9.Current;
							this.$current = err5;
							if (!this.$disposing)
							{
								this.$PC = 42;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator9 != null)
							{
								enumerator9.Dispose();
							}
						}
					}
					goto IL_12DB;
				case 43u:
					Block_126:
					try
					{
						switch (num)
						{
						}
						if (enumerator10.MoveNext())
						{
							err6 = enumerator10.Current;
							this.$current = err6;
							if (!this.$disposing)
							{
								this.$PC = 43;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator10 != null)
							{
								enumerator10.Dispose();
							}
						}
					}
					j++;
					goto IL_13AA;
				case 44u:
					Block_128:
					try
					{
						switch (num)
						{
						}
						if (enumerator11.MoveNext())
						{
							e = enumerator11.Current;
							this.$current = e;
							if (!this.$disposing)
							{
								this.$PC = 44;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator11 != null)
							{
								enumerator11.Dispose();
							}
						}
					}
					goto IL_1469;
				case 45u:
					Block_130:
					try
					{
						switch (num)
						{
						}
						if (enumerator12.MoveNext())
						{
							e2 = enumerator12.Current;
							this.$current = e2;
							if (!this.$disposing)
							{
								this.$PC = 45;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator12 != null)
							{
								enumerator12.Dispose();
							}
						}
					}
					goto IL_150D;
				case 46u:
					Block_132:
					try
					{
						switch (num)
						{
						}
						if (enumerator13.MoveNext())
						{
							e3 = enumerator13.Current;
							this.$current = e3;
							if (!this.$disposing)
							{
								this.$PC = 46;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator13 != null)
							{
								enumerator13.Dispose();
							}
						}
					}
					goto IL_15B1;
				case 47u:
					IL_1623:
					enumerator14 = this.tools.GetEnumerator();
					num = 4294967293u;
					goto Block_136;
				case 48u:
					goto IL_163C;
				default:
					return false;
				}
				try
				{
					switch (num)
					{
					}
					if (enumerator.MoveNext())
					{
						str = enumerator.Current;
						this.$current = str;
						if (!this.$disposing)
						{
							this.$PC = 1;
						}
						flag = true;
						return true;
					}
				}
				finally
				{
					if (!flag)
					{
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
				}
				if (this.label.NullOrEmpty())
				{
					this.$current = "no label";
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				}
				IL_1A0:
				if (this.graphicData == null)
				{
					goto IL_248;
				}
				enumerator2 = this.graphicData.ConfigErrors(this).GetEnumerator();
				num = 4294967293u;
				try
				{
					IL_1D4:
					switch (num)
					{
					}
					if (enumerator2.MoveNext())
					{
						err = enumerator2.Current;
						this.$current = err;
						if (!this.$disposing)
						{
							this.$PC = 3;
						}
						flag = true;
						return true;
					}
				}
				finally
				{
					if (!flag)
					{
						if (enumerator2 != null)
						{
							enumerator2.Dispose();
						}
					}
				}
				IL_248:
				if (this.projectile != null)
				{
					enumerator3 = this.projectile.ConfigErrors(this).GetEnumerator();
					num = 4294967293u;
					goto Block_8;
				}
				IL_2F0:
				if (this.statBases != null)
				{
					enumerator4 = this.statBases.GetEnumerator();
					num = 4294967293u;
					goto Block_10;
				}
				IL_3E9:
				if (!BeautyUtility.BeautyRelevant(this.category) && this.StatBaseDefined(StatDefOf.Beauty))
				{
					this.$current = "Beauty stat base is defined, but Things of category " + this.category + " cannot have beauty.";
					if (!this.$disposing)
					{
						this.$PC = 6;
					}
					return true;
				}
				goto IL_44C;
				IL_497:
				if (this.thingClass == null)
				{
					this.$current = "has null thingClass.";
					if (!this.$disposing)
					{
						this.$PC = 8;
					}
					return true;
				}
				IL_4C6:
				if (this.comps.Count > 0 && !typeof(ThingWithComps).IsAssignableFrom(this.thingClass))
				{
					this.$current = "has components but it's thingClass is not a ThingWithComps";
					if (!this.$disposing)
					{
						this.$PC = 9;
					}
					return true;
				}
				IL_51B:
				if (base.ConnectToPower && this.drawerType == DrawerType.RealtimeOnly && base.IsFrame)
				{
					this.$current = "connects to power but does not add to map mesh. Will not create wire meshes.";
					if (!this.$disposing)
					{
						this.$PC = 10;
					}
					return true;
				}
				IL_56C:
				if (this.costList == null)
				{
					goto IL_62A;
				}
				enumerator5 = this.costList.GetEnumerator();
				num = 4294967293u;
				try
				{
					IL_595:
					switch (num)
					{
					}
					while (enumerator5.MoveNext())
					{
						cost = enumerator5.Current;
						if (cost.count == 0)
						{
							this.$current = "cost in " + cost.thingDef + " is zero.";
							if (!this.$disposing)
							{
								this.$PC = 11;
							}
							flag = true;
							return true;
						}
					}
				}
				finally
				{
					if (!flag)
					{
						((IDisposable)enumerator5).Dispose();
					}
				}
				IL_62A:
				if (this.thingCategories == null)
				{
					goto IL_697;
				}
				doubleCat = this.thingCategories.FirstOrDefault((ThingCategoryDef cat) => this.thingCategories.Count((ThingCategoryDef c) => c == cat) > 1);
				if (doubleCat != null)
				{
					this.$current = "has duplicate thingCategory " + doubleCat + ".";
					if (!this.$disposing)
					{
						this.$PC = 12;
					}
					return true;
				}
				goto IL_697;
				IL_6D9:
				if (this.comps.Any((CompProperties c) => c.compClass == typeof(CompPowerTrader)) && this.drawerType == DrawerType.MapMeshOnly)
				{
					this.$current = "has PowerTrader comp but does not draw real time. It won't draw a needs-power overlay.";
					if (!this.$disposing)
					{
						this.$PC = 14;
					}
					return true;
				}
				IL_73C:
				if (this.equipmentType == EquipmentType.None)
				{
					goto IL_7CE;
				}
				if (this.techLevel == TechLevel.Undefined)
				{
					this.$current = "is equipment but has no tech level.";
					if (!this.$disposing)
					{
						this.$PC = 15;
					}
					return true;
				}
				IL_77C:
				if (!this.comps.Any((CompProperties c) => c.compClass == typeof(CompEquippable)))
				{
					this.$current = "is equipment but has no CompEquippable";
					if (!this.$disposing)
					{
						this.$PC = 16;
					}
					return true;
				}
				IL_7CE:
				if (this.thingClass == typeof(Bullet) && this.projectile.damageDef == null)
				{
					this.$current = " is a bullet but has no damageDef.";
					if (!this.$disposing)
					{
						this.$PC = 17;
					}
					return true;
				}
				IL_81D:
				if (!this.destroyOnDrop)
				{
					goto IL_8A2;
				}
				if (!this.menuHidden)
				{
					this.$current = "destroyOnDrop but not menuHidden.";
					if (!this.$disposing)
					{
						this.$PC = 18;
					}
					return true;
				}
				IL_85D:
				if (this.tradeability != Tradeability.None)
				{
					this.$current = "destroyOnDrop but tradeability is " + this.tradeability;
					if (!this.$disposing)
					{
						this.$PC = 19;
					}
					return true;
				}
				IL_8A2:
				if (this.stackLimit > 1 && !this.drawGUIOverlay)
				{
					this.$current = "has stackLimit > 1 but also has drawGUIOverlay = false.";
					if (!this.$disposing)
					{
						this.$PC = 20;
					}
					return true;
				}
				IL_8E3:
				if (this.damageMultipliers == null)
				{
					goto IL_9DE;
				}
				enumerator6 = this.damageMultipliers.GetEnumerator();
				num = 4294967293u;
				try
				{
					IL_90C:
					switch (num)
					{
					case 21u:
						break;
					default:
						while (enumerator6.MoveNext())
						{
							DamageMultiplier mult = enumerator6.Current;
							if ((from m in this.damageMultipliers
							where m.damageDef == mult.damageDef
							select m).Count<DamageMultiplier>() > 1)
							{
								this.$current = "has multiple damage multipliers for damageDef " + mult.damageDef;
								if (!this.$disposing)
								{
									this.$PC = 21;
								}
								flag = true;
								return true;
							}
						}
						break;
					}
				}
				finally
				{
					if (!flag)
					{
						((IDisposable)enumerator6).Dispose();
					}
				}
				IL_9DE:
				if (base.Fillage == FillCategory.Full && !this.IsEdifice())
				{
					this.$current = "fillPercent is 1.00 but is not edifice";
					if (!this.$disposing)
					{
						this.$PC = 22;
					}
					return true;
				}
				goto IL_A1F;
				IL_A5F:
				if (base.MadeFromStuff && this.stuffCategories.NullOrEmpty<StuffCategoryDef>())
				{
					this.$current = "madeFromStuff but has no stuffCategories.";
					if (!this.$disposing)
					{
						this.$PC = 24;
					}
					return true;
				}
				IL_AA4:
				if (this.costList.NullOrEmpty<ThingDefCountClass>() && this.costStuffCount <= 0 && this.recipeMaker != null)
				{
					this.$current = "has a recipeMaker but no costList or costStuffCount.";
					if (!this.$disposing)
					{
						this.$PC = 25;
					}
					return true;
				}
				IL_AFA:
				if (this.GetStatValueAbstract(StatDefOf.DeteriorationRate, null) > 1E-05f && !base.CanEverDeteriorate)
				{
					this.$current = "has >0 DeteriorationRate but can't deteriorate.";
					if (!this.$disposing)
					{
						this.$PC = 26;
					}
					return true;
				}
				IL_B45:
				if (this.drawerType == DrawerType.MapMeshOnly)
				{
					if (this.comps.Any((CompProperties c) => c.compClass == typeof(CompForbiddable)))
					{
						this.$current = "drawerType=MapMeshOnly but has a CompForbiddable, which must draw in real time.";
						if (!this.$disposing)
						{
							this.$PC = 27;
						}
						return true;
					}
				}
				IL_BA8:
				if (this.smeltProducts != null && this.smeltable)
				{
					this.$current = "has smeltProducts but has smeltable=false";
					if (!this.$disposing)
					{
						this.$PC = 28;
					}
					return true;
				}
				IL_BE8:
				if (this.equipmentType != EquipmentType.None && this.verbs.NullOrEmpty<VerbProperties>() && this.tools.NullOrEmpty<Tool>())
				{
					this.$current = "is equipment but has no verbs or tools";
					if (!this.$disposing)
					{
						this.$PC = 29;
					}
					return true;
				}
				IL_C42:
				if (base.Minifiable && this.thingCategories.NullOrEmpty<ThingCategoryDef>())
				{
					this.$current = "is minifiable but not in any thing category";
					if (!this.$disposing)
					{
						this.$PC = 30;
					}
					return true;
				}
				IL_C87:
				if (this.category == ThingCategory.Building && !base.Minifiable && !this.thingCategories.NullOrEmpty<ThingCategoryDef>())
				{
					this.$current = "is not minifiable yet has thing categories (could be confusing in thing filters because it can't be moved/stored anyway)";
					if (!this.$disposing)
					{
						this.$PC = 31;
					}
					return true;
				}
				IL_CDD:
				if (this != ThingDefOf.MinifiedThing && (base.EverHaulable || base.Minifiable))
				{
					if (!this.statBases.NullOrEmpty<StatModifier>())
					{
						if (this.statBases.Any((StatModifier s) => s.stat == StatDefOf.Mass))
						{
							goto IL_D74;
						}
					}
					this.$current = "is haulable, but does not have an authored mass value";
					if (!this.$disposing)
					{
						this.$PC = 32;
					}
					return true;
				}
				IL_D74:
				if (this.ingestible == null && this.GetStatValueAbstract(StatDefOf.Nutrition, null) != 0f)
				{
					this.$current = "has nutrition but ingestible properties are null";
					if (!this.$disposing)
					{
						this.$PC = 33;
					}
					return true;
				}
				IL_DBF:
				if (base.BaseFlammability != 0f && !this.useHitPoints && this.category != ThingCategory.Pawn)
				{
					this.$current = "flammable but has no hitpoints (will burn indefinitely)";
					if (!this.$disposing)
					{
						this.$PC = 34;
					}
					return true;
				}
				IL_E15:
				if (this.graphicData != null && this.graphicData.shadowData != null && this.staticSunShadowHeight > 0f)
				{
					this.$current = "graphicData defines a shadowInfo but staticSunShadowHeight > 0";
					if (!this.$disposing)
					{
						this.$PC = 35;
					}
					return true;
				}
				IL_E6F:
				if (this.saveCompressible && base.Claimable)
				{
					this.$current = "claimable item is compressible; faction will be unset after load";
					if (!this.$disposing)
					{
						this.$PC = 36;
					}
					return true;
				}
				IL_EAF:
				if (this.deepCommonality > 0f != this.deepLumpSizeRange.TrueMax > 0)
				{
					this.$current = "if deepCommonality or deepLumpSizeRange is set, the other also must be set";
					if (!this.$disposing)
					{
						this.$PC = 37;
					}
					return true;
				}
				IL_EF9:
				if (this.deepCommonality > 0f && this.deepCountPerPortion <= 0)
				{
					this.$current = "deepCommonality > 0 but deepCountPerPortion is not set";
					if (!this.$disposing)
					{
						this.$PC = 38;
					}
					return true;
				}
				IL_F3F:
				if (this.verbs == null)
				{
					goto IL_103E;
				}
				i = 0;
				IL_1023:
				if (i < this.verbs.Count)
				{
					enumerator7 = this.verbs[i].ConfigErrors(this).GetEnumerator();
					num = 4294967293u;
					goto Block_115;
				}
				IL_103E:
				if (this.race == null || this.tools == null)
				{
					goto IL_1187;
				}
				i = 0;
				IL_1167:
				if (i < this.tools.Count)
				{
					if (this.tools[i].linkedBodyPartsGroup != null && !this.race.body.AllParts.Any((BodyPartRecord part) => part.groups.Contains(this.tools[i].linkedBodyPartsGroup)))
					{
						this.$current = string.Concat(new object[]
						{
							"has tool with linkedBodyPartsGroup ",
							this.tools[i].linkedBodyPartsGroup,
							" but body ",
							this.race.body,
							" has no parts with that group."
						});
						if (!this.$disposing)
						{
							this.$PC = 40;
						}
						return true;
					}
					goto IL_114F;
				}
				IL_1187:
				if (this.building != null)
				{
					enumerator8 = this.building.ConfigErrors(this).GetEnumerator();
					num = 4294967293u;
					goto Block_122;
				}
				IL_1231:
				if (this.apparel != null)
				{
					enumerator9 = this.apparel.ConfigErrors(this).GetEnumerator();
					num = 4294967293u;
					goto Block_124;
				}
				IL_12DB:
				if (this.comps == null)
				{
					goto IL_13C5;
				}
				j = 0;
				IL_13AA:
				if (j < this.comps.Count)
				{
					enumerator10 = this.comps[j].ConfigErrors(this).GetEnumerator();
					num = 4294967293u;
					goto Block_126;
				}
				IL_13C5:
				if (this.race != null)
				{
					enumerator11 = this.race.ConfigErrors().GetEnumerator();
					num = 4294967293u;
					goto Block_128;
				}
				IL_1469:
				if (this.ingestible != null)
				{
					enumerator12 = this.ingestible.ConfigErrors().GetEnumerator();
					num = 4294967293u;
					goto Block_130;
				}
				IL_150D:
				if (this.plant != null)
				{
					enumerator13 = this.plant.ConfigErrors().GetEnumerator();
					num = 4294967293u;
					goto Block_132;
				}
				IL_15B1:
				if (this.tools == null)
				{
					goto IL_1719;
				}
				dupeTool = this.tools.SelectMany((Tool lhs) => from rhs in this.tools
				where lhs != rhs && lhs.id == rhs.id
				select rhs).FirstOrDefault<Tool>();
				if (dupeTool != null)
				{
					this.$current = string.Format("duplicate thingdef tool id {0}", dupeTool.id);
					if (!this.$disposing)
					{
						this.$PC = 47;
					}
					return true;
				}
				goto IL_1623;
				Block_136:
				try
				{
					IL_163C:
					switch (num)
					{
					case 48u:
						Block_215:
						try
						{
							switch (num)
							{
							}
							if (enumerator15.MoveNext())
							{
								e4 = enumerator15.Current;
								this.$current = e4;
								if (!this.$disposing)
								{
									this.$PC = 48;
								}
								flag = true;
								return true;
							}
						}
						finally
						{
							if (!flag)
							{
								if (enumerator15 != null)
								{
									enumerator15.Dispose();
								}
							}
						}
						break;
					}
					if (enumerator14.MoveNext())
					{
						t = enumerator14.Current;
						enumerator15 = t.ConfigErrors().GetEnumerator();
						num = 4294967293u;
						goto Block_215;
					}
				}
				finally
				{
					if (!flag)
					{
						((IDisposable)enumerator14).Dispose();
					}
				}
				IL_1719:
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
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					break;
				case 3u:
					try
					{
					}
					finally
					{
						if (enumerator2 != null)
						{
							enumerator2.Dispose();
						}
					}
					break;
				case 4u:
					try
					{
					}
					finally
					{
						if (enumerator3 != null)
						{
							enumerator3.Dispose();
						}
					}
					break;
				case 5u:
					try
					{
					}
					finally
					{
						((IDisposable)enumerator4).Dispose();
					}
					break;
				case 11u:
					try
					{
					}
					finally
					{
						((IDisposable)enumerator5).Dispose();
					}
					break;
				case 21u:
					try
					{
					}
					finally
					{
						((IDisposable)enumerator6).Dispose();
					}
					break;
				case 39u:
					try
					{
					}
					finally
					{
						if (enumerator7 != null)
						{
							enumerator7.Dispose();
						}
					}
					break;
				case 41u:
					try
					{
					}
					finally
					{
						if (enumerator8 != null)
						{
							enumerator8.Dispose();
						}
					}
					break;
				case 42u:
					try
					{
					}
					finally
					{
						if (enumerator9 != null)
						{
							enumerator9.Dispose();
						}
					}
					break;
				case 43u:
					try
					{
					}
					finally
					{
						if (enumerator10 != null)
						{
							enumerator10.Dispose();
						}
					}
					break;
				case 44u:
					try
					{
					}
					finally
					{
						if (enumerator11 != null)
						{
							enumerator11.Dispose();
						}
					}
					break;
				case 45u:
					try
					{
					}
					finally
					{
						if (enumerator12 != null)
						{
							enumerator12.Dispose();
						}
					}
					break;
				case 46u:
					try
					{
					}
					finally
					{
						if (enumerator13 != null)
						{
							enumerator13.Dispose();
						}
					}
					break;
				case 48u:
					try
					{
						try
						{
						}
						finally
						{
							if (enumerator15 != null)
							{
								enumerator15.Dispose();
							}
						}
					}
					finally
					{
						((IDisposable)enumerator14).Dispose();
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
				return this.System.Collections.Generic.IEnumerable<string>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<string> IEnumerable<string>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				ThingDef.<ConfigErrors>c__Iterator0 <ConfigErrors>c__Iterator = new ThingDef.<ConfigErrors>c__Iterator0();
				<ConfigErrors>c__Iterator.$this = this;
				return <ConfigErrors>c__Iterator;
			}

			internal bool <>m__0(ThingCategoryDef cat)
			{
				return this.thingCategories.Count((ThingCategoryDef c) => c == cat) > 1;
			}

			private static bool <>m__1(CompProperties c)
			{
				return c.compClass == typeof(CompPowerTrader);
			}

			private static bool <>m__2(CompProperties c)
			{
				return c.compClass == typeof(CompEquippable);
			}

			private static bool <>m__3(CompProperties c)
			{
				return c.compClass == typeof(CompForbiddable);
			}

			private static bool <>m__4(StatModifier s)
			{
				return s.stat == StatDefOf.Mass;
			}

			internal IEnumerable<Tool> <>m__5(Tool lhs)
			{
				return from rhs in this.tools
				where lhs != rhs && lhs.id == rhs.id
				select rhs;
			}

			private sealed class <ConfigErrors>c__AnonStorey2
			{
				internal StatModifier statBase;

				internal ThingDef.<ConfigErrors>c__Iterator0 <>f__ref$0;

				public <ConfigErrors>c__AnonStorey2()
				{
				}

				internal bool <>m__0(StatModifier st)
				{
					return st.stat == this.statBase.stat;
				}
			}

			private sealed class <ConfigErrors>c__AnonStorey4
			{
				internal DamageMultiplier mult;

				internal ThingDef.<ConfigErrors>c__Iterator0 <>f__ref$0;

				public <ConfigErrors>c__AnonStorey4()
				{
				}

				internal bool <>m__0(DamageMultiplier m)
				{
					return m.damageDef == this.mult.damageDef;
				}
			}

			private sealed class <ConfigErrors>c__AnonStorey5
			{
				internal int i;

				internal ThingDef.<ConfigErrors>c__Iterator0 <>f__ref$0;

				public <ConfigErrors>c__AnonStorey5()
				{
				}

				internal bool <>m__0(BodyPartRecord part)
				{
					return part.groups.Contains(this.<>f__ref$0.$this.tools[this.i].linkedBodyPartsGroup);
				}
			}

			private sealed class <ConfigErrors>c__AnonStorey3
			{
				internal ThingCategoryDef cat;

				internal ThingDef $this;

				public <ConfigErrors>c__AnonStorey3()
				{
				}

				internal bool <>m__0(ThingCategoryDef c)
				{
					return c == this.cat;
				}
			}

			private sealed class <ConfigErrors>c__AnonStorey6
			{
				internal Tool lhs;

				internal ThingDef $this;

				public <ConfigErrors>c__AnonStorey6()
				{
				}

				internal bool <>m__0(Tool rhs)
				{
					return this.lhs != rhs && this.lhs.id == rhs.id;
				}
			}
		}

		[CompilerGenerated]
		private sealed class <SpecialDisplayStats>c__Iterator1 : IEnumerable, IEnumerable<StatDrawEntry>, IEnumerator, IDisposable, IEnumerator<StatDrawEntry>
		{
			internal StatRequest req;

			internal IEnumerator<StatDrawEntry> $locvar0;

			internal StatDrawEntry <stat>__1;

			internal string <coveredParts>__2;

			internal StatDrawEntry <sde>__3;

			internal VerbProperties <verb>__4;

			internal StatCategoryDef <verbStatCategory>__4;

			internal float <warmup>__4;

			internal string <warmupLabel>__5;

			internal StringBuilder <damageAmountExplanation>__6;

			internal float <dam>__6;

			internal StringBuilder <armorPenetrationExplanation>__7;

			internal float <ap>__7;

			internal string <fullExplanation>__7;

			internal int <burstShotCount>__8;

			internal float <burstShotFireRate>__8;

			internal float <range>__8;

			internal IEnumerator<StatDrawEntry> $locvar1;

			internal StatDrawEntry <s>__9;

			internal IEnumerator<StatDrawEntry> $locvar2;

			internal StatDrawEntry <s>__10;

			internal IEnumerator<StatDrawEntry> $locvar3;

			internal StatDrawEntry <s>__11;

			internal IEnumerator<StatDrawEntry> $locvar4;

			internal StatDrawEntry <s>__12;

			internal IEnumerator<RecipeDef> $locvar5;

			internal RecipeDef <def>__13;

			internal IEnumerator<StatDrawEntry> $locvar6;

			internal StatDrawEntry <s>__15;

			internal HediffCompProperties_VerbGiver <vg>__16;

			internal VerbProperties <verb>__17;

			internal int <projDamage>__18;

			internal float <projArmorPenetration>__19;

			internal int <meleeDamage>__20;

			internal float <armorPenetration>__21;

			internal Tool <tool>__22;

			internal float <armorPenetration>__23;

			internal ThoughtDef <thought>__16;

			internal int <i>__24;

			internal IEnumerator<StatDrawEntry> $locvar7;

			internal StatDrawEntry <s>__25;

			internal ThingDef $this;

			internal StatDrawEntry $current;

			internal bool $disposing;

			internal int $PC;

			private static Func<VerbProperties, bool> <>f__am$cache0;

			private ThingDef.<SpecialDisplayStats>c__Iterator1.<SpecialDisplayStats>c__AnonStorey7 $locvar8;

			[DebuggerHidden]
			public <SpecialDisplayStats>c__Iterator1()
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
					enumerator = base.<SpecialDisplayStats>__BaseCallProxy1(req).GetEnumerator();
					num = 4294967293u;
					break;
				case 1u:
					break;
				case 2u:
					this.$current = new StatDrawEntry(StatCategoryDefOf.Apparel, "Layer".Translate(), this.apparel.GetLayersString(), 95, string.Empty);
					if (!this.$disposing)
					{
						this.$PC = 3;
					}
					return true;
				case 3u:
					goto IL_1D1;
				case 4u:
					goto IL_23A;
				case 5u:
					goto IL_2FC;
				case 6u:
					goto IL_368;
				case 7u:
					goto IL_485;
				case 8u:
					if (verb.defaultProjectile.projectile.damageDef.armorCategory != null)
					{
						armorPenetrationExplanation = new StringBuilder();
						ap = verb.defaultProjectile.projectile.GetArmorPenetration(req.Thing, armorPenetrationExplanation);
						fullExplanation = "ArmorPenetrationExplanation".Translate();
						if (armorPenetrationExplanation.Length != 0)
						{
							fullExplanation = fullExplanation + "\n\n" + armorPenetrationExplanation;
						}
						this.$current = new StatDrawEntry(verbStatCategory, "ArmorPenetration".Translate(), ap.ToStringPercent(), 49, fullExplanation);
						if (!this.$disposing)
						{
							this.$PC = 9;
						}
						return true;
					}
					goto IL_5EF;
				case 9u:
					goto IL_5EF;
				case 10u:
					this.$current = new StatDrawEntry(verbStatCategory, "BurstShotFireRate".Translate(), burstShotFireRate.ToString("0.##") + " rpm", 19, string.Empty);
					if (!this.$disposing)
					{
						this.$PC = 11;
					}
					return true;
				case 11u:
					goto IL_6E2;
				case 12u:
					if (verb.defaultProjectile != null && verb.defaultProjectile.projectile != null && verb.defaultProjectile.projectile.stoppingPower != 0f)
					{
						StatCategoryDef category = verbStatCategory;
						string text = "StoppingPower".Translate();
						string valueString = verb.defaultProjectile.projectile.stoppingPower.ToString("F1");
						string text2 = "StoppingPowerExplanation".Translate();
						this.$current = new StatDrawEntry(category, text, valueString, 0, text2);
						if (!this.$disposing)
						{
							this.$PC = 13;
						}
						return true;
					}
					goto IL_7D3;
				case 13u:
					goto IL_7D3;
				case 14u:
					this.$current = new StatDrawEntry(verbStatCategory, "DirectHitChance".Translate(), (1f / (float)GenRadial.NumCellsInRadius(verb.forcedMissRadius)).ToStringPercent(), 29, string.Empty);
					if (!this.$disposing)
					{
						this.$PC = 15;
					}
					return true;
				case 15u:
					goto IL_887;
				case 16u:
					goto IL_8B5;
				case 17u:
					Block_42:
					try
					{
						switch (num)
						{
						}
						if (enumerator3.MoveNext())
						{
							s2 = enumerator3.Current;
							this.$current = s2;
							if (!this.$disposing)
							{
								this.$PC = 17;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator3 != null)
							{
								enumerator3.Dispose();
							}
						}
					}
					goto IL_9CF;
				case 18u:
					Block_44:
					try
					{
						switch (num)
						{
						}
						if (enumerator4.MoveNext())
						{
							s3 = enumerator4.Current;
							this.$current = s3;
							if (!this.$disposing)
							{
								this.$PC = 18;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator4 != null)
							{
								enumerator4.Dispose();
							}
						}
					}
					goto IL_A79;
				case 19u:
					Block_46:
					try
					{
						switch (num)
						{
						}
						if (enumerator5.MoveNext())
						{
							s4 = enumerator5.Current;
							this.$current = s4;
							if (!this.$disposing)
							{
								this.$PC = 19;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator5 != null)
							{
								enumerator5.Dispose();
							}
						}
					}
					goto IL_B23;
				case 20u:
				case 21u:
				case 22u:
				case 23u:
				case 24u:
				case 25u:
				case 26u:
				case 27u:
				case 28u:
					Block_48:
					try
					{
						switch (num)
						{
						case 20u:
							goto IL_C33;
						case 21u:
							goto IL_C56;
						case 22u:
							if (verb2.defaultProjectile.projectile.damageDef.armorCategory != null)
							{
								projArmorPenetration = verb2.defaultProjectile.projectile.GetArmorPenetration(null, null);
								StatCategoryDef category = StatCategoryDefOf.Basics;
								string text2 = "ArmorPenetration".Translate();
								string valueString = projArmorPenetration.ToStringPercent();
								string text = "ArmorPenetrationExplanation".Translate();
								this.$current = new StatDrawEntry(category, text2, valueString, 0, text);
								if (!this.$disposing)
								{
									this.$PC = 23;
								}
								flag = true;
								return true;
							}
							goto IL_E2D;
						case 23u:
							goto IL_E2D;
						case 24u:
							if (verb2.meleeDamageDef.armorCategory != null)
							{
								armorPenetration = verb2.meleeArmorPenetrationBase;
								if (armorPenetration < 0f)
								{
									armorPenetration = (float)meleeDamage * 0.015f;
								}
								StatCategoryDef category = StatCategoryDefOf.Weapon;
								string text = "ArmorPenetration".Translate();
								string valueString = armorPenetration.ToStringPercent();
								string text2 = "ArmorPenetrationExplanation".Translate();
								this.$current = new StatDrawEntry(category, text, valueString, 0, text2);
								if (!this.$disposing)
								{
									this.$PC = 25;
								}
								flag = true;
								return true;
							}
							goto IL_F27;
						case 25u:
							goto IL_F27;
						case 26u:
							if (ThingUtility.PrimaryMeleeWeaponDamageType(vg.tools).armorCategory != null)
							{
								armorPenetration2 = tool.armorPenetration;
								if (armorPenetration2 < 0f)
								{
									armorPenetration2 = tool.power * 0.015f;
								}
								StatCategoryDef category = StatCategoryDefOf.Weapon;
								string text2 = "ArmorPenetration".Translate();
								string valueString = armorPenetration2.ToStringPercent();
								string text = "ArmorPenetrationExplanation".Translate();
								this.$current = new StatDrawEntry(category, text2, valueString, 0, text);
								if (!this.$disposing)
								{
									this.$PC = 27;
								}
								flag = true;
								return true;
							}
							goto IL_104A;
						case 27u:
							goto IL_104A;
						}
						IL_10EE:
						while (enumerator6.MoveNext())
						{
							def = enumerator6.Current;
							HediffDef diff = def.addsHediff;
							if (diff != null)
							{
								if (diff.addedPartProps != null)
								{
									this.$current = new StatDrawEntry(StatCategoryDefOf.Basics, "BodyPartEfficiency".Translate(), diff.addedPartProps.partEfficiency.ToStringByStyle(ToStringStyle.PercentZero, ToStringNumberSense.Absolute), 0, string.Empty);
									if (!this.$disposing)
									{
										this.$PC = 20;
									}
									flag = true;
									return true;
								}
								goto IL_C33;
							}
						}
						goto IL_111E;
						IL_C33:
						enumerator7 = <SpecialDisplayStats>c__AnonStorey.diff.SpecialDisplayStats(StatRequest.ForEmpty()).GetEnumerator();
						num = 4294967293u;
						try
						{
							IL_C56:
							switch (num)
							{
							}
							if (enumerator7.MoveNext())
							{
								s5 = enumerator7.Current;
								this.$current = s5;
								if (!this.$disposing)
								{
									this.$PC = 21;
								}
								flag = true;
								return true;
							}
						}
						finally
						{
							if (!flag)
							{
								if (enumerator7 != null)
								{
									enumerator7.Dispose();
								}
							}
						}
						vg = <SpecialDisplayStats>c__AnonStorey.diff.CompProps<HediffCompProperties_VerbGiver>();
						if (vg != null)
						{
							if (!vg.verbs.NullOrEmpty<VerbProperties>())
							{
								verb2 = vg.verbs[0];
								if (verb2.IsMeleeAttack)
								{
									meleeDamage = verb2.meleeDamageBaseAmount;
									this.$current = new StatDrawEntry(StatCategoryDefOf.Weapon, "Damage".Translate(), meleeDamage.ToString(), 0, string.Empty);
									if (!this.$disposing)
									{
										this.$PC = 24;
									}
									flag = true;
									return true;
								}
								if (verb2.defaultProjectile != null)
								{
									projDamage = verb2.defaultProjectile.projectile.GetDamageAmount(null, null);
									this.$current = new StatDrawEntry(StatCategoryDefOf.Basics, "Damage".Translate(), projDamage.ToString(), 0, string.Empty);
									if (!this.$disposing)
									{
										this.$PC = 22;
									}
									flag = true;
									return true;
								}
							}
							else if (!vg.tools.NullOrEmpty<Tool>())
							{
								tool = vg.tools[0];
								this.$current = new StatDrawEntry(StatCategoryDefOf.Weapon, "Damage".Translate(), tool.power.ToString(), 0, string.Empty);
								if (!this.$disposing)
								{
									this.$PC = 26;
								}
								flag = true;
								return true;
							}
						}
						IL_E2D:
						IL_F27:
						IL_104A:
						thought = DefDatabase<ThoughtDef>.AllDefs.FirstOrDefault((ThoughtDef x) => x.hediff == <SpecialDisplayStats>c__AnonStorey.diff);
						if (thought != null && thought.stages != null && thought.stages.Any<ThoughtStage>())
						{
							this.$current = new StatDrawEntry(StatCategoryDefOf.Basics, "MoodChange".Translate(), thought.stages.First<ThoughtStage>().baseMoodEffect.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Offset), 0, string.Empty);
							if (!this.$disposing)
							{
								this.$PC = 28;
							}
							flag = true;
							return true;
						}
						goto IL_10EE;
					}
					finally
					{
						if (!flag)
						{
							if (enumerator6 != null)
							{
								enumerator6.Dispose();
							}
						}
					}
					goto IL_111E;
				case 29u:
					Block_49:
					try
					{
						switch (num)
						{
						}
						if (enumerator8.MoveNext())
						{
							s6 = enumerator8.Current;
							this.$current = s6;
							if (!this.$disposing)
							{
								this.$PC = 29;
							}
							flag = true;
							return true;
						}
					}
					finally
					{
						if (!flag)
						{
							if (enumerator8 != null)
							{
								enumerator8.Dispose();
							}
						}
					}
					i++;
					goto IL_11D7;
				default:
					return false;
				}
				try
				{
					switch (num)
					{
					}
					if (enumerator.MoveNext())
					{
						stat = enumerator.Current;
						this.$current = stat;
						if (!this.$disposing)
						{
							this.$PC = 1;
						}
						flag = true;
						return true;
					}
				}
				finally
				{
					if (!flag)
					{
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
				}
				if (this.apparel != null)
				{
					coveredParts = this.apparel.GetCoveredOuterPartsString(BodyDefOf.Human);
					this.$current = new StatDrawEntry(StatCategoryDefOf.Apparel, "Covers".Translate(), coveredParts, 100, string.Empty);
					if (!this.$disposing)
					{
						this.$PC = 2;
					}
					return true;
				}
				IL_1D1:
				if (base.IsMedicine && base.MedicineTendXpGainFactor != 1f)
				{
					this.$current = new StatDrawEntry(StatCategoryDefOf.Basics, "MedicineXpGainFactor".Translate(), base.MedicineTendXpGainFactor.ToStringPercent(), 0, string.Empty);
					if (!this.$disposing)
					{
						this.$PC = 4;
					}
					return true;
				}
				IL_23A:
				if (this.fillPercent > 0f && this.fillPercent < 1f && (this.category == ThingCategory.Item || this.category == ThingCategory.Building || this.category == ThingCategory.Plant))
				{
					StatDrawEntry sde = new StatDrawEntry(StatCategoryDefOf.Basics, "CoverEffectiveness".Translate(), this.BaseBlockChance().ToStringPercent(), 0, string.Empty);
					sde.overrideReportText = "CoverEffectivenessExplanation".Translate();
					this.$current = sde;
					if (!this.$disposing)
					{
						this.$PC = 5;
					}
					return true;
				}
				IL_2FC:
				if (this.constructionSkillPrerequisite > 0)
				{
					StatCategoryDef category = StatCategoryDefOf.Basics;
					string text2 = "ConstructionSkillRequired".Translate();
					string valueString = this.constructionSkillPrerequisite.ToString();
					string text = "ConstructionSkillRequiredExplanation".Translate();
					this.$current = new StatDrawEntry(category, text2, valueString, 0, text);
					if (!this.$disposing)
					{
						this.$PC = 6;
					}
					return true;
				}
				IL_368:
				if (this.verbs.NullOrEmpty<VerbProperties>())
				{
					goto IL_887;
				}
				verb = this.verbs.First((VerbProperties x) => x.isPrimary);
				verbStatCategory = ((this.category != ThingCategory.Pawn) ? (verbStatCategory = StatCategoryDefOf.Weapon) : (verbStatCategory = StatCategoryDefOf.PawnCombat));
				warmup = verb.warmupTime;
				if (warmup > 0f)
				{
					warmupLabel = ((this.category != ThingCategory.Pawn) ? "WarmupTime".Translate() : "MeleeWarmupTime".Translate());
					this.$current = new StatDrawEntry(verbStatCategory, warmupLabel, warmup.ToString("0.##") + " s", 40, string.Empty);
					if (!this.$disposing)
					{
						this.$PC = 7;
					}
					return true;
				}
				IL_485:
				if (verb.defaultProjectile != null)
				{
					damageAmountExplanation = new StringBuilder();
					dam = (float)verb.defaultProjectile.projectile.GetDamageAmount(req.Thing, damageAmountExplanation);
					this.$current = new StatDrawEntry(verbStatCategory, "Damage".Translate(), dam.ToString(), 50, damageAmountExplanation.ToString());
					if (!this.$disposing)
					{
						this.$PC = 8;
					}
					return true;
				}
				IL_5EF:
				if (!verb.LaunchesProjectile)
				{
					goto IL_7D3;
				}
				burstShotCount = verb.burstShotCount;
				burstShotFireRate = 60f / verb.ticksBetweenBurstShots.TicksToSeconds();
				range = verb.range;
				if (burstShotCount > 1)
				{
					this.$current = new StatDrawEntry(verbStatCategory, "BurstShotCount".Translate(), burstShotCount.ToString(), 20, string.Empty);
					if (!this.$disposing)
					{
						this.$PC = 10;
					}
					return true;
				}
				IL_6E2:
				this.$current = new StatDrawEntry(verbStatCategory, "Range".Translate(), range.ToString("F0"), 10, string.Empty);
				if (!this.$disposing)
				{
					this.$PC = 12;
				}
				return true;
				IL_7D3:
				if (verb.forcedMissRadius > 0f)
				{
					this.$current = new StatDrawEntry(verbStatCategory, "MissRadius".Translate(), verb.forcedMissRadius.ToString("0.#"), 30, string.Empty);
					if (!this.$disposing)
					{
						this.$PC = 14;
					}
					return true;
				}
				IL_887:
				if (this.plant == null)
				{
					goto IL_92B;
				}
				enumerator2 = this.plant.SpecialDisplayStats().GetEnumerator();
				num = 4294967293u;
				try
				{
					IL_8B5:
					switch (num)
					{
					}
					if (enumerator2.MoveNext())
					{
						s = enumerator2.Current;
						this.$current = s;
						if (!this.$disposing)
						{
							this.$PC = 16;
						}
						flag = true;
						return true;
					}
				}
				finally
				{
					if (!flag)
					{
						if (enumerator2 != null)
						{
							enumerator2.Dispose();
						}
					}
				}
				IL_92B:
				if (this.ingestible != null)
				{
					enumerator3 = this.ingestible.SpecialDisplayStats().GetEnumerator();
					num = 4294967293u;
					goto Block_42;
				}
				IL_9CF:
				if (this.race != null)
				{
					enumerator4 = this.race.SpecialDisplayStats(this).GetEnumerator();
					num = 4294967293u;
					goto Block_44;
				}
				IL_A79:
				if (this.building != null)
				{
					enumerator5 = this.building.SpecialDisplayStats(this).GetEnumerator();
					num = 4294967293u;
					goto Block_46;
				}
				IL_B23:
				if (this.isTechHediff)
				{
					enumerator6 = (from x in DefDatabase<RecipeDef>.AllDefs
					where x.IsIngredient(this)
					select x).GetEnumerator();
					num = 4294967293u;
					goto Block_48;
				}
				IL_111E:
				i = 0;
				IL_11D7:
				if (i < this.comps.Count)
				{
					enumerator8 = this.comps[i].SpecialDisplayStats().GetEnumerator();
					num = 4294967293u;
					goto Block_49;
				}
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
						if (enumerator != null)
						{
							enumerator.Dispose();
						}
					}
					break;
				case 16u:
					try
					{
					}
					finally
					{
						if (enumerator2 != null)
						{
							enumerator2.Dispose();
						}
					}
					break;
				case 17u:
					try
					{
					}
					finally
					{
						if (enumerator3 != null)
						{
							enumerator3.Dispose();
						}
					}
					break;
				case 18u:
					try
					{
					}
					finally
					{
						if (enumerator4 != null)
						{
							enumerator4.Dispose();
						}
					}
					break;
				case 19u:
					try
					{
					}
					finally
					{
						if (enumerator5 != null)
						{
							enumerator5.Dispose();
						}
					}
					break;
				case 20u:
				case 21u:
				case 22u:
				case 23u:
				case 24u:
				case 25u:
				case 26u:
				case 27u:
				case 28u:
					try
					{
						switch (num)
						{
						case 21u:
							try
							{
							}
							finally
							{
								if (enumerator7 != null)
								{
									enumerator7.Dispose();
								}
							}
							break;
						}
					}
					finally
					{
						if (enumerator6 != null)
						{
							enumerator6.Dispose();
						}
					}
					break;
				case 29u:
					try
					{
					}
					finally
					{
						if (enumerator8 != null)
						{
							enumerator8.Dispose();
						}
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
				return this.System.Collections.Generic.IEnumerable<RimWorld.StatDrawEntry>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<StatDrawEntry> IEnumerable<StatDrawEntry>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				ThingDef.<SpecialDisplayStats>c__Iterator1 <SpecialDisplayStats>c__Iterator = new ThingDef.<SpecialDisplayStats>c__Iterator1();
				<SpecialDisplayStats>c__Iterator.$this = this;
				<SpecialDisplayStats>c__Iterator.req = req;
				return <SpecialDisplayStats>c__Iterator;
			}

			private static bool <>m__0(VerbProperties x)
			{
				return x.isPrimary;
			}

			internal bool <>m__1(RecipeDef x)
			{
				return x.IsIngredient(this);
			}

			private sealed class <SpecialDisplayStats>c__AnonStorey7
			{
				internal HediffDef diff;

				public <SpecialDisplayStats>c__AnonStorey7()
				{
				}

				internal bool <>m__0(ThoughtDef x)
				{
					return x.hediff == this.diff;
				}
			}
		}
	}
}
