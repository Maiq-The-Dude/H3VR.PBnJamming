using Valve.Newtonsoft.Json;

namespace PBnJamming
{
	public readonly struct FailureMask
	{
		public static FailureMask Never { get; } = new FailureMask(-1, -1, -1, -1);
		public static FailureMask Always { get; } = new FailureMask(1, 1, 1, 1);

		public readonly float Fire;
		public readonly float Feed;
		public readonly float Extract;
		public readonly float Lock;

		[JsonConstructor]
		public FailureMask(float fire, float feed, float extract, float lockOpen)
		{
			Fire = fire;
			Feed = feed;
			Extract = extract;
			Lock = lockOpen;
		}

		public static FailureMask operator +(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire + b.Fire,
				a.Feed + b.Feed,
				a.Extract + b.Extract,
				a.Lock + b.Lock
			);
		}

		public static FailureMask operator *(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire * b.Fire,
				a.Feed * b.Feed,
				a.Extract * b.Extract,
				a.Lock * b.Lock
			);
		}
	}
}
