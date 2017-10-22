#define DEBUG
using System;
using System.Diagnostics;

namespace Verse.Noise
{
	public class Exponent : ModuleBase
	{
		private double m_exponent = 1.0;

		public double Value
		{
			get
			{
				return this.m_exponent;
			}
			set
			{
				this.m_exponent = value;
			}
		}

		public Exponent() : base(1)
		{
		}

		public Exponent(ModuleBase input) : base(1)
		{
			base.modules[0] = input;
		}

		public Exponent(double exponent, ModuleBase input) : base(1)
		{
			base.modules[0] = input;
			this.Value = exponent;
		}

		public override double GetValue(double x, double y, double z)
		{
			Debug.Assert(base.modules[0] != null);
			double value = base.modules[0].GetValue(x, y, z);
			return Math.Pow(Math.Abs((value + 1.0) / 2.0), this.m_exponent) * 2.0 - 1.0;
		}
	}
}
