﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Verse.AI;

namespace RimWorld
{
	public class WorkGiver_DoBill : WorkGiver_Scanner
	{
		private List<ThingCount> chosenIngThings = new List<ThingCount>();

		private static readonly IntRange ReCheckFailedBillTicksRange = new IntRange(500, 600);

		private static string MissingMaterialsTranslated;

		private static List<Thing> relevantThings = new List<Thing>();

		private static HashSet<Thing> processedThings = new HashSet<Thing>();

		private static List<Thing> newRelevantThings = new List<Thing>();

		private static List<IngredientCount> ingredientsOrdered = new List<IngredientCount>();

		private static List<Thing> tmpMedicine = new List<Thing>();

		private static WorkGiver_DoBill.DefCountList availableCounts = new WorkGiver_DoBill.DefCountList();

		[CompilerGenerated]
		private static Func<Thing, float> <>f__am$cache0;

		public WorkGiver_DoBill()
		{
		}

		public override PathEndMode PathEndMode
		{
			get
			{
				return PathEndMode.InteractionCell;
			}
		}

		public override Danger MaxPathDanger(Pawn pawn)
		{
			return Danger.Some;
		}

		public override ThingRequest PotentialWorkThingRequest
		{
			get
			{
				if (this.def.fixedBillGiverDefs != null && this.def.fixedBillGiverDefs.Count == 1)
				{
					return ThingRequest.ForDef(this.def.fixedBillGiverDefs[0]);
				}
				return ThingRequest.ForGroup(ThingRequestGroup.PotentialBillGiver);
			}
		}

		public static void ResetStaticData()
		{
			WorkGiver_DoBill.MissingMaterialsTranslated = "MissingMaterials".Translate();
		}

		public override Job JobOnThing(Pawn pawn, Thing thing, bool forced = false)
		{
			IBillGiver billGiver = thing as IBillGiver;
			if (billGiver != null && this.ThingIsUsableBillGiver(thing) && billGiver.BillStack.AnyShouldDoNow && billGiver.UsableForBillsAfterFueling())
			{
				LocalTargetInfo target = thing;
				if (pawn.CanReserve(target, 1, -1, null, forced) && !thing.IsBurning() && !thing.IsForbidden(pawn))
				{
					CompRefuelable compRefuelable = thing.TryGetComp<CompRefuelable>();
					if (compRefuelable == null || compRefuelable.HasFuel)
					{
						billGiver.BillStack.RemoveIncompletableBills();
						return this.StartOrResumeBillJob(pawn, billGiver);
					}
					if (!RefuelWorkGiverUtility.CanRefuel(pawn, thing, forced))
					{
						return null;
					}
					return RefuelWorkGiverUtility.RefuelJob(pawn, thing, forced, null, null);
				}
			}
			return null;
		}

		private static UnfinishedThing ClosestUnfinishedThingForBill(Pawn pawn, Bill_ProductionWithUft bill)
		{
			Predicate<Thing> predicate = (Thing t) => !t.IsForbidden(pawn) && ((UnfinishedThing)t).Recipe == bill.recipe && ((UnfinishedThing)t).Creator == pawn && ((UnfinishedThing)t).ingredients.TrueForAll((Thing x) => bill.IsFixedOrAllowedIngredient(x.def)) && pawn.CanReserve(t, 1, -1, null, false);
			IntVec3 position = pawn.Position;
			Map map = pawn.Map;
			ThingRequest thingReq = ThingRequest.ForDef(bill.recipe.unfinishedThingDef);
			PathEndMode peMode = PathEndMode.InteractionCell;
			TraverseParms traverseParams = TraverseParms.For(pawn, pawn.NormalMaxDanger(), TraverseMode.ByPawn, false);
			Predicate<Thing> validator = predicate;
			return (UnfinishedThing)GenClosest.ClosestThingReachable(position, map, thingReq, peMode, traverseParams, 9999f, validator, null, 0, -1, false, RegionType.Set_Passable, false);
		}

		private static Job FinishUftJob(Pawn pawn, UnfinishedThing uft, Bill_ProductionWithUft bill)
		{
			if (uft.Creator != pawn)
			{
				Log.Error(string.Concat(new object[]
				{
					"Tried to get FinishUftJob for ",
					pawn,
					" finishing ",
					uft,
					" but its creator is ",
					uft.Creator
				}), false);
				return null;
			}
			Job job = WorkGiverUtility.HaulStuffOffBillGiverJob(pawn, bill.billStack.billGiver, uft);
			if (job != null && job.targetA.Thing != uft)
			{
				return job;
			}
			return new Job(JobDefOf.DoBill, (Thing)bill.billStack.billGiver)
			{
				bill = bill,
				targetQueueB = new List<LocalTargetInfo>
				{
					uft
				},
				countQueue = new List<int>
				{
					1
				},
				haulMode = HaulMode.ToCellNonStorage
			};
		}

		private Job StartOrResumeBillJob(Pawn pawn, IBillGiver giver)
		{
			for (int i = 0; i < giver.BillStack.Count; i++)
			{
				Bill bill = giver.BillStack[i];
				if (bill.recipe.requiredGiverWorkType == null || bill.recipe.requiredGiverWorkType == this.def.workType)
				{
					if (Find.TickManager.TicksGame >= bill.lastIngredientSearchFailTicks + WorkGiver_DoBill.ReCheckFailedBillTicksRange.RandomInRange || FloatMenuMakerMap.makingFor == pawn)
					{
						bill.lastIngredientSearchFailTicks = 0;
						if (bill.ShouldDoNow())
						{
							if (bill.PawnAllowedToStartAnew(pawn))
							{
								SkillRequirement skillRequirement = bill.recipe.FirstSkillRequirementPawnDoesntSatisfy(pawn);
								if (skillRequirement != null)
								{
									JobFailReason.Is("UnderRequiredSkill".Translate(new object[]
									{
										skillRequirement.minLevel
									}), bill.Label);
								}
								else
								{
									Bill_ProductionWithUft bill_ProductionWithUft = bill as Bill_ProductionWithUft;
									if (bill_ProductionWithUft != null)
									{
										if (bill_ProductionWithUft.BoundUft != null)
										{
											if (bill_ProductionWithUft.BoundWorker != pawn || !pawn.CanReserveAndReach(bill_ProductionWithUft.BoundUft, PathEndMode.Touch, Danger.Deadly, 1, -1, null, false) || bill_ProductionWithUft.BoundUft.IsForbidden(pawn))
											{
												goto IL_1D9;
											}
											return WorkGiver_DoBill.FinishUftJob(pawn, bill_ProductionWithUft.BoundUft, bill_ProductionWithUft);
										}
										else
										{
											UnfinishedThing unfinishedThing = WorkGiver_DoBill.ClosestUnfinishedThingForBill(pawn, bill_ProductionWithUft);
											if (unfinishedThing != null)
											{
												return WorkGiver_DoBill.FinishUftJob(pawn, unfinishedThing, bill_ProductionWithUft);
											}
										}
									}
									if (WorkGiver_DoBill.TryFindBestBillIngredients(bill, pawn, (Thing)giver, this.chosenIngThings))
									{
										Job result = this.TryStartNewDoBillJob(pawn, bill, giver);
										this.chosenIngThings.Clear();
										return result;
									}
									if (FloatMenuMakerMap.makingFor != pawn)
									{
										bill.lastIngredientSearchFailTicks = Find.TickManager.TicksGame;
									}
									else
									{
										JobFailReason.Is(WorkGiver_DoBill.MissingMaterialsTranslated, bill.Label);
									}
									this.chosenIngThings.Clear();
								}
							}
						}
					}
				}
				IL_1D9:;
			}
			this.chosenIngThings.Clear();
			return null;
		}

		private Job TryStartNewDoBillJob(Pawn pawn, Bill bill, IBillGiver giver)
		{
			Job job = WorkGiverUtility.HaulStuffOffBillGiverJob(pawn, giver, null);
			if (job != null)
			{
				return job;
			}
			Job job2 = new Job(JobDefOf.DoBill, (Thing)giver);
			job2.targetQueueB = new List<LocalTargetInfo>(this.chosenIngThings.Count);
			job2.countQueue = new List<int>(this.chosenIngThings.Count);
			for (int i = 0; i < this.chosenIngThings.Count; i++)
			{
				job2.targetQueueB.Add(this.chosenIngThings[i].Thing);
				job2.countQueue.Add(this.chosenIngThings[i].Count);
			}
			job2.haulMode = HaulMode.ToCellNonStorage;
			job2.bill = bill;
			return job2;
		}

		public bool ThingIsUsableBillGiver(Thing thing)
		{
			Pawn pawn = thing as Pawn;
			Corpse corpse = thing as Corpse;
			Pawn pawn2 = null;
			if (corpse != null)
			{
				pawn2 = corpse.InnerPawn;
			}
			if (this.def.fixedBillGiverDefs != null && this.def.fixedBillGiverDefs.Contains(thing.def))
			{
				return true;
			}
			if (pawn != null)
			{
				if (this.def.billGiversAllHumanlikes && pawn.RaceProps.Humanlike)
				{
					return true;
				}
				if (this.def.billGiversAllMechanoids && pawn.RaceProps.IsMechanoid)
				{
					return true;
				}
				if (this.def.billGiversAllAnimals && pawn.RaceProps.Animal)
				{
					return true;
				}
			}
			if (corpse != null && pawn2 != null)
			{
				if (this.def.billGiversAllHumanlikesCorpses && pawn2.RaceProps.Humanlike)
				{
					return true;
				}
				if (this.def.billGiversAllMechanoidsCorpses && pawn2.RaceProps.IsMechanoid)
				{
					return true;
				}
				if (this.def.billGiversAllAnimalsCorpses && pawn2.RaceProps.Animal)
				{
					return true;
				}
			}
			return false;
		}

		private static bool TryFindBestBillIngredients(Bill bill, Pawn pawn, Thing billGiver, List<ThingCount> chosen)
		{
			chosen.Clear();
			WorkGiver_DoBill.newRelevantThings.Clear();
			if (bill.recipe.ingredients.Count == 0)
			{
				return true;
			}
			IntVec3 rootCell = WorkGiver_DoBill.GetBillGiverRootCell(billGiver, pawn);
			Region rootReg = rootCell.GetRegion(pawn.Map, RegionType.Set_Passable);
			if (rootReg == null)
			{
				return false;
			}
			WorkGiver_DoBill.MakeIngredientsListInProcessingOrder(WorkGiver_DoBill.ingredientsOrdered, bill);
			WorkGiver_DoBill.relevantThings.Clear();
			WorkGiver_DoBill.processedThings.Clear();
			bool foundAll = false;
			Predicate<Thing> baseValidator = (Thing t) => t.Spawned && !t.IsForbidden(pawn) && (float)(t.Position - billGiver.Position).LengthHorizontalSquared < bill.ingredientSearchRadius * bill.ingredientSearchRadius && bill.IsFixedOrAllowedIngredient(t) && bill.recipe.ingredients.Any((IngredientCount ingNeed) => ingNeed.filter.Allows(t)) && pawn.CanReserve(t, 1, -1, null, false);
			bool billGiverIsPawn = billGiver is Pawn;
			if (billGiverIsPawn)
			{
				WorkGiver_DoBill.AddEveryMedicineToRelevantThings(pawn, billGiver, WorkGiver_DoBill.relevantThings, baseValidator, pawn.Map);
				if (WorkGiver_DoBill.TryFindBestBillIngredientsInSet(WorkGiver_DoBill.relevantThings, bill, chosen))
				{
					WorkGiver_DoBill.relevantThings.Clear();
					WorkGiver_DoBill.ingredientsOrdered.Clear();
					return true;
				}
			}
			TraverseParms traverseParams = TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false);
			RegionEntryPredicate entryCondition = (Region from, Region r) => r.Allows(traverseParams, false);
			int adjacentRegionsAvailable = rootReg.Neighbors.Count((Region region) => entryCondition(rootReg, region));
			int regionsProcessed = 0;
			WorkGiver_DoBill.processedThings.AddRange(WorkGiver_DoBill.relevantThings);
			RegionProcessor regionProcessor = delegate(Region r)
			{
				List<Thing> list = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
				for (int i = 0; i < list.Count; i++)
				{
					Thing thing = list[i];
					if (!WorkGiver_DoBill.processedThings.Contains(thing))
					{
						if (ReachabilityWithinRegion.ThingFromRegionListerReachable(thing, r, PathEndMode.ClosestTouch, pawn))
						{
							if (baseValidator(thing) && (!thing.def.IsMedicine || !billGiverIsPawn))
							{
								WorkGiver_DoBill.newRelevantThings.Add(thing);
								WorkGiver_DoBill.processedThings.Add(thing);
							}
						}
					}
				}
				regionsProcessed++;
				if (WorkGiver_DoBill.newRelevantThings.Count > 0 && regionsProcessed > adjacentRegionsAvailable)
				{
					Comparison<Thing> comparison = delegate(Thing t1, Thing t2)
					{
						float num = (float)(t1.Position - rootCell).LengthHorizontalSquared;
						float value = (float)(t2.Position - rootCell).LengthHorizontalSquared;
						return num.CompareTo(value);
					};
					WorkGiver_DoBill.newRelevantThings.Sort(comparison);
					WorkGiver_DoBill.relevantThings.AddRange(WorkGiver_DoBill.newRelevantThings);
					WorkGiver_DoBill.newRelevantThings.Clear();
					if (WorkGiver_DoBill.TryFindBestBillIngredientsInSet(WorkGiver_DoBill.relevantThings, bill, chosen))
					{
						foundAll = true;
						return true;
					}
				}
				return false;
			};
			RegionTraverser.BreadthFirstTraverse(rootReg, entryCondition, regionProcessor, 99999, RegionType.Set_Passable);
			WorkGiver_DoBill.relevantThings.Clear();
			WorkGiver_DoBill.newRelevantThings.Clear();
			WorkGiver_DoBill.processedThings.Clear();
			WorkGiver_DoBill.ingredientsOrdered.Clear();
			return foundAll;
		}

		private static IntVec3 GetBillGiverRootCell(Thing billGiver, Pawn forPawn)
		{
			Building building = billGiver as Building;
			if (building == null)
			{
				return billGiver.Position;
			}
			if (building.def.hasInteractionCell)
			{
				return building.InteractionCell;
			}
			Log.Error("Tried to find bill ingredients for " + billGiver + " which has no interaction cell.", false);
			return forPawn.Position;
		}

		private static void AddEveryMedicineToRelevantThings(Pawn pawn, Thing billGiver, List<Thing> relevantThings, Predicate<Thing> baseValidator, Map map)
		{
			MedicalCareCategory medicalCareCategory = WorkGiver_DoBill.GetMedicalCareCategory(billGiver);
			List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine);
			WorkGiver_DoBill.tmpMedicine.Clear();
			for (int i = 0; i < list.Count; i++)
			{
				Thing thing = list[i];
				if (medicalCareCategory.AllowsMedicine(thing.def) && baseValidator(thing) && pawn.CanReach(thing, PathEndMode.OnCell, Danger.Deadly, false, TraverseMode.ByPawn))
				{
					WorkGiver_DoBill.tmpMedicine.Add(thing);
				}
			}
			WorkGiver_DoBill.tmpMedicine.SortBy((Thing x) => -x.GetStatValue(StatDefOf.MedicalPotency, true), (Thing x) => x.Position.DistanceToSquared(billGiver.Position));
			relevantThings.AddRange(WorkGiver_DoBill.tmpMedicine);
			WorkGiver_DoBill.tmpMedicine.Clear();
		}

		private static MedicalCareCategory GetMedicalCareCategory(Thing billGiver)
		{
			Pawn pawn = billGiver as Pawn;
			if (pawn != null && pawn.playerSettings != null)
			{
				return pawn.playerSettings.medCare;
			}
			return MedicalCareCategory.Best;
		}

		private static void MakeIngredientsListInProcessingOrder(List<IngredientCount> ingredientsOrdered, Bill bill)
		{
			ingredientsOrdered.Clear();
			if (bill.recipe.productHasIngredientStuff)
			{
				ingredientsOrdered.Add(bill.recipe.ingredients[0]);
			}
			for (int i = 0; i < bill.recipe.ingredients.Count; i++)
			{
				if (!bill.recipe.productHasIngredientStuff || i != 0)
				{
					IngredientCount ingredientCount = bill.recipe.ingredients[i];
					if (ingredientCount.IsFixedIngredient)
					{
						ingredientsOrdered.Add(ingredientCount);
					}
				}
			}
			for (int j = 0; j < bill.recipe.ingredients.Count; j++)
			{
				IngredientCount item = bill.recipe.ingredients[j];
				if (!ingredientsOrdered.Contains(item))
				{
					ingredientsOrdered.Add(item);
				}
			}
		}

		private static bool TryFindBestBillIngredientsInSet(List<Thing> availableThings, Bill bill, List<ThingCount> chosen)
		{
			if (bill.recipe.allowMixingIngredients)
			{
				return WorkGiver_DoBill.TryFindBestBillIngredientsInSet_AllowMix(availableThings, bill, chosen);
			}
			return WorkGiver_DoBill.TryFindBestBillIngredientsInSet_NoMix(availableThings, bill, chosen);
		}

		private static bool TryFindBestBillIngredientsInSet_NoMix(List<Thing> availableThings, Bill bill, List<ThingCount> chosen)
		{
			RecipeDef recipe = bill.recipe;
			chosen.Clear();
			WorkGiver_DoBill.availableCounts.Clear();
			WorkGiver_DoBill.availableCounts.GenerateFrom(availableThings);
			for (int i = 0; i < WorkGiver_DoBill.ingredientsOrdered.Count; i++)
			{
				IngredientCount ingredientCount = recipe.ingredients[i];
				bool flag = false;
				for (int j = 0; j < WorkGiver_DoBill.availableCounts.Count; j++)
				{
					float num = (float)ingredientCount.CountRequiredOfFor(WorkGiver_DoBill.availableCounts.GetDef(j), bill.recipe);
					if (num <= WorkGiver_DoBill.availableCounts.GetCount(j))
					{
						if (ingredientCount.filter.Allows(WorkGiver_DoBill.availableCounts.GetDef(j)))
						{
							if (ingredientCount.IsFixedIngredient || bill.ingredientFilter.Allows(WorkGiver_DoBill.availableCounts.GetDef(j)))
							{
								for (int k = 0; k < availableThings.Count; k++)
								{
									if (availableThings[k].def == WorkGiver_DoBill.availableCounts.GetDef(j))
									{
										int num2 = availableThings[k].stackCount - ThingCountUtility.CountOf(chosen, availableThings[k]);
										if (num2 > 0)
										{
											int num3 = Mathf.Min(Mathf.FloorToInt(num), num2);
											ThingCountUtility.AddToList(chosen, availableThings[k], num3);
											num -= (float)num3;
											if (num < 0.001f)
											{
												flag = true;
												float num4 = WorkGiver_DoBill.availableCounts.GetCount(j);
												num4 -= (float)ingredientCount.CountRequiredOfFor(WorkGiver_DoBill.availableCounts.GetDef(j), bill.recipe);
												WorkGiver_DoBill.availableCounts.SetCount(j, num4);
												break;
											}
										}
									}
								}
								if (flag)
								{
									break;
								}
							}
						}
					}
				}
				if (!flag)
				{
					return false;
				}
			}
			return true;
		}

		private static bool TryFindBestBillIngredientsInSet_AllowMix(List<Thing> availableThings, Bill bill, List<ThingCount> chosen)
		{
			chosen.Clear();
			for (int i = 0; i < bill.recipe.ingredients.Count; i++)
			{
				IngredientCount ingredientCount = bill.recipe.ingredients[i];
				float num = ingredientCount.GetBaseCount();
				for (int j = 0; j < availableThings.Count; j++)
				{
					Thing thing = availableThings[j];
					if (ingredientCount.filter.Allows(thing))
					{
						if (ingredientCount.IsFixedIngredient || bill.ingredientFilter.Allows(thing))
						{
							float num2 = bill.recipe.IngredientValueGetter.ValuePerUnitOf(thing.def);
							int num3 = Mathf.Min(Mathf.CeilToInt(num / num2), thing.stackCount);
							ThingCountUtility.AddToList(chosen, thing, num3);
							num -= (float)num3 * num2;
							if (num <= 0.0001f)
							{
								break;
							}
						}
					}
				}
				if (num > 0.0001f)
				{
					return false;
				}
			}
			return true;
		}

		// Note: this type is marked as 'beforefieldinit'.
		static WorkGiver_DoBill()
		{
		}

		[CompilerGenerated]
		private static float <AddEveryMedicineToRelevantThings>m__0(Thing x)
		{
			return -x.GetStatValue(StatDefOf.MedicalPotency, true);
		}

		private class DefCountList
		{
			private List<ThingDef> defs = new List<ThingDef>();

			private List<float> counts = new List<float>();

			public DefCountList()
			{
			}

			public int Count
			{
				get
				{
					return this.defs.Count;
				}
			}

			public float this[ThingDef def]
			{
				get
				{
					int num = this.defs.IndexOf(def);
					if (num < 0)
					{
						return 0f;
					}
					return this.counts[num];
				}
				set
				{
					int num = this.defs.IndexOf(def);
					if (num < 0)
					{
						this.defs.Add(def);
						this.counts.Add(value);
						num = this.defs.Count - 1;
					}
					else
					{
						this.counts[num] = value;
					}
					this.CheckRemove(num);
				}
			}

			public float GetCount(int index)
			{
				return this.counts[index];
			}

			public void SetCount(int index, float val)
			{
				this.counts[index] = val;
				this.CheckRemove(index);
			}

			public ThingDef GetDef(int index)
			{
				return this.defs[index];
			}

			private void CheckRemove(int index)
			{
				if (this.counts[index] == 0f)
				{
					this.counts.RemoveAt(index);
					this.defs.RemoveAt(index);
				}
			}

			public void Clear()
			{
				this.defs.Clear();
				this.counts.Clear();
			}

			public void GenerateFrom(List<Thing> things)
			{
				this.Clear();
				for (int i = 0; i < things.Count; i++)
				{
					ThingDef def;
					this[def = things[i].def] = this[def] + (float)things[i].stackCount;
				}
			}
		}

		[CompilerGenerated]
		private sealed class <ClosestUnfinishedThingForBill>c__AnonStorey0
		{
			internal Pawn pawn;

			internal Bill_ProductionWithUft bill;

			public <ClosestUnfinishedThingForBill>c__AnonStorey0()
			{
			}

			internal bool <>m__0(Thing t)
			{
				return !t.IsForbidden(this.pawn) && ((UnfinishedThing)t).Recipe == this.bill.recipe && ((UnfinishedThing)t).Creator == this.pawn && ((UnfinishedThing)t).ingredients.TrueForAll((Thing x) => this.bill.IsFixedOrAllowedIngredient(x.def)) && this.pawn.CanReserve(t, 1, -1, null, false);
			}

			internal bool <>m__1(Thing x)
			{
				return this.bill.IsFixedOrAllowedIngredient(x.def);
			}
		}

		[CompilerGenerated]
		private sealed class <TryFindBestBillIngredients>c__AnonStorey1
		{
			internal Pawn pawn;

			internal Thing billGiver;

			internal Bill bill;

			internal TraverseParms traverseParams;

			internal RegionEntryPredicate entryCondition;

			internal Region rootReg;

			internal Predicate<Thing> baseValidator;

			internal bool billGiverIsPawn;

			internal int regionsProcessed;

			internal int adjacentRegionsAvailable;

			internal IntVec3 rootCell;

			internal List<ThingCount> chosen;

			internal bool foundAll;

			public <TryFindBestBillIngredients>c__AnonStorey1()
			{
			}

			internal bool <>m__0(Thing t)
			{
				return t.Spawned && !t.IsForbidden(this.pawn) && (float)(t.Position - this.billGiver.Position).LengthHorizontalSquared < this.bill.ingredientSearchRadius * this.bill.ingredientSearchRadius && this.bill.IsFixedOrAllowedIngredient(t) && this.bill.recipe.ingredients.Any((IngredientCount ingNeed) => ingNeed.filter.Allows(t)) && this.pawn.CanReserve(t, 1, -1, null, false);
			}

			internal bool <>m__1(Region from, Region r)
			{
				return r.Allows(this.traverseParams, false);
			}

			internal bool <>m__2(Region region)
			{
				return this.entryCondition(this.rootReg, region);
			}

			internal bool <>m__3(Region r)
			{
				List<Thing> list = r.ListerThings.ThingsMatching(ThingRequest.ForGroup(ThingRequestGroup.HaulableEver));
				for (int i = 0; i < list.Count; i++)
				{
					Thing thing = list[i];
					if (!WorkGiver_DoBill.processedThings.Contains(thing))
					{
						if (ReachabilityWithinRegion.ThingFromRegionListerReachable(thing, r, PathEndMode.ClosestTouch, this.pawn))
						{
							if (this.baseValidator(thing) && (!thing.def.IsMedicine || !this.billGiverIsPawn))
							{
								WorkGiver_DoBill.newRelevantThings.Add(thing);
								WorkGiver_DoBill.processedThings.Add(thing);
							}
						}
					}
				}
				this.regionsProcessed++;
				if (WorkGiver_DoBill.newRelevantThings.Count > 0 && this.regionsProcessed > this.adjacentRegionsAvailable)
				{
					Comparison<Thing> comparison = delegate(Thing t1, Thing t2)
					{
						float num = (float)(t1.Position - this.rootCell).LengthHorizontalSquared;
						float value = (float)(t2.Position - this.rootCell).LengthHorizontalSquared;
						return num.CompareTo(value);
					};
					WorkGiver_DoBill.newRelevantThings.Sort(comparison);
					WorkGiver_DoBill.relevantThings.AddRange(WorkGiver_DoBill.newRelevantThings);
					WorkGiver_DoBill.newRelevantThings.Clear();
					if (WorkGiver_DoBill.TryFindBestBillIngredientsInSet(WorkGiver_DoBill.relevantThings, this.bill, this.chosen))
					{
						this.foundAll = true;
						return true;
					}
				}
				return false;
			}

			internal int <>m__4(Thing t1, Thing t2)
			{
				float num = (float)(t1.Position - this.rootCell).LengthHorizontalSquared;
				float value = (float)(t2.Position - this.rootCell).LengthHorizontalSquared;
				return num.CompareTo(value);
			}

			private sealed class <TryFindBestBillIngredients>c__AnonStorey2
			{
				internal Thing t;

				internal WorkGiver_DoBill.<TryFindBestBillIngredients>c__AnonStorey1 <>f__ref$1;

				public <TryFindBestBillIngredients>c__AnonStorey2()
				{
				}

				internal bool <>m__0(IngredientCount ingNeed)
				{
					return ingNeed.filter.Allows(this.t);
				}
			}
		}

		[CompilerGenerated]
		private sealed class <AddEveryMedicineToRelevantThings>c__AnonStorey3
		{
			internal Thing billGiver;

			public <AddEveryMedicineToRelevantThings>c__AnonStorey3()
			{
			}

			internal int <>m__0(Thing x)
			{
				return x.Position.DistanceToSquared(this.billGiver.Position);
			}
		}
	}
}
