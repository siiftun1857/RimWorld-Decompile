using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	[StaticConstructorOnStartup]
	public class WorldLayer
	{
		protected List<LayerSubMesh> subMeshes = new List<LayerSubMesh>();

		private bool dirty = true;

		private static MaterialPropertyBlock propertyBlock = new MaterialPropertyBlock();

		private const int MaxVerticesPerMesh = 40000;

		public virtual bool ShouldRegenerate
		{
			get
			{
				return this.dirty;
			}
		}

		protected virtual int Layer
		{
			get
			{
				return WorldCameraManager.WorldLayer;
			}
		}

		protected virtual Quaternion Rotation
		{
			get
			{
				return Quaternion.identity;
			}
		}

		protected virtual float Alpha
		{
			get
			{
				return 1f;
			}
		}

		public bool Dirty
		{
			get
			{
				return this.dirty;
			}
		}

		protected LayerSubMesh GetSubMesh(Material material)
		{
			int num = default(int);
			return this.GetSubMesh(material, out num);
		}

		protected LayerSubMesh GetSubMesh(Material material, out int subMeshIndex)
		{
			int num = 0;
			LayerSubMesh result;
			while (true)
			{
				if (num < this.subMeshes.Count)
				{
					LayerSubMesh layerSubMesh = this.subMeshes[num];
					if ((Object)layerSubMesh.material == (Object)material && layerSubMesh.verts.Count < 40000)
					{
						subMeshIndex = num;
						result = layerSubMesh;
						break;
					}
					num++;
					continue;
				}
				Mesh mesh = new Mesh();
				if (UnityData.isEditor)
				{
					mesh.name = "WorldLayerSubMesh_" + base.GetType().Name + "_" + Find.World.info.seedString;
				}
				LayerSubMesh layerSubMesh2 = new LayerSubMesh(mesh, material);
				subMeshIndex = this.subMeshes.Count;
				this.subMeshes.Add(layerSubMesh2);
				result = layerSubMesh2;
				break;
			}
			return result;
		}

		protected void FinalizeMesh(MeshParts tags)
		{
			for (int i = 0; i < this.subMeshes.Count; i++)
			{
				if (this.subMeshes[i].verts.Count > 0)
				{
					this.subMeshes[i].FinalizeMesh(tags);
				}
			}
		}

		public void RegenerateNow()
		{
			this.dirty = false;
			this.Regenerate().ExecuteEnumerable();
		}

		public void Render()
		{
			if (this.ShouldRegenerate)
			{
				this.RegenerateNow();
			}
			int layer = this.Layer;
			Quaternion rotation = this.Rotation;
			float alpha = this.Alpha;
			for (int i = 0; i < this.subMeshes.Count; i++)
			{
				if (this.subMeshes[i].finalized)
				{
					if (alpha != 1.0)
					{
						Color color = this.subMeshes[i].material.color;
						WorldLayer.propertyBlock.SetColor(ShaderPropertyIDs.Color, new Color(color.r, color.g, color.b, color.a * alpha));
						Graphics.DrawMesh(this.subMeshes[i].mesh, Vector3.zero, rotation, this.subMeshes[i].material, layer, null, 0, WorldLayer.propertyBlock);
					}
					else
					{
						Graphics.DrawMesh(this.subMeshes[i].mesh, Vector3.zero, rotation, this.subMeshes[i].material, layer);
					}
				}
			}
		}

		public virtual IEnumerable Regenerate()
		{
			this.dirty = false;
			this.ClearSubMeshes(MeshParts.All);
			yield break;
		}

		public void SetDirty()
		{
			this.dirty = true;
		}

		private void ClearSubMeshes(MeshParts parts)
		{
			for (int i = 0; i < this.subMeshes.Count; i++)
			{
				this.subMeshes[i].Clear(parts);
			}
		}
	}
}
