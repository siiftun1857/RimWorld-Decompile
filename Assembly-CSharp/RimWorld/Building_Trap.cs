using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace RimWorld
{
	public abstract class Building_Trap : Building
	{
		private List<Pawn> touchingPawns = new List<Pawn>();

		private const float KnowerSpringChance = 0.004f;

		private const ushort KnowerPathFindCost = 800;

		private const ushort KnowerPathWalkCost = 30;

		private const float AnimalSpringChanceFactor = 0.1f;

		public virtual bool Armed
		{
			get
			{
				return true;
			}
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look<Pawn>(ref this.touchingPawns, "testees", LookMode.Reference, new object[0]);
		}

		public override void Tick()
		{
			if (this.Armed)
			{
				List<Thing> thingList = base.Position.GetThingList(base.Map);
				for (int i = 0; i < thingList.Count; i++)
				{
					Pawn pawn = thingList[i] as Pawn;
					if (pawn != null && !this.touchingPawns.Contains(pawn))
					{
						this.touchingPawns.Add(pawn);
						this.CheckSpring(pawn);
					}
				}
			}
			for (int j = 0; j < this.touchingPawns.Count; j++)
			{
				Pawn pawn2 = this.touchingPawns[j];
				if (!pawn2.Spawned || pawn2.Position != base.Position)
				{
					this.touchingPawns.Remove(pawn2);
				}
			}
			base.Tick();
		}

		protected virtual float SpringChance(Pawn p)
		{
			float num = (float)((!this.KnowsOfTrap(p)) ? this.GetStatValue(StatDefOf.TrapSpringChance, true) : 0.0040000001899898052);
			num *= GenMath.LerpDouble(0.4f, 0.8f, 0f, 1f, p.BodySize);
			if (p.RaceProps.Animal)
			{
				num = (float)(num * 0.10000000149011612);
			}
			return Mathf.Clamp01(num);
		}

		private void CheckSpring(Pawn p)
		{
			if (Rand.Value < this.SpringChance(p))
			{
				this.Spring(p);
				if (p.Faction != Faction.OfPlayer && p.HostFaction != Faction.OfPlayer)
					return;
				Find.LetterStack.ReceiveLetter("LetterFriendlyTrapSprungLabel".Translate(p.NameStringShort), "LetterFriendlyTrapSprung".Translate(p.NameStringShort), LetterDefOf.NegativeEvent, new TargetInfo(base.Position, base.Map, false), (string)null);
			}
		}

		public bool KnowsOfTrap(Pawn p)
		{
			bool result;
			if (p.Faction != null && !p.Faction.HostileTo(base.Faction))
			{
				result = true;
			}
			else if (p.Faction == null && p.RaceProps.Animal && !p.InAggroMentalState)
			{
				result = true;
			}
			else if (p.guest != null && p.guest.Released)
			{
				result = true;
			}
			else
			{
				Lord lord = p.GetLord();
				result = ((byte)((p.RaceProps.Humanlike && lord != null && lord.LordJob is LordJob_FormAndSendCaravan) ? 1 : 0) != 0);
			}
			return result;
		}

		public override ushort PathFindCostFor(Pawn p)
		{
			return (ushort)(this.Armed ? (this.KnowsOfTrap(p) ? 800 : 0) : 0);
		}

		public override ushort PathWalkCostFor(Pawn p)
		{
			return (ushort)(this.Armed ? (this.KnowsOfTrap(p) ? 30 : 0) : 0);
		}

		public override bool IsDangerousFor(Pawn p)
		{
			return this.Armed && this.KnowsOfTrap(p);
		}

		public override string GetInspectString()
		{
			string text = base.GetInspectString();
			if (!text.NullOrEmpty())
			{
				text += "\n";
			}
			return (!this.Armed) ? (text + "TrapNotArmed".Translate()) : (text + "TrapArmed".Translate());
		}

		public void Spring(Pawn p)
		{
			SoundDef.Named("DeadfallSpring").PlayOneShot(new TargetInfo(base.Position, base.Map, false));
			if (p != null && p.Faction != null)
			{
				p.Faction.TacticalMemory.TrapRevealed(base.Position, base.Map);
			}
			this.SpringSub(p);
		}

		protected abstract void SpringSub(Pawn p);
	}
}
