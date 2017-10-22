#define DEBUG
using System.Diagnostics;

namespace Verse.Noise
{
	public class Translate : ModuleBase
	{
		private double m_x = 1.0;

		private double m_y = 1.0;

		private double m_z = 1.0;

		public double X
		{
			get
			{
				return this.m_x;
			}
			set
			{
				this.m_x = value;
			}
		}

		public double Y
		{
			get
			{
				return this.m_y;
			}
			set
			{
				this.m_y = value;
			}
		}

		public double Z
		{
			get
			{
				return this.m_z;
			}
			set
			{
				this.m_z = value;
			}
		}

		public Translate() : base(1)
		{
		}

		public Translate(ModuleBase input) : base(1)
		{
			base.modules[0] = input;
		}

		public Translate(double x, double y, double z, ModuleBase input) : base(1)
		{
			base.modules[0] = input;
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		public override double GetValue(double x, double y, double z)
		{
			Debug.Assert(base.modules[0] != null);
			return base.modules[0].GetValue(x + this.m_x, y + this.m_y, z + this.m_z);
		}
	}
}
