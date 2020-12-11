using System;
using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class FailureSourcesConfig : IDisposable
	{
		private static class Defaults
		{
			// Really shouldn't make generalizations about an attribute that's unique to every firearm...
			public static readonly FailureMask ID = default;
			// Nothing here that the bottom 3 don't cover.
			public static readonly FailureMask Era = default;
			// Magazines are the thing that feeds, so it makes sense to fail-by-default here.
			public static readonly FailureMask Magazine = new FailureMask(0, 0.05f, 0, 0, 0);
			// Mechanicals have a lot to do with every failure.
			public static readonly FailureMask Action = new FailureMask(0.005f, 0.025f, 0.05f, 0.01f, 0.005f);
			// Round type is very insignificant, but could have a small amount of impact.
			public static readonly FailureMask RoundType = new FailureMask(0.001f, 0, 0, 0, 0);
		}

		public FailureSourceConfig ID { get; }
		public FailureSourceConfig Era { get; }
		public FailureSourceConfig Magazine { get; }
		public FailureSourceConfig Action { get; }
		public FailureSourceConfig RoundType { get; }

		public FailureSourcesConfig(string section, ConfigFile config)
		{
			ID = new FailureSourceConfig(section + "." + nameof(ID), config, Defaults.ID);
			Era = new FailureSourceConfig(section + "." + nameof(Era), config, Defaults.Era);
			Magazine = new FailureSourceConfig(section + "." + nameof(Magazine), config, Defaults.Magazine);
			Action = new FailureSourceConfig(section + "." + nameof(Action), config, Defaults.Action);
			RoundType = new FailureSourceConfig(section + "." + nameof(RoundType), config, Defaults.RoundType);
		}

		public void Dispose()
		{
			ID?.Dispose();
			Era?.Dispose();
			Magazine?.Dispose();
			Action?.Dispose();
			RoundType?.Dispose();
		}

		public FailureSourceConfig this[FailureSource type] =>
			type switch
			{
				FailureSource.ID => ID,
				FailureSource.Era => Era,
				FailureSource.Magazine => Magazine,
				FailureSource.Action => Action,
				FailureSource.RoundType => RoundType,
				_ => throw new ArgumentOutOfRangeException()
			};
	}
}
