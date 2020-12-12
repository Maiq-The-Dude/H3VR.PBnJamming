using System;
using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class FailureSourcesConfig : IDisposable
	{
		private static class Defaults
		{
			private const int AVG_COUNT = 8 * 30;

			// Mechanicals have a lot to do with every failure.
			public static readonly FailureMask Action = new FailureMask(3, 2, 6, 4, 2) / AVG_COUNT;
			// Each era is too different.
			public static readonly FailureMask Era = default;
			// Unique to each firearm.
			public static readonly FailureMask ID = default;
			// Magazines are the thing that feeds, so it makes sense to fail here.
			public static readonly FailureMask Magazine = new FailureMask(feed: 7) / AVG_COUNT;
			// Round type is very insignificant, but could have a small amount of impact.
			public static readonly FailureMask RoundClass = new FailureMask(fire: 3) / AVG_COUNT;
			// Unique to each round size.
			public static readonly FailureMask RoundType = default;
		}

		public FailureSourceConfig Action { get; }
		public FailureSourceConfig Era { get; }
		public FailureSourceConfig ID { get; }
		public FailureSourceConfig Magazine { get; }
		public FailureSourceConfig RoundClass { get; }
		public FailureSourceConfig RoundType { get; }

		public FailureSourcesConfig(string section, ConfigFile config)
		{
			Action = new FailureSourceConfig(section + "." + nameof(Action), config, Defaults.Action);
			Era = new FailureSourceConfig(section + "." + nameof(Era), config, Defaults.Era);
			ID = new FailureSourceConfig(section + "." + nameof(ID), config, Defaults.ID);
			Magazine = new FailureSourceConfig(section + "." + nameof(Magazine), config, Defaults.Magazine);
			RoundClass = new FailureSourceConfig(section + "." + nameof(RoundClass), config, Defaults.RoundClass);
			RoundType = new FailureSourceConfig(section + "." + nameof(RoundType), config, Defaults.RoundType);
		}

		public void Dispose()
		{
			Action?.Dispose();
			Era?.Dispose();
			ID?.Dispose();
			Magazine?.Dispose();
			RoundClass?.Dispose();
			RoundType?.Dispose();
		}

		public FailureSourceConfig this[FailureSource type] =>
			type switch
			{
				FailureSource.Action => Action,
				FailureSource.Era => Era,
				FailureSource.ID => ID,
				FailureSource.Magazine => Magazine,
				FailureSource.RoundClass => RoundClass,
				FailureSource.RoundType => RoundType,
				_ => throw new ArgumentOutOfRangeException()
			};
	}
}
