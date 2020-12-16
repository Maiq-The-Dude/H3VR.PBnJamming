using System;
using BepInEx.Configuration;

namespace PBnJamming.Configs
{
	public class RootConfig : IDisposable
	{
		public LogConfig Log { get; }
		public FailureMaskConfig GlobalMultiplier { get; }
		public FailureSourcesConfig Failures { get; }

		public RootConfig(ConfigFile config)
		{
			Log = new LogConfig(nameof(Log), config);
			GlobalMultiplier = new FailureMaskConfig(nameof(GlobalMultiplier), config, FailureMask.Unit);
			Failures = new FailureSourcesConfig(nameof(Failures), config);
		}

		public void Dispose()
		{
			GlobalMultiplier?.Dispose();
			Failures?.Dispose();
		}
	}
}
