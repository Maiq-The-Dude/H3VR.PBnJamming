using System;
using ADepIn;
using FistVR;

namespace PBnJamming.Failures
{
	public class FallbackFailure : IFailure
	{
		private readonly IFailure _inner;
		private readonly Func<Option<FailureMask>> _fallback;

		public FallbackFailure(IFailure inner, Func<Option<FailureMask>> fallback)
		{
			_inner = inner;
			_fallback = fallback;
		}

		public Option<FailureMask> this[FVRFireArm gun] => _inner[gun].Or(_fallback());
	}
}
