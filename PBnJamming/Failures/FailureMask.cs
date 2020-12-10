using Valve.Newtonsoft.Json;

namespace PBnJamming
{
	public readonly struct FailureMask
	{
		public readonly float Fire;
		public readonly float Feed;
		public readonly float Extract;
		public readonly float LockOpen;
		public readonly float Discharge;

		[JsonConstructor]
		public FailureMask(float fire, float feed, float extract, float lockOpen, float discharge)
		{
			Fire = fire;
			Feed = feed;
			Extract = extract;
			LockOpen = lockOpen;
			Discharge = discharge;
		}

		public static FailureMask operator +(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire + b.Fire,
				a.Feed + b.Feed,
				a.Extract + b.Extract,
				a.LockOpen + b.LockOpen,
				a.Discharge + b.Discharge
			);
		}

		public static FailureMask operator -(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire - b.Fire,
				a.Feed - b.Feed,
				a.Extract - b.Extract,
				a.LockOpen - b.LockOpen,
				a.Discharge - b.Discharge
			);
		}

		public static FailureMask operator *(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire * b.Fire,
				a.Feed * b.Feed,
				a.Extract * b.Extract,
				a.LockOpen * b.LockOpen,
				a.Discharge * b.Discharge
			);
		}

		public static FailureMask operator /(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire / b.Fire,
				a.Feed / b.Feed,
				a.Extract / b.Extract,
				a.LockOpen / b.LockOpen,
				a.Discharge / b.Discharge
			);
		}

		public static FailureMask operator -(FailureMask a)
		{
			return new FailureMask(
				-a.Fire,
				-a.Feed,
				-a.Extract,
				-a.LockOpen,
				-a.Discharge
			);
		}

		public static FailureMask operator *(FailureMask a, float b)
		{
			return new FailureMask(
				a.Fire * b,
				a.Feed * b,
				a.Extract * b,
				a.LockOpen * b,
				a.Discharge * b
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
