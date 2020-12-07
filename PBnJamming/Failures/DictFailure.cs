using ADepIn;
using Deli;
using FistVR;
using System;
using System.Collections.Generic;

namespace PBnJamming
{
	public class DictFailure<TKey> : IFailure
	{
		private readonly Dictionary<TKey, FailureMask> _allFailures;
		private readonly Mapper<FVRFireArm, TKey> _keyFromGun;

		public DictFailure(Dictionary<TKey, FailureMask> allFailures, Mapper<FVRFireArm, TKey> keyFromGun)
		{
			_allFailures = allFailures;
			_keyFromGun = keyFromGun;
		}

		public FailureMask this[FVRFireArm gun] => _allFailures.OptionGetValue(_keyFromGun(gun)).UnwrapOr(default);
	}
}
