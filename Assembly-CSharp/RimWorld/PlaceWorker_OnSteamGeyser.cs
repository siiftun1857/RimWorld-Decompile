using Verse;

namespace RimWorld
{
	public class PlaceWorker_OnSteamGeyser : PlaceWorker
	{
		public override AcceptanceReport AllowsPlacing(BuildableDef checkingDef, IntVec3 loc, Rot4 rot, Map map, Thing thingToIgnore = null)
		{
			Thing thing = map.thingGrid.ThingAt(loc, ThingDefOf.SteamGeyser);
			return (thing != null && !(thing.Position != loc)) ? true : "MustPlaceOnSteamGeyser".Translate();
		}

		public override bool ForceAllowPlaceOver(BuildableDef otherDef)
		{
			return otherDef == ThingDefOf.SteamGeyser;
		}
	}
}
