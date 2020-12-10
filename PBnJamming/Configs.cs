using BepInEx.Configuration;

namespace PBnJamming
{
	public class RootConfig
	{
		public ConfigEntry<bool> EnableLogging { get; }

		public FailureMaskConfig Multiplier { get; }
		public FailureLeafsConfig Weights { get; }
		public FailureLeafsConfig Fallbacks { get; }

		public RootConfig(ConfigFile config)
		{
			EnableLogging = config.Bind("Settings", nameof(EnableLogging), false, "Enable Console Logging");

			Multiplier = new FailureMaskConfig(nameof(Multiplier), config, 1);
			Weights = new FailureLeafsConfig(nameof(Weights), config, 1);
			Fallbacks = new FailureLeafsConfig(nameof(Fallbacks), config, 0);
		}
	}

	public class FailureLeafsConfig
	{
		public FailureMaskConfig ID { get; }
		public FailureMaskConfig Era { get; }
		public FailureMaskConfig Magazine { get; }
		public FailureMaskConfig Action { get; }
		public FailureMaskConfig RoundType { get; }

		// TODO: calculate this once and only update if configs are updated.
		public FailureMask GlobalMask => ID.Mask + Era.Mask + Magazine.Mask + Action.Mask + RoundType.Mask;

		public FailureLeafsConfig(string section, ConfigFile config, float @default)
		{
			ID = new FailureMaskConfig(section + "." + nameof(ID), config, @default);
			Era = new FailureMaskConfig(section + "." + nameof(Era), config, @default);
			Magazine = new FailureMaskConfig(section + "." + nameof(Magazine), config, @default);
			Action = new FailureMaskConfig(section + "." + nameof(Action), config, @default);
			RoundType = new FailureMaskConfig(section + "." + nameof(RoundType), config, @default);
		}
	}

	public class FailureMaskConfig
	{
		public ConfigEntry<float> Fire { get; }
		public ConfigEntry<float> Feed { get; }
		public ConfigEntry<float> Extract { get; }
		public ConfigEntry<float> LockOpen { get; }
		public ConfigEntry<float> Discharge { get; }

		// TODO: calculate this once and only update if configs are updated.
		public FailureMask Mask => new FailureMask(Fire.Value, Feed.Value, Extract.Value, LockOpen.Value, Discharge.Value);

		public FailureMaskConfig(string section, ConfigFile config, float @default)
		{
			const string prefix = "The default rate at which a firearm should ";
			const string suffix = ".";

			Fire = config.Bind(section, nameof(Fire), @default, prefix + "fail to fire a chambered round" + suffix);
			Feed = config.Bind(section, nameof(Feed), @default, prefix + "fail to feed a round into the chamber" + suffix);
			Extract = config.Bind(section, nameof(Extract), @default, prefix + "fail to extract a round from the chamber" + suffix);
			LockOpen = config.Bind(section, nameof(LockOpen), @default, prefix + "lock the bolt open" + suffix);
			Discharge = config.Bind(section, nameof(Discharge), @default, prefix + "accidential discharge" + suffix);
		}
	}
}
