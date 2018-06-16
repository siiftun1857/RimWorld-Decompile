﻿using System;
using Verse;
using Verse.Sound;

namespace RimWorld
{
	// Token: 0x0200072B RID: 1835
	public class CompPsychicDrone : ThingComp
	{
		// Token: 0x17000632 RID: 1586
		// (get) Token: 0x06002861 RID: 10337 RVA: 0x001588C0 File Offset: 0x00156CC0
		public CompProperties_PsychicDrone Props
		{
			get
			{
				return (CompProperties_PsychicDrone)this.props;
			}
		}

		// Token: 0x17000633 RID: 1587
		// (get) Token: 0x06002862 RID: 10338 RVA: 0x001588E0 File Offset: 0x00156CE0
		public PsychicDroneLevel DroneLevel
		{
			get
			{
				return this.droneLevel;
			}
		}

		// Token: 0x06002863 RID: 10339 RVA: 0x001588FB File Offset: 0x00156CFB
		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			if (!respawningAfterLoad)
			{
				this.ticksToIncreaseDroneLevel = this.Props.droneLevelIncreaseInterval;
			}
			SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(this.parent.Map);
		}

		// Token: 0x06002864 RID: 10340 RVA: 0x00158931 File Offset: 0x00156D31
		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look<int>(ref this.ticksToIncreaseDroneLevel, "ticksToIncreaseDroneLevel", 0, false);
			Scribe_Values.Look<PsychicDroneLevel>(ref this.droneLevel, "droneLevel", PsychicDroneLevel.None, false);
		}

		// Token: 0x06002865 RID: 10341 RVA: 0x00158960 File Offset: 0x00156D60
		public override void CompTick()
		{
			if (this.parent.Spawned)
			{
				this.ticksToIncreaseDroneLevel--;
				if (this.ticksToIncreaseDroneLevel <= 0)
				{
					this.IncreaseDroneLevel();
					this.ticksToIncreaseDroneLevel = this.Props.droneLevelIncreaseInterval;
				}
			}
		}

		// Token: 0x06002866 RID: 10342 RVA: 0x001589B8 File Offset: 0x00156DB8
		private void IncreaseDroneLevel()
		{
			if (this.droneLevel != PsychicDroneLevel.BadExtreme)
			{
				this.droneLevel += 1;
				string text = "LetterPsychicDroneLevelIncreased".Translate();
				Find.LetterStack.ReceiveLetter("LetterLabelPsychicDroneLevelIncreased".Translate(), text, LetterDefOf.NegativeEvent, null);
				SoundDefOf.PsychicPulseGlobal.PlayOneShotOnCamera(this.parent.Map);
			}
		}

		// Token: 0x06002867 RID: 10343 RVA: 0x00158A24 File Offset: 0x00156E24
		public override string CompInspectStringExtra()
		{
			string text = "Error";
			switch (this.droneLevel)
			{
			case PsychicDroneLevel.BadLow:
				text = "PsychicDroneLevelLow".Translate();
				break;
			case PsychicDroneLevel.BadMedium:
				text = "PsychicDroneLevelMedium".Translate();
				break;
			case PsychicDroneLevel.BadHigh:
				text = "PsychicDroneLevelHigh".Translate();
				break;
			case PsychicDroneLevel.BadExtreme:
				text = "PsychicDroneLevelExtreme".Translate();
				break;
			}
			return "PsychicDroneLevel".Translate(new object[]
			{
				text
			});
		}

		// Token: 0x04001615 RID: 5653
		private int ticksToIncreaseDroneLevel;

		// Token: 0x04001616 RID: 5654
		private PsychicDroneLevel droneLevel = PsychicDroneLevel.BadLow;
	}
}