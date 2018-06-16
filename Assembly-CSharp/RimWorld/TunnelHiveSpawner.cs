﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x020006C2 RID: 1730
	[StaticConstructorOnStartup]
	public class TunnelHiveSpawner : ThingWithComps
	{
		// Token: 0x0600253E RID: 9534 RVA: 0x0013F5E8 File Offset: 0x0013D9E8
		public static void ResetStaticData()
		{
			TunnelHiveSpawner.filthTypes.Clear();
			TunnelHiveSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
			TunnelHiveSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
			TunnelHiveSpawner.filthTypes.Add(ThingDefOf.Filth_Dirt);
			TunnelHiveSpawner.filthTypes.Add(ThingDefOf.Filth_RubbleRock);
		}

		// Token: 0x0600253F RID: 9535 RVA: 0x0013F63C File Offset: 0x0013DA3C
		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look<int>(ref this.secondarySpawnTick, "secondarySpawnTick", 0, false);
			Scribe_Values.Look<bool>(ref this.spawnHive, "spawnHive", true, false);
			Scribe_Values.Look<float>(ref this.insectsPoints, "insectsPoints", 0f, false);
		}

		// Token: 0x06002540 RID: 9536 RVA: 0x0013F68C File Offset: 0x0013DA8C
		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				this.secondarySpawnTick = Find.TickManager.TicksGame + this.ResultSpawnDelay.RandomInRange.SecondsToTicks();
			}
			this.CreateSustainer();
		}

		// Token: 0x06002541 RID: 9537 RVA: 0x0013F6D4 File Offset: 0x0013DAD4
		public override void Tick()
		{
			if (base.Spawned)
			{
				this.sustainer.Maintain();
				Vector3 vector = base.Position.ToVector3Shifted();
				if (Rand.MTBEventOccurs(TunnelHiveSpawner.FilthSpawnMTB, 1f, 1.TicksToSeconds()))
				{
					IntVec3 c;
					if (CellFinder.TryFindRandomReachableCellNear(base.Position, base.Map, TunnelHiveSpawner.FilthSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false), null, null, out c, 999999))
					{
						FilthMaker.MakeFilth(c, base.Map, TunnelHiveSpawner.filthTypes.RandomElement<ThingDef>(), 1);
					}
				}
				if (Rand.MTBEventOccurs(TunnelHiveSpawner.DustMoteSpawnMTB, 1f, 1.TicksToSeconds()))
				{
					MoteMaker.ThrowDustPuffThick(new Vector3(vector.x, 0f, vector.z)
					{
						y = AltitudeLayer.MoteOverhead.AltitudeFor()
					}, base.Map, Rand.Range(1.5f, 3f), new Color(1f, 1f, 1f, 2.5f));
				}
				if (this.secondarySpawnTick <= Find.TickManager.TicksGame)
				{
					this.sustainer.End();
					Map map = base.Map;
					IntVec3 position = base.Position;
					this.Destroy(DestroyMode.Vanish);
					if (this.spawnHive)
					{
						Hive hive = (Hive)GenSpawn.Spawn(ThingMaker.MakeThing(ThingDefOf.Hive, null), position, map, WipeMode.Vanish);
						hive.SetFaction(Faction.OfInsects, null);
						foreach (CompSpawner compSpawner in hive.GetComps<CompSpawner>())
						{
							if (compSpawner.PropsSpawner.thingToSpawn == ThingDefOf.InsectJelly)
							{
								compSpawner.TryDoSpawn();
								break;
							}
						}
					}
					if (this.insectsPoints > 0f)
					{
						this.insectsPoints = Mathf.Max(this.insectsPoints, Hive.spawnablePawnKinds.Min((PawnKindDef x) => x.combatPower));
						float pointsLeft = this.insectsPoints;
						List<Pawn> list = new List<Pawn>();
						int num = 0;
						while (pointsLeft > 0f)
						{
							num++;
							if (num > 1000)
							{
								Log.Error("Too many iterations.", false);
								break;
							}
							IEnumerable<PawnKindDef> source = from x in Hive.spawnablePawnKinds
							where x.combatPower <= pointsLeft
							select x;
							PawnKindDef pawnKindDef;
							if (!source.TryRandomElement(out pawnKindDef))
							{
								break;
							}
							Pawn pawn = PawnGenerator.GeneratePawn(pawnKindDef, Faction.OfInsects);
							GenSpawn.Spawn(pawn, CellFinder.RandomClosewalkCellNear(position, map, 2, null), map, WipeMode.Vanish);
							list.Add(pawn);
							pointsLeft -= pawnKindDef.combatPower;
						}
						if (list.Any<Pawn>())
						{
							LordMaker.MakeNewLord(Faction.OfInsects, new LordJob_AssaultColony(Faction.OfInsects, true, false, false, false, true), map, list);
						}
					}
				}
			}
		}

		// Token: 0x06002542 RID: 9538 RVA: 0x0013FA00 File Offset: 0x0013DE00
		public override void Draw()
		{
			Rand.PushState();
			Rand.Seed = this.thingIDNumber;
			for (int i = 0; i < 6; i++)
			{
				this.DrawDustPart(Rand.Range(0f, 360f), Rand.Range(0.9f, 1.1f) * (float)Rand.Sign * 4f, Rand.Range(1f, 1.5f));
			}
			Rand.PopState();
		}

		// Token: 0x06002543 RID: 9539 RVA: 0x0013FA78 File Offset: 0x0013DE78
		private void DrawDustPart(float initialAngle, float speedMultiplier, float scale)
		{
			float num = (Find.TickManager.TicksGame - this.secondarySpawnTick).TicksToSeconds();
			Vector3 pos = base.Position.ToVector3ShiftedWithAltitude(AltitudeLayer.Filth);
			pos.y += 0.046875f * Rand.Range(0f, 1f);
			Color value = new Color(0.470588237f, 0.384313732f, 0.3254902f, 0.7f);
			TunnelHiveSpawner.matPropertyBlock.SetColor(ShaderPropertyIDs.Color, value);
			Matrix4x4 matrix = Matrix4x4.TRS(pos, Quaternion.Euler(0f, initialAngle + speedMultiplier * num, 0f), Vector3.one * scale);
			Graphics.DrawMesh(MeshPool.plane10, matrix, TunnelHiveSpawner.TunnelMaterial, 0, null, 0, TunnelHiveSpawner.matPropertyBlock);
		}

		// Token: 0x06002544 RID: 9540 RVA: 0x0013FB3B File Offset: 0x0013DF3B
		private void CreateSustainer()
		{
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				SoundDef tunnel = SoundDefOf.Tunnel;
				this.sustainer = tunnel.TrySpawnSustainer(SoundInfo.InMap(this, MaintenanceType.PerTick));
			});
		}

		// Token: 0x040014AF RID: 5295
		private int secondarySpawnTick;

		// Token: 0x040014B0 RID: 5296
		public bool spawnHive = true;

		// Token: 0x040014B1 RID: 5297
		public float insectsPoints;

		// Token: 0x040014B2 RID: 5298
		private Sustainer sustainer;

		// Token: 0x040014B3 RID: 5299
		private static MaterialPropertyBlock matPropertyBlock = new MaterialPropertyBlock();

		// Token: 0x040014B4 RID: 5300
		private readonly FloatRange ResultSpawnDelay = new FloatRange(12f, 16f);

		// Token: 0x040014B5 RID: 5301
		[TweakValue("Gameplay", 0f, 1f)]
		private static float DustMoteSpawnMTB = 0.2f;

		// Token: 0x040014B6 RID: 5302
		[TweakValue("Gameplay", 0f, 1f)]
		private static float FilthSpawnMTB = 0.3f;

		// Token: 0x040014B7 RID: 5303
		[TweakValue("Gameplay", 0f, 10f)]
		private static float FilthSpawnRadius = 3f;

		// Token: 0x040014B8 RID: 5304
		private static readonly Material TunnelMaterial = MaterialPool.MatFrom("Things/Filth/Grainy/GrainyA", ShaderDatabase.Transparent);

		// Token: 0x040014B9 RID: 5305
		private static List<ThingDef> filthTypes = new List<ThingDef>();
	}
}