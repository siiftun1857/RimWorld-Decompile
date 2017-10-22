using RimWorld.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace RimWorld
{
	public class IncidentWorker_CaravanMeeting : IncidentWorker
	{
		private const int MapSize = 100;

		protected override bool CanFireNowSub(IIncidentTarget target)
		{
			return target is Map || CaravanIncidentUtility.CanFireIncidentWhichWantsToGenerateMapAt(target.Tile);
		}

		protected override bool TryExecuteWorker(IncidentParms parms)
		{
			bool result;
			if (parms.target is Map)
			{
				result = IncidentDefOf.TravelerGroup.Worker.TryExecute(parms);
			}
			else
			{
				Caravan caravan = (Caravan)parms.target;
				Faction faction;
				if (!(from x in Find.FactionManager.AllFactionsListForReading
				where !x.IsPlayer && !x.HostileTo(Faction.OfPlayer) && !x.def.hidden && x.def.humanlikeFaction && x.def.caravanTraderKinds.Any()
				select x).TryRandomElement<Faction>(out faction))
				{
					result = false;
				}
				else
				{
					CameraJumper.TryJumpAndSelect((WorldObject)caravan);
					List<Pawn> pawns = this.GenerateCaravanPawns(faction);
					Caravan metCaravan = CaravanMaker.MakeCaravan(pawns, faction, -1, false);
					string text = "CaravanMeeting".Translate(faction.Name, PawnUtility.PawnKindsToCommaList(metCaravan.PawnsListForReading));
					DiaNode diaNode = new DiaNode(text);
					Pawn bestPlayerNegotiator = BestCaravanPawnUtility.FindBestNegotiator(caravan);
					if (metCaravan.CanTradeNow)
					{
						DiaOption diaOption = new DiaOption("CaravanMeeting_Trade".Translate());
						diaOption.action = (Action)delegate
						{
							Find.WindowStack.Add(new Dialog_Trade(bestPlayerNegotiator, metCaravan));
							string label = "";
							string text3 = "";
							PawnRelationUtility.Notify_PawnsSeenByPlayer_Letter(metCaravan.Goods.OfType<Pawn>(), ref label, ref text3, "LetterRelatedPawnsTradingWithOtherCaravan".Translate(), false, true);
							if (!text3.NullOrEmpty())
							{
								Find.LetterStack.ReceiveLetter(label, text3, LetterDefOf.NeutralEvent, new GlobalTargetInfo(caravan.Tile), (string)null);
							}
						};
						if (bestPlayerNegotiator == null)
						{
							diaOption.Disable("CaravanMeeting_TradeIncapable".Translate());
						}
						diaNode.options.Add(diaOption);
					}
					DiaOption diaOption2 = new DiaOption("CaravanMeeting_Attack".Translate());
					diaOption2.action = (Action)delegate
					{
						LongEventHandler.QueueLongEvent((Action)delegate
						{
							if (!faction.HostileTo(Faction.OfPlayer))
							{
								faction.SetHostileTo(Faction.OfPlayer, true);
							}
							Pawn t = caravan.PawnsListForReading[0];
							Map map = CaravanIncidentUtility.GetOrGenerateMapForIncident(caravan, new IntVec3(100, 1, 100), WorldObjectDefOf.AttackedCaravan);
							IntVec3 playerSpot;
							IntVec3 enemySpot;
							MultipleCaravansCellFinder.FindStartingCellsFor2Groups(map, out playerSpot, out enemySpot);
							CaravanEnterMapUtility.Enter(caravan, map, (Func<Pawn, IntVec3>)((Pawn p) => CellFinder.RandomClosewalkCellNear(playerSpot, map, 12, null)), CaravanDropInventoryMode.DoNotDrop, true);
							List<Pawn> list = metCaravan.PawnsListForReading.ToList();
							CaravanEnterMapUtility.Enter(metCaravan, map, (Func<Pawn, IntVec3>)((Pawn p) => CellFinder.RandomClosewalkCellNear(enemySpot, map, 12, null)), CaravanDropInventoryMode.DoNotDrop, false);
							LordMaker.MakeNewLord(faction, new LordJob_DefendAttackedTraderCaravan(list[0].Position), map, list);
							Find.TickManager.CurTimeSpeed = TimeSpeed.Paused;
							CameraJumper.TryJumpAndSelect((Thing)t);
							Messages.Message("MessageAttackedCaravanIsNowHostile".Translate(faction.Name), new GlobalTargetInfo(list[0].Position, list[0].Map, false), MessageTypeDefOf.ThreatBig);
						}, "GeneratingMapForNewEncounter", false, null);
					};
					diaOption2.resolveTree = true;
					diaNode.options.Add(diaOption2);
					DiaOption diaOption3 = new DiaOption("CaravanMeeting_MoveOn".Translate());
					diaOption3.action = (Action)delegate
					{
						this.RemoveAllPawnsAndPassToWorld(metCaravan);
					};
					diaOption3.resolveTree = true;
					diaNode.options.Add(diaOption3);
					string text2 = "CaravanMeetingTitle".Translate(caravan.Label);
					WindowStack windowStack = Find.WindowStack;
					DiaNode nodeRoot = diaNode;
					bool delayInteractivity = true;
					string title = text2;
					windowStack.Add(new Dialog_NodeTree(nodeRoot, delayInteractivity, false, title));
					result = true;
				}
			}
			return result;
		}

		private List<Pawn> GenerateCaravanPawns(Faction faction)
		{
			PawnGroupMakerParms pawnGroupMakerParms = new PawnGroupMakerParms();
			pawnGroupMakerParms.faction = faction;
			pawnGroupMakerParms.points = TraderCaravanUtility.GenerateGuardPoints();
			return PawnGroupMakerUtility.GeneratePawns(PawnGroupKindDefOf.Trader, pawnGroupMakerParms, true).ToList();
		}

		private void RemoveAllPawnsAndPassToWorld(Caravan caravan)
		{
			List<Pawn> pawnsListForReading = caravan.PawnsListForReading;
			for (int i = 0; i < pawnsListForReading.Count; i++)
			{
				Find.WorldPawns.PassToWorld(pawnsListForReading[i], PawnDiscardDecideMode.Discard);
			}
			caravan.RemoveAllPawns();
		}
	}
}
