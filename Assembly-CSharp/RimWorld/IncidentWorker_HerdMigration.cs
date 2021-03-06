﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	public class IncidentWorker_HerdMigration : IncidentWorker
	{
		private static readonly IntRange AnimalsCount = new IntRange(3, 5);

		private const float MinTotalBodySize = 4f;

		[CompilerGenerated]
		private static Func<PawnKindDef, float> <>f__am$cache0;

		public IncidentWorker_HerdMigration()
		{
		}

		protected override bool CanFireNowSub(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			PawnKindDef pawnKindDef;
			IntVec3 intVec;
			IntVec3 intVec2;
			return this.TryFindAnimalKind(map.Tile, out pawnKindDef) && this.TryFindStartAndEndCells(map, out intVec, out intVec2);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			Map map = (Map)parms.target;
			PawnKindDef pawnKindDef;
			if (!this.TryFindAnimalKind(map.Tile, out pawnKindDef))
			{
				return false;
			}
			IntVec3 intVec;
			IntVec3 near;
			if (!this.TryFindStartAndEndCells(map, out intVec, out near))
			{
				return false;
			}
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
			return true;
		}

		private bool TryFindAnimalKind(int tile, out PawnKindDef animalKind)
		{
			return (from k in DefDatabase<PawnKindDef>.AllDefs
			where k.RaceProps.CanDoHerdMigration && Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(tile, k.race)
			select k).TryRandomElementByWeight((PawnKindDef x) => Mathf.Lerp(0.2f, 1f, x.RaceProps.wildness), out animalKind);
		}

		private bool TryFindStartAndEndCells(Map map, out IntVec3 start, out IntVec3 end)
		{
			if (!RCellFinder.TryFindRandomPawnEntryCell(out start, map, CellFinder.EdgeRoadChance_Animal, null))
			{
				end = IntVec3.Invalid;
				return false;
			}
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
			return end.IsValid;
		}

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

		// Note: this type is marked as 'beforefieldinit'.
		static IncidentWorker_HerdMigration()
		{
		}

		[CompilerGenerated]
		private static float <TryFindAnimalKind>m__0(PawnKindDef x)
		{
			return Mathf.Lerp(0.2f, 1f, x.RaceProps.wildness);
		}

		[CompilerGenerated]
		private sealed class <TryFindAnimalKind>c__AnonStorey0
		{
			internal int tile;

			public <TryFindAnimalKind>c__AnonStorey0()
			{
			}

			internal bool <>m__0(PawnKindDef k)
			{
				return k.RaceProps.CanDoHerdMigration && Find.World.tileTemperatures.SeasonAndOutdoorTemperatureAcceptableFor(this.tile, k.race);
			}
		}

		[CompilerGenerated]
		private sealed class <TryFindStartAndEndCells>c__AnonStorey1
		{
			internal Map map;

			public <TryFindStartAndEndCells>c__AnonStorey1()
			{
			}
		}

		[CompilerGenerated]
		private sealed class <TryFindStartAndEndCells>c__AnonStorey2
		{
			internal IntVec3 startLocal;

			internal IncidentWorker_HerdMigration.<TryFindStartAndEndCells>c__AnonStorey1 <>f__ref$1;

			public <TryFindStartAndEndCells>c__AnonStorey2()
			{
			}

			internal bool <>m__0(IntVec3 x)
			{
				return this.<>f__ref$1.map.reachability.CanReach(this.startLocal, x, PathEndMode.OnCell, TraverseMode.NoPassClosedDoors, Danger.Deadly);
			}
		}
	}
}
