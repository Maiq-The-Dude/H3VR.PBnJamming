using System;
using BepInEx.Configuration;

namespace PBnJamming.Configs
{
	public class FailureSourcesConfig : IDisposable
	{
		private static class Defaults
		{
			private const int NUM_ROUNDS = 1200;
			private const int NUM_FAIL_ROLL_PER_CYCLE = 4;
			private const int AVG_FAIL = NUM_ROUNDS * NUM_FAIL_ROLL_PER_CYCLE;

			// Mechanicals have a lot to do with every failure.
			public static readonly FailureMask Action = new FailureMask(3, 2, 6, 4, 2) / AVG_FAIL;
			// Each era is too different.
			public static readonly FailureMask Era = default;
			// Unique to each firearm.
			public static readonly FailureMask Firearm = default;
			// Magazines are the thing that feeds, so it makes sense to fail here.
			public static readonly FailureMask Magazine = new FailureMask(feed: 7) / AVG_FAIL;
			// Round type is very insignificant, but could have a small amount of impact.
			public static readonly FailureMask RoundClass = new FailureMask(fire: 3) / AVG_FAIL;
			// Unique to each round size.
			public static readonly FailureMask RoundType = default;
		}

		public FailureSourceConfig Action { get; }
		public FailureSourceConfig Era { get; }
		public FailureSourceConfig Firearm { get; }
		public FailureSourceConfig Magazine { get; }
		public FailureSourceConfig RoundClass { get; }
		public FailureSourceConfig RoundType { get; }

		public FailureSourcesConfig(string section, ConfigFile config)
		{
			Action = new FailureSourceConfig(section + "." + nameof(Action), config, Defaults.Action);
			Era = new FailureSourceConfig(section + "." + nameof(Era), config, Defaults.Era);
			Firearm = new FailureSourceConfig(section + "." + nameof(Firearm), config, Defaults.Firearm);
			Magazine = new FailureSourceConfig(section + "." + nameof(Magazine), config, Defaults.Magazine);
			RoundClass = new FailureSourceConfig(section + "." + nameof(RoundClass), config, Defaults.RoundClass);
			RoundType = new FailureSourceConfig(section + "." + nameof(RoundType), config, Defaults.RoundType);
		}

		public void Dispose()
		{
			Action?.Dispose();
			Era?.Dispose();
			Firearm?.Dispose();
			Magazine?.Dispose();
			RoundClass?.Dispose();
			RoundType?.Dispose();
		}

		public FailureSourceConfig this[FailureSource type] =>
			type switch
			{
				FailureSource.Action => Action,
				FailureSource.Era => Era,
				FailureSource.Firearm => Firearm,
				FailureSource.Magazine => Magazine,
				FailureSource.RoundClass => RoundClass,
				FailureSource.RoundType => RoundType,
				_ => throw new ArgumentOutOfRangeException()
			};
	}
}
