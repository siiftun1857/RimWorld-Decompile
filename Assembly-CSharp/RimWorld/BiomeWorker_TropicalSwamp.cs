﻿using System;
using RimWorld.Planet;

namespace RimWorld
{
	// Token: 0x02000554 RID: 1364
	public class BiomeWorker_TropicalSwamp : BiomeWorker
	{
		// Token: 0x06001960 RID: 6496 RVA: 0x000DBFA8 File Offset: 0x000DA3A8
		public override float GetScore(Tile tile, int tileID)
		{
			float result;
			if (tile.WaterCovered)
			{
				result = -100f;
			}
			else if (tile.temperature < 15f)
			{
				result = 0f;
			}
			else if (tile.rainfall < 2000f)
			{
				result = 0f;
			}
			else if (tile.swampiness < 0.5f)
			{
				result = 0f;
			}
			else
			{
				result = 28f + (tile.temperature - 20f) * 1.5f + (tile.rainfall - 600f) / 165f + tile.swampiness * 3f;
			}
			return result;
		}
	}
}