using System.Collections.Generic;
using Verse;

namespace RimWorld.Planet
{
	public static class FactionBaseNameGenerator
	{
		private static List<string> usedNames = new List<string>();

		public static string GenerateFactionBaseName(FactionBase factionBase)
		{
			string result;
			if (factionBase.Faction == null || factionBase.Faction.def.baseNameMaker == null)
			{
				result = factionBase.def.label;
			}
			else
			{
				FactionBaseNameGenerator.usedNames.Clear();
				List<FactionBase> factionBases = Find.WorldObjects.FactionBases;
				for (int i = 0; i < factionBases.Count; i++)
				{
					FactionBase factionBase2 = factionBases[i];
					if (factionBase2.Name != null)
					{
						FactionBaseNameGenerator.usedNames.Add(factionBase2.Name);
					}
				}
				result = NameGenerator.GenerateName(factionBase.Faction.def.baseNameMaker, FactionBaseNameGenerator.usedNames, true, (string)null);
			}
			return result;
		}
	}
}
