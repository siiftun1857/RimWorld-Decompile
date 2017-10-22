using System.Xml;

namespace Verse
{
	public class PatchOperation
	{
		private enum Success
		{
			Normal = 0,
			Invert = 1,
			Always = 2,
			Never = 3
		}

		private bool neverSucceeded = true;

		private Success success = Success.Normal;

		public bool Apply(XmlDocument xml)
		{
			bool flag = this.ApplyWorker(xml);
			if (this.success == Success.Always)
			{
				flag = true;
			}
			else if (this.success == Success.Never)
			{
				flag = false;
			}
			else if (this.success == Success.Invert)
			{
				flag = !flag;
			}
			if (flag)
			{
				this.neverSucceeded = false;
			}
			return flag;
		}

		protected virtual bool ApplyWorker(XmlDocument xml)
		{
			Log.Error("Attempted to use PatchOperation directly; patch will always fail");
			return false;
		}

		public void Complete(string modIdentifier)
		{
			if (this.neverSucceeded)
			{
				Log.Error(string.Format("[{0}] Patch operation {1} failed", modIdentifier, this));
			}
		}
	}
}
