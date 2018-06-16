﻿using System;
using UnityEngine;

namespace Verse
{
	// Token: 0x02000DED RID: 3565
	public class MoteLeaf : Mote
	{
		// Token: 0x17000CF5 RID: 3317
		// (get) Token: 0x06004FC5 RID: 20421 RVA: 0x00295E04 File Offset: 0x00294204
		protected override bool EndOfLife
		{
			get
			{
				return base.AgeSecs >= this.spawnDelay + this.FallTime + this.def.mote.solidTime + this.def.mote.fadeOutTime;
			}
		}

		// Token: 0x17000CF6 RID: 3318
		// (get) Token: 0x06004FC6 RID: 20422 RVA: 0x00295E54 File Offset: 0x00294254
		private float FallTime
		{
			get
			{
				return this.startSpatialPosition.y / MoteLeaf.FallSpeed;
			}
		}

		// Token: 0x17000CF7 RID: 3319
		// (get) Token: 0x06004FC7 RID: 20423 RVA: 0x00295E7C File Offset: 0x0029427C
		public override float Alpha
		{
			get
			{
				float num = base.AgeSecs;
				float result;
				if (num <= this.spawnDelay)
				{
					result = 0f;
				}
				else
				{
					num -= this.spawnDelay;
					if (num <= this.def.mote.fadeInTime)
					{
						if (this.def.mote.fadeInTime > 0f)
						{
							result = num / this.def.mote.fadeInTime;
						}
						else
						{
							result = 1f;
						}
					}
					else if (num <= this.FallTime + this.def.mote.solidTime)
					{
						result = 1f;
					}
					else
					{
						num -= this.FallTime + this.def.mote.solidTime;
						if (num <= this.def.mote.fadeOutTime)
						{
							result = 1f - Mathf.InverseLerp(0f, this.def.mote.fadeOutTime, num);
						}
						else
						{
							num -= this.def.mote.fadeOutTime;
							result = 0f;
						}
					}
				}
				return result;
			}
		}

		// Token: 0x06004FC8 RID: 20424 RVA: 0x00295FA6 File Offset: 0x002943A6
		public void Initialize(Vector3 position, float spawnDelay, bool front, float treeHeight)
		{
			this.startSpatialPosition = position;
			this.spawnDelay = spawnDelay;
			this.front = front;
			this.treeHeight = treeHeight;
			this.TimeInterval(0f);
		}

		// Token: 0x06004FC9 RID: 20425 RVA: 0x00295FD4 File Offset: 0x002943D4
		protected override void TimeInterval(float deltaTime)
		{
			base.TimeInterval(deltaTime);
			if (!base.Destroyed)
			{
				float ageSecs = base.AgeSecs;
				this.exactPosition = this.startSpatialPosition;
				if (ageSecs > this.spawnDelay)
				{
					this.exactPosition.y = this.exactPosition.y - MoteLeaf.FallSpeed * (ageSecs - this.spawnDelay);
				}
				this.exactPosition.y = Mathf.Max(this.exactPosition.y, 0f);
				this.currentSpatialPosition = this.exactPosition;
				this.exactPosition.z = this.exactPosition.z + this.exactPosition.y;
				this.exactPosition.y = 0f;
			}
		}

		// Token: 0x06004FCA RID: 20426 RVA: 0x00296094 File Offset: 0x00294494
		public override void Draw()
		{
			base.Draw((!this.front) ? this.def.altitudeLayer.AltitudeFor() : (this.def.altitudeLayer.AltitudeFor() + 0.1f * GenMath.InverseLerp(0f, this.treeHeight, this.currentSpatialPosition.y) * 2f));
		}

		// Token: 0x040034DC RID: 13532
		private Vector3 startSpatialPosition;

		// Token: 0x040034DD RID: 13533
		private Vector3 currentSpatialPosition;

		// Token: 0x040034DE RID: 13534
		private float spawnDelay;

		// Token: 0x040034DF RID: 13535
		private bool front;

		// Token: 0x040034E0 RID: 13536
		private float treeHeight;

		// Token: 0x040034E1 RID: 13537
		[TweakValue("Graphics", 0f, 5f)]
		private static float FallSpeed = 0.5f;
	}
}