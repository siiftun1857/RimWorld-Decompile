﻿using System;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace RimWorld
{
	public class SitePartWorker_Manhunters : SitePartWorker
	{
		[CompilerGenerated]
		private static Func<Pawn, bool> <>f__am$cache0;

		public SitePartWorker_Manhunters()
		{
		}

		public override string GetArrivedLetterPart(Map map, out string preferredLabel, out LetterDef preferredLetterDef, out LookTargets lookTargets)
		{
			string arrivedLetterPart = base.GetArrivedLetterPart(map, out preferredLabel, out preferredLetterDef, out lookTargets);
			lookTargets = (from x in map.mapPawns.AllPawnsSpawned
			where x.MentalStateDef == MentalStateDefOf.Manhunter || x.MentalStateDef == MentalStateDefOf.ManhunterPermanent
			select x).FirstOrDefault<Pawn>();
			return arrivedLetterPart;
		}

		public override SiteCoreOrPartParams GenerateDefaultParams(Site site, float myThreatPoints)
		{
			SiteCoreOrPartParams siteCoreOrPartParams = base.GenerateDefaultParams(site, myThreatPoints);
			if (ManhunterPackGenStepUtility.TryGetAnimalsKind(siteCoreOrPartParams.threatPoints, site.Tile, out siteCoreOrPartParams.animalKind))
			{
				siteCoreOrPartParams.threatPoints = Mathf.Max(siteCoreOrPartParams.threatPoints, siteCoreOrPartParams.animalKind.combatPower);
			}
			return siteCoreOrPartParams;
		}

		public override string GetPostProcessedDescriptionDialogue(Site site, SiteCoreOrPartBase siteCoreOrPart)
		{
			int animalsCount = this.GetAnimalsCount(siteCoreOrPart.parms);
			return string.Format(base.GetPostProcessedDescriptionDialogue(site, siteCoreOrPart), animalsCount, GenLabel.BestKindLabel(siteCoreOrPart.parms.animalKind, Gender.None, true, animalsCount));
		}

		public override string GetPostProcessedThreatLabel(Site site, SiteCoreOrPartBase siteCoreOrPart)
		{
			int animalsCount = this.GetAnimalsCount(siteCoreOrPart.parms);
			return string.Concat(new object[]
			{
				base.GetPostProcessedThreatLabel(site, siteCoreOrPart),
				" (",
				animalsCount,
				" ",
				GenLabel.BestKindLabel(siteCoreOrPart.parms.animalKind, Gender.None, true, animalsCount),
				")"
			});
		}

		private int GetAnimalsCount(SiteCoreOrPartParams parms)
		{
			return ManhunterPackIncidentUtility.GetAnimalsCount(parms.animalKind, parms.threatPoints);
		}

		[CompilerGenerated]
		private static bool <GetArrivedLetterPart>m__0(Pawn x)
		{
			return x.MentalStateDef == MentalStateDefOf.Manhunter || x.MentalStateDef == MentalStateDefOf.ManhunterPermanent;
		}
	}
}
