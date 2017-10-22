using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse.AI
{
	public static class AttackTargetFinder
	{
		private static List<IAttackTarget> tmpTargets = new List<IAttackTarget>();

		private static List<Pair<IAttackTarget, float>> availableShootingTargets = new List<Pair<IAttackTarget, float>>();

		private static List<float> tmpTargetScores = new List<float>();

		private static List<bool> tmpCanShootAtTarget = new List<bool>();

		private static List<IntVec3> tempDestList = new List<IntVec3>();

		private static List<IntVec3> tempSourceList = new List<IntVec3>();

		public static IAttackTarget BestAttackTarget(IAttackTargetSearcher searcher, TargetScanFlags flags, Predicate<Thing> validator = null, float minDist = 0f, float maxDist = 9999f, IntVec3 locus = default(IntVec3), float maxTravelRadiusFromLocus = 3.40282347E+38f, bool canBash = false)
		{
			Thing searcherThing = searcher.Thing;
			Pawn searcherPawn = searcher as Pawn;
			Verb verb = searcher.CurrentEffectiveVerb;
			IAttackTarget result;
			if (verb == null)
			{
				Log.Error("BestAttackTarget with " + searcher + " who has no attack verb.");
				result = null;
			}
			else
			{
				bool onlyTargetMachines = verb != null && verb.IsEMP();
				float minDistanceSquared = minDist * minDist;
				float num = maxTravelRadiusFromLocus + verb.verbProps.range;
				float maxLocusDistSquared = num * num;
				Func<IntVec3, bool> losValidator = null;
				if ((byte)((int)flags & 64) != 0)
				{
					losValidator = (Func<IntVec3, bool>)delegate(IntVec3 vec3)
					{
						Gas gas = vec3.GetGas(searcherThing.Map);
						return gas == null || !gas.def.gas.blockTurretTracking;
					};
				}
				Predicate<IAttackTarget> innerValidator = (Predicate<IAttackTarget>)delegate(IAttackTarget t)
				{
					Thing thing = t.Thing;
					bool result2;
					if (t == searcher)
					{
						result2 = false;
					}
					else if (minDistanceSquared > 0.0 && (float)(searcherThing.Position - thing.Position).LengthHorizontalSquared < minDistanceSquared)
					{
						result2 = false;
					}
					else if (maxTravelRadiusFromLocus < 9999.0 && (float)(thing.Position - locus).LengthHorizontalSquared > maxLocusDistSquared)
					{
						result2 = false;
					}
					else if (!searcherThing.HostileTo(thing))
					{
						result2 = false;
					}
					else if ((object)validator != null && !validator(thing))
					{
						result2 = false;
					}
					else
					{
						if ((byte)((int)flags & 3) != 0 && !searcherThing.CanSee(thing, losValidator))
						{
							if (t is Pawn)
							{
								if ((byte)((int)flags & 1) != 0)
								{
									result2 = false;
									goto IL_02bb;
								}
							}
							else if ((byte)((int)flags & 2) != 0)
							{
								result2 = false;
								goto IL_02bb;
							}
						}
						if ((byte)((int)flags & 32) != 0 && t.ThreatDisabled())
						{
							result2 = false;
						}
						else
						{
							Pawn pawn2 = t as Pawn;
							if (onlyTargetMachines && pawn2 != null && pawn2.RaceProps.IsFlesh)
							{
								result2 = false;
							}
							else if ((byte)((int)flags & 16) != 0 && thing.IsBurning())
							{
								result2 = false;
							}
							else
							{
								if (searcherThing.def.race != null && (int)searcherThing.def.race.intelligence >= 2)
								{
									CompExplosive compExplosive = thing.TryGetComp<CompExplosive>();
									if (compExplosive != null && compExplosive.wickStarted)
									{
										result2 = false;
										goto IL_02bb;
									}
								}
								if (thing.def.size.x == 1 && thing.def.size.z == 1)
								{
									if (thing.Position.Fogged(thing.Map))
									{
										result2 = false;
										goto IL_02bb;
									}
								}
								else
								{
									bool flag2 = false;
									CellRect.CellRectIterator iterator = thing.OccupiedRect().GetIterator();
									while (!iterator.Done())
									{
										if (iterator.Current.Fogged(thing.Map))
										{
											iterator.MoveNext();
											continue;
										}
										flag2 = true;
										break;
									}
									if (!flag2)
									{
										result2 = false;
										goto IL_02bb;
									}
								}
								result2 = true;
							}
						}
					}
					goto IL_02bb;
					IL_02bb:
					return result2;
				};
				if (AttackTargetFinder.HasRangedAttack(searcher))
				{
					AttackTargetFinder.tmpTargets.Clear();
					AttackTargetFinder.tmpTargets.AddRange(searcherThing.Map.attackTargetsCache.GetPotentialTargetsFor(searcher));
					if ((byte)((int)flags & 4) != 0)
					{
						Predicate<IAttackTarget> oldValidator = innerValidator;
						_003CBestAttackTarget_003Ec__AnonStorey0 _003CBestAttackTarget_003Ec__AnonStorey;
						innerValidator = (Predicate<IAttackTarget>)((IAttackTarget t) => oldValidator(t) && AttackTargetFinder.CanReach(_003CBestAttackTarget_003Ec__AnonStorey.searcherThing, t.Thing, canBash));
					}
					bool flag = false;
					if (searcherThing.Faction != Faction.OfPlayer)
					{
						for (int i = 0; i < AttackTargetFinder.tmpTargets.Count; i++)
						{
							IAttackTarget attackTarget = AttackTargetFinder.tmpTargets[i];
							if (attackTarget.Thing.Position.InHorDistOf(searcherThing.Position, maxDist) && innerValidator(attackTarget) && AttackTargetFinder.CanShootAtFromCurrentPosition(attackTarget, searcher, verb))
							{
								flag = true;
								break;
							}
						}
					}
					IAttackTarget attackTarget2;
					if (flag)
					{
						AttackTargetFinder.tmpTargets.RemoveAll((Predicate<IAttackTarget>)((IAttackTarget x) => !x.Thing.Position.InHorDistOf(searcherThing.Position, maxDist) || !innerValidator(x)));
						attackTarget2 = AttackTargetFinder.GetRandomShootingTargetByScore(AttackTargetFinder.tmpTargets, searcher, verb);
					}
					else
					{
						Predicate<Thing> validator2 = ((((byte)((int)flags & 8) == 0) ? 1 : ((byte)((int)flags & 4))) != 0) ? ((Predicate<Thing>)((Thing t) => innerValidator((IAttackTarget)t))) : ((Predicate<Thing>)((Thing t) => innerValidator((IAttackTarget)t) && (AttackTargetFinder.CanReach(searcherThing, t, canBash) || AttackTargetFinder.CanShootAtFromCurrentPosition((IAttackTarget)t, searcher, verb))));
						attackTarget2 = (IAttackTarget)GenClosest.ClosestThing_Global(searcherThing.Position, AttackTargetFinder.tmpTargets, maxDist, validator2, null);
					}
					AttackTargetFinder.tmpTargets.Clear();
					result = attackTarget2;
				}
				else
				{
					if (searcherPawn != null && searcherPawn.mindState.duty != null && searcherPawn.mindState.duty.radius > 0.0 && !searcherPawn.InMentalState)
					{
						Predicate<IAttackTarget> oldValidator2 = innerValidator;
						_003CBestAttackTarget_003Ec__AnonStorey0 _003CBestAttackTarget_003Ec__AnonStorey2;
						innerValidator = (Predicate<IAttackTarget>)((IAttackTarget t) => (byte)(oldValidator2(t) ? (t.Thing.Position.InHorDistOf(_003CBestAttackTarget_003Ec__AnonStorey2.searcherPawn.mindState.duty.focus.Cell, _003CBestAttackTarget_003Ec__AnonStorey2.searcherPawn.mindState.duty.radius) ? 1 : 0) : 0) != 0);
					}
					IntVec3 root = searcherThing.Position;
					Map map = searcherThing.Map;
					ThingRequest thingReq = ThingRequest.ForGroup(ThingRequestGroup.AttackTarget);
					PathEndMode peMode = PathEndMode.Touch;
					Pawn pawn = searcherPawn;
					Danger maxDanger = Danger.Deadly;
					bool canBash2 = canBash;
					TraverseParms traverseParams = TraverseParms.For(pawn, maxDanger, TraverseMode.ByPawn, canBash2);
					float maxDistance = maxDist;
					Predicate<Thing> validator3 = (Predicate<Thing>)((Thing x) => innerValidator((IAttackTarget)x));
					int searchRegionsMax = (!(maxDist > 800.0)) ? 40 : (-1);
					IAttackTarget attackTarget3 = (IAttackTarget)GenClosest.ClosestThingReachable(root, map, thingReq, peMode, traverseParams, maxDistance, validator3, null, 0, searchRegionsMax, false, RegionType.Set_Passable, false);
					if (attackTarget3 != null && PawnUtility.ShouldCollideWithPawns(searcherPawn))
					{
						IAttackTarget attackTarget4 = AttackTargetFinder.FindBestReachableMeleeTarget(innerValidator, searcherPawn, maxDist, canBash);
						if (attackTarget4 != null)
						{
							root = searcherPawn.Position - attackTarget3.Thing.Position;
							float lengthHorizontal = root.LengthHorizontal;
							float lengthHorizontal2 = (searcherPawn.Position - attackTarget4.Thing.Position).LengthHorizontal;
							if (Mathf.Abs(lengthHorizontal - lengthHorizontal2) < 50.0)
							{
								attackTarget3 = attackTarget4;
							}
						}
					}
					result = attackTarget3;
				}
			}
			return result;
		}

		private static bool CanReach(Thing searcher, Thing target, bool canBash)
		{
			Pawn pawn = searcher as Pawn;
			bool result;
			if (pawn != null)
			{
				if (!pawn.CanReach(target, PathEndMode.Touch, Danger.Some, canBash, TraverseMode.ByPawn))
				{
					result = false;
					goto IL_0079;
				}
			}
			else
			{
				TraverseMode mode = (TraverseMode)(canBash ? 1 : 2);
				if (!searcher.Map.reachability.CanReach(searcher.Position, target, PathEndMode.Touch, TraverseParms.For(mode, Danger.Deadly, false)))
				{
					result = false;
					goto IL_0079;
				}
			}
			result = true;
			goto IL_0079;
			IL_0079:
			return result;
		}

		private static IAttackTarget FindBestReachableMeleeTarget(Predicate<IAttackTarget> validator, Pawn searcherPawn, float maxTargDist, bool canBash)
		{
			maxTargDist = Mathf.Min(maxTargDist, 30f);
			IAttackTarget reachableTarget = null;
			Func<IntVec3, IAttackTarget> bestTargetOnCell = (Func<IntVec3, IAttackTarget>)delegate(IntVec3 x)
			{
				List<Thing> thingList = x.GetThingList(searcherPawn.Map);
				int num = 0;
				IAttackTarget result2;
				while (true)
				{
					if (num < thingList.Count)
					{
						Thing thing = thingList[num];
						IAttackTarget attackTarget2 = thing as IAttackTarget;
						if (attackTarget2 != null && validator(attackTarget2) && ReachabilityImmediate.CanReachImmediate(x, thing, searcherPawn.Map, PathEndMode.Touch, searcherPawn) && (searcherPawn.CanReachImmediate(thing, PathEndMode.Touch) || searcherPawn.Map.attackTargetReservationManager.CanReserve(searcherPawn, attackTarget2)))
						{
							result2 = attackTarget2;
							break;
						}
						num++;
						continue;
					}
					result2 = null;
					break;
				}
				return result2;
			};
			searcherPawn.Map.floodFiller.FloodFill(searcherPawn.Position, (Predicate<IntVec3>)delegate(IntVec3 x)
			{
				bool result;
				if (!x.Walkable(searcherPawn.Map))
				{
					result = false;
				}
				else if ((float)x.DistanceToSquared(searcherPawn.Position) > maxTargDist * maxTargDist)
				{
					result = false;
				}
				else
				{
					if (!canBash)
					{
						Building_Door building_Door = x.GetEdifice(searcherPawn.Map) as Building_Door;
						if (building_Door != null && !building_Door.CanPhysicallyPass(searcherPawn))
						{
							result = false;
							goto IL_00ac;
						}
					}
					result = ((byte)((!PawnUtility.AnyPawnBlockingPathAt(x, searcherPawn, true, false)) ? 1 : 0) != 0);
				}
				goto IL_00ac;
				IL_00ac:
				return result;
			}, (Func<IntVec3, bool>)delegate(IntVec3 x)
			{
				for (int i = 0; i < 8; i++)
				{
					IntVec3 intVec = x + GenAdj.AdjacentCells[i];
					if (intVec.InBounds(searcherPawn.Map))
					{
						IAttackTarget attackTarget = bestTargetOnCell(intVec);
						if (attackTarget != null)
						{
							reachableTarget = attackTarget;
							break;
						}
					}
				}
				return reachableTarget != null;
			}, 2147483647, false, null);
			return reachableTarget;
		}

		private static bool HasRangedAttack(IAttackTargetSearcher t)
		{
			Verb currentEffectiveVerb = t.CurrentEffectiveVerb;
			return currentEffectiveVerb != null && !currentEffectiveVerb.verbProps.MeleeRange;
		}

		private static bool CanShootAtFromCurrentPosition(IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
		{
			return verb != null && verb.CanHitTargetFrom(searcher.Thing.Position, target.Thing);
		}

		private static IAttackTarget GetRandomShootingTargetByScore(List<IAttackTarget> targets, IAttackTargetSearcher searcher, Verb verb)
		{
			Pair<IAttackTarget, float> pair = default(Pair<IAttackTarget, float>);
			return (!((IEnumerable<Pair<IAttackTarget, float>>)AttackTargetFinder.GetAvailableShootingTargetsByScore(targets, searcher, verb)).TryRandomElementByWeight<Pair<IAttackTarget, float>>((Func<Pair<IAttackTarget, float>, float>)((Pair<IAttackTarget, float> x) => x.Second), out pair)) ? null : pair.First;
		}

		private static List<Pair<IAttackTarget, float>> GetAvailableShootingTargetsByScore(List<IAttackTarget> rawTargets, IAttackTargetSearcher searcher, Verb verb)
		{
			AttackTargetFinder.availableShootingTargets.Clear();
			List<Pair<IAttackTarget, float>> result;
			if (rawTargets.Count == 0)
			{
				result = AttackTargetFinder.availableShootingTargets;
			}
			else
			{
				AttackTargetFinder.tmpTargetScores.Clear();
				AttackTargetFinder.tmpCanShootAtTarget.Clear();
				float num = 0f;
				IAttackTarget attackTarget = null;
				for (int i = 0; i < rawTargets.Count; i++)
				{
					AttackTargetFinder.tmpTargetScores.Add(-3.40282347E+38f);
					AttackTargetFinder.tmpCanShootAtTarget.Add(false);
					if (rawTargets[i] != searcher)
					{
						bool flag = AttackTargetFinder.CanShootAtFromCurrentPosition(rawTargets[i], searcher, verb);
						AttackTargetFinder.tmpCanShootAtTarget[i] = flag;
						if (flag)
						{
							float shootingTargetScore = AttackTargetFinder.GetShootingTargetScore(rawTargets[i], searcher, verb);
							AttackTargetFinder.tmpTargetScores[i] = shootingTargetScore;
							if (attackTarget == null || shootingTargetScore > num)
							{
								attackTarget = rawTargets[i];
								num = shootingTargetScore;
							}
						}
					}
				}
				if (num < 1.0)
				{
					if (attackTarget != null)
					{
						AttackTargetFinder.availableShootingTargets.Add(new Pair<IAttackTarget, float>(attackTarget, 1f));
					}
				}
				else
				{
					float num2 = (float)(num - 30.0);
					for (int j = 0; j < rawTargets.Count; j++)
					{
						if (rawTargets[j] != searcher && AttackTargetFinder.tmpCanShootAtTarget[j])
						{
							float num3 = AttackTargetFinder.tmpTargetScores[j];
							if (num3 >= num2)
							{
								float second = Mathf.InverseLerp((float)(num - 30.0), num, num3);
								AttackTargetFinder.availableShootingTargets.Add(new Pair<IAttackTarget, float>(rawTargets[j], second));
							}
						}
					}
				}
				result = AttackTargetFinder.availableShootingTargets;
			}
			return result;
		}

		private static float GetShootingTargetScore(IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
		{
			float num = 60f;
			num -= Mathf.Min((target.Thing.Position - searcher.Thing.Position).LengthHorizontal, 40f);
			if (target.TargetCurrentlyAimingAt == searcher.Thing)
			{
				num = (float)(num + 10.0);
			}
			if (searcher.LastAttackedTarget == target.Thing && Find.TickManager.TicksGame - searcher.LastAttackTargetTick <= 300)
			{
				num = (float)(num + 40.0);
			}
			num = (float)(num - CoverUtility.CalculateOverallBlockChance(target.Thing.Position, searcher.Thing.Position, searcher.Thing.Map) * 10.0);
			Pawn pawn = target as Pawn;
			if (pawn != null && pawn.RaceProps.Animal && pawn.Faction != null && !pawn.IsFighting())
			{
				num = (float)(num - 50.0);
			}
			return num + AttackTargetFinder.FriendlyFireShootingTargetScoreOffset(target, searcher, verb);
		}

		private static float FriendlyFireShootingTargetScoreOffset(IAttackTarget target, IAttackTargetSearcher searcher, Verb verb)
		{
			float result;
			if (verb.verbProps.ai_AvoidFriendlyFireRadius <= 0.0)
			{
				result = 0f;
			}
			else
			{
				Map map = target.Thing.Map;
				IntVec3 position = target.Thing.Position;
				int num = GenRadial.NumCellsInRadius(verb.verbProps.ai_AvoidFriendlyFireRadius);
				float num2 = 0f;
				for (int num3 = 0; num3 < num; num3++)
				{
					IntVec3 intVec = position + GenRadial.RadialPattern[num3];
					if (intVec.InBounds(map))
					{
						bool flag = true;
						List<Thing> thingList = intVec.GetThingList(map);
						for (int i = 0; i < thingList.Count; i++)
						{
							if (thingList[i] is IAttackTarget && thingList[i] != target)
							{
								if (flag)
								{
									if (!GenSight.LineOfSight(position, intVec, map, true, null, 0, 0))
									{
										break;
									}
									flag = false;
								}
								float num4 = (float)((thingList[i] != searcher) ? ((!(thingList[i] is Pawn)) ? 10.0 : ((!thingList[i].def.race.Animal) ? 18.0 : 7.0)) : 40.0);
								num2 = (float)((!searcher.Thing.HostileTo(thingList[i])) ? (num2 - num4) : (num2 + num4 * 0.60000002384185791));
							}
						}
					}
				}
				result = Mathf.Min(num2, 0f);
			}
			return result;
		}

		public static IAttackTarget BestShootTargetFromCurrentPosition(IAttackTargetSearcher searcher, Predicate<Thing> validator, float maxDistance, float minDistance, TargetScanFlags flags)
		{
			return AttackTargetFinder.BestAttackTarget(searcher, flags, validator, minDistance, maxDistance, default(IntVec3), 3.40282347E+38f, false);
		}

		public static bool CanSee(this Thing seer, Thing target, Func<IntVec3, bool> validator = null)
		{
			ShootLeanUtility.CalcShootableCellsOf(AttackTargetFinder.tempDestList, target);
			int num = 0;
			bool result;
			while (true)
			{
				if (num < AttackTargetFinder.tempDestList.Count)
				{
					if (GenSight.LineOfSight(seer.Position, AttackTargetFinder.tempDestList[num], seer.Map, true, validator, 0, 0))
					{
						result = true;
						break;
					}
					num++;
					continue;
				}
				ShootLeanUtility.LeanShootingSourcesFromTo(seer.Position, target.Position, seer.Map, AttackTargetFinder.tempSourceList);
				for (int i = 0; i < AttackTargetFinder.tempSourceList.Count; i++)
				{
					for (int j = 0; j < AttackTargetFinder.tempDestList.Count; j++)
					{
						if (GenSight.LineOfSight(AttackTargetFinder.tempSourceList[i], AttackTargetFinder.tempDestList[j], seer.Map, true, validator, 0, 0))
							goto IL_00ab;
					}
				}
				result = false;
				break;
				IL_00ab:
				result = true;
				break;
			}
			return result;
		}

		public static void DebugDrawAttackTargetScores_Update()
		{
			IAttackTargetSearcher attackTargetSearcher = Find.Selector.SingleSelectedThing as IAttackTargetSearcher;
			if (attackTargetSearcher != null && attackTargetSearcher.Thing.Map == Find.VisibleMap)
			{
				Verb currentEffectiveVerb = attackTargetSearcher.CurrentEffectiveVerb;
				if (currentEffectiveVerb != null)
				{
					AttackTargetFinder.tmpTargets.Clear();
					List<Thing> list = attackTargetSearcher.Thing.Map.listerThings.ThingsInGroup(ThingRequestGroup.AttackTarget);
					for (int i = 0; i < list.Count; i++)
					{
						AttackTargetFinder.tmpTargets.Add((IAttackTarget)list[i]);
					}
					List<Pair<IAttackTarget, float>> availableShootingTargetsByScore = AttackTargetFinder.GetAvailableShootingTargetsByScore(AttackTargetFinder.tmpTargets, attackTargetSearcher, currentEffectiveVerb);
					for (int j = 0; j < availableShootingTargetsByScore.Count; j++)
					{
						GenDraw.DrawLineBetween(attackTargetSearcher.Thing.DrawPos, availableShootingTargetsByScore[j].First.Thing.DrawPos);
					}
				}
			}
		}

		public static void DebugDrawAttackTargetScores_OnGUI()
		{
			IAttackTargetSearcher attackTargetSearcher = Find.Selector.SingleSelectedThing as IAttackTargetSearcher;
			if (attackTargetSearcher != null && attackTargetSearcher.Thing.Map == Find.VisibleMap)
			{
				Verb currentEffectiveVerb = attackTargetSearcher.CurrentEffectiveVerb;
				if (currentEffectiveVerb != null)
				{
					List<Thing> list = attackTargetSearcher.Thing.Map.listerThings.ThingsInGroup(ThingRequestGroup.AttackTarget);
					Text.Anchor = TextAnchor.MiddleCenter;
					Text.Font = GameFont.Tiny;
					for (int i = 0; i < list.Count; i++)
					{
						Thing thing = list[i];
						if (thing != attackTargetSearcher)
						{
							string text;
							Color textColor;
							if (!AttackTargetFinder.CanShootAtFromCurrentPosition((IAttackTarget)thing, attackTargetSearcher, currentEffectiveVerb))
							{
								text = "out of range";
								textColor = Color.red;
							}
							else
							{
								text = AttackTargetFinder.GetShootingTargetScore((IAttackTarget)thing, attackTargetSearcher, currentEffectiveVerb).ToString("F0");
								textColor = new Color(0.25f, 1f, 0.25f);
							}
							Vector2 screenPos = thing.DrawPos.MapToUIPosition();
							GenMapUI.DrawThingLabel(screenPos, text, textColor);
						}
					}
					Text.Anchor = TextAnchor.UpperLeft;
					Text.Font = GameFont.Small;
				}
			}
		}
	}
}
