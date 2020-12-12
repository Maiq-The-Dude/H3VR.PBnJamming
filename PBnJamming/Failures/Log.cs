using ADepIn;
using BepInEx.Configuration;
using BepInEx.Logging;
using FistVR;

namespace PBnJamming.Failures
{
	public class LogFailure : IFailure
	{
		private readonly IFailure _inner;
		private readonly ManualLogSource _log;
		private readonly ConfigEntry<bool> _config;
		private readonly string _name;

		public LogFailure(IFailure inner, ManualLogSource log, ConfigEntry<bool> config, string name)
		{
			_inner = inner;
			_log = log;
			_config = config;
			_name = name;
		}

		public Option<FailureMask> this[FVRFireArmChamber chamber]
		{
			get
			{
				var result = _inner[chamber];
				if (_config.Value)
				{
					_log.LogDebug(_name + ": " + result);
				}

				return result;
			}
		}
	}
}
