using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public class Alert_NeedDoctor : Alert
	{
		private IEnumerable<Pawn> Patients
		{
			get
			{
				List<Map> maps = Find.Maps;
				for (int i = 0; i < maps.Count; i++)
				{
					if (maps[i].IsPlayerHome)
					{
						bool healthyDoc = false;
						foreach (Pawn item in maps[i].mapPawns.FreeColonistsSpawned)
						{
							if (!item.Downed && item.workSettings != null && item.workSettings.WorkIsActive(WorkTypeDefOf.Doctor))
							{
								healthyDoc = true;
								break;
							}
						}
						if (!healthyDoc)
						{
							foreach (Pawn item2 in maps[i].mapPawns.FreeColonistsSpawned)
							{
								if (item2.Downed && (int)item2.needs.food.CurCategory < 0 && item2.InBed())
								{
									goto IL_01a9;
								}
								if (HealthAIUtility.ShouldBeTendedNow(item2))
									goto IL_01a9;
								continue;
								IL_01a9:
								yield return item2;
								/*Error: Unable to find new state assignment for yield return*/;
							}
						}
					}
				}
				yield break;
				IL_022c:
				/*Error near IL_022d: Unexpected return in MoveNext()*/;
			}
		}

		public Alert_NeedDoctor()
		{
			base.defaultLabel = "NeedDoctor".Translate();
			base.defaultPriority = AlertPriority.High;
		}

		public override string GetExplanation()
		{
			StringBuilder stringBuilder = new StringBuilder();
			foreach (Pawn patient in this.Patients)
			{
				stringBuilder.AppendLine("    " + patient.NameStringShort);
			}
			return string.Format("NeedDoctorDesc".Translate(), stringBuilder.ToString());
		}

		public override AlertReport GetReport()
		{
			AlertReport result;
			if (Find.AnyPlayerHomeMap == null)
			{
				result = false;
			}
			else
			{
				Pawn pawn = this.Patients.FirstOrDefault();
				result = ((pawn != null) ? AlertReport.CulpritIs((Thing)pawn) : false);
			}
			return result;
		}
	}
}
