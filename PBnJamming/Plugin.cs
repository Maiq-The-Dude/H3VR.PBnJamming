using ADepIn;
using Deli;
using DeliFramework = Deli.Deli;
using FistVR;
using System.Collections.Generic;
using System.Linq;
using PBnJamming.Configs;
using PBnJamming.Failures;
using UnityEngine.SceneManagement;

namespace PBnJamming
{
	internal class Plugin : DeliBehaviour
	{
		private static Mapper<FVRFireArmChamber, Option<TKey>> WrapperMapper<TKey>(Mapper<FVRObject, Option<TKey>> keyFromObject)
		{
			return v =>
			{
				var wrapper = v.Firearm.ObjectWrapper;
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

			_patches = new Patches(Logger, tree, _config.Log.Fires);
			
			SceneManager.activeSceneChanged += SceneChanged;
		}

		private void OnDestroy()
		{
			SceneManager.activeSceneChanged -= SceneChanged;

			_config?.Dispose();
			_patches?.Dispose();
		}

		private void SceneChanged(Scene current, Scene next)
		{
			Config.Reload();
		}

		private IEnumerable<IFailure> CreateFailureLeafs()
		{
			yield return CreateFailureLeaf("action", FailureSource.Action, WrapperMapper(v => Option.Some(v.TagFirearmAction)));
			yield return CreateFailureLeaf("era", FailureSource.Era, WrapperMapper(v => Option.Some(v.TagEra)));
			yield return CreateFailureLeaf("firearm", FailureSource.Firearm, WrapperMapper(v => Option.Some(v.ItemID)));
			yield return CreateFailureLeaf("magazine", FailureSource.Magazine, c =>
			{
				var mag = c.Firearm.Magazine;
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
			yield return CreateFailureLeaf("round.class", FailureSource.RoundClass, c =>
			{
				var round = c.m_round;
				if (round == null)
				{
					return Option.None<FireArmRoundClass>();
				}
				return Option.Some(c.m_round.RoundClass);
			}); 
			yield return CreateFailureLeaf("round.type", FailureSource.RoundType, c =>
			{
				var round = c.m_round;
				if (round == null)
				{
					return Option.None<FireArmRoundType>();
				}
				return Option.Some(c.RoundType);
			}); 
		}

		private IFailure CreateFailureLeaf<TKey>(string name, FailureSource source, Mapper<FVRFireArmChamber, Option<TKey>> keyFromChamber)
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
			failure = new DictionaryFailure<TKey>(dict, keyFromChamber);
			failure = new LogFailure(failure, Logger, _config.Log.Sources, name);
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
