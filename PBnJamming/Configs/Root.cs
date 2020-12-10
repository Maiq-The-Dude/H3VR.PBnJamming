using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class RootConfig
	{
		public ConfigEntry<bool> EnableLogging { get; }

		public FailureMaskConfig GlobalMultiplier { get; }
		public FailureTypesConfig Failures { get; }

		public RootConfig(ConfigFile config)
		{
			EnableLogging = config.Bind("General Settings", nameof(EnableLogging), false, "Enable Console Logging");

			GlobalMultiplier = new FailureMaskConfig(nameof(GlobalMultiplier), config, FailureMask.Unit);
			Failures = new FailureTypesConfig(nameof(Failures), config);
		}
	}
}
