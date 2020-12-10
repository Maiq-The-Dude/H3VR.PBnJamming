using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class FailureTypeConfig
	{
		public FailureMaskConfig Multiplier { get; }
		public FailureMaskConfig Fallback { get; }

		public FailureTypeConfig(string section, ConfigFile config, FailureMask fallback)
		{
			Multiplier = new FailureMaskConfig(section + "." + nameof(Multiplier), config, FailureMask.Unit);
			Fallback = new FailureMaskConfig(section + "." + nameof(Multiplier), config, fallback);
		}
	}
}
