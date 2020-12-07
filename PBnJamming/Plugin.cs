using ADepIn;
using Deli;
using DeliFramework = Deli.Deli;
using FistVR;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Text;
using System;

namespace PBnJamming
{
	internal class Plugin : DeliBehaviour
	{
		public IFailure Failure { get; }

		public RootConfigs Configs { get; }

		public enum FailureType
		{
			Fire,
			Feed,
			Extract,
			LockOpen
		}

		private static string FailureLocale(FailureType value)
		{
			switch (value)
			{
				case FailureType.Fire: return "Fire";
				case FailureType.Feed: return "Feed";
				case FailureType.Extract: return "Extract";
				case FailureType.LockOpen: return "LockOpen";
				default: throw new ArgumentOutOfRangeException();
			}
		}

		public Plugin()
		{
			Configs = new RootConfigs(Config);

			Failure = AddFailure("pbnj.magazine", g => g.Magazine)
				.AddFailure("pbnj.roundtype", g => g.RoundType)
				.AddFailure("pbnj.action", g => g.ObjectWrapper.TagFirearmAction)
				.AddFailure("pbnj.era", g => g.ObjectWrapper.TagEra)
				.AddFailure("pbnj.id", g => g.ObjectWrapper.ItemID);

			// Fire
			On.FistVR.ClosedBoltWeapon.Fire += ClosedBoltWeapon_Fire;
			On.FistVR.OpenBoltReceiver.Fire += OpenBoltReceiver_Fire;
			On.FistVR.Handgun.Fire += Handgun_Fire;
			On.FistVR.TubeFedShotgun.Fire += TubeFedShotgun_Fire;

			// Feed
			On.FistVR.ClosedBoltWeapon.BeginChamberingRound += ClosedBoltWeapon_BeginChamberingRound;
			On.FistVR.OpenBoltReceiver.BeginChamberingRound += OpenBoltReceiver_BeginChamberingRound;
			On.FistVR.Handgun.ExtractRound += Handgun_ExtractRound;
			On.FistVR.TubeFedShotgun.ExtractRound += TubeFedShotgun_ExtractRound;

			// Extract
			On.FistVR.ClosedBolt.ImpartFiringImpulse += ClosedBolt_ImpartFiringImpulse;
			On.FistVR.OpenBoltReceiverBolt.ImpartFiringImpulse += OpenBoltReceiverBolt_ImpartFiringImpulse;
			On.FistVR.HandgunSlide.ImpartFiringImpulse += HandgunSlide_ImpartFiringImpulse;
			On.FistVR.TubeFedShotgun.EjectExtractedRound += TubeFedShotgun_EjectExtractedRound;

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
			On.FistVR.TubeFedShotgun.Fire -= TubeFedShotgun_Fire;

			// Feed
			On.FistVR.ClosedBoltWeapon.BeginChamberingRound -= ClosedBoltWeapon_BeginChamberingRound;
			On.FistVR.OpenBoltReceiver.BeginChamberingRound -= OpenBoltReceiver_BeginChamberingRound;
			On.FistVR.Handgun.ExtractRound -= Handgun_ExtractRound;
			On.FistVR.TubeFedShotgun.ExtractRound -= TubeFedShotgun_ExtractRound;

			// Extract
			On.FistVR.ClosedBolt.ImpartFiringImpulse -= ClosedBolt_ImpartFiringImpulse;
			On.FistVR.OpenBoltReceiverBolt.ImpartFiringImpulse -= OpenBoltReceiverBolt_ImpartFiringImpulse;
			On.FistVR.HandgunSlide.ImpartFiringImpulse -= HandgunSlide_ImpartFiringImpulse;
			On.FistVR.TubeFedShotgun.EjectExtractedRound += TubeFedShotgun_EjectExtractedRound;

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
					.Append("┌─────Failure Roll Report─────")
					.Append("│ ItemID: " + gun.ObjectWrapper.ItemID).AppendLine()
					.Append("│  Era: " + gun.ObjectWrapper.TagEra).AppendLine()
					.Append("│  Action: " + gun.ObjectWrapper.TagFirearmAction).AppendLine()
					.Append("│  Round: " + gun.RoundType).AppendLine()
					.Append("│  Magazine: " + gun.Magazine).AppendLine()
					.Append("│ Failure Rolled: " + failure).AppendLine()
					.Append("│  Random: " + ran).AppendLine()
					.Append("│  Chance: " + chance).AppendLine()
					.Append("└─────────────────────────────");

				Logger.LogDebug(builder);
			}

			return ran <= chance;
		}

		#region Fire	
		private bool ClosedBoltWeapon_Fire(On.FistVR.ClosedBoltWeapon.orig_Fire orig, ClosedBoltWeapon self)
		{
			if (Failed(self, m => m.Fire, FailureLocale(FailureType.Fire)))
			{
				return false;
			}

			return orig(self);
		}

		private bool OpenBoltReceiver_Fire(On.FistVR.OpenBoltReceiver.orig_Fire orig, OpenBoltReceiver self)
		{
			if (Failed(self, m => m.Fire, FailureLocale(FailureType.Fire)))
			{
				return false;
			}

			return orig(self);
		}

		private bool Handgun_Fire(On.FistVR.Handgun.orig_Fire orig, Handgun self)
		{
			if (Failed(self, m => m.Fire, FailureLocale(FailureType.Fire)))
			{
				return false;
			}

			return orig(self);
		}

		private bool TubeFedShotgun_Fire(On.FistVR.TubeFedShotgun.orig_Fire orig, TubeFedShotgun self)
		{
			if (Failed(self, m => m.Fire, FailureLocale(FailureType.Fire)))
			{
				return false;
			}

			return orig(self);
		}

		#endregion

		#region Feed
		private void ClosedBoltWeapon_BeginChamberingRound(On.FistVR.ClosedBoltWeapon.orig_BeginChamberingRound orig, ClosedBoltWeapon self)
		{
			if (Failed(self, m => m.Feed, FailureLocale(FailureType.Feed)))
			{
				return;
			}

			orig(self);
		}

		private void OpenBoltReceiver_BeginChamberingRound(On.FistVR.OpenBoltReceiver.orig_BeginChamberingRound orig, OpenBoltReceiver self)
		{
			if (Failed(self, m => m.Feed, FailureLocale(FailureType.Feed)))
			{
				return;
			}

			orig(self);
		}

		private void Handgun_ExtractRound(On.FistVR.Handgun.orig_ExtractRound orig, Handgun self)
		{
			if (Failed(self, m => m.Feed, FailureLocale(FailureType.Feed)))
			{
				return;
			}

			orig(self);
		}

		private void TubeFedShotgun_ExtractRound(On.FistVR.TubeFedShotgun.orig_ExtractRound orig, TubeFedShotgun self)
		{
			if (Failed(self, m => m.Feed, FailureLocale(FailureType.Feed)))
			{
				return;
			}

			orig(self);
		}

		#endregion

		#region Extract
		private void ClosedBolt_ImpartFiringImpulse(On.FistVR.ClosedBolt.orig_ImpartFiringImpulse orig, ClosedBolt self)
		{
			if (Failed(self.Weapon, m => m.Extract, FailureLocale(FailureType.Extract)))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		private void OpenBoltReceiverBolt_ImpartFiringImpulse(On.FistVR.OpenBoltReceiverBolt.orig_ImpartFiringImpulse orig, OpenBoltReceiverBolt self)
		{
			if (Failed(self.Receiver, m => m.Extract, FailureLocale(FailureType.Extract)))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		private void HandgunSlide_ImpartFiringImpulse(On.FistVR.HandgunSlide.orig_ImpartFiringImpulse orig, HandgunSlide self)
		{
			if (Failed(self.Handgun, m => m.Extract, FailureLocale(FailureType.Extract)))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		private void TubeFedShotgun_EjectExtractedRound(On.FistVR.TubeFedShotgun.orig_EjectExtractedRound orig, TubeFedShotgun self)
		{
			if (Failed(self, m => m.Extract, FailureLocale(FailureType.Extract)))
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
			if (Failed(self, m => m.LockOpen, FailureLocale(FailureType.LockOpen)))
			{
				return;
			}

			orig(self);
		}

		private void ClosedBolt_LockBolt(On.FistVR.ClosedBolt.orig_LockBolt orig, ClosedBolt self)
		{
			if (Failed(self.Weapon, m => m.LockOpen, FailureLocale(FailureType.LockOpen)))
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
