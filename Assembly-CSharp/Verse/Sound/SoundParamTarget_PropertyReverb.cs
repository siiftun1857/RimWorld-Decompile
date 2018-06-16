﻿using System;
using UnityEngine;

namespace Verse.Sound
{
	// Token: 0x02000B9D RID: 2973
	public class SoundParamTarget_PropertyReverb : SoundParamTarget
	{
		// Token: 0x170009D6 RID: 2518
		// (get) Token: 0x0600405A RID: 16474 RVA: 0x0021CB84 File Offset: 0x0021AF84
		public override string Label
		{
			get
			{
				return "ReverbFilter-interpolant";
			}
		}

		// Token: 0x170009D7 RID: 2519
		// (get) Token: 0x0600405B RID: 16475 RVA: 0x0021CBA0 File Offset: 0x0021AFA0
		public override Type NeededFilterType
		{
			get
			{
				return typeof(SoundFilterReverb);
			}
		}

		// Token: 0x0600405C RID: 16476 RVA: 0x0021CBC0 File Offset: 0x0021AFC0
		public override void SetOn(Sample sample, float value)
		{
			AudioReverbFilter audioReverbFilter = sample.source.GetComponent<AudioReverbFilter>();
			if (audioReverbFilter == null)
			{
				audioReverbFilter = sample.source.gameObject.AddComponent<AudioReverbFilter>();
			}
			ReverbSetup reverbSetup;
			if (value < 0.001f)
			{
				reverbSetup = this.baseSetup;
			}
			if (value > 0.999f)
			{
				reverbSetup = this.targetSetup;
			}
			else
			{
				reverbSetup = ReverbSetup.Lerp(this.baseSetup, this.targetSetup, value);
			}
			reverbSetup.ApplyTo(audioReverbFilter);
		}

		// Token: 0x04002B2E RID: 11054
		[Description("The base setup for the reverb.\n\nOnly used if no parameters are touching this filter.")]
		private ReverbSetup baseSetup = new ReverbSetup();

		// Token: 0x04002B2F RID: 11055
		[Description("The interpolation target setup for this filter.\n\nWhen the interpolant parameter is at 1, these settings will be active.")]
		private ReverbSetup targetSetup = new ReverbSetup();
	}
}