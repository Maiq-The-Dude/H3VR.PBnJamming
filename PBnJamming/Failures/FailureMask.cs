using Valve.Newtonsoft.Json;

namespace PBnJamming
{
	public readonly struct FailureMask
	{
		public readonly float Fire;
		public readonly float Feed;
		public readonly float Extract;
		public readonly float LockOpen;
		public readonly float Slamfire;

		[JsonConstructor]
		public FailureMask(float fire, float feed, float extract, float lockOpen, float slamfire)
		{
			Fire = fire;
			Feed = feed;
			Extract = extract;
			LockOpen = lockOpen;
			Slamfire = slamfire;
		}

		public static FailureMask operator +(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire + b.Fire,
				a.Feed + b.Feed,
				a.Extract + b.Extract,
				a.LockOpen + b.LockOpen,
				a.Slamfire + b.Slamfire
			);
		}

		public static FailureMask operator -(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire - b.Fire,
				a.Feed - b.Feed,
				a.Extract - b.Extract,
				a.LockOpen - b.LockOpen,
				a.Slamfire - b.Slamfire
			);
		}

		public static FailureMask operator *(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire * b.Fire,
				a.Feed * b.Feed,
				a.Extract * b.Extract,
				a.LockOpen * b.LockOpen,
				a.Slamfire * b.Slamfire
			);
		}

		public static FailureMask operator /(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire / b.Fire,
				a.Feed / b.Feed,
				a.Extract / b.Extract,
				a.LockOpen / b.LockOpen,
				a.Slamfire / b.Slamfire
			);
		}

		public static FailureMask operator -(FailureMask a)
		{
			return new FailureMask(
				-a.Fire,
				-a.Feed,
				-a.Extract,
				-a.LockOpen,
				-a.Slamfire
			);
		}

		public static FailureMask operator *(FailureMask a, float b)
		{
			return new FailureMask(
				a.Fire * b,
				a.Feed * b,
				a.Extract * b,
				a.LockOpen * b,
				a.Slamfire * b
			);
		}

		public static FailureMask operator *(float a, FailureMask b)
		{
			return b * a;
		}

		public static FailureMask operator /(FailureMask a, float b)
		{
			return a * (1 / b);
		}

		public static FailureMask operator /(float a, FailureMask b)
		{
			return b / a;
		}
	}
}
