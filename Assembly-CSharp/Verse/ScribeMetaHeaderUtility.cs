﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml;
using RimWorld;

namespace Verse
{
	public class ScribeMetaHeaderUtility
	{
		private static ScribeMetaHeaderUtility.ScribeHeaderMode lastMode;

		public static string loadedGameVersion;

		public static List<string> loadedModIdsList;

		public static List<string> loadedModNamesList;

		public const string MetaNodeName = "meta";

		public const string GameVersionNodeName = "gameVersion";

		public const string ModIdsNodeName = "modIds";

		public const string ModNamesNodeName = "modNames";

		[CompilerGenerated]
		private static Func<ModContentPack, string> <>f__am$cache0;

		[CompilerGenerated]
		private static Func<ModContentPack, string> <>f__am$cache1;

		[CompilerGenerated]
		private static Func<ModContentPack, string> <>f__am$cache2;

		[CompilerGenerated]
		private static Func<string, string> <>f__am$cache3;

		public ScribeMetaHeaderUtility()
		{
		}

		public static void WriteMetaHeader()
		{
			if (Scribe.EnterNode("meta"))
			{
				try
				{
					string currentVersionStringWithRev = VersionControl.CurrentVersionStringWithRev;
					Scribe_Values.Look<string>(ref currentVersionStringWithRev, "gameVersion", null, false);
					List<string> list = (from mod in LoadedModManager.RunningMods
					select mod.Identifier).ToList<string>();
					Scribe_Collections.Look<string>(ref list, "modIds", LookMode.Undefined, new object[0]);
					List<string> list2 = (from mod in LoadedModManager.RunningMods
					select mod.Name).ToList<string>();
					Scribe_Collections.Look<string>(ref list2, "modNames", LookMode.Undefined, new object[0]);
				}
				finally
				{
					Scribe.ExitNode();
				}
			}
		}

		public static void LoadGameDataHeader(ScribeMetaHeaderUtility.ScribeHeaderMode mode, bool logVersionConflictWarning)
		{
			ScribeMetaHeaderUtility.loadedGameVersion = "Unknown";
			ScribeMetaHeaderUtility.loadedModIdsList = null;
			ScribeMetaHeaderUtility.loadedModNamesList = null;
			ScribeMetaHeaderUtility.lastMode = mode;
			if (Scribe.mode != LoadSaveMode.Inactive && Scribe.EnterNode("meta"))
			{
				try
				{
					Scribe_Values.Look<string>(ref ScribeMetaHeaderUtility.loadedGameVersion, "gameVersion", null, false);
					Scribe_Collections.Look<string>(ref ScribeMetaHeaderUtility.loadedModIdsList, "modIds", LookMode.Undefined, new object[0]);
					Scribe_Collections.Look<string>(ref ScribeMetaHeaderUtility.loadedModNamesList, "modNames", LookMode.Undefined, new object[0]);
				}
				finally
				{
					Scribe.ExitNode();
				}
			}
			if (logVersionConflictWarning && (mode == ScribeMetaHeaderUtility.ScribeHeaderMode.Map || !UnityData.isEditor) && !ScribeMetaHeaderUtility.VersionsMatch())
			{
				Log.Warning(string.Concat(new object[]
				{
					"Loaded file (",
					mode,
					") is from version ",
					ScribeMetaHeaderUtility.loadedGameVersion,
					", we are running version ",
					VersionControl.CurrentVersionStringWithRev,
					"."
				}), false);
			}
		}

		private static bool VersionsMatch()
		{
			return VersionControl.BuildFromVersionString(ScribeMetaHeaderUtility.loadedGameVersion) == VersionControl.BuildFromVersionString(VersionControl.CurrentVersionStringWithRev);
		}

		public static bool TryCreateDialogsForVersionMismatchWarnings(Action confirmedAction)
		{
			string text = null;
			string text2 = null;
			if (!BackCompatibility.IsSaveCompatibleWith(ScribeMetaHeaderUtility.loadedGameVersion) && !ScribeMetaHeaderUtility.VersionsMatch())
			{
				text2 = "VersionMismatch".Translate();
				string text3 = (!ScribeMetaHeaderUtility.loadedGameVersion.NullOrEmpty()) ? ScribeMetaHeaderUtility.loadedGameVersion : ("(" + "UnknownLower".Translate() + ")");
				if (ScribeMetaHeaderUtility.lastMode == ScribeMetaHeaderUtility.ScribeHeaderMode.Map)
				{
					text = "SaveGameIncompatibleWarningText".Translate(new object[]
					{
						text3,
						VersionControl.CurrentVersionString
					});
				}
				else if (ScribeMetaHeaderUtility.lastMode == ScribeMetaHeaderUtility.ScribeHeaderMode.World)
				{
					text = "WorldFileVersionMismatch".Translate(new object[]
					{
						text3,
						VersionControl.CurrentVersionString
					});
				}
				else
				{
					text = "FileIncompatibleWarning".Translate(new object[]
					{
						text3,
						VersionControl.CurrentVersionString
					});
				}
			}
			bool flag = false;
			string text4;
			string text5;
			if (!ScribeMetaHeaderUtility.LoadedModsMatchesActiveMods(out text4, out text5))
			{
				flag = true;
				string text6 = "ModsMismatchWarningText".Translate(new object[]
				{
					text4,
					text5
				});
				if (text == null)
				{
					text = text6;
				}
				else
				{
					text = text + "\n\n" + text6;
				}
				if (text2 == null)
				{
					text2 = "ModsMismatchWarningTitle".Translate();
				}
			}
			if (text != null)
			{
				ScribeMetaHeaderUtility.<TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey0 <TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey = new ScribeMetaHeaderUtility.<TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey0();
				ScribeMetaHeaderUtility.<TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey0 <TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey2 = <TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey;
				string text7 = text;
				string title = text2;
				<TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey2.dialog = Dialog_MessageBox.CreateConfirmation(text7, confirmedAction, false, title);
				<TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey.dialog.buttonAText = "LoadAnyway".Translate();
				if (flag)
				{
					<TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey.dialog.buttonCText = "ChangeLoadedMods".Translate();
					<TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey.dialog.buttonCAction = delegate()
					{
						int num = ModLister.InstalledModsListHash(false);
						ModsConfig.SetActiveToList(ScribeMetaHeaderUtility.loadedModIdsList);
						ModsConfig.Save();
						if (num == ModLister.InstalledModsListHash(false))
						{
							IEnumerable<string> items = from id in Enumerable.Range(0, ScribeMetaHeaderUtility.loadedModIdsList.Count)
							where ModLister.GetModWithIdentifier(ScribeMetaHeaderUtility.loadedModIdsList[id]) == null
							select ScribeMetaHeaderUtility.loadedModNamesList[id];
							Messages.Message(string.Format("{0}: {1}", "MissingMods".Translate(), items.ToCommaList(false)), MessageTypeDefOf.RejectInput, false);
							<TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey.dialog.buttonCClose = false;
						}
						else
						{
							ModsConfig.RestartFromChangedMods();
						}
					};
				}
				Find.WindowStack.Add(<TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey.dialog);
				return true;
			}
			return false;
		}

		public static bool LoadedModsMatchesActiveMods(out string loadedModsSummary, out string runningModsSummary)
		{
			loadedModsSummary = null;
			runningModsSummary = null;
			List<string> list = (from mod in LoadedModManager.RunningMods
			select mod.Identifier).ToList<string>();
			if (ScribeMetaHeaderUtility.ModListsMatch(ScribeMetaHeaderUtility.loadedModIdsList, list))
			{
				return true;
			}
			if (ScribeMetaHeaderUtility.loadedModNamesList == null)
			{
				loadedModsSummary = "None".Translate();
			}
			else
			{
				loadedModsSummary = ScribeMetaHeaderUtility.loadedModNamesList.ToCommaList(false);
			}
			runningModsSummary = (from id in list
			select ModLister.GetModWithIdentifier(id).Name).ToCommaList(false);
			return false;
		}

		private static bool ModListsMatch(List<string> a, List<string> b)
		{
			if (a == null || b == null)
			{
				return false;
			}
			if (a.Count != b.Count)
			{
				return false;
			}
			for (int i = 0; i < a.Count; i++)
			{
				if (a[i] != b[i])
				{
					return false;
				}
			}
			return true;
		}

		public static string GameVersionOf(FileInfo file)
		{
			if (!file.Exists)
			{
				throw new ArgumentException();
			}
			try
			{
				using (StreamReader streamReader = new StreamReader(file.FullName))
				{
					using (XmlTextReader xmlTextReader = new XmlTextReader(streamReader))
					{
						if (ScribeMetaHeaderUtility.ReadToMetaElement(xmlTextReader) && xmlTextReader.ReadToDescendant("gameVersion"))
						{
							return VersionControl.VersionStringWithoutRev(xmlTextReader.ReadString());
						}
					}
				}
			}
			catch (Exception ex)
			{
				Log.Error("Exception getting game version of " + file.Name + ": " + ex.ToString(), false);
			}
			return null;
		}

		public static bool ReadToMetaElement(XmlTextReader textReader)
		{
			return ScribeMetaHeaderUtility.ReadToNextElement(textReader) && ScribeMetaHeaderUtility.ReadToNextElement(textReader) && !(textReader.Name != "meta");
		}

		private static bool ReadToNextElement(XmlTextReader textReader)
		{
			while (textReader.Read())
			{
				if (textReader.NodeType == XmlNodeType.Element)
				{
					return true;
				}
			}
			return false;
		}

		[CompilerGenerated]
		private static string <WriteMetaHeader>m__0(ModContentPack mod)
		{
			return mod.Identifier;
		}

		[CompilerGenerated]
		private static string <WriteMetaHeader>m__1(ModContentPack mod)
		{
			return mod.Name;
		}

		[CompilerGenerated]
		private static string <LoadedModsMatchesActiveMods>m__2(ModContentPack mod)
		{
			return mod.Identifier;
		}

		[CompilerGenerated]
		private static string <LoadedModsMatchesActiveMods>m__3(string id)
		{
			return ModLister.GetModWithIdentifier(id).Name;
		}

		public enum ScribeHeaderMode
		{
			None,
			Map,
			World,
			Scenario
		}

		[CompilerGenerated]
		private sealed class <TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey0
		{
			internal Dialog_MessageBox dialog;

			private static Func<int, bool> <>f__am$cache0;

			private static Func<int, string> <>f__am$cache1;

			public <TryCreateDialogsForVersionMismatchWarnings>c__AnonStorey0()
			{
			}

			internal void <>m__0()
			{
				int num = ModLister.InstalledModsListHash(false);
				ModsConfig.SetActiveToList(ScribeMetaHeaderUtility.loadedModIdsList);
				ModsConfig.Save();
				if (num == ModLister.InstalledModsListHash(false))
				{
					IEnumerable<string> items = from id in Enumerable.Range(0, ScribeMetaHeaderUtility.loadedModIdsList.Count)
					where ModLister.GetModWithIdentifier(ScribeMetaHeaderUtility.loadedModIdsList[id]) == null
					select ScribeMetaHeaderUtility.loadedModNamesList[id];
					Messages.Message(string.Format("{0}: {1}", "MissingMods".Translate(), items.ToCommaList(false)), MessageTypeDefOf.RejectInput, false);
					this.dialog.buttonCClose = false;
				}
				else
				{
					ModsConfig.RestartFromChangedMods();
				}
			}

			private static bool <>m__1(int id)
			{
				return ModLister.GetModWithIdentifier(ScribeMetaHeaderUtility.loadedModIdsList[id]) == null;
			}

			private static string <>m__2(int id)
			{
				return ScribeMetaHeaderUtility.loadedModNamesList[id];
			}
		}
	}
}
