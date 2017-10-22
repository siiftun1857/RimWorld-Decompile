using System;
using UnityEngine;
using Verse;

namespace RimWorld
{
	[StaticConstructorOnStartup]
	internal class Gizmo_RefuelableFuelStatus : Gizmo
	{
		public CompRefuelable refuelable;

		private static readonly Texture2D FullBarTex = SolidColorMaterials.NewSolidColorTexture(new Color(0.35f, 0.35f, 0.2f));

		private static readonly Texture2D EmptyBarTex = SolidColorMaterials.NewSolidColorTexture(Color.black);

		private static readonly Texture2D TargetLevelArrow = ContentFinder<Texture2D>.Get("UI/Misc/BarInstantMarkerRotated", true);

		private const float ArrowScale = 0.5f;

		public override float Width
		{
			get
			{
				return 140f;
			}
		}

		public override GizmoResult GizmoOnGUI(Vector2 topLeft)
		{
			Rect overRect = new Rect(topLeft.x, topLeft.y, this.Width, 75f);
			Find.WindowStack.ImmediateWindow(1523289473, overRect, WindowLayer.GameUI, (Action)delegate
			{
				Rect rect;
				Rect rect2 = rect = overRect.AtZero().ContractedBy(6f);
				rect.height = (float)(overRect.height / 2.0);
				Text.Font = GameFont.Tiny;
				Widgets.Label(rect, "FuelLevelGizmoLabel".Translate());
				Rect rect3 = rect2;
				rect3.yMin = (float)(overRect.height / 2.0);
				float fillPercent = this.refuelable.Fuel / this.refuelable.Props.fuelCapacity;
				Widgets.FillableBar(rect3, fillPercent, Gizmo_RefuelableFuelStatus.FullBarTex, Gizmo_RefuelableFuelStatus.EmptyBarTex, false);
				if (this.refuelable.Props.targetFuelLevelConfigurable)
				{
					float num = this.refuelable.TargetFuelLevel / this.refuelable.Props.fuelCapacity;
					float x = (float)(rect3.x + num * rect3.width - (float)Gizmo_RefuelableFuelStatus.TargetLevelArrow.width * 0.5 / 2.0);
					float y = (float)(rect3.y - (float)Gizmo_RefuelableFuelStatus.TargetLevelArrow.height * 0.5);
					GUI.DrawTexture(new Rect(x, y, (float)((float)Gizmo_RefuelableFuelStatus.TargetLevelArrow.width * 0.5), (float)((float)Gizmo_RefuelableFuelStatus.TargetLevelArrow.height * 0.5)), Gizmo_RefuelableFuelStatus.TargetLevelArrow);
				}
				Text.Font = GameFont.Small;
				Text.Anchor = TextAnchor.MiddleCenter;
				Widgets.Label(rect3, this.refuelable.Fuel.ToString("F0") + " / " + this.refuelable.Props.fuelCapacity.ToString("F0"));
				Text.Anchor = TextAnchor.UpperLeft;
			}, true, false, 1f);
			return new GizmoResult(GizmoState.Clear);
		}
	}
}
