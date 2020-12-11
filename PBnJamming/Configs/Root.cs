using System;
using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class RootConfig : IDisposable
	{
		public ConfigEntry<bool> EnableLogging { get; }

		public FailureMaskConfig GlobalMultiplier { get; }
		public FailureSourcesConfig Failures { get; }

		public RootConfig(ConfigFile config)
		{
			EnableLogging = config.Bind("General Settings", nameof(EnableLogging), false, "Enable Console Logging");

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
