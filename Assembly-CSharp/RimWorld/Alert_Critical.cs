using UnityEngine;
using Verse;

namespace RimWorld
{
	public abstract class Alert_Critical : Alert
	{
		private int lastActiveFrame = -1;

		private const float PulseFreq = 0.5f;

		private const float PulseAmpCritical = 0.6f;

		private const float PulseAmpTutorial = 0.2f;

		protected override Color BGColor
		{
			get
			{
				float num = Pulser.PulseBrightness(0.5f, Pulser.PulseBrightness(0.5f, 0.6f));
				return new Color(num, num, num) * Color.red;
			}
		}

		public Alert_Critical()
		{
			base.defaultPriority = AlertPriority.Critical;
		}

		public override void AlertActiveUpdate()
		{
			if (this.lastActiveFrame < Time.frameCount - 1)
			{
				string text = "MessageCriticalAlert".Translate(this.GetLabel());
				AlertReport report = this.GetReport();
				Messages.Message(text, report.culprit, MessageTypeDefOf.ThreatBig);
			}
			this.lastActiveFrame = Time.frameCount;
		}
	}
}
