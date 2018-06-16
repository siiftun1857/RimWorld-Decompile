﻿using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace RimWorld.Planet
{
	// Token: 0x020008F2 RID: 2290
	public static class WorldSelectionDrawer
	{
		// Token: 0x17000884 RID: 2180
		// (get) Token: 0x060034CA RID: 13514 RVA: 0x001C3470 File Offset: 0x001C1870
		public static Dictionary<WorldObject, float> SelectTimes
		{
			get
			{
				return WorldSelectionDrawer.selectTimes;
			}
		}

		// Token: 0x060034CB RID: 13515 RVA: 0x001C348A File Offset: 0x001C188A
		public static void Notify_Selected(WorldObject t)
		{
			WorldSelectionDrawer.selectTimes[t] = Time.realtimeSinceStartup;
		}

		// Token: 0x060034CC RID: 13516 RVA: 0x001C349D File Offset: 0x001C189D
		public static void Clear()
		{
			WorldSelectionDrawer.selectTimes.Clear();
		}

		// Token: 0x060034CD RID: 13517 RVA: 0x001C34AC File Offset: 0x001C18AC
		public static void SelectionOverlaysOnGUI()
		{
			List<WorldObject> selectedObjects = Find.WorldSelector.SelectedObjects;
			for (int i = 0; i < selectedObjects.Count; i++)
			{
				WorldObject worldObject = selectedObjects[i];
				WorldSelectionDrawer.DrawSelectionBracketOnGUIFor(worldObject);
				worldObject.ExtraSelectionOverlaysOnGUI();
			}
		}

		// Token: 0x060034CE RID: 13518 RVA: 0x001C34F4 File Offset: 0x001C18F4
		public static void DrawSelectionOverlays()
		{
			List<WorldObject> selectedObjects = Find.WorldSelector.SelectedObjects;
			for (int i = 0; i < selectedObjects.Count; i++)
			{
				WorldObject worldObject = selectedObjects[i];
				worldObject.DrawExtraSelectionOverlays();
			}
		}

		// Token: 0x060034CF RID: 13519 RVA: 0x001C3534 File Offset: 0x001C1934
		private static void DrawSelectionBracketOnGUIFor(WorldObject obj)
		{
			Vector2 vector = obj.ScreenPos();
			Rect rect = new Rect(vector.x - 17.5f, vector.y - 17.5f, 35f, 35f);
			Vector2 textureSize = new Vector2((float)SelectionDrawerUtility.SelectedTexGUI.width * 0.4f, (float)SelectionDrawerUtility.SelectedTexGUI.height * 0.4f);
			SelectionDrawerUtility.CalculateSelectionBracketPositionsUI<WorldObject>(WorldSelectionDrawer.bracketLocs, obj, rect, WorldSelectionDrawer.selectTimes, textureSize, 25f);
			if (obj.HiddenBehindTerrainNow())
			{
				GUI.color = WorldSelectionDrawer.HiddenSelectionBracketColor;
			}
			else
			{
				GUI.color = Color.white;
			}
			int num = 90;
			for (int i = 0; i < 4; i++)
			{
				Widgets.DrawTextureRotated(WorldSelectionDrawer.bracketLocs[i], SelectionDrawerUtility.SelectedTexGUI, (float)num, 0.4f);
				num += 90;
			}
			GUI.color = Color.white;
		}

		// Token: 0x04001C87 RID: 7303
		private static Dictionary<WorldObject, float> selectTimes = new Dictionary<WorldObject, float>();

		// Token: 0x04001C88 RID: 7304
		private const float BaseSelectedTexJump = 25f;

		// Token: 0x04001C89 RID: 7305
		private const float BaseSelectedTexScale = 0.4f;

		// Token: 0x04001C8A RID: 7306
		private const float BaseSelectionRectSize = 35f;

		// Token: 0x04001C8B RID: 7307
		private static readonly Color HiddenSelectionBracketColor = new Color(1f, 1f, 1f, 0.35f);

		// Token: 0x04001C8C RID: 7308
		private static Vector2[] bracketLocs = new Vector2[4];
	}
}