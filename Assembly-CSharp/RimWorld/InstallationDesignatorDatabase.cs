using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	public static class InstallationDesignatorDatabase
	{
		private static Dictionary<ThingDef, Designator_Install> designators = new Dictionary<ThingDef, Designator_Install>();

		public static Designator_Install DesignatorFor(ThingDef artDef)
		{
			Designator_Install designator_Install = default(Designator_Install);
			Designator_Install result;
			if (InstallationDesignatorDatabase.designators.TryGetValue(artDef, out designator_Install))
			{
				result = designator_Install;
			}
			else
			{
				designator_Install = InstallationDesignatorDatabase.NewDesignatorFor(artDef);
				InstallationDesignatorDatabase.designators.Add(artDef, designator_Install);
				result = designator_Install;
			}
			return result;
		}

		private static Designator_Install NewDesignatorFor(ThingDef artDef)
		{
			Designator_Install designator_Install = new Designator_Install();
			designator_Install.hotKey = KeyBindingDefOf.Misc1;
			return designator_Install;
		}
	}
}
