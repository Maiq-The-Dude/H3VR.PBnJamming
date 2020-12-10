using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADepIn;
using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class FailureTypesConfig
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

		public FailureTypeConfig ID { get; }
		public FailureTypeConfig Era { get; }
		public FailureTypeConfig Magazine { get; }
		public FailureTypeConfig Action { get; }
		public FailureTypeConfig RoundType { get; }

		public FailureTypesConfig(string section, ConfigFile config)
		{
			ID = new FailureTypeConfig(section + "." + nameof(ID), config, Defaults.ID);
			Era = new FailureTypeConfig(section + "." + nameof(Era), config, Defaults.Era);
			Magazine = new FailureTypeConfig(section + "." + nameof(Magazine), config, Defaults.Magazine);
			Action = new FailureTypeConfig(section + "." + nameof(Action), config, Defaults.Action);
			RoundType = new FailureTypeConfig(section + "." + nameof(RoundType), config, Defaults.RoundType);
		}
	}
}
