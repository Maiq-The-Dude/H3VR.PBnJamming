using Valve.Newtonsoft.Json;

namespace PBnJamming
{
	public readonly struct FailureMask
	{
		public static FailureMask Never { get; } = new FailureMask(-1, -1, -1);
		public static FailureMask Always { get; } = new FailureMask(1, 1, 1);

		public readonly float Fire;
		public readonly float Feed;
		public readonly float Eject;

		[JsonConstructor]
		public FailureMask(float fire, float feed, float eject)
		{
			Fire = fire;
			Feed = feed;
			Eject = eject;
		}

		public static FailureMask operator +(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire + b.Fire,
				a.Feed + b.Feed,
				a.Eject + b.Eject
			);
		}

		public static FailureMask operator *(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire * b.Fire,
				a.Feed * b.Feed,
				a.Eject * b.Eject
			);
		}
	}
}
