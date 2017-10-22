using System;
using UnityEngine;

namespace Verse
{
	internal class SectionLayer_EdgeShadows : SectionLayer
	{
		private const float InDist = 0.45f;

		private const byte ShadowBrightness = 195;

		private static readonly Color32 Shadowed = new Color32((byte)195, (byte)195, (byte)195, (byte)255);

		private static readonly Color32 Lit = new Color32((byte)255, (byte)255, (byte)255, (byte)255);

		public override bool Visible
		{
			get
			{
				return DebugViewSettings.drawShadows;
			}
		}

		public SectionLayer_EdgeShadows(Section section) : base(section)
		{
			base.relevantChangeTypes = MapMeshFlag.Buildings;
		}

		public override void Regenerate()
		{
			Building[] innerArray = base.Map.edificeGrid.InnerArray;
			float y = Altitudes.AltitudeFor(AltitudeLayer.Shadows);
			CellRect cellRect = new CellRect(base.section.botLeft.x, base.section.botLeft.z, 17, 17);
			cellRect.ClipInsideMap(base.Map);
			LayerSubMesh sm = base.GetSubMesh(MatBases.EdgeShadow);
			sm.Clear(MeshParts.All);
			sm.verts.Capacity = cellRect.Area * 4;
			sm.colors.Capacity = cellRect.Area * 4;
			sm.tris.Capacity = cellRect.Area * 8;
			bool[] array = new bool[4];
			bool[] array2 = new bool[4];
			bool[] array3 = new bool[4];
			float num = 0f;
			float num2 = 0f;
			CellIndices cellIndices = base.Map.cellIndices;
			for (int i = cellRect.minX; i <= cellRect.maxX; i++)
			{
				for (int j = cellRect.minZ; j <= cellRect.maxZ; j++)
				{
					Thing thing = innerArray[cellIndices.CellToIndex(i, j)];
					if (thing != null && thing.def.castEdgeShadows)
					{
						sm.verts.Add(new Vector3((float)i, y, (float)j));
						sm.verts.Add(new Vector3((float)i, y, (float)(j + 1)));
						sm.verts.Add(new Vector3((float)(i + 1), y, (float)(j + 1)));
						sm.verts.Add(new Vector3((float)(i + 1), y, (float)j));
						sm.colors.Add(SectionLayer_EdgeShadows.Shadowed);
						sm.colors.Add(SectionLayer_EdgeShadows.Shadowed);
						sm.colors.Add(SectionLayer_EdgeShadows.Shadowed);
						sm.colors.Add(SectionLayer_EdgeShadows.Shadowed);
						int count = sm.verts.Count;
						sm.tris.Add(count - 4);
						sm.tris.Add(count - 3);
						sm.tris.Add(count - 2);
						sm.tris.Add(count - 4);
						sm.tris.Add(count - 2);
						sm.tris.Add(count - 1);
					}
					else
					{
						array[0] = false;
						array[1] = false;
						array[2] = false;
						array[3] = false;
						array2[0] = false;
						array2[1] = false;
						array2[2] = false;
						array2[3] = false;
						array3[0] = false;
						array3[1] = false;
						array3[2] = false;
						array3[3] = false;
						IntVec3 a = new IntVec3(i, 0, j);
						IntVec3[] cardinalDirectionsAround = GenAdj.CardinalDirectionsAround;
						for (int k = 0; k < 4; k++)
						{
							IntVec3 c = a + cardinalDirectionsAround[k];
							if (c.InBounds(base.Map))
							{
								thing = innerArray[cellIndices.CellToIndex(c)];
								if (thing != null && thing.def.castEdgeShadows)
								{
									array2[k] = true;
									array[(k + 3) % 4] = true;
									array[k] = true;
								}
							}
						}
						IntVec3[] diagonalDirectionsAround = GenAdj.DiagonalDirectionsAround;
						for (int l = 0; l < 4; l++)
						{
							if (!array[l])
							{
								IntVec3 c = a + diagonalDirectionsAround[l];
								if (c.InBounds(base.Map))
								{
									thing = innerArray[cellIndices.CellToIndex(c)];
									if (thing != null && thing.def.castEdgeShadows)
									{
										array[l] = true;
										array3[l] = true;
									}
								}
							}
						}
						Action<int> action = (Action<int>)delegate(int idx)
						{
							sm.tris.Add(sm.verts.Count - 2);
							sm.tris.Add(idx);
							sm.tris.Add(sm.verts.Count - 1);
							sm.tris.Add(sm.verts.Count - 1);
							sm.tris.Add(idx);
							sm.tris.Add(idx + 1);
						};
						Action action2 = (Action)delegate
						{
							sm.colors.Add(SectionLayer_EdgeShadows.Shadowed);
							sm.colors.Add(SectionLayer_EdgeShadows.Lit);
							sm.colors.Add(SectionLayer_EdgeShadows.Lit);
							sm.tris.Add(sm.verts.Count - 3);
							sm.tris.Add(sm.verts.Count - 2);
							sm.tris.Add(sm.verts.Count - 1);
						};
						int count2 = sm.verts.Count;
						if (array[0])
						{
							if (array2[0] || array2[1])
							{
								num = (num2 = 0f);
								if (array2[0])
								{
									num2 = 0.45f;
								}
								if (array2[1])
								{
									num = 0.45f;
								}
								sm.verts.Add(new Vector3((float)i, y, (float)j));
								sm.colors.Add(SectionLayer_EdgeShadows.Shadowed);
								sm.verts.Add(new Vector3((float)i + num, y, (float)j + num2));
								sm.colors.Add(SectionLayer_EdgeShadows.Lit);
								if (array[1] && !array3[1])
								{
									action(sm.verts.Count);
								}
							}
							else
							{
								sm.verts.Add(new Vector3((float)i, y, (float)j));
								sm.verts.Add(new Vector3((float)i, y, (float)((float)j + 0.44999998807907104)));
								sm.verts.Add(new Vector3((float)((float)i + 0.44999998807907104), y, (float)j));
								action2();
							}
						}
						if (array[1])
						{
							if (array2[1] || array2[2])
							{
								num = (num2 = 0f);
								if (array2[1])
								{
									num = 0.45f;
								}
								if (array2[2])
								{
									num2 = -0.45f;
								}
								sm.verts.Add(new Vector3((float)i, y, (float)(j + 1)));
								sm.colors.Add(SectionLayer_EdgeShadows.Shadowed);
								sm.verts.Add(new Vector3((float)i + num, y, (float)(j + 1) + num2));
								sm.colors.Add(SectionLayer_EdgeShadows.Lit);
								if (array[2] && !array3[2])
								{
									action(sm.verts.Count);
								}
							}
							else
							{
								sm.verts.Add(new Vector3((float)i, y, (float)(j + 1)));
								sm.verts.Add(new Vector3((float)((float)i + 0.44999998807907104), y, (float)(j + 1)));
								sm.verts.Add(new Vector3((float)i, y, (float)((float)(j + 1) - 0.44999998807907104)));
								action2();
							}
						}
						if (array[2])
						{
							if (array2[2] || array2[3])
							{
								num = (num2 = 0f);
								if (array2[2])
								{
									num2 = -0.45f;
								}
								if (array2[3])
								{
									num = -0.45f;
								}
								sm.verts.Add(new Vector3((float)(i + 1), y, (float)(j + 1)));
								sm.colors.Add(SectionLayer_EdgeShadows.Shadowed);
								sm.verts.Add(new Vector3((float)(i + 1) + num, y, (float)(j + 1) + num2));
								sm.colors.Add(SectionLayer_EdgeShadows.Lit);
								if (array[3] && !array3[3])
								{
									action(sm.verts.Count);
								}
							}
							else
							{
								sm.verts.Add(new Vector3((float)(i + 1), y, (float)(j + 1)));
								sm.verts.Add(new Vector3((float)(i + 1), y, (float)((float)(j + 1) - 0.44999998807907104)));
								sm.verts.Add(new Vector3((float)((float)(i + 1) - 0.44999998807907104), y, (float)(j + 1)));
								action2();
							}
						}
						if (array[3])
						{
							if (array2[3] || array2[0])
							{
								num = (num2 = 0f);
								if (array2[3])
								{
									num = -0.45f;
								}
								if (array2[0])
								{
									num2 = 0.45f;
								}
								sm.verts.Add(new Vector3((float)(i + 1), y, (float)j));
								sm.colors.Add(SectionLayer_EdgeShadows.Shadowed);
								sm.verts.Add(new Vector3((float)(i + 1) + num, y, (float)j + num2));
								sm.colors.Add(SectionLayer_EdgeShadows.Lit);
								if (array[0] && !array3[0])
								{
									action(count2);
								}
							}
							else
							{
								sm.verts.Add(new Vector3((float)(i + 1), y, (float)j));
								sm.verts.Add(new Vector3((float)((float)(i + 1) - 0.44999998807907104), y, (float)j));
								sm.verts.Add(new Vector3((float)(i + 1), y, (float)((float)j + 0.44999998807907104)));
								action2();
							}
						}
					}
				}
			}
			if (sm.verts.Count > 0)
			{
				sm.FinalizeMesh(MeshParts.Verts | MeshParts.Tris | MeshParts.Colors);
			}
		}
	}
}
