using BepInEx.Configuration;

namespace PBnJamming.Configs
{
	public class FailureTypeConfig
	{
		public FailureMaskConfig Fallback { get; }
		public FailureMaskConfig Weight { get; }

		public FailureTypeConfig(string section, ConfigFile config, FailureLeafMask defaults)
		{
			Fallback = new FailureMaskConfig(section + "." + nameof(Fallback), config, defaults.Fallback);
			Weight = new FailureMaskConfig(section + "." + nameof(Weight), config, defaults.Weight);
		}
	}
}
