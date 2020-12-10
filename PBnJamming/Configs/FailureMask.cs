using System;
using ADepIn;
using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class FailureMaskConfig : IDisposable
	{
		public ConfigEntry<float> Fire { get; }
		public ConfigEntry<float> Feed { get; }
		public ConfigEntry<float> Extract { get; }
		public ConfigEntry<float> LockOpen { get; }
		public ConfigEntry<float> Discharge { get; }

		private Option<FailureMask> _mask;
		public FailureMask Mask => _mask.GetOrInsertWith(Recalculate);

		public event Action Updated;

		public FailureMaskConfig(string section, ConfigFile config, FailureMask @default)
		{
			const string prefix = "The default rate at which a firearm should ";
			const string suffix = ".";

			Fire = config.Bind(section, nameof(Fire), @default.Fire, prefix + "fail to fire a chambered round" + suffix);
			Feed = config.Bind(section, nameof(Feed), @default.Feed, prefix + "fail to feed a round into the chamber" + suffix);
			Extract = config.Bind(section, nameof(Extract), @default.Extract, prefix + "fail to extract a round from the chamber" + suffix);
			LockOpen = config.Bind(section, nameof(LockOpen), @default.LockOpen, prefix + "lock the bolt open" + suffix);
			Discharge = config.Bind(section, nameof(Discharge), @default.Discharge, prefix + "accidential discharge" + suffix);

			Fire.SettingChanged += SettingRecalculate;
			Feed.SettingChanged += SettingRecalculate;
			Extract.SettingChanged += SettingRecalculate;
			LockOpen.SettingChanged += SettingRecalculate;
			Discharge.SettingChanged += SettingRecalculate;
		}

		public void Dispose()
		{
			Fire.SettingChanged -= SettingRecalculate;
			Feed.SettingChanged -= SettingRecalculate;
			Extract.SettingChanged -= SettingRecalculate;
			LockOpen.SettingChanged -= SettingRecalculate;
			Discharge.SettingChanged -= SettingRecalculate;
		}

		private void SettingRecalculate(object sender, EventArgs e)
		{
			_mask.Replace(Recalculate());

			Updated?.Invoke();
		}

		private FailureMask Recalculate()
		{
			return new FailureMask(Fire.Value, Feed.Value, Extract.Value, LockOpen.Value, Discharge.Value);
		}
	}
}
