namespace Verse.Sound
{
	public class SoundParamSource_External : SoundParamSource
	{
		[Description("The name of the independent parameter that the game will change to drive this relationship.\n\nThis must exactly match a string that the code will use to modify this sound. If the code doesn't reference this, it will have no effect.\n\nOn the graph, this is the X axis.")]
		public string inParamName = "";

		[Description("If the code has never set this parameter on a sustainer, it will use this value.")]
		private float defaultValue = 1f;

		public override string Label
		{
			get
			{
				return (!(this.inParamName == "")) ? this.inParamName : "Undefined external";
			}
		}

		public override float ValueFor(Sample samp)
		{
			float num = default(float);
			return (!samp.ExternalParams.TryGetValue(this.inParamName, out num)) ? this.defaultValue : num;
		}
	}
}
