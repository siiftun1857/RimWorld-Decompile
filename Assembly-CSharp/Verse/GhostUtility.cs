﻿using System;
using System.Collections.Generic;
using RimWorld;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000E9C RID: 3740
	public static class GhostUtility
	{
		// Token: 0x06005840 RID: 22592 RVA: 0x002D30C4 File Offset: 0x002D14C4
		public static Graphic GhostGraphicFor(Graphic baseGraphic, ThingDef thingDef, Color ghostCol)
		{
			int num = 0;
			num = Gen.HashCombine<Graphic>(num, baseGraphic);
			num = Gen.HashCombine<ThingDef>(num, thingDef);
			num = Gen.HashCombineStruct<Color>(num, ghostCol);
			Graphic graphic;
			if (!GhostUtility.ghostGraphics.TryGetValue(num, out graphic))
			{
				if (thingDef.graphicData.Linked || thingDef.IsDoor)
				{
					graphic = GraphicDatabase.Get<Graphic_Single>(thingDef.uiIconPath, ShaderTypeDefOf.EdgeDetect.Shader, thingDef.graphicData.drawSize, ghostCol);
				}
				else
				{
					if (baseGraphic == null)
					{
						baseGraphic = thingDef.graphic;
					}
					GraphicData graphicData = null;
					if (baseGraphic.data != null)
					{
						graphicData = new GraphicData();
						graphicData.CopyFrom(baseGraphic.data);
						graphicData.shadowData = null;
					}
					graphic = GraphicDatabase.Get(baseGraphic.GetType(), baseGraphic.path, ShaderTypeDefOf.EdgeDetect.Shader, baseGraphic.drawSize, ghostCol, Color.white, graphicData, null);
				}
				GhostUtility.ghostGraphics.Add(num, graphic);
			}
			return graphic;
		}

		// Token: 0x04003A62 RID: 14946
		private static Dictionary<int, Graphic> ghostGraphics = new Dictionary<int, Graphic>();
	}
}