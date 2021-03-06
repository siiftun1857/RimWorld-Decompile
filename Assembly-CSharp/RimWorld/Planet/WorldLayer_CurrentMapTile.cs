﻿using System;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	public class WorldLayer_CurrentMapTile : WorldLayer_SingleTile
	{
		public WorldLayer_CurrentMapTile()
		{
		}

		protected override int Tile
		{
			get
			{
				if (Current.ProgramState != ProgramState.Playing)
				{
					return -1;
				}
				if (Find.CurrentMap == null)
				{
					return -1;
				}
				return Find.CurrentMap.Tile;
			}
		}

		protected override Material Material
		{
			get
			{
				return WorldMaterials.CurrentMapTile;
			}
		}
	}
}
