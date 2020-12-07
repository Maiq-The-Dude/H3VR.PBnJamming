using System;
using System.Collections.Generic;
using ADepIn;
using Deli;

namespace PBnJamming
{
	public class DictLoader<TKey> : IAssetLoader
	{
		private readonly Dictionary<TKey, FailureMask> _allFailures;

		public DictLoader(Dictionary<TKey, FailureMask> allFailures)
		{
			_allFailures = allFailures;
		}

		public void LoadAsset(IServiceKernel kernel, Mod mod, string path)
		{
			var failures = mod.Resources.Get<Option<Dictionary<TKey, FailureMask>>>(path).Expect("Failure dictionary not found: " + path).Expect("Invalid JSON in failure dictionary.");

			foreach (var failure in failures)
			{
				if (_allFailures.ContainsKey(failure.Key))
				{
					throw new InvalidOperationException("A dictionary entry was already present for " + failure.Key);
				}

				_allFailures.Add(failure.Key, failure.Value);
			}
		}
	}
}
