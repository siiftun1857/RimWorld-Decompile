using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	public class CompFlickable : ThingComp
	{
		private bool switchOnInt = true;

		private bool wantSwitchOn = true;

		private Graphic offGraphic;

		private Texture2D cachedCommandTex;

		private const string OffGraphicSuffix = "_Off";

		public const string FlickedOnSignal = "FlickedOn";

		public const string FlickedOffSignal = "FlickedOff";

		private CompProperties_Flickable Props
		{
			get
			{
				return (CompProperties_Flickable)base.props;
			}
		}

		private Texture2D CommandTex
		{
			get
			{
				if ((UnityEngine.Object)this.cachedCommandTex == (UnityEngine.Object)null)
				{
					this.cachedCommandTex = ContentFinder<Texture2D>.Get(this.Props.commandTexture, true);
				}
				return this.cachedCommandTex;
			}
		}

		public bool SwitchIsOn
		{
			get
			{
				return this.switchOnInt;
			}
			set
			{
				if (this.switchOnInt != value)
				{
					this.switchOnInt = value;
					if (this.switchOnInt)
					{
						base.parent.BroadcastCompSignal("FlickedOn");
					}
					else
					{
						base.parent.BroadcastCompSignal("FlickedOff");
					}
					if (base.parent.Spawned)
					{
						base.parent.Map.mapDrawer.MapMeshDirty(base.parent.Position, MapMeshFlag.Things | MapMeshFlag.Buildings);
					}
				}
			}
		}

		public Graphic CurrentGraphic
		{
			get
			{
				Graphic defaultGraphic;
				if (this.SwitchIsOn)
				{
					defaultGraphic = base.parent.DefaultGraphic;
				}
				else
				{
					if (this.offGraphic == null)
					{
						this.offGraphic = GraphicDatabase.Get(base.parent.def.graphicData.graphicClass, base.parent.def.graphicData.texPath + "_Off", ShaderDatabase.ShaderFromType(base.parent.def.graphicData.shaderType), base.parent.def.graphicData.drawSize, base.parent.DrawColor, base.parent.DrawColorTwo);
					}
					defaultGraphic = this.offGraphic;
				}
				return defaultGraphic;
			}
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<bool>(ref this.switchOnInt, "switchOn", true, false);
			Scribe_Values.Look<bool>(ref this.wantSwitchOn, "wantSwitchOn", true, false);
		}

		public bool WantsFlick()
		{
			return this.wantSwitchOn != this.switchOnInt;
		}

		public void DoFlick()
		{
			this.SwitchIsOn = !this.SwitchIsOn;
			SoundDefOf.FlickSwitch.PlayOneShot(new TargetInfo(base.parent.Position, base.parent.Map, false));
		}

		public void ResetToOn()
		{
			this.switchOnInt = true;
			this.wantSwitchOn = true;
		}

		public override IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			using (IEnumerator<Gizmo> enumerator = this._003CCompGetGizmosExtra_003E__BaseCallProxy0().GetEnumerator())
			{
				if (enumerator.MoveNext())
				{
					Gizmo c = enumerator.Current;
					yield return c;
					/*Error: Unable to find new state assignment for yield return*/;
				}
			}
			if (base.parent.Faction != Faction.OfPlayer)
				yield break;
			yield return (Gizmo)new Command_Toggle
			{
				hotKey = KeyBindingDefOf.CommandTogglePower,
				icon = this.CommandTex,
				defaultLabel = this.Props.commandLabelKey.Translate(),
				defaultDesc = this.Props.commandDescKey.Translate(),
				isActive = (Func<bool>)(() => ((_003CCompGetGizmosExtra_003Ec__Iterator0)/*Error near IL_014a: stateMachine*/)._0024this.wantSwitchOn),
				toggleAction = (Action)delegate
				{
					((_003CCompGetGizmosExtra_003Ec__Iterator0)/*Error near IL_0161: stateMachine*/)._0024this.wantSwitchOn = !((_003CCompGetGizmosExtra_003Ec__Iterator0)/*Error near IL_0161: stateMachine*/)._0024this.wantSwitchOn;
					FlickUtility.UpdateFlickDesignation(((_003CCompGetGizmosExtra_003Ec__Iterator0)/*Error near IL_0161: stateMachine*/)._0024this.parent);
				}
			};
			/*Error: Unable to find new state assignment for yield return*/;
			IL_019c:
			/*Error near IL_019d: Unexpected return in MoveNext()*/;
		}
	}
}
