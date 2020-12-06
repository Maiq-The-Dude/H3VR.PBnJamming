using ADepIn;
using Deli;
using FistVR;
using System.Collections.Generic;
using DeliFramework = Deli.Deli;

namespace PBnJamming
{
    internal class Plugin : DeliBehaviour
    {
        public static Plugin Instance { get; private set; }

        public IFailure Failure { get; }

        public Plugin()
        {
            Instance = this;

            Failure = AddFailure("pbnj.action", g => g.ObjectWrapper.TagFirearmAction)
                .AddFailure("pbnj.id", g => g.ObjectWrapper.ItemID);
        }

        public static IFailure AddFailure<TKey>(string name, Mapper<FVRFireArm, TKey> keyFromGun)
        {
            if (Module.Kernel.Get<IAssetReader<Option<Dictionary<TKey, FailureMask>>>>().IsNone)
            {
                Module.Kernel.BindJson<Dictionary<TKey, FailureMask>>();
            }

            var dict = new Dictionary<TKey, FailureMask>();
            var failure = new DictFailure<TKey>(dict, keyFromGun);
            var loader = new DictLoader<TKey>(dict);

            DeliFramework.AddAssetLoader(name, loader);

            return failure;
        }
    }

    internal static class ExtPlugin
    {
        public static IFailure AddFailure<TKey>(this IFailure inner, string name, Mapper<FVRFireArm, TKey> keyFromGun)
        {
            var weighted = Plugin.AddFailure<TKey>(name, keyFromGun);
            return new WeightedSumFailure(weighted, inner);
        }
    }
}