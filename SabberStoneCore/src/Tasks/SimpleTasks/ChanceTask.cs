using SabberStoneCore.Model;

namespace SabberStoneCore.Tasks.SimpleTasks
{
	public class ChanceTask : SimpleTask
	{
		public ChanceTask(bool useFlag = false)
		{
			UseFlag = useFlag;
		}

		public bool UseFlag { get; set; }

		public override TaskState Process()
		{
			int random = Game.RandomController.CoinFlip(Source, Target);

			if (!UseFlag)
			{
				return random == 0 ? TaskState.COMPLETE : TaskState.STOP;
			}

			Flag = random != 0;
			return TaskState.COMPLETE;
		}

		public override ISimpleTask Clone()
		{
			var clone = new ChanceTask(UseFlag);
			clone.Copy(this);
			return clone;
		}
	}
}
