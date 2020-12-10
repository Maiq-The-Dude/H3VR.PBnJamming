using System;
using ADepIn;
using FistVR;

namespace PBnJamming
{
	public class MultiplicativeFailure : IFailure
	{
		private readonly IFailure _failure;
		private readonly Func<FailureMask> _weight;

		public MultiplicativeFailure(IFailure failure, Func<FailureMask> weight)
		{
			_failure = failure;
			_weight = weight;
		}

		public Option<FailureMask> this[FVRFireArm gun] => _failure[gun].Map(v => v * _weight());
	}
}
