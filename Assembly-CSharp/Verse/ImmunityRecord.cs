using RimWorld;
using UnityEngine;

namespace Verse
{
	public class ImmunityRecord : IExposable
	{
		public HediffDef hediffDef = null;

		public float immunity = 0f;

		public float ImmunityChangePerTick(Pawn pawn, bool sick, Hediff diseaseInstance)
		{
			float result;
			if (!pawn.RaceProps.IsFlesh)
			{
				result = 0f;
			}
			else
			{
				HediffCompProperties_Immunizable hediffCompProperties_Immunizable = this.hediffDef.CompProps<HediffCompProperties_Immunizable>();
				float num = (!sick) ? hediffCompProperties_Immunizable.immunityPerDayNotSick : hediffCompProperties_Immunizable.immunityPerDaySick;
				num = (float)(num / 60000.0);
				float num2 = pawn.GetStatValue(StatDefOf.ImmunityGainSpeed, true);
				if (diseaseInstance != null)
				{
					int value = Gen.HashCombineInt(diseaseInstance.GetHashCode(), 156482735);
					num2 *= Mathf.Lerp(0.8f, 1.2f, (float)((float)Mathf.Abs(value) / 2147483648.0));
				}
				result = ((!(num > 0.0)) ? (num / num2) : (num * num2));
			}
			return result;
		}

		public void ImmunityTick(Pawn pawn, bool sick, Hediff diseaseInstance)
		{
			this.immunity += this.ImmunityChangePerTick(pawn, sick, diseaseInstance);
			this.immunity = Mathf.Clamp01(this.immunity);
		}

		public void ExposeData()
		{
			Scribe_Defs.Look<HediffDef>(ref this.hediffDef, "hediffDef");
			Scribe_Values.Look<float>(ref this.immunity, "immunity", 0f, false);
		}
	}
}
