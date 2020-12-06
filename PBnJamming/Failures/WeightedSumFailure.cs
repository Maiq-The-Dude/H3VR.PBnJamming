using FistVR;

namespace PBnJamming
{
	public class WeightedSumFailure : IFailure
	{
		private readonly IFailure _weight;
		private readonly IFailure _inner;

		public WeightedSumFailure(IFailure weight, IFailure inner)
		{
			_weight = weight;
			_inner = inner;
		}

		public FailureMask this[FVRFireArm weapon] => _weight[weapon] * (FailureMask.Always + _inner[weapon]);
	}
}