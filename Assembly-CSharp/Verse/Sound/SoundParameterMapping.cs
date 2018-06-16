﻿using System;

namespace Verse.Sound
{
	// Token: 0x02000B9F RID: 2975
	public class SoundParameterMapping
	{
		// Token: 0x0600405D RID: 16477 RVA: 0x0021CC3C File Offset: 0x0021B03C
		public SoundParameterMapping()
		{
			this.curve = new SimpleCurve();
			this.curve.Add(new CurvePoint(0f, 0f), true);
			this.curve.Add(new CurvePoint(1f, 1f), true);
		}

		// Token: 0x0600405E RID: 16478 RVA: 0x0021CCA8 File Offset: 0x0021B0A8
		public void DoEditWidgets(WidgetRow widgetRow)
		{
			string title = ((this.inParam == null) ? "null" : this.inParam.Label) + " -> " + ((this.outParam == null) ? "null" : this.outParam.Label);
			if (widgetRow.ButtonText("Edit curve", "Edit the curve mapping the in parameter to the out parameter.", true, false))
			{
				Find.WindowStack.Add(new EditWindow_CurveEditor(this.curve, title));
			}
		}

		// Token: 0x0600405F RID: 16479 RVA: 0x0021CD30 File Offset: 0x0021B130
		public void Apply(Sample samp)
		{
			if (this.inParam != null && this.outParam != null)
			{
				float num = this.inParam.ValueFor(samp);
				float value = this.curve.Evaluate(num);
				this.outParam.SetOn(samp, value);
				if (UnityData.isDebugBuild && this.curve.HasView)
				{
					this.curve.View.SetDebugInput(samp, num);
				}
			}
		}

		// Token: 0x04002B33 RID: 11059
		[Description("The independent parameter that the game will change to drive this relationship.\n\nOn the graph, this is the X axis.")]
		public SoundParamSource inParam = null;

		// Token: 0x04002B34 RID: 11060
		[Description("The dependent parameter that will respond to changes to the in-parameter.\n\nThis must match something the game can change about this sound.\n\nOn the graph, this is the y-axis.")]
		public SoundParamTarget outParam = null;

		// Token: 0x04002B35 RID: 11061
		[Description("Determines when sound parameters should be applies to samples.\n\nConstant means the parameters are updated every frame and can change continuously.\n\nOncePerSample means that the parameters are applied exactly once to each sample that plays.")]
		public SoundParamUpdateMode paramUpdateMode = SoundParamUpdateMode.Constant;

		// Token: 0x04002B36 RID: 11062
		[EditorHidden]
		public SimpleCurve curve;
	}
}