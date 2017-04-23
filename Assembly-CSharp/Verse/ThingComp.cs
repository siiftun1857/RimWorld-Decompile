using RimWorld;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Verse
{
	public abstract class ThingComp
	{
		public ThingWithComps parent;

		public CompProperties props;

		public IThingHolder ParentHolder
		{
			get
			{
				return this.parent.ParentHolder;
			}
		}

		public virtual void Initialize(CompProperties props)
		{
			this.props = props;
		}

		public virtual void ReceiveCompSignal(string signal)
		{
		}

		public virtual void PostExposeData()
		{
		}

		public virtual void PostSpawnSetup(bool respawningAfterLoad)
		{
		}

		public virtual void PostDeSpawn(Map map)
		{
		}

		public virtual void PostDestroy(DestroyMode mode, Map previousMap)
		{
		}

		public virtual void CompTick()
		{
		}

		public virtual void CompTickRare()
		{
		}

		public virtual void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
		{
			absorbed = false;
		}

		public virtual void PostPostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
		{
		}

		public virtual void PostDraw()
		{
		}

		public virtual void PostDrawExtraSelectionOverlays()
		{
		}

		public virtual void PostPrintOnto(SectionLayer layer)
		{
		}

		public virtual void CompPrintForPowerGrid(SectionLayer layer)
		{
		}

		public virtual void PreAbsorbStack(Thing otherStack, int count)
		{
		}

		public virtual void PostSplitOff(Thing piece)
		{
		}

		public virtual string TransformLabel(string label)
		{
			return label;
		}

		[DebuggerHidden]
		public virtual IEnumerable<Gizmo> CompGetGizmosExtra()
		{
			ThingComp.<CompGetGizmosExtra>c__IteratorAF <CompGetGizmosExtra>c__IteratorAF = new ThingComp.<CompGetGizmosExtra>c__IteratorAF();
			ThingComp.<CompGetGizmosExtra>c__IteratorAF expr_07 = <CompGetGizmosExtra>c__IteratorAF;
			expr_07.$PC = -2;
			return expr_07;
		}

		public virtual bool AllowStackWith(Thing other)
		{
			return true;
		}

		public virtual string CompInspectStringExtra()
		{
			return null;
		}

		public virtual string GetDescriptionPart()
		{
			return null;
		}

		[DebuggerHidden]
		public virtual IEnumerable<FloatMenuOption> CompFloatMenuOptions(Pawn selPawn)
		{
			ThingComp.<CompFloatMenuOptions>c__IteratorB0 <CompFloatMenuOptions>c__IteratorB = new ThingComp.<CompFloatMenuOptions>c__IteratorB0();
			ThingComp.<CompFloatMenuOptions>c__IteratorB0 expr_07 = <CompFloatMenuOptions>c__IteratorB;
			expr_07.$PC = -2;
			return expr_07;
		}

		public virtual void PrePreTraded(TradeAction action, Pawn playerNegotiator, ITrader trader)
		{
		}

		public virtual void PostIngested(Pawn ingester)
		{
		}

		public virtual void PostPostGeneratedForTrader(TraderKindDef trader, int forTile, Faction forFaction)
		{
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				base.GetType().Name,
				"(parent=",
				this.parent,
				" at=",
				(this.parent == null) ? IntVec3.Invalid : this.parent.Position,
				")"
			});
		}
	}
}