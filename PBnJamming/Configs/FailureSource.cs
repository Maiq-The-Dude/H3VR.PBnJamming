using System;
using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class FailureSourceConfig : IDisposable
	{
		public FailureMaskConfig Multiplier { get; }
		public FailureMaskConfig Fallback { get; }

		public FailureSourceConfig(string section, ConfigFile config, FailureMask fallback)
		{
			Multiplier = new FailureMaskConfig(section + "." + nameof(Multiplier), config, FailureMask.Unit);
			Fallback = new FailureMaskConfig(section + "." + nameof(Multiplier), config, fallback);
		}

		public void Dispose()
		{
			Multiplier?.Dispose();
			Fallback?.Dispose();
		}
	}
}
