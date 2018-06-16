﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	// Token: 0x0200032F RID: 815
	public class IncidentWorker_HerdMigration : IncidentWorker
	{
		// Token: 0x06000DEC RID: 3564 RVA: 0x00076C04 File Offset: 0x00075004
		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			PawnKindDef pawnKindDef;
			IntVec3 intVec;
			IntVec3 intVec2;
			return this.TryFindAnimalKind(map.Tile, out pawnKindDef) && this.TryFindStartAndEndCells(map, out intVec, out intVec2);
		}

		// Token: 0x06000DED RID: 3565 RVA: 0x00076C48 File Offset: 0x00075048
		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			PawnKindDef pawnKindDef;
			bool result;
			IntVec3 intVec;
			IntVec3 near;
			if (!this.TryFindAnimalKind(map.Tile, out pawnKindDef))
			{
				result = false;
			}
			else if (!this.TryFindStartAndEndCells(map, out intVec, out near))
			{
				result = false;
			}
			else
			{
				Rot4 rot = Rot4.FromAngleFlat((map.Center - intVec).AngleFlat);
				List<Pawn> list = this.GenerateAnimals(pawnKindDef, map.Tile);
				for (int i = 0; i < list.Count; i++)
				{
					Pawn newThing = list[i];
					IntVec3 loc = CellFinder.RandomClosewalkCellNear(intVec, map, 10, null);
					GenSpawn.Spawn(newThing, loc, map, rot, WipeMode.Vanish, false);
				}
				LordMaker.MakeNewLord(null, new LordJob_ExitMapNear(near, LocomotionUrgency.Walk, 12f, false, false), map, list);
				string text = string.Format(this.def.letterText, pawnKindDef.GetLabelPlural(-1)).CapitalizeFirst();
				string label = string.Format(this.def.letterLabel, pawnKindDef.GetLabelPlural(-1).CapitalizeFirst());
				Find.LetterStack.ReceiveLetter(label, text, this.def.letterDef, list[0], null, null);
				result = true;
			}
			return result;
		}

		// Token: 0x06000DEE RID: 3566 RVA: 0x00076D88 File Offset: 0x00075188
		private bool TryFindAnimalKind(int tile, out PawnKindDef animalKind)
		{
			return (from k in DefDatabase<PawnKindDef>.AllDefs
			where k.RaceProps.CanDoHerdMigration && Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tile, k.race)
			select k).TryRandomElementByWeight((PawnKindDef x) => Mathf.Lerp(0.2f, 1f, x.RaceProps.wildness), out animalKind);
		}

		// Token: 0x06000DEF RID: 3567 RVA: 0x00076DE4 File Offset: 0x000751E4
		private bool TryFindStartAndEndCells(Map map, out IntVec3 start, out IntVec3 end)
		{
			bool result;
			if (!RCellFinder.TryFindRandomPawnEntryCell(out start, map, CellFinder.EdgeRoadChance_Animal, null))
			{
				end = IntVec3.Invalid;
				result = false;
			}
			else
			{
				end = IntVec3.Invalid;
				for (int i = 0; i < 8; i++)
				{
					IntVec3 startLocal = start;
					IntVec3 intVec;
					if (!CellFinder.TryFindRandomEdgeCellWith((IntVec3 x) => map.reachability.CanReach(startLocal, x, PathEndMode.OnCell, TraverseMode.NoPassClosedDoors, Danger.Deadly), map, CellFinder.EdgeRoadChance_Ignore, out intVec))
					{
						break;
					}
					if (!end.IsValid || intVec.DistanceToSquared(start) > end.DistanceToSquared(start))
					{
						end = intVec;
					}
				}
				result = end.IsValid;
			}
			return result;
		}

		// Token: 0x06000DF0 RID: 3568 RVA: 0x00076ED0 File Offset: 0x000752D0
		private List<Pawn> GenerateAnimals(PawnKindDef animalKind, int tile)
		{
			int num = IncidentWorker_HerdMigration.AnimalsCount.RandomInRange;
			num = Mathf.Max(num, Mathf.CeilToInt(4f / animalKind.RaceProps.baseBodySize));
			List<Pawn> list = new List<Pawn>();
			for (int i = 0; i < num; i++)
			{
				PawnGenerationRequest request = new PawnGenerationRequest(animalKind, null, PawnGenerationContext.NonPlayer, tile, false, false, false, false, true, false, 1f, false, true, true, false, false, false, false, null, null, null, null, null, null, null, null);
				Pawn item = PawnGenerator.GeneratePawn(request);
				list.Add(item);
			}
			return list;
		}

		// Token: 0x040008D0 RID: 2256
		private static readonly IntRange AnimalsCount = new IntRange(3, 5);

		// Token: 0x040008D1 RID: 2257
		private const float MinTotalBodySize = 4f;
	}
}