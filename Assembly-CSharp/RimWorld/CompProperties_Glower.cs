using Verse;

namespace RimWorld
{
	public class CompProperties_Glower : CompProperties
	{
		public float overlightRadius = 0f;

		public float glowRadius = 14f;

		public ColorInt glowColor = new ColorInt(255, 255, 255, 0) * 1.45f;

		public CompProperties_Glower()
		{
			base.compClass = typeof(CompGlower);
		}
	}
}
