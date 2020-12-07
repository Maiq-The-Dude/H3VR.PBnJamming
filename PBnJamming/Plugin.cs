using ADepIn;
using BepInEx.Logging;
using Deli;
using DeliFramework = Deli.Deli;
using FistVR;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Text;

namespace PBnJamming
{
	internal class Plugin : DeliBehaviour
	{
		public IFailure Failure { get; }

		public RootConfigs Configs { get; }

		public Plugin()
		{
			Configs = new RootConfigs(Config);

			Failure = AddFailure("pbnj.magazine", g => g.Magazine.ObjectWrapper.ItemID)
				.AddFailure("pbnj.roundtype", g => g.RoundType)
				.AddFailure("pbnj.action", g => g.ObjectWrapper.TagFirearmAction)
				.AddFailure("pbnj.era", g => g.ObjectWrapper.TagEra)
				.AddFailure("pbnj.id", g => g.ObjectWrapper.ItemID);

			// Fire
			On.FistVR.ClosedBoltWeapon.Fire += ClosedBoltWeapon_Fire;
			On.FistVR.OpenBoltReceiver.Fire += OpenBoltReceiver_Fire;
			On.FistVR.Handgun.Fire += Handgun_Fire;

			// Feed
			On.FistVR.ClosedBoltWeapon.BeginChamberingRound += ClosedBoltWeapon_BeginChamberingRound;
			On.FistVR.OpenBoltReceiver.BeginChamberingRound += OpenBoltReceiver_BeginChamberingRound;
			On.FistVR.Handgun.ExtractRound += Handgun_ExtractRound;

			// Extract
			On.FistVR.ClosedBolt.ImpartFiringImpulse += ClosedBolt_ImpartFiringImpulse;
			On.FistVR.OpenBoltReceiverBolt.ImpartFiringImpulse += OpenBoltReceiverBolt_ImpartFiringImpulse;
			On.FistVR.HandgunSlide.ImpartFiringImpulse += HandgunSlide_ImpartFiringImpulse;

			// LockOpen
			On.FistVR.Handgun.EngageSlideRelease += Handgun_EngageSlideRelease;
			On.FistVR.ClosedBolt.LockBolt += ClosedBolt_LockBolt;
		}

		private void OnDestroy()
		{
			// Fire
			//On.FistVR.FVRFireArmChamber.Fire -= FVRFireArmChamber_Fire;
			On.FistVR.ClosedBoltWeapon.Fire -= ClosedBoltWeapon_Fire;
			On.FistVR.OpenBoltReceiver.Fire -= OpenBoltReceiver_Fire;
			On.FistVR.Handgun.Fire -= Handgun_Fire;

			// Feed
			On.FistVR.ClosedBoltWeapon.BeginChamberingRound -= ClosedBoltWeapon_BeginChamberingRound;
			On.FistVR.OpenBoltReceiver.BeginChamberingRound -= OpenBoltReceiver_BeginChamberingRound;
			On.FistVR.Handgun.ExtractRound -= Handgun_ExtractRound;

			// Extract
			On.FistVR.ClosedBolt.ImpartFiringImpulse -= ClosedBolt_ImpartFiringImpulse;
			On.FistVR.OpenBoltReceiverBolt.ImpartFiringImpulse -= OpenBoltReceiverBolt_ImpartFiringImpulse;
			On.FistVR.HandgunSlide.ImpartFiringImpulse -= HandgunSlide_ImpartFiringImpulse;

			// LockOpen
			On.FistVR.Handgun.EngageSlideRelease -= Handgun_EngageSlideRelease;
			On.FistVR.ClosedBolt.LockBolt -= ClosedBolt_LockBolt;
		}

		private bool Failed(FVRFireArm gun, Mapper<FailureMask, float> type, string failure)
		{
			var ran = Random.Range(0f, 1f);
			var chance = type(Failure[gun]) * Configs.Multiplier.Value;

			if (Configs.EnableLogging.Value)
			{
				var builder = new StringBuilder().AppendLine()
					.Append("ItemID: " + gun.ObjectWrapper.ItemID).AppendLine()
					.Append("Era: " + gun.ObjectWrapper.TagEra).AppendLine()
					.Append("Action: " + gun.ObjectWrapper.TagFirearmAction).AppendLine()
					.Append("Round: " + gun.RoundType).AppendLine()
					.Append("Magazine: " + gun.Magazine.ObjectWrapper.ItemID).AppendLine()
					.Append("Random: " + ran).AppendLine()
					.Append("Chance: " + chance).AppendLine()
					.Append("Failure: " + failure).AppendLine();

				Logger.LogDebug(builder);
			}

			return ran <= chance;
		}

		#region Fire	
		private bool ClosedBoltWeapon_Fire(On.FistVR.ClosedBoltWeapon.orig_Fire orig, ClosedBoltWeapon self)
		{
			var fail = "Fire";
			if (Failed(self, m => m.Fire, fail))
			{
				return false;
			}

			return orig(self);
		}

		private bool OpenBoltReceiver_Fire(On.FistVR.OpenBoltReceiver.orig_Fire orig, OpenBoltReceiver self)
		{
			var fail = "Fire";
			if (Failed(self, m => m.Fire, fail))
			{
				return false;
			}

			return orig(self);
		}

		private bool Handgun_Fire(On.FistVR.Handgun.orig_Fire orig, Handgun self)
		{
			var fail = "Fire";
			if (Failed(self, m => m.Fire, fail))
			{
				return false;
			}

			return orig(self);
		}

		#endregion

		#region Feed
		private void ClosedBoltWeapon_BeginChamberingRound(On.FistVR.ClosedBoltWeapon.orig_BeginChamberingRound orig, ClosedBoltWeapon self)
		{
			var fail = "Feed";
			if (Failed(self, m => m.Feed, fail))
			{
				return;
			}

			orig(self);
		}

		private void OpenBoltReceiver_BeginChamberingRound(On.FistVR.OpenBoltReceiver.orig_BeginChamberingRound orig, OpenBoltReceiver self)
		{
			var fail = "Feed";
			if (Failed(self, m => m.Feed, fail))
			{
				return;
			}

			orig(self);
		}

		private void Handgun_ExtractRound(On.FistVR.Handgun.orig_ExtractRound orig, Handgun self)
		{
			var fail = "Feed";
			if (Failed(self, m => m.Feed, fail))
			{
				return;
			}

			orig(self);
		}

		#endregion

		#region Extract
		private void ClosedBolt_ImpartFiringImpulse(On.FistVR.ClosedBolt.orig_ImpartFiringImpulse orig, ClosedBolt self)
		{
			var fail = "Extract";
			if (Failed(self.Weapon, m => m.Extract, fail))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		private void OpenBoltReceiverBolt_ImpartFiringImpulse(On.FistVR.OpenBoltReceiverBolt.orig_ImpartFiringImpulse orig, OpenBoltReceiverBolt self)
		{
			var fail = "Extract";
			if (Failed(self.Receiver, m => m.Extract, fail))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		private void HandgunSlide_ImpartFiringImpulse(On.FistVR.HandgunSlide.orig_ImpartFiringImpulse orig, HandgunSlide self)
		{
			var fail = "Extract";
			if (Failed(self.Handgun, m => m.Extract, fail))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		#endregion

		#region LockOpen
		private void Handgun_EngageSlideRelease(On.FistVR.Handgun.orig_EngageSlideRelease orig, Handgun self)
		{
			var fail = "Lock";
			if (Failed(self, m => m.Lock, fail))
			{
				return;
			}

			orig(self);
		}

		private void ClosedBolt_LockBolt(On.FistVR.ClosedBolt.orig_LockBolt orig, ClosedBolt self)
		{
			var fail = "Lock";
			if (Failed(self.Weapon, m => m.Lock, fail))
			{
				return;
			}

			orig(self);
		}

		#endregion

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
