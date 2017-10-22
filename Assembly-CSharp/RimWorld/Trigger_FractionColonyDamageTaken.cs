using UnityEngine;
using Verse.AI.Group;

namespace RimWorld
{
	public class Trigger_FractionColonyDamageTaken : Trigger
	{
		private float desiredColonyDamageFraction;

		private float minDamage;

		private TriggerData_FractionColonyDamageTaken Data
		{
			get
			{
				return (TriggerData_FractionColonyDamageTaken)base.data;
			}
		}

		public Trigger_FractionColonyDamageTaken(float desiredColonyDamageFraction, float minDamage = 3.40282347E+38f)
		{
			base.data = new TriggerData_FractionColonyDamageTaken();
			this.desiredColonyDamageFraction = desiredColonyDamageFraction;
			this.minDamage = minDamage;
		}

		public override void SourceToilBecameActive(Transition transition, LordToil previousToil)
		{
			if (!transition.sources.Contains(previousToil))
			{
				this.Data.startColonyDamage = transition.Map.damageWatcher.DamageTakenEver;
			}
		}

		public override bool ActivateOn(Lord lord, TriggerSignal signal)
		{
			bool result;
			if (signal.type == TriggerSignalType.Tick)
			{
				float num = Mathf.Max((float)lord.initialColonyHealthTotal * this.desiredColonyDamageFraction, this.minDamage);
				result = (lord.Map.damageWatcher.DamageTakenEver > this.Data.startColonyDamage + num);
			}
			else
			{
				result = false;
			}
			return result;
		}
	}
}
