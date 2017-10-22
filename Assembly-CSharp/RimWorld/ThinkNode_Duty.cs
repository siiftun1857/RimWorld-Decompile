using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace RimWorld
{
	public class ThinkNode_Duty : ThinkNode
	{
		public override ThinkResult TryIssueJobPackage(Pawn pawn, JobIssueParams jobParams)
		{
			ThinkResult result;
			if (pawn.GetLord() == null)
			{
				Log.Error(pawn + " doing ThinkNode_Duty with no Lord.");
				result = ThinkResult.NoJob;
			}
			else if (pawn.mindState.duty == null)
			{
				Log.Error(pawn + " doing ThinkNode_Duty with no duty.");
				result = ThinkResult.NoJob;
			}
			else
			{
				result = base.subNodes[pawn.mindState.duty.def.index].TryIssueJobPackage(pawn, jobParams);
			}
			return result;
		}

		protected override void ResolveSubnodes()
		{
			foreach (DutyDef allDef in DefDatabase<DutyDef>.AllDefs)
			{
				allDef.thinkNode.ResolveSubnodesAndRecur();
				base.subNodes.Add(allDef.thinkNode.DeepCopy(true));
			}
		}
	}
}
