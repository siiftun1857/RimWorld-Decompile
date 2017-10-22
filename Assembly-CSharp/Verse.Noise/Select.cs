#define DEBUG
using System.Diagnostics;

namespace Verse.Noise
{
	public class Select : ModuleBase
	{
		private double m_fallOff = 0.0;

		private double m_raw = 0.0;

		private double m_min = -1.0;

		private double m_max = 1.0;

		public ModuleBase Controller
		{
			get
			{
				return base.modules[2];
			}
			set
			{
				Debug.Assert(value != null);
				base.modules[2] = value;
			}
		}

		public double FallOff
		{
			get
			{
				return this.m_fallOff;
			}
			set
			{
				double num = this.m_max - this.m_min;
				this.m_raw = value;
				this.m_fallOff = ((!(value > num / 2.0)) ? value : (num / 2.0));
			}
		}

		public double Maximum
		{
			get
			{
				return this.m_max;
			}
			set
			{
				this.m_max = value;
				this.FallOff = this.m_raw;
			}
		}

		public double Minimum
		{
			get
			{
				return this.m_min;
			}
			set
			{
				this.m_min = value;
				this.FallOff = this.m_raw;
			}
		}

		public Select() : base(3)
		{
		}

		public Select(ModuleBase inputA, ModuleBase inputB, ModuleBase controller) : base(3)
		{
			base.modules[0] = inputA;
			base.modules[1] = inputB;
			base.modules[2] = controller;
		}

		public Select(double min, double max, double fallOff, ModuleBase inputA, ModuleBase inputB) : this(inputA, inputB, null)
		{
			this.m_min = min;
			this.m_max = max;
			this.FallOff = fallOff;
		}

		public void SetBounds(double min, double max)
		{
			Debug.Assert(min < max);
			this.m_min = min;
			this.m_max = max;
			this.FallOff = this.m_fallOff;
		}

		public override double GetValue(double x, double y, double z)
		{
			Debug.Assert(base.modules[0] != null);
			Debug.Assert(base.modules[1] != null);
			Debug.Assert(base.modules[2] != null);
			double value = base.modules[2].GetValue(x, y, z);
			double result;
			if (this.m_fallOff > 0.0)
			{
				if (value < this.m_min - this.m_fallOff)
				{
					result = base.modules[0].GetValue(x, y, z);
				}
				else if (value < this.m_min + this.m_fallOff)
				{
					double num = this.m_min - this.m_fallOff;
					double num2 = this.m_min + this.m_fallOff;
					double position = Utils.MapCubicSCurve((value - num) / (num2 - num));
					result = Utils.InterpolateLinear(base.modules[0].GetValue(x, y, z), base.modules[1].GetValue(x, y, z), position);
				}
				else if (value < this.m_max - this.m_fallOff)
				{
					result = base.modules[1].GetValue(x, y, z);
				}
				else if (value < this.m_max + this.m_fallOff)
				{
					double num3 = this.m_max - this.m_fallOff;
					double num4 = this.m_max + this.m_fallOff;
					double position = Utils.MapCubicSCurve((value - num3) / (num4 - num3));
					result = Utils.InterpolateLinear(base.modules[1].GetValue(x, y, z), base.modules[0].GetValue(x, y, z), position);
				}
				else
				{
					result = base.modules[0].GetValue(x, y, z);
				}
			}
			else
			{
				result = ((!(value < this.m_min) && !(value > this.m_max)) ? base.modules[1].GetValue(x, y, z) : base.modules[0].GetValue(x, y, z));
			}
			return result;
		}
	}
}
