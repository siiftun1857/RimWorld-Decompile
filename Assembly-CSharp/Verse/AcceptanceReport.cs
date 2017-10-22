namespace Verse
{
	public struct AcceptanceReport
	{
		private string reasonTextInt;

		private bool acceptedInt;

		public string Reason
		{
			get
			{
				return this.reasonTextInt;
			}
		}

		public bool Accepted
		{
			get
			{
				return this.acceptedInt;
			}
		}

		public static AcceptanceReport WasAccepted
		{
			get
			{
				return new AcceptanceReport("")
				{
					acceptedInt = true
				};
			}
		}

		public static AcceptanceReport WasRejected
		{
			get
			{
				return new AcceptanceReport("")
				{
					acceptedInt = false
				};
			}
		}

		public AcceptanceReport(string reasonText)
		{
			this.acceptedInt = false;
			this.reasonTextInt = reasonText;
		}

		public static implicit operator AcceptanceReport(bool value)
		{
			return (!value) ? AcceptanceReport.WasRejected : AcceptanceReport.WasAccepted;
		}

		public static implicit operator AcceptanceReport(string value)
		{
			return new AcceptanceReport(value);
		}
	}
}
