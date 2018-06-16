﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Verse.AI
{
	// Token: 0x02000A96 RID: 2710
	public class PawnPath : IDisposable
	{
		// Token: 0x1700091C RID: 2332
		// (get) Token: 0x06003C29 RID: 15401 RVA: 0x001FC870 File Offset: 0x001FAC70
		public bool Found
		{
			get
			{
				return this.totalCostInt >= 0f;
			}
		}

		// Token: 0x1700091D RID: 2333
		// (get) Token: 0x06003C2A RID: 15402 RVA: 0x001FC898 File Offset: 0x001FAC98
		public float TotalCost
		{
			get
			{
				return this.totalCostInt;
			}
		}

		// Token: 0x1700091E RID: 2334
		// (get) Token: 0x06003C2B RID: 15403 RVA: 0x001FC8B4 File Offset: 0x001FACB4
		public int NodesLeftCount
		{
			get
			{
				return this.curNodeIndex + 1;
			}
		}

		// Token: 0x1700091F RID: 2335
		// (get) Token: 0x06003C2C RID: 15404 RVA: 0x001FC8D4 File Offset: 0x001FACD4
		public List<IntVec3> NodesReversed
		{
			get
			{
				return this.nodes;
			}
		}

		// Token: 0x17000920 RID: 2336
		// (get) Token: 0x06003C2D RID: 15405 RVA: 0x001FC8F0 File Offset: 0x001FACF0
		public IntVec3 FirstNode
		{
			get
			{
				return this.nodes[this.nodes.Count - 1];
			}
		}

		// Token: 0x17000921 RID: 2337
		// (get) Token: 0x06003C2E RID: 15406 RVA: 0x001FC920 File Offset: 0x001FAD20
		public IntVec3 LastNode
		{
			get
			{
				return this.nodes[0];
			}
		}

		// Token: 0x17000922 RID: 2338
		// (get) Token: 0x06003C2F RID: 15407 RVA: 0x001FC944 File Offset: 0x001FAD44
		public static PawnPath NotFound
		{
			get
			{
				return PawnPathPool.NotFoundPath;
			}
		}

		// Token: 0x06003C30 RID: 15408 RVA: 0x001FC95E File Offset: 0x001FAD5E
		public void AddNode(IntVec3 nodePosition)
		{
			this.nodes.Add(nodePosition);
		}

		// Token: 0x06003C31 RID: 15409 RVA: 0x001FC970 File Offset: 0x001FAD70
		public void SetupFound(float totalCost)
		{
			if (this == PawnPath.NotFound)
			{
				Log.Warning("Calling SetupFound with totalCost=" + totalCost + " on PawnPath.NotFound", false);
			}
			else
			{
				this.totalCostInt = totalCost;
				this.curNodeIndex = this.nodes.Count - 1;
			}
		}

		// Token: 0x06003C32 RID: 15410 RVA: 0x001FC9C4 File Offset: 0x001FADC4
		public void Dispose()
		{
			this.ReleaseToPool();
		}

		// Token: 0x06003C33 RID: 15411 RVA: 0x001FC9CD File Offset: 0x001FADCD
		public void ReleaseToPool()
		{
			if (this != PawnPath.NotFound)
			{
				this.totalCostInt = 0f;
				this.nodes.Clear();
				this.inUse = false;
			}
		}

		// Token: 0x06003C34 RID: 15412 RVA: 0x001FC9FC File Offset: 0x001FADFC
		public static PawnPath NewNotFound()
		{
			return new PawnPath
			{
				totalCostInt = -1f
			};
		}

		// Token: 0x06003C35 RID: 15413 RVA: 0x001FCA24 File Offset: 0x001FAE24
		public IntVec3 ConsumeNextNode()
		{
			IntVec3 result = this.Peek(1);
			this.curNodeIndex--;
			return result;
		}

		// Token: 0x06003C36 RID: 15414 RVA: 0x001FCA50 File Offset: 0x001FAE50
		public IntVec3 Peek(int nodesAhead)
		{
			return this.nodes[this.curNodeIndex - nodesAhead];
		}

		// Token: 0x06003C37 RID: 15415 RVA: 0x001FCA78 File Offset: 0x001FAE78
		public override string ToString()
		{
			string result;
			if (!this.Found)
			{
				result = "PawnPath(not found)";
			}
			else if (!this.inUse)
			{
				result = "PawnPath(not in use)";
			}
			else
			{
				result = string.Concat(new object[]
				{
					"PawnPath(nodeCount= ",
					this.nodes.Count,
					(this.nodes.Count <= 0) ? "" : string.Concat(new object[]
					{
						" first=",
						this.FirstNode,
						" last=",
						this.LastNode
					}),
					" cost=",
					this.totalCostInt,
					" )"
				});
			}
			return result;
		}

		// Token: 0x06003C38 RID: 15416 RVA: 0x001FCB54 File Offset: 0x001FAF54
		public void DrawPath(Pawn pathingPawn)
		{
			if (this.Found)
			{
				float y = AltitudeLayer.Item.AltitudeFor();
				if (this.NodesLeftCount > 0)
				{
					for (int i = 0; i < this.NodesLeftCount - 1; i++)
					{
						Vector3 a = this.Peek(i).ToVector3Shifted();
						a.y = y;
						Vector3 b = this.Peek(i + 1).ToVector3Shifted();
						b.y = y;
						GenDraw.DrawLineBetween(a, b);
					}
					if (pathingPawn != null)
					{
						Vector3 drawPos = pathingPawn.DrawPos;
						drawPos.y = y;
						Vector3 b2 = this.Peek(0).ToVector3Shifted();
						b2.y = y;
						if ((drawPos - b2).sqrMagnitude > 0.01f)
						{
							GenDraw.DrawLineBetween(drawPos, b2);
						}
					}
				}
			}
		}

		// Token: 0x040025F9 RID: 9721
		private List<IntVec3> nodes = new List<IntVec3>(128);

		// Token: 0x040025FA RID: 9722
		private float totalCostInt = 0f;

		// Token: 0x040025FB RID: 9723
		private int curNodeIndex;

		// Token: 0x040025FC RID: 9724
		public bool inUse = false;
	}
}