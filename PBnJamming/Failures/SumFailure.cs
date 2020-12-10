using System.Collections.Generic;
using ADepIn;
using FistVR;

namespace PBnJamming.Failures
{
	public class SumFailure : IFailure
	{
		private readonly IEnumerable<IFailure> _failures;

		public SumFailure(IEnumerable<IFailure> failures)
		{
			_failures = failures;
		}

		public Option<FailureMask> this[FVRFireArm gun]
		{
			get
			{
				// Yeah, LINQ, but think of the picoseconds
				var result = Option.None<FailureMask>();
				foreach (var failure in _failures)
				{
					var maskOpt = failure[gun];
					if (!maskOpt.MatchSome(out var mask))
					{
						continue;
					}

					var current = result.UnwrapOr(default);
					result.Replace(current + mask);
				}

				return result;
			}
		}
	}
}
