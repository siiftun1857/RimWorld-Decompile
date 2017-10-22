using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public sealed class History : IExposable
	{
		private List<HistoryAutoRecorderGroup> autoRecorderGroups;

		public SimpleCurveDrawerStyle curveDrawerStyle;

		public History()
		{
			this.autoRecorderGroups = new List<HistoryAutoRecorderGroup>();
			foreach (HistoryAutoRecorderGroupDef allDef in DefDatabase<HistoryAutoRecorderGroupDef>.AllDefs)
			{
				HistoryAutoRecorderGroup historyAutoRecorderGroup = new HistoryAutoRecorderGroup
				{
					def = allDef
				};
				historyAutoRecorderGroup.CreateRecorders();
				this.autoRecorderGroups.Add(historyAutoRecorderGroup);
			}
			this.curveDrawerStyle = new SimpleCurveDrawerStyle();
			this.curveDrawerStyle.DrawMeasures = true;
			this.curveDrawerStyle.DrawPoints = false;
			this.curveDrawerStyle.DrawBackground = true;
			this.curveDrawerStyle.DrawBackgroundLines = false;
			this.curveDrawerStyle.DrawLegend = true;
			this.curveDrawerStyle.DrawCurveMousePoint = true;
			this.curveDrawerStyle.OnlyPositiveValues = true;
			this.curveDrawerStyle.UseFixedSection = true;
			this.curveDrawerStyle.UseAntiAliasedLines = true;
			this.curveDrawerStyle.PointsRemoveOptimization = true;
			this.curveDrawerStyle.MeasureLabelsXCount = 10;
			this.curveDrawerStyle.MeasureLabelsYCount = 5;
			this.curveDrawerStyle.XIntegersOnly = true;
			this.curveDrawerStyle.LabelX = "Day".Translate();
		}

		public void HistoryTick()
		{
			for (int i = 0; i < this.autoRecorderGroups.Count; i++)
			{
				this.autoRecorderGroups[i].Tick();
			}
		}

		public List<HistoryAutoRecorderGroup> Groups()
		{
			return this.autoRecorderGroups;
		}

		public void ExposeData()
		{
			Scribe_Collections.Look<HistoryAutoRecorderGroup>(ref this.autoRecorderGroups, "autoRecorderGroups", LookMode.Deep, new object[0]);
		}
	}
}
