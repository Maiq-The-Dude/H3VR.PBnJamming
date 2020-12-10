using ADepIn;
using FistVR;
using System.Collections.Generic;

namespace PBnJamming.Failures
{
	public class DictFailure<TKey> : IFailure
	{
		private readonly Dictionary<TKey, FailureMask> _config;
		private readonly Mapper<FVRFireArm, Option<TKey>> _keyFromGun;

		public DictFailure(Dictionary<TKey, FailureMask> config, Mapper<FVRFireArm, Option<TKey>> keyFromGun)
		{
			_config = config;
			_keyFromGun = keyFromGun;
		}

		public Option<FailureMask> this[FVRFireArm gun] => _keyFromGun(gun).Map(_config.OptionGetValue).Flatten();
	}
}
