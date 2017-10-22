using Verse;

namespace RimWorld
{
	public static class PrisonerWillingToJoinQuestUtility
	{
		private const float RelationWithColonistWeight = 75f;

		public static Pawn GeneratePrisoner(int tile, Faction hostFaction)
		{
			PawnKindDef slave = PawnKindDefOf.Slave;
			PawnGenerationRequest request = new PawnGenerationRequest(slave, hostFaction, PawnGenerationContext.NonPlayer, tile, false, false, false, false, true, false, 75f, false, true, true, false, false, true, true, null, default(float?), default(float?), default(float?), default(Gender?), default(float?), (string)null);
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			pawn.guest.SetGuestStatus(hostFaction, true);
			return pawn;
		}
	}
}
