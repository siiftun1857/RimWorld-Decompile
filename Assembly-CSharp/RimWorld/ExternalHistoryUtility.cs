using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Verse;

namespace RimWorld
{
	public static class ExternalHistoryUtility
	{
		private static List<FileInfo> cachedFiles;

		private static int gameplayIDLength;

		private static string gameplayIDAvailableChars;

		public static IEnumerable<FileInfo> Files
		{
			get
			{
				int i = 0;
				if (i < ExternalHistoryUtility.cachedFiles.Count)
				{
					yield return ExternalHistoryUtility.cachedFiles[i];
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
		}

		static ExternalHistoryUtility()
		{
			ExternalHistoryUtility.gameplayIDLength = 20;
			ExternalHistoryUtility.gameplayIDAvailableChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";
			try
			{
				ExternalHistoryUtility.cachedFiles = GenFilePaths.AllExternalHistoryFiles.ToList();
			}
			catch (Exception ex)
			{
				Log.Error("Could not get external history files: " + ex.Message);
			}
		}

		public static ExternalHistory Load(string path)
		{
			ExternalHistory result = null;
			try
			{
				result = new ExternalHistory();
				Scribe.loader.InitLoading(path);
				try
				{
					Scribe_Deep.Look(ref result, "externalHistory");
					Scribe.loader.FinalizeLoading();
				}
				catch
				{
					Scribe.ForceStop();
					throw;
				}
			}
			catch (Exception ex)
			{
				Log.Error("Could not load external history (" + path + "): " + ex.Message);
				return null;
			}
			return result;
		}

		public static string GetRandomGameplayID()
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < ExternalHistoryUtility.gameplayIDLength; i++)
			{
				int index = Rand.Range(0, ExternalHistoryUtility.gameplayIDAvailableChars.Length);
				stringBuilder.Append(ExternalHistoryUtility.gameplayIDAvailableChars[index]);
			}
			return stringBuilder.ToString();
		}

		public static bool IsValidGameplayID(string ID)
		{
			bool result;
			if (ID.NullOrEmpty() || ID.Length != ExternalHistoryUtility.gameplayIDLength)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < ID.Length; i++)
				{
					bool flag = false;
					int num = 0;
					while (num < ExternalHistoryUtility.gameplayIDAvailableChars.Length)
					{
						if (ID[i] != ExternalHistoryUtility.gameplayIDAvailableChars[num])
						{
							num++;
							continue;
						}
						flag = true;
						break;
					}
					if (!flag)
						goto IL_006f;
				}
				result = true;
			}
			goto IL_008e;
			IL_008e:
			return result;
			IL_006f:
			result = false;
			goto IL_008e;
		}

		public static string GetCurrentUploadDate()
		{
			return DateTime.UtcNow.ToString("yyMMdd");
		}

		public static int GetCurrentUploadTime()
		{
			return (int)(DateTime.UtcNow.TimeOfDay.TotalSeconds / 2.0);
		}
	}
}
