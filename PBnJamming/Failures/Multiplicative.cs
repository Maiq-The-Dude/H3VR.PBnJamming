using System;
using ADepIn;
using FistVR;

namespace PBnJamming.Failures
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

		public Option<FailureMask> this[FVRFireArmChamber chamber] => _failure[chamber].Map(v => v * _weight());
	}
}
