using RimWorld;
using UnityEngine;

namespace Verse
{
	internal class SectionLayer_Watergen : SectionLayer_Terrain
	{
		public SectionLayer_Watergen(Section section) : base(section)
		{
			base.relevantChangeTypes = MapMeshFlag.Terrain;
		}

		public override Material GetMaterialFor(TerrainDef terrain)
		{
			return terrain.waterDepthMaterial;
		}

		public override void DrawLayer()
		{
			if (this.Visible)
			{
				int count = base.subMeshes.Count;
				for (int num = 0; num < count; num++)
				{
					LayerSubMesh layerSubMesh = base.subMeshes[num];
					if (layerSubMesh.finalized && !layerSubMesh.disabled)
					{
						Graphics.DrawMesh(layerSubMesh.mesh, Vector3.zero, Quaternion.identity, layerSubMesh.material, SubcameraDefOf.WaterDepth.LayerId);
					}
				}
			}
		}
	}
}
