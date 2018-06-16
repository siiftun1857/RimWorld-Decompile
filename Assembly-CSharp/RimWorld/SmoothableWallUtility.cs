﻿using System;
using Verse;

namespace RimWorld
{
	// Token: 0x02000997 RID: 2455
	public static class SmoothableWallUtility
	{
		// Token: 0x06003720 RID: 14112 RVA: 0x001D7DF0 File Offset: 0x001D61F0
		public static void Notify_SmoothedByPawn(Thing t, Pawn p)
		{
			for (int i = 0; i < GenAdj.CardinalDirections.Length; i++)
			{
				IntVec3 c = t.Position + GenAdj.CardinalDirections[i];
				if (c.InBounds(t.Map))
				{
					Building edifice = c.GetEdifice(t.Map);
					if (edifice != null && edifice.def.IsSmoothable)
					{
						bool flag = true;
						int num = 0;
						for (int j = 0; j < GenAdj.CardinalDirections.Length; j++)
						{
							IntVec3 intVec = edifice.Position + GenAdj.CardinalDirections[j];
							if (!SmoothableWallUtility.IsBlocked(intVec, t.Map))
							{
								flag = false;
								break;
							}
							Building edifice2 = intVec.GetEdifice(t.Map);
							if (edifice2 != null && edifice2.def.IsSmoothed)
							{
								num++;
							}
						}
						if (flag && num >= 2)
						{
							for (int k = 0; k < GenAdj.DiagonalDirections.Length; k++)
							{
								IntVec3 pos = edifice.Position + GenAdj.DiagonalDirections[k];
								if (!SmoothableWallUtility.IsBlocked(pos, t.Map))
								{
									SmoothableWallUtility.SmoothWall(edifice, p);
									break;
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003721 RID: 14113 RVA: 0x001D7F6C File Offset: 0x001D636C
		public static void Notify_BuildingDestroying(Thing t, DestroyMode mode)
		{
			if (mode == DestroyMode.KillFinalize || mode == DestroyMode.Deconstruct)
			{
				if (t.def.IsSmoothed)
				{
					for (int i = 0; i < GenAdj.CardinalDirections.Length; i++)
					{
						IntVec3 c = t.Position + GenAdj.CardinalDirections[i];
						if (c.InBounds(t.Map))
						{
							Building edifice = c.GetEdifice(t.Map);
							if (edifice != null && edifice.def.IsSmoothed)
							{
								bool flag = true;
								for (int j = 0; j < GenAdj.CardinalDirections.Length; j++)
								{
									IntVec3 pos = edifice.Position + GenAdj.CardinalDirections[j];
									if (!SmoothableWallUtility.IsBlocked(pos, t.Map))
									{
										flag = false;
										break;
									}
								}
								if (flag)
								{
									edifice.Destroy(DestroyMode.WillReplace);
									GenSpawn.Spawn(ThingMaker.MakeThing(edifice.def.building.unsmoothedThing, edifice.Stuff), edifice.Position, t.Map, edifice.Rotation, WipeMode.Vanish, false);
								}
							}
						}
					}
				}
			}
		}

		// Token: 0x06003722 RID: 14114 RVA: 0x001D80B8 File Offset: 0x001D64B8
		public static Thing SmoothWall(Thing target, Pawn smoother)
		{
			Map map = target.Map;
			target.Destroy(DestroyMode.WillReplace);
			Thing thing = ThingMaker.MakeThing(target.def.building.smoothedThing, target.Stuff);
			thing.SetFaction(smoother.Faction, null);
			GenSpawn.Spawn(thing, target.Position, map, target.Rotation, WipeMode.Vanish, false);
			map.designationManager.TryRemoveDesignation(target.Position, DesignationDefOf.SmoothWall);
			return thing;
		}

		// Token: 0x06003723 RID: 14115 RVA: 0x001D8134 File Offset: 0x001D6534
		private static bool IsBlocked(IntVec3 pos, Map map)
		{
			bool result;
			if (!pos.InBounds(map))
			{
				result = false;
			}
			else if (pos.Walkable(map))
			{
				result = false;
			}
			else
			{
				Building edifice = pos.GetEdifice(map);
				result = (edifice != null && (edifice.def.IsSmoothed || edifice.def.building.isNaturalRock));
			}
			return result;
		}
	}
}