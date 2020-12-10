using System.Collections.Generic;
using ADepIn;
using FistVR;

namespace PBnJamming
{
	public class AverageFailure : IFailure
	{
		private readonly IEnumerable<IFailure> _failures;

		public AverageFailure(IEnumerable<IFailure> failures)
		{
			_failures = failures;
		}

		public Option<FailureMask> this[FVRFireArm weapon]
		{
			get
			{
				FailureMask result = default;
				int count = 0;
				foreach (var mask in _failures.WhereSelect(f => f[weapon]))
				{
					result += mask;
					++count;
				}

				return count > 0 ? Option.Some(result /= count) : Option.None<FailureMask>();
			}
		}
	}
}
