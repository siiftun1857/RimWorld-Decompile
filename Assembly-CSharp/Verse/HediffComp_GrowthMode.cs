using RimWorld;
using System;
using System.Linq;
using System.Text;

namespace Verse
{
	public class HediffComp_GrowthMode : HediffComp_SeverityPerDay
	{
		private const int CheckGrowthModeChangeInterval = 5000;

		private const float GrowthModeChangeMtbDays = 100f;

		public HediffGrowthMode growthMode = HediffGrowthMode.Growing;

		private float severityPerDayGrowingRandomFactor = 1f;

		private float severityPerDayRemissionRandomFactor = 1f;

		public HediffCompProperties_GrowthMode Props
		{
			get
			{
				return (HediffCompProperties_GrowthMode)base.props;
			}
		}

		public override string CompLabelInBracketsExtra
		{
			get
			{
				return this.growthMode.GetLabel();
			}
		}

		public override void CompExposeData()
		{
			base.CompExposeData();
			Scribe_Values.Look<HediffGrowthMode>(ref this.growthMode, "growthMode", HediffGrowthMode.Growing, false);
			Scribe_Values.Look<float>(ref this.severityPerDayGrowingRandomFactor, "severityPerDayGrowingRandomFactor", 1f, false);
			Scribe_Values.Look<float>(ref this.severityPerDayRemissionRandomFactor, "severityPerDayRemissionRandomFactor", 1f, false);
		}

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			base.CompPostPostAdd(dinfo);
			this.growthMode = ((HediffGrowthMode[])Enum.GetValues(typeof(HediffGrowthMode))).RandomElement();
			this.severityPerDayGrowingRandomFactor = this.Props.severityPerDayGrowingRandomFactor.RandomInRange;
			this.severityPerDayRemissionRandomFactor = this.Props.severityPerDayRemissionRandomFactor.RandomInRange;
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			base.CompPostTick(ref severityAdjustment);
			if (base.Pawn.IsHashIntervalTick(5000) && Rand.MTBEventOccurs(100f, 60000f, 5000f))
			{
				this.ChangeGrowthMode();
			}
		}

		protected override float SeverityChangePerDay()
		{
			float result;
			switch (this.growthMode)
			{
			case HediffGrowthMode.Growing:
			{
				result = this.Props.severityPerDayGrowing * this.severityPerDayGrowingRandomFactor;
				break;
			}
			case HediffGrowthMode.Stable:
			{
				result = 0f;
				break;
			}
			case HediffGrowthMode.Remission:
			{
				result = this.Props.severityPerDayRemission * this.severityPerDayRemissionRandomFactor;
				break;
			}
			default:
			{
				throw new NotImplementedException("GrowthMode");
			}
			}
			return result;
		}

		private void ChangeGrowthMode()
		{
			this.growthMode = (from x in (HediffGrowthMode[])Enum.GetValues(typeof(HediffGrowthMode))
			where x != this.growthMode
			select x).RandomElement();
			if (PawnUtility.ShouldSendNotificationAbout(base.Pawn))
			{
				switch (this.growthMode)
				{
				case HediffGrowthMode.Growing:
				{
					Messages.Message("DiseaseGrowthModeChanged_Growing".Translate(base.Pawn.LabelShort, base.Def.label), (Thing)base.Pawn, MessageTypeDefOf.NegativeHealthEvent);
					break;
				}
				case HediffGrowthMode.Stable:
				{
					Messages.Message("DiseaseGrowthModeChanged_Stable".Translate(base.Pawn.LabelShort, base.Def.label), (Thing)base.Pawn, MessageTypeDefOf.NeutralEvent);
					break;
				}
				case HediffGrowthMode.Remission:
				{
					Messages.Message("DiseaseGrowthModeChanged_Remission".Translate(base.Pawn.LabelShort, base.Def.label), (Thing)base.Pawn, MessageTypeDefOf.PositiveEvent);
					break;
				}
				}
			}
		}

		public override string CompDebugString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(base.CompDebugString());
			stringBuilder.AppendLine("severity: " + base.parent.Severity.ToString("F3") + ((!(base.parent.Severity >= base.Def.maxSeverity)) ? "" : " (reached max)"));
			stringBuilder.AppendLine("severityPerDayGrowingRandomFactor: " + this.severityPerDayGrowingRandomFactor.ToString("0.##"));
			stringBuilder.AppendLine("severityPerDayRemissionRandomFactor: " + this.severityPerDayRemissionRandomFactor.ToString("0.##"));
			return stringBuilder.ToString();
		}
	}
}
