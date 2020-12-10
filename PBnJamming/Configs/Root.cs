using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class RootConfig
	{
		public ConfigEntry<bool> EnableLogging { get; }

		public FailureMaskConfig Multiplier { get; }
		public FailureTypesConfig Failures { get; }

		public RootConfig(ConfigFile config)
		{
			EnableLogging = config.Bind("Settings", nameof(EnableLogging), false, "Enable Console Logging");

			Multiplier = new FailureMaskConfig(nameof(Multiplier), config, FailureMask.Unit);
			Failures = new FailureTypesConfig(nameof(Failures), config);
		}
	}
}
