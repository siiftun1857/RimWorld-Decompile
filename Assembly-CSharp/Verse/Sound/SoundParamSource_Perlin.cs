﻿using System;
using UnityEngine;
using Verse.Noise;

namespace Verse.Sound
{
	// Token: 0x02000B90 RID: 2960
	public class SoundParamSource_Perlin : SoundParamSource
	{
		// Token: 0x170009C9 RID: 2505
		// (get) Token: 0x0600403A RID: 16442 RVA: 0x0021C634 File Offset: 0x0021AA34
		public override string Label
		{
			get
			{
				return "Perlin noise";
			}
		}

		// Token: 0x0600403B RID: 16443 RVA: 0x0021C650 File Offset: 0x0021AA50
		public override float ValueFor(Sample samp)
		{
			float num;
			if (this.syncType == PerlinMappingSyncType.Sync)
			{
				num = samp.ParentHashCode % 100f;
			}
			else
			{
				num = (float)(samp.GetHashCode() % 100);
			}
			if (this.timeType == TimeType.Ticks && Current.ProgramState == ProgramState.Playing)
			{
				float num2;
				if (this.syncType == PerlinMappingSyncType.Sync)
				{
					num2 = (float)Find.TickManager.TicksGame - samp.ParentStartTick;
				}
				else
				{
					num2 = (float)(Find.TickManager.TicksGame - samp.startTick);
				}
				num2 /= 60f;
				num += num2;
			}
			else
			{
				float num3;
				if (this.syncType == PerlinMappingSyncType.Sync)
				{
					num3 = Time.realtimeSinceStartup - samp.ParentStartRealTime;
				}
				else
				{
					num3 = Time.realtimeSinceStartup - samp.startRealTime;
				}
				num += num3;
			}
			num *= this.perlinFrequency;
			float num4 = (float)SoundParamSource_Perlin.perlin.GetValue((double)num, 0.0, 0.0);
			num4 *= 2f;
			num4 += 1f;
			return num4 / 2f;
		}

		// Token: 0x04002B17 RID: 11031
		[Description("The type of time on which this perlin randomizer will work. If you use Ticks, it will freeze when paused and speed up in fast forward.")]
		public TimeType timeType = TimeType.Ticks;

		// Token: 0x04002B18 RID: 11032
		[Description("The frequency of the perlin output. The input time is multiplied by this amount.")]
		public float perlinFrequency = 1f;

		// Token: 0x04002B19 RID: 11033
		[Description("Whether to synchronize the Perlin output across different samples. If set to desync, each playing sample will get a separate Perlin output.")]
		public PerlinMappingSyncType syncType = PerlinMappingSyncType.Sync;

		// Token: 0x04002B1A RID: 11034
		private static Perlin perlin = new Perlin(0.0099999997764825821, 2.0, 0.5, 4, Rand.Range(0, int.MaxValue), QualityMode.Medium);
	}
}