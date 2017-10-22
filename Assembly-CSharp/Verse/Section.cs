#define ENABLE_PROFILER
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Profiling;

namespace Verse
{
	public class Section
	{
		public IntVec3 botLeft;

		public Map map;

		public MapMeshFlag dirtyFlags = MapMeshFlag.None;

		private List<SectionLayer> layers = new List<SectionLayer>();

		private bool foundRect = false;

		private CellRect calculatedRect;

		public const int Size = 17;

		public CellRect CellRect
		{
			get
			{
				if (!this.foundRect)
				{
					this.calculatedRect = new CellRect(this.botLeft.x, this.botLeft.z, 17, 17);
					this.calculatedRect.ClipInsideMap(this.map);
					this.foundRect = true;
				}
				return this.calculatedRect;
			}
		}

		public Section(IntVec3 sectCoords, Map map)
		{
			this.botLeft = sectCoords * 17;
			this.map = map;
			foreach (Type item in typeof(SectionLayer).AllSubclassesNonAbstract())
			{
				this.layers.Add((SectionLayer)Activator.CreateInstance(item, this));
			}
		}

		public void DrawSection(bool drawSunShadowsOnly)
		{
			int count = this.layers.Count;
			for (int num = 0; num < count; num++)
			{
				if (!drawSunShadowsOnly || this.layers[num] is SectionLayer_SunShadows)
				{
					this.layers[num].DrawLayer();
				}
			}
			if (!drawSunShadowsOnly && DebugViewSettings.drawSectionEdges)
			{
				GenDraw.DrawLineBetween(this.botLeft.ToVector3(), this.botLeft.ToVector3() + new Vector3(0f, 0f, 17f));
				GenDraw.DrawLineBetween(this.botLeft.ToVector3(), this.botLeft.ToVector3() + new Vector3(17f, 0f, 0f));
			}
		}

		public void RegenerateAllLayers()
		{
			for (int i = 0; i < this.layers.Count; i++)
			{
				if (this.layers[i].Visible)
				{
					this.layers[i].Regenerate();
				}
			}
		}

		public void RegenerateLayers(MapMeshFlag changeType)
		{
			for (int i = 0; i < this.layers.Count; i++)
			{
				SectionLayer sectionLayer = this.layers[i];
				if ((sectionLayer.relevantChangeTypes & changeType) != 0)
				{
					Profiler.BeginSample("Regen " + sectionLayer.GetType().Name + " " + this.botLeft);
					sectionLayer.Regenerate();
					Profiler.EndSample();
				}
			}
		}

		public SectionLayer GetLayer(Type type)
		{
			return (from sect in this.layers
			where sect.GetType() == type
			select sect).FirstOrDefault();
		}
	}
}
