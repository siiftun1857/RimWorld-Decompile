using System.Collections.Generic;
using Verse;

namespace RimWorld
{
	[CaseInsensitiveXMLParsing]
	public class PawnBio
	{
		public GenderPossibility gender;

		public NameTriple name;

		public Backstory childhood;

		public Backstory adulthood;

		public bool pirateKing = false;

		public PawnBioType BioType
		{
			get
			{
				return (PawnBioType)((!this.pirateKing) ? ((this.adulthood != null) ? 1 : 0) : 2);
			}
		}

		public void PostLoad()
		{
			if (this.childhood != null)
			{
				this.childhood.PostLoad();
			}
			if (this.adulthood != null)
			{
				this.adulthood.PostLoad();
			}
		}

		public void ResolveReferences()
		{
			if (this.adulthood.spawnCategories.Count == 1 && this.adulthood.spawnCategories[0] == "Trader")
			{
				this.adulthood.spawnCategories.Add("Civil");
			}
			if (this.childhood != null)
			{
				this.childhood.ResolveReferences();
			}
			if (this.adulthood != null)
			{
				this.adulthood.ResolveReferences();
			}
		}

		public IEnumerable<string> ConfigErrors()
		{
			if (this.childhood != null)
			{
				using (IEnumerator<string> enumerator = this.childhood.ConfigErrors(true).GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						string error2 = enumerator.Current;
						yield return this.name + ", " + this.childhood.Title + ": " + error2;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			if (this.adulthood != null)
			{
				using (IEnumerator<string> enumerator2 = this.adulthood.ConfigErrors(false).GetEnumerator())
				{
					if (enumerator2.MoveNext())
					{
						string error = enumerator2.Current;
						yield return this.name + ", " + this.adulthood.Title + ": " + error;
						/*Error: Unable to find new state assignment for yield return*/;
					}
				}
			}
			yield break;
			IL_01ff:
			/*Error near IL_0200: Unexpected return in MoveNext()*/;
		}

		public override string ToString()
		{
			return "PawnBio(" + this.name + ")";
		}
	}
}
