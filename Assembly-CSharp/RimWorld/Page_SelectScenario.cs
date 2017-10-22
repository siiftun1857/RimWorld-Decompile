using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;
using Verse.Steam;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	public class Page_SelectScenario : Page
	{
		private Scenario curScen = null;

		private Vector2 infoScrollPosition = Vector2.zero;

		private const float ScenarioEntryHeight = 62f;

		private static readonly Texture2D CanUploadIcon = ContentFinder<Texture2D>.Get("UI/Icons/ContentSources/CanUpload", true);

		private Vector2 scenariosScrollPosition = Vector2.zero;

		private float totalScenarioListHeight;

		public override string PageTitle
		{
			get
			{
				return "ChooseScenario".Translate();
			}
		}

		public override void PreOpen()
		{
			base.PreOpen();
			this.infoScrollPosition = Vector2.zero;
			ScenarioLister.MarkDirty();
			this.EnsureValidSelection();
		}

		public override void DoWindowContents(Rect rect)
		{
			base.DrawPageTitle(rect);
			Rect mainRect = base.GetMainRect(rect, 0f, false);
			GUI.BeginGroup(mainRect);
			Rect rect2 = new Rect(0f, 0f, (float)(mainRect.width * 0.34999999403953552), mainRect.height).Rounded();
			this.DoScenarioSelectionList(rect2);
			Rect rect3 = new Rect((float)(rect2.xMax + 17.0), 0f, (float)(mainRect.width - rect2.width - 17.0), mainRect.height).Rounded();
			ScenarioUI.DrawScenarioInfo(rect3, this.curScen, ref this.infoScrollPosition);
			GUI.EndGroup();
			string midLabel = "ScenarioEditor".Translate();
			base.DoBottomButtons(rect, (string)null, midLabel, new Action(this.GoToScenarioEditor), true);
		}

		private bool CanEditScenario(Scenario scen)
		{
			return (byte)((scen.Category == ScenarioCategory.CustomLocal) ? 1 : (scen.CanToUploadToWorkshop() ? 1 : 0)) != 0;
		}

		private void GoToScenarioEditor()
		{
			Scenario scen = (!this.CanEditScenario(this.curScen)) ? this.curScen.CopyForEditing() : this.curScen;
			Page_ScenarioEditor page_ScenarioEditor = new Page_ScenarioEditor(scen);
			page_ScenarioEditor.prev = this;
			Find.WindowStack.Add(page_ScenarioEditor);
			this.Close(true);
		}

		private void DoScenarioSelectionList(Rect rect)
		{
			rect.xMax += 2f;
			Rect rect2 = new Rect(0f, 0f, (float)(rect.width - 16.0 - 2.0), (float)(this.totalScenarioListHeight + 250.0));
			Widgets.BeginScrollView(rect, ref this.scenariosScrollPosition, rect2, true);
			Rect rect3 = rect2.AtZero();
			rect3.height = 999999f;
			Listing_Standard listing_Standard = new Listing_Standard();
			listing_Standard.ColumnWidth = rect2.width;
			listing_Standard.Begin(rect3);
			Text.Font = GameFont.Small;
			this.ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.FromDef));
			listing_Standard.Gap(12f);
			Text.Font = GameFont.Small;
			listing_Standard.Label("ScenariosCustom".Translate(), -1f);
			this.ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.CustomLocal));
			listing_Standard.Gap(12f);
			Text.Font = GameFont.Small;
			listing_Standard.Label("ScenariosSteamWorkshop".Translate(), -1f);
			if (listing_Standard.ButtonText("OpenSteamWorkshop".Translate(), (string)null))
			{
				SteamUtility.OpenSteamWorkshopPage();
			}
			this.ListScenariosOnListing(listing_Standard, ScenarioLister.ScenariosInCategory(ScenarioCategory.SteamWorkshop));
			listing_Standard.End();
			this.totalScenarioListHeight = listing_Standard.CurHeight;
			Widgets.EndScrollView();
		}

		private void ListScenariosOnListing(Listing_Standard listing, IEnumerable<Scenario> scenarios)
		{
			bool flag = false;
			foreach (Scenario item in scenarios)
			{
				if (flag)
				{
					listing.Gap(12f);
				}
				Scenario scen = item;
				Rect rect = listing.GetRect(62f);
				this.DoScenarioListEntry(rect, scen);
				flag = true;
			}
			if (!flag)
			{
				GUI.color = new Color(1f, 1f, 1f, 0.5f);
				listing.Label("(" + "NoneLower".Translate() + ")", -1f);
				GUI.color = Color.white;
			}
		}

		private void DoScenarioListEntry(Rect rect, Scenario scen)
		{
			bool flag = this.curScen == scen;
			Widgets.DrawOptionBackground(rect, flag);
			MouseoverSounds.DoRegion(rect);
			Rect rect2 = rect.ContractedBy(4f);
			Text.Font = GameFont.Small;
			Rect rect3 = rect2;
			rect3.height = Text.CalcHeight(scen.name, rect3.width);
			Widgets.Label(rect3, scen.name);
			Text.Font = GameFont.Tiny;
			Rect rect4 = rect2;
			rect4.yMin = rect3.yMax;
			Widgets.Label(rect4, scen.GetSummary());
			if (scen.enabled)
			{
				WidgetRow widgetRow = new WidgetRow(rect.xMax, rect.y, UIDirection.LeftThenDown, 99999f, 4f);
				if (scen.Category == ScenarioCategory.CustomLocal && widgetRow.ButtonIcon(TexButton.DeleteX, "Delete".Translate()))
				{
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmDelete".Translate(scen.File.Name), (Action)delegate()
					{
						scen.File.Delete();
						ScenarioLister.MarkDirty();
					}, true, (string)null));
				}
				if (scen.Category == ScenarioCategory.SteamWorkshop && widgetRow.ButtonIcon(TexButton.DeleteX, "Unsubscribe".Translate()))
				{
					Find.WindowStack.Add(Dialog_MessageBox.CreateConfirmation("ConfirmUnsubscribe".Translate(scen.File.Name), (Action)delegate()
					{
						scen.enabled = false;
						if (this.curScen == scen)
						{
							this.curScen = null;
							this.EnsureValidSelection();
						}
						Workshop.Unsubscribe(scen);
					}, true, (string)null));
				}
				if (scen.GetPublishedFileId() != PublishedFileId_t.Invalid)
				{
					if (widgetRow.ButtonIcon(ContentSource.SteamWorkshop.GetIcon(), "WorkshopPage".Translate()))
					{
						SteamUtility.OpenWorkshopPage(scen.GetPublishedFileId());
					}
					if (scen.CanToUploadToWorkshop())
					{
						widgetRow.Icon(Page_SelectScenario.CanUploadIcon, "CanBeUpdatedOnWorkshop".Translate());
					}
				}
				if (!flag && Widgets.ButtonInvisible(rect, false))
				{
					this.curScen = scen;
					SoundDefOf.Click.PlayOneShotOnCamera(null);
				}
			}
		}

		protected override bool CanDoNext()
		{
			bool result;
			if (!base.CanDoNext())
			{
				result = false;
			}
			else if (this.curScen == null)
			{
				result = false;
			}
			else
			{
				Page_SelectScenario.BeginScenarioConfiguration(this.curScen, this);
				result = true;
			}
			return result;
		}

		public static void BeginScenarioConfiguration(Scenario scen, Page originPage)
		{
			Current.Game = new Game();
			Current.Game.InitData = new GameInitData();
			Current.Game.Scenario = scen;
			Current.Game.Scenario.PreConfigure();
			Page firstConfigPage = Current.Game.Scenario.GetFirstConfigPage();
			if (firstConfigPage == null)
			{
				PageUtility.InitGameStart();
			}
			else
			{
				originPage.next = firstConfigPage;
				firstConfigPage.prev = originPage;
			}
		}

		private void EnsureValidSelection()
		{
			if (this.curScen != null && ScenarioLister.ScenarioIsListedAnywhere(this.curScen))
				return;
			this.curScen = ScenarioLister.ScenariosInCategory(ScenarioCategory.FromDef).FirstOrDefault();
		}

		internal void Notify_ScenarioListChanged()
		{
			PublishedFileId_t selModId = this.curScen.GetPublishedFileId();
			this.curScen = ScenarioLister.AllScenarios().FirstOrDefault((Func<Scenario, bool>)((Scenario sc) => sc.GetPublishedFileId() == selModId));
			this.EnsureValidSelection();
		}

		internal void Notify_SteamItemUnsubscribed(PublishedFileId_t pfid)
		{
			if (this.curScen != null && this.curScen.GetPublishedFileId() == pfid)
			{
				this.curScen = null;
			}
			this.EnsureValidSelection();
		}
	}
}
