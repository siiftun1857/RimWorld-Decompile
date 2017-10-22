using UnityEngine;

namespace Verse
{
	public class SectionLayer_FogOfWar : SectionLayer
	{
		private bool[] vertsCovered = new bool[9];

		private const byte FogBrightness = 35;

		public override bool Visible
		{
			get
			{
				return DebugViewSettings.drawFog;
			}
		}

		public SectionLayer_FogOfWar(Section section) : base(section)
		{
			base.relevantChangeTypes = MapMeshFlag.FogOfWar;
		}

		public override void Regenerate()
		{
			LayerSubMesh subMesh = base.GetSubMesh(MatBases.FogOfWar);
			if (subMesh.mesh.vertexCount == 0)
			{
				SectionLayerGeometryMaker_Solid.MakeBaseGeometry(base.section, subMesh, AltitudeLayer.FogOfWar);
			}
			subMesh.Clear(MeshParts.Colors);
			bool[] fogGrid = base.Map.fogGrid.fogGrid;
			CellRect cellRect = base.section.CellRect;
			IntVec3 size = base.Map.Size;
			int num = size.z - 1;
			IntVec3 size2 = base.Map.Size;
			int num2 = size2.x - 1;
			bool flag = false;
			CellIndices cellIndices = base.Map.cellIndices;
			for (int i = cellRect.minX; i <= cellRect.maxX; i++)
			{
				for (int j = cellRect.minZ; j <= cellRect.maxZ; j++)
				{
					if (fogGrid[cellIndices.CellToIndex(i, j)])
					{
						for (int k = 0; k < 9; k++)
						{
							this.vertsCovered[k] = true;
						}
					}
					else
					{
						for (int l = 0; l < 9; l++)
						{
							this.vertsCovered[l] = false;
						}
						if (j < num && fogGrid[cellIndices.CellToIndex(i, j + 1)])
						{
							this.vertsCovered[2] = true;
							this.vertsCovered[3] = true;
							this.vertsCovered[4] = true;
						}
						if (j > 0 && fogGrid[cellIndices.CellToIndex(i, j - 1)])
						{
							this.vertsCovered[6] = true;
							this.vertsCovered[7] = true;
							this.vertsCovered[0] = true;
						}
						if (i < num2 && fogGrid[cellIndices.CellToIndex(i + 1, j)])
						{
							this.vertsCovered[4] = true;
							this.vertsCovered[5] = true;
							this.vertsCovered[6] = true;
						}
						if (i > 0 && fogGrid[cellIndices.CellToIndex(i - 1, j)])
						{
							this.vertsCovered[0] = true;
							this.vertsCovered[1] = true;
							this.vertsCovered[2] = true;
						}
						if (j > 0 && i > 0 && fogGrid[cellIndices.CellToIndex(i - 1, j - 1)])
						{
							this.vertsCovered[0] = true;
						}
						if (j < num && i > 0 && fogGrid[cellIndices.CellToIndex(i - 1, j + 1)])
						{
							this.vertsCovered[2] = true;
						}
						if (j < num && i < num2 && fogGrid[cellIndices.CellToIndex(i + 1, j + 1)])
						{
							this.vertsCovered[4] = true;
						}
						if (j > 0 && i < num2 && fogGrid[cellIndices.CellToIndex(i + 1, j - 1)])
						{
							this.vertsCovered[6] = true;
						}
					}
					for (int m = 0; m < 9; m++)
					{
						byte a;
						if (this.vertsCovered[m])
						{
							a = (byte)255;
							flag = true;
						}
						else
						{
							a = (byte)0;
						}
						subMesh.colors.Add(new Color32((byte)255, (byte)255, (byte)255, a));
					}
				}
			}
			if (flag)
			{
				subMesh.disabled = false;
				subMesh.FinalizeMesh(MeshParts.Colors);
			}
			else
			{
				subMesh.disabled = true;
			}
		}
	}
}
