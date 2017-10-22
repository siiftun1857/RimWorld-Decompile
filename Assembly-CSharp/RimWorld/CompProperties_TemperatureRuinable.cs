using Verse;

namespace RimWorld
{
	public class CompProperties_TemperatureRuinable : CompProperties
	{
		public float minSafeTemperature = 0f;

		public float maxSafeTemperature = 100f;

		public float progressPerDegreePerTick = 1E-05f;

		public CompProperties_TemperatureRuinable()
		{
			base.compClass = typeof(CompTemperatureRuinable);
		}
	}
}
