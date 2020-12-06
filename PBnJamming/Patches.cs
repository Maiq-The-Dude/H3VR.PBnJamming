using ADepIn;
using FistVR;
using HarmonyLib;
using Random = UnityEngine.Random;

namespace PBnJamming
{
	[HarmonyPatch]
	internal static class Patches
	{
		private static IFailure Failure => Plugin.Instance.Failure;

		private static bool Failed(FVRFireArm gun, Mapper<FailureMask, float> type)
		{
			return Random.Range(0f, 1f) <= type(Failure[gun]);
		}

		[HarmonyPatch(typeof(FVRFireArmChamber), "Fire")]
		[HarmonyPrefix]
		private static bool Fire(ref bool __result, FVRFireArmChamber __instance)
		{
			if (Failed(__instance.Firearm, m => m.Fire))
			{
				__result = false;
				return false;
			}

			return true;
		}
	}
}