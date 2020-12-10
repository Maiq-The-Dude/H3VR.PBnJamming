using System;
using ADepIn;
using FistVR;

namespace PBnJamming
{
	public class FallbackFailure : IFailure
	{
		private readonly IFailure _inner;
		private readonly Func<FailureMask> _fallback;

		public FallbackFailure(IFailure inner, Func<FailureMask> fallback)
		{
			_inner = inner;
			_fallback = fallback;
		}

		public Option<FailureMask> this[FVRFireArm gun] => Option.Some(_inner[gun].UnwrapOrElse(_fallback));
	}
}
