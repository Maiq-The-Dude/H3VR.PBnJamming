using ADepIn;
using Deli;
using DeliFramework = Deli.Deli;
using FistVR;
using System.Collections.Generic;
using System.Linq;
using PBnJamming.Configs;
using PBnJamming.Failures;

namespace PBnJamming
{
	internal class Plugin : DeliBehaviour
	{
		private static Mapper<FVRFireArm, Option<TKey>> WrapperMapper<TKey>(Mapper<FVRObject, Option<TKey>> keyFromObject)
		{
			return v =>
			{
				var wrapper = v.ObjectWrapper;
				return wrapper == null ? Option.None<TKey>() : keyFromObject(wrapper);
			};
		}

		private readonly RootConfig _config;
		private readonly Patches _patches;

		public Plugin()
		{
			_config = new RootConfig(Config);

			IFailure tree;
			tree = new SumFailure(CreateFailureLeafs().ToArray());
			tree = new MultiplicativeFailure(tree, () => _config.GlobalMultiplier.Mask);

			_patches = new Patches(Logger, tree, _config);
		}

		private void OnDestroy()
		{
			_config?.Dispose();
			_patches?.Dispose();
		}

		private IEnumerable<IFailure> CreateFailureLeafs()
		{
			yield return CreateFailureLeaf("action", FailureSource.Action, WrapperMapper(v => Option.Some(v.TagFirearmAction)));
			yield return CreateFailureLeaf("era", FailureSource.Era, WrapperMapper(v => Option.Some(v.TagEra)));
			yield return CreateFailureLeaf("id", FailureSource.ID, WrapperMapper(v => Option.Some(v.ItemID)));
			yield return CreateFailureLeaf("magazine", FailureSource.Magazine, g =>
			{
				var mag = g.Magazine;
				if (mag == null)
				{
					return Option.None<string>();
				}

				var wrapper = mag.IsIntegrated ? mag.FireArm.ObjectWrapper : mag.ObjectWrapper;
				if (wrapper == null)
				{
					return Option.None<string>();
				}

				return Option.Some(wrapper.ItemID);
			});
			yield return CreateFailureLeaf("pbnj.roundtype", FailureSource.RoundType, g => Option.Some(g.RoundType));
		}

		private IFailure CreateFailureLeaf<TKey>(string name, FailureSource source, Mapper<FVRFireArm, Option<TKey>> keyFromGun)
		{
			// Add to Deli loaders
			if (Module.Kernel.Get<IAssetReader<Option<Dictionary<TKey, FailureMask>>>>().IsNone)
			{
				Module.Kernel.BindJson<Dictionary<TKey, FailureMask>>();
			}

			var dict = new Dictionary<TKey, FailureMask>();
			var loader = new DictLoader<TKey>(dict);
			DeliFramework.AddAssetLoader("pbnj." + name, loader);

			// Create failure subtree
			var sourceConfig = _config.Failures[source];

			IFailure failure;
			failure = new DictFailure<TKey>(dict, keyFromGun);
			failure = new FallbackFailure(failure, () =>
			{
				var mask = sourceConfig.Fallback.Mask;
				return mask == default ? Option.None<FailureMask>() : Option.Some(mask);
			});
			failure = new MultiplicativeFailure(failure, () => sourceConfig.Multiplier.Mask);

			return failure;
		}
	}
}
