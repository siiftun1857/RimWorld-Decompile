using Verse;

namespace RimWorld
{
	public static class DownedRefugeeQuestUtility
	{
		private const float RelationWithColonistWeight = 20f;

		private const float ChanceToRedressWorldPawn = 0.2f;

		public static Pawn GenerateRefugee(int tile)
		{
			PawnKindDef spaceRefugee = PawnKindDefOf.SpaceRefugee;
			Faction ofSpacer = Faction.OfSpacer;
			PawnGenerationRequest request = new PawnGenerationRequest(spaceRefugee, ofSpacer, PawnGenerationContext.NonPlayer, tile, false, false, false, false, true, false, 20f, false, true, true, false, false, false, false, null, new float?(0.2f), default(float?), default(float?), default(Gender?), default(float?), (string)null);
			Pawn pawn = PawnGenerator.GeneratePawn(request);
			HealthUtility.BreakLegs(pawn);
			return pawn;
		}
	}
}
