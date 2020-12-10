using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ADepIn;
using BepInEx.Configuration;
using PBnJamming.Failures;

namespace PBnJamming.Configs
{
	public class FailureTypesConfig : IEnumerable<FailureTypeConfig>, IDisposable
	{
		private static class Defaults
		{
			public static readonly FailureLeafMask ID = new FailureLeafMask(FailureMask.Unit, default);
			public static readonly FailureLeafMask Era = new FailureLeafMask(FailureMask.Unit * 0.3f, default);
			public static readonly FailureLeafMask Magazine = new FailureLeafMask(FailureMask.Unit * 0.7f, new FailureMask(0, 0.05f, 0.025f, 0, 0));
			public static readonly FailureLeafMask Action = new FailureLeafMask(FailureMask.Unit * 0.4f, default);
			public static readonly FailureLeafMask RoundType = new FailureLeafMask(FailureMask.Unit * 0.2f, default);
		}

		private Option<FailureMask> _totalWeight;

		public FailureTypeConfig ID { get; }
		public FailureTypeConfig Era { get; }
		public FailureTypeConfig Magazine { get; }
		public FailureTypeConfig Action { get; }
		public FailureTypeConfig RoundType { get; }

		public FailureMask TotalWeight => _totalWeight.GetOrInsertWith(RecalculateTotalWeight);

		public event Action UpdatedTotalWeight;

		public FailureTypesConfig(string section, ConfigFile config)
		{
			ID = new FailureTypeConfig(section + "." + nameof(ID), config, Defaults.ID);
			Era = new FailureTypeConfig(section + "." + nameof(Era), config, Defaults.Era);
			Magazine = new FailureTypeConfig(section + "." + nameof(Magazine), config, Defaults.Magazine);
			Action = new FailureTypeConfig(section + "." + nameof(Action), config, Defaults.Action);
			RoundType = new FailureTypeConfig(section + "." + nameof(RoundType), config, Defaults.RoundType);

			ID.Weight.Updated += SettingRecalculateTotalWeight;
			Era.Weight.Updated += SettingRecalculateTotalWeight;
			Magazine.Weight.Updated += SettingRecalculateTotalWeight;
			Action.Weight.Updated += SettingRecalculateTotalWeight;
			RoundType.Weight.Updated += SettingRecalculateTotalWeight;
		}

		public void Dispose()
		{
			ID.Weight.Updated -= SettingRecalculateTotalWeight;
			Era.Weight.Updated -= SettingRecalculateTotalWeight;
			Magazine.Weight.Updated -= SettingRecalculateTotalWeight;
			Action.Weight.Updated -= SettingRecalculateTotalWeight;
			RoundType.Weight.Updated -= SettingRecalculateTotalWeight;
		}

		private FailureMask RecalculateTotalWeight()
		{
			return this.Aggregate(default(FailureMask), (p, c) => p + c.Weight.Mask);
		}

		private void SettingRecalculateTotalWeight()
		{
			_totalWeight.Replace(RecalculateTotalWeight());

			UpdatedTotalWeight?.Invoke();
		}

		public IEnumerator<FailureTypeConfig> GetEnumerator()
		{
			yield return ID;
			yield return Era;
			yield return Magazine;
			yield return Action;
			yield return RoundType;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
