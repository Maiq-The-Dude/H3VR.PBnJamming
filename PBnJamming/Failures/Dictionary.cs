using ADepIn;
using FistVR;
using System.Collections.Generic;

namespace PBnJamming.Failures
{
	public class DictionaryFailure<TKey> : IFailure
	{
		private readonly Dictionary<TKey, FailureMask> _config;
		private readonly Mapper<FVRFireArmChamber, Option<TKey>> _keyFromChamber;

		public DictionaryFailure(Dictionary<TKey, FailureMask> config, Mapper<FVRFireArmChamber, Option<TKey>> keyFromChamber)
		{
			_config = config;
			_keyFromChamber = keyFromChamber;
		}

		public Option<FailureMask> this[FVRFireArmChamber chamber] => _keyFromChamber(chamber).Map(_config.OptionGetValue).Flatten();
	}
}
