﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class Designator_Strip : Designator
	{
		public Designator_Strip()
		{
			this.defaultLabel = "DesignatorStrip".Translate();
			this.defaultDesc = "DesignatorStripDesc".Translate();
			this.icon = ContentFinder<Texture2D>.Get("UI/Designators/Strip", true);
			this.soundDragSustain = SoundDefOf.Designate_DragStandard;
			this.soundDragChanged = SoundDefOf.Designate_DragStandard_Changed;
			this.useMouseIcon = true;
			this.soundSucceeded = SoundDefOf.Designate_Claim;
			this.hotKey = KeyBindingDefOf.Misc11;
		}

		public override int DraggableDimensions
		{
			get
			{
				return 2;
			}
		}

		protected override DesignationDef Designation
		{
			get
			{
				return DesignationDefOf.Strip;
			}
		}

		public override AcceptanceReport CanDesignateCell(IntVec3 c)
		{
			if (!c.InBounds(base.Map))
			{
				return false;
			}
			if (!this.StrippablesInCell(c).Any<Thing>())
			{
				return "MessageMustDesignateStrippable".Translate();
			}
			return true;
		}

		public override void DesignateSingleCell(IntVec3 c)
		{
			foreach (Thing t in this.StrippablesInCell(c))
			{
				this.DesignateThing(t);
			}
		}

		public override AcceptanceReport CanDesignateThing(Thing t)
		{
			if (base.Map.designationManager.DesignationOn(t, this.Designation) != null)
			{
				return false;
			}
			return StrippableUtility.CanBeStrippedByColony(t);
		}

		public override void DesignateThing(Thing t)
		{
			base.Map.designationManager.AddDesignation(new Designation(t, this.Designation));
		}

		private IEnumerable<Thing> StrippablesInCell(IntVec3 c)
		{
			if (c.Fogged(base.Map))
			{
				yield break;
			}
			List<Thing> thingList = c.GetThingList(base.Map);
			for (int i = 0; i < thingList.Count; i++)
			{
				if (this.CanDesignateThing(thingList[i]).Accepted)
				{
					yield return thingList[i];
				}
			}
			yield break;
		}

		[CompilerGenerated]
		private sealed class <StrippablesInCell>c__Iterator0 : IEnumerable, IEnumerable<Thing>, IEnumerator, IDisposable, IEnumerator<Thing>
		{
			internal IntVec3 c;

			internal List<Thing> <thingList>__0;

			internal int <i>__1;

			internal Designator_Strip $this;

			internal Thing $current;

			internal bool $disposing;

			internal int $PC;

			[DebuggerHidden]
			public <StrippablesInCell>c__Iterator0()
			{
			}

			public bool MoveNext()
			{
				uint num = (uint)this.$PC;
				this.$PC = -1;
				switch (num)
				{
				case 0u:
					if (c.Fogged(base.Map))
					{
						return false;
					}
					thingList = c.GetThingList(base.Map);
					i = 0;
					break;
				case 1u:
					IL_BD:
					i++;
					break;
				default:
					return false;
				}
				if (i >= thingList.Count)
				{
					this.$PC = -1;
				}
				else
				{
					if (this.CanDesignateThing(thingList[i]).Accepted)
					{
						this.$current = thingList[i];
						if (!this.$disposing)
						{
							this.$PC = 1;
						}
						return true;
					}
					goto IL_BD;
				}
				return false;
			}

			Thing IEnumerator<Thing>.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			object IEnumerator.Current
			{
				[DebuggerHidden]
				get
				{
					return this.$current;
				}
			}

			[DebuggerHidden]
			public void Dispose()
			{
				this.$disposing = true;
				this.$PC = -1;
			}

			[DebuggerHidden]
			public void Reset()
			{
				throw new NotSupportedException();
			}

			[DebuggerHidden]
			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.System.Collections.Generic.IEnumerable<Verse.Thing>.GetEnumerator();
			}

			[DebuggerHidden]
			IEnumerator<Thing> IEnumerable<Thing>.GetEnumerator()
			{
				if (Interlocked.CompareExchange(ref this.$PC, 0, -2) == -2)
				{
					return this;
				}
				Designator_Strip.<StrippablesInCell>c__Iterator0 <StrippablesInCell>c__Iterator = new Designator_Strip.<StrippablesInCell>c__Iterator0();
				<StrippablesInCell>c__Iterator.$this = this;
				<StrippablesInCell>c__Iterator.c = c;
				return <StrippablesInCell>c__Iterator;
			}
		}
	}
}
