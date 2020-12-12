using BepInEx.Configuration;

namespace PBnJamming.Configs
{
	public class LogConfig
	{
		public ConfigEntry<bool> Fires { get; }
		public ConfigEntry<bool> Sources { get; }

		public LogConfig(string section, ConfigFile config)
		{
			Fires = config.Bind(section, nameof(Fires), false, "Whether or not to log information after each firing call.");
			Sources = config.Bind(section, nameof(Sources), false, "Whether or not to log the result of each dictionary lookup.");
		}
	}
}
