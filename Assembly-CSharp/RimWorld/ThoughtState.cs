namespace RimWorld
{
	public struct ThoughtState
	{
		private int stageIndex;

		private string reason;

		private const int InactiveIndex = -99999;

		public bool Active
		{
			get
			{
				return this.stageIndex != -99999;
			}
		}

		public int StageIndex
		{
			get
			{
				return this.stageIndex;
			}
		}

		public string Reason
		{
			get
			{
				return this.reason;
			}
		}

		public static ThoughtState ActiveDefault
		{
			get
			{
				return ThoughtState.ActiveAtStage(0);
			}
		}

		public static ThoughtState Inactive
		{
			get
			{
				return new ThoughtState
				{
					stageIndex = -99999
				};
			}
		}

		public static ThoughtState ActiveAtStage(int stageIndex)
		{
			return new ThoughtState
			{
				stageIndex = stageIndex
			};
		}

		public static ThoughtState ActiveAtStage(int stageIndex, string reason)
		{
			return new ThoughtState
			{
				stageIndex = stageIndex,
				reason = reason
			};
		}

		public static ThoughtState ActiveWithReason(string reason)
		{
			ThoughtState activeDefault = ThoughtState.ActiveDefault;
			activeDefault.reason = reason;
			return activeDefault;
		}

		public static implicit operator ThoughtState(bool value)
		{
			return (!value) ? ThoughtState.Inactive : ThoughtState.ActiveDefault;
		}
	}
}
