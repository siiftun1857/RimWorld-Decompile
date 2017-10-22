namespace Verse.Noise
{
	public class InverseLerp : ModuleBase
	{
		private float from;

		private float to;

		public InverseLerp() : base(1)
		{
		}

		public InverseLerp(ModuleBase module, float from, float to) : base(1)
		{
			base.modules[0] = module;
			this.from = from;
			this.to = to;
		}

		public override double GetValue(double x, double y, double z)
		{
			double value = base.modules[0].GetValue(x, y, z);
			double num = (value - (double)this.from) / (double)(this.to - this.from);
			return (!(num < 0.0)) ? ((!(num > 1.0)) ? num : 1.0) : 0.0;
		}
	}
}
