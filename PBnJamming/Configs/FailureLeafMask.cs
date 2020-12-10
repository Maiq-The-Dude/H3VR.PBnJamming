using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public readonly struct FailureLeafMask
	{
		public FailureMask Weight { get; }
		public FailureMask Fallback { get; }

		public FailureLeafMask(FailureMask weight, FailureMask fallback)
		{
			Weight = weight;
			Fallback = fallback;
		}
	}
}
