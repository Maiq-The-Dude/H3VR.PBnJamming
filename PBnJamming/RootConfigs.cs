using BepInEx.Configuration;

namespace PBnJamming
{
	class RootConfigs
	{
		public ConfigEntry<float> Multiplier { get; }
		public ConfigEntry<bool> EnableLogging { get; }

		public RootConfigs(ConfigFile config)
		{
			Multiplier = config.Bind("Settings", nameof(Multiplier), 1f, "Failure Rate Multiplier");
			EnableLogging = config.Bind("Settings", nameof(EnableLogging), false, "Enable Console Logging");
		}
	}
}
