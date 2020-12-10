using ADepIn;
using Deli;
using DeliFramework = Deli.Deli;
using FistVR;
using Random = UnityEngine.Random;
using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace PBnJamming
{
	internal class Plugin : DeliBehaviour
	{
		public IFailure Failure { get; }

		public RootConfig Configs { get; }

		private bool _extractFlag;
		private bool _lockFlag;

		public enum FailureType
		{
			Fire,
			Feed,
			Extract,
			LockOpen,
			AccDischarge
		}

		private static Mapper<FVRFireArm, Option<TKey>> WrapperMapper<TKey>(Mapper<FVRObject, Option<TKey>> keyFromObject)
		{
			return v =>
			{
				var wrapper = v.ObjectWrapper;
				return wrapper == null ? Option.None<TKey>() : keyFromObject(wrapper);
			};
		}

		public Plugin()
		{
			Configs = new RootConfig(Config);

			IFailure failure;
			failure = new AverageFailure(CreateFailureLeafs().ToArray());
			failure = new MultiplicativeFailure(failure, () => Configs.Multiplier.Mask / Configs.Weights.GlobalMask);

			Failure = failure;

			// Patches
			On.FistVR.BreakActionWeapon.Awake += BreakActionWeapon_Awake;

			// Fire
			On.FistVR.ClosedBoltWeapon.Fire += ClosedBoltWeapon_Fire;
			On.FistVR.OpenBoltReceiver.Fire += OpenBoltReceiver_Fire;
			On.FistVR.Handgun.Fire += Handgun_Fire;
			On.FistVR.TubeFedShotgun.Fire += TubeFedShotgun_Fire;
			On.FistVR.Revolver.Fire += Revolver_Fire;
			On.FistVR.RevolvingShotgun.Fire += RevolvingShotgun_Fire;
			On.FistVR.RollingBlock.Fire += RollingBlock_Fire;
			On.FistVR.BreakActionWeapon.Fire += BreakActionWeapon_Fire;
			On.FistVR.BoltActionRifle.Fire += BoltActionRifle_Fire;
			On.FistVR.LeverActionFirearm.Fire += LeverActionFirearm_Fire;

			// Feed
			On.FistVR.ClosedBoltWeapon.BeginChamberingRound += ClosedBoltWeapon_BeginChamberingRound;
			On.FistVR.OpenBoltReceiver.BeginChamberingRound += OpenBoltReceiver_BeginChamberingRound;
			On.FistVR.Handgun.ExtractRound += Handgun_ExtractRound;
			On.FistVR.TubeFedShotgun.ExtractRound += TubeFedShotgun_ExtractRound;
			On.FistVR.FVRFireArmChamber.SetRound += FVRFireArmChamber_SetRound;	// LeverActionFirearm & BoltActionRifle

			// Extract
			On.FistVR.ClosedBolt.ImpartFiringImpulse += ClosedBolt_ImpartFiringImpulse;
			On.FistVR.OpenBoltReceiverBolt.ImpartFiringImpulse += OpenBoltReceiverBolt_ImpartFiringImpulse;
			On.FistVR.HandgunSlide.ImpartFiringImpulse += HandgunSlide_ImpartFiringImpulse;
			On.FistVR.TubeFedShotgun.EjectExtractedRound += TubeFedShotgun_EjectExtractedRound;
			On.FistVR.BreakActionWeapon.PopOutRound += BreakActionWeapon_PopOutRound;
			On.FistVR.BreakActionWeapon.PopOutEmpties += BreakActionWeapon_PopOutEmpties;

			// LockOpen
			On.FistVR.Handgun.EngageSlideRelease += Handgun_EngageSlideRelease;
			On.FistVR.ClosedBolt.LockBolt += ClosedBolt_LockBolt;

			// AccDischarge
			On.FistVR.HandgunSlide.SlideEvent_ArriveAtFore += HandgunSlide_SlideEvent_ArriveAtFore;
			On.FistVR.ClosedBolt.BoltEvent_ArriveAtFore += ClosedBolt_BoltEvent_ArriveAtFore;
			On.FistVR.TubeFedShotgunBolt.BoltEvent_ArriveAtFore += TubeFedShotgunBolt_BoltEvent_ArriveAtFore;
			On.FistVR.OpenBoltReceiverBolt.BoltEvent_BoltCaught += OpenBoltReceiverBolt_BoltEvent_BoltCaught;
		}

		private IEnumerable<IFailure> CreateFailureLeafs()
		{
			yield return CreateFailureLeaf("pbnj.action", c => c.Action, WrapperMapper(v => Option.Some(v.TagFirearmAction)));
			yield return CreateFailureLeaf("pbnj.era", c => c.Era, WrapperMapper(v => Option.Some(v.TagEra)));
			yield return CreateFailureLeaf("pbnj.id", c => c.ID, WrapperMapper(v => Option.Some(v.ItemID)));
			yield return CreateFailureLeaf("pbnj.magazine", c => c.Magazine, g =>
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
			yield return CreateFailureLeaf("pbnj.roundtype", c => c.RoundType, g => Option.Some(g.RoundType));
		}

		private void OnDestroy()
		{
			// Patches
			On.FistVR.BreakActionWeapon.Awake -= BreakActionWeapon_Awake;

			// Fire
			On.FistVR.ClosedBoltWeapon.Fire -= ClosedBoltWeapon_Fire;
			On.FistVR.OpenBoltReceiver.Fire -= OpenBoltReceiver_Fire;
			On.FistVR.Handgun.Fire -= Handgun_Fire;
			On.FistVR.TubeFedShotgun.Fire -= TubeFedShotgun_Fire;
			On.FistVR.Revolver.Fire -= Revolver_Fire;
			On.FistVR.RevolvingShotgun.Fire -= RevolvingShotgun_Fire;
			On.FistVR.RollingBlock.Fire -= RollingBlock_Fire;
			On.FistVR.BreakActionWeapon.Fire -= BreakActionWeapon_Fire;
			On.FistVR.BoltActionRifle.Fire -= BoltActionRifle_Fire;
			On.FistVR.LeverActionFirearm.Fire -= LeverActionFirearm_Fire;

			// Feed
			On.FistVR.ClosedBoltWeapon.BeginChamberingRound -= ClosedBoltWeapon_BeginChamberingRound;
			On.FistVR.OpenBoltReceiver.BeginChamberingRound -= OpenBoltReceiver_BeginChamberingRound;
			On.FistVR.Handgun.ExtractRound -= Handgun_ExtractRound;
			On.FistVR.TubeFedShotgun.ExtractRound -= TubeFedShotgun_ExtractRound;
			On.FistVR.BreakActionWeapon.PopOutEmpties -= BreakActionWeapon_PopOutEmpties;
			On.FistVR.FVRFireArmChamber.SetRound -= FVRFireArmChamber_SetRound;

			// Extract
			On.FistVR.ClosedBolt.ImpartFiringImpulse -= ClosedBolt_ImpartFiringImpulse;
			On.FistVR.OpenBoltReceiverBolt.ImpartFiringImpulse -= OpenBoltReceiverBolt_ImpartFiringImpulse;
			On.FistVR.HandgunSlide.ImpartFiringImpulse -= HandgunSlide_ImpartFiringImpulse;
			On.FistVR.TubeFedShotgun.EjectExtractedRound -= TubeFedShotgun_EjectExtractedRound;
			On.FistVR.BreakActionWeapon.PopOutRound -= BreakActionWeapon_PopOutRound;
			On.FistVR.BreakActionWeapon.PopOutEmpties -= BreakActionWeapon_PopOutEmpties;

			// LockOpen
			On.FistVR.Handgun.EngageSlideRelease -= Handgun_EngageSlideRelease;
			On.FistVR.ClosedBolt.LockBolt -= ClosedBolt_LockBolt;

			// AccDischarge
			On.FistVR.HandgunSlide.SlideEvent_ArriveAtFore -= HandgunSlide_SlideEvent_ArriveAtFore;
			On.FistVR.ClosedBolt.BoltEvent_ArriveAtFore -= ClosedBolt_BoltEvent_ArriveAtFore;
			On.FistVR.TubeFedShotgunBolt.BoltEvent_ArriveAtFore -= TubeFedShotgunBolt_BoltEvent_ArriveAtFore;
			On.FistVR.OpenBoltReceiverBolt.BoltEvent_BoltCaught -= OpenBoltReceiverBolt_BoltEvent_BoltCaught;
		}

		private bool Failed(FVRFireArm gun, Mapper<FailureMask, float> type, FailureType failure)
		{
			var ran = Random.Range(0f, 1f);
			var mask = Failure[gun].Unwrap();
			var chance = type(mask);

			if (Configs.EnableLogging.Value && !_lockFlag && !_extractFlag)
			{
				var builder = new StringBuilder().AppendLine()
					.Append("┌─────Failure Roll Report─────").AppendLine()
					.Append("│ ItemID: ").Append(gun.ObjectWrapper == null ? "" : gun.ObjectWrapper.ItemID).AppendLine()
					.Append("│  Era: ").Append(gun.ObjectWrapper == null ? FVRObject.OTagFirearmAction.None : gun.ObjectWrapper.TagFirearmAction).AppendLine()
					.Append("│  Action: ").Append(gun.ObjectWrapper == null ? FVRObject.OTagFirearmAction.None : gun.ObjectWrapper.TagFirearmAction).AppendLine()
					.Append("│  Round: ").Append(gun.RoundType).AppendLine()
					.Append("│  Magazine: ").Append((gun.Magazine == null || gun.Magazine.ObjectWrapper == null) ? "" : (gun.Magazine.IsIntegrated ? gun.Magazine.FireArm.ObjectWrapper.ItemID : gun.Magazine.ObjectWrapper.ItemID)).AppendLine()
					.Append("│ Failure Rolled: ").Append(failure).AppendLine()
					.Append("│  Random: ").Append(ran).AppendLine()
					.Append("│  Chance: ").Append(chance).AppendLine()
					.Append("└─────────────────────────────");

				Logger.LogDebug(builder);
			}

			// These base methods are run in Update() - use dumb lock to prevent console spam for now
			_extractFlag = false;
			_lockFlag = false;
			if (failure == FailureType.Extract)
			{
				_extractFlag = true;
			}
			else if (failure == FailureType.LockOpen)
			{
				_lockFlag = true;
			}

			return ran <= chance;
		}

		#region Patches
		private void BreakActionWeapon_Awake(On.FistVR.BreakActionWeapon.orig_Awake orig, BreakActionWeapon self)
		{
			self.RotationInterpSpeed = 1;
			orig(self);
		}

		#endregion

		#region Fire
		private bool ClosedBoltWeapon_Fire(On.FistVR.ClosedBoltWeapon.orig_Fire orig, ClosedBoltWeapon self)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				return false;
			}

			return orig(self);
		}

		private bool OpenBoltReceiver_Fire(On.FistVR.OpenBoltReceiver.orig_Fire orig, OpenBoltReceiver self)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				return false;
			}

			return orig(self);
		}

		private bool Handgun_Fire(On.FistVR.Handgun.orig_Fire orig, Handgun self)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				return false;
			}

			return orig(self);
		}

		private bool TubeFedShotgun_Fire(On.FistVR.TubeFedShotgun.orig_Fire orig, TubeFedShotgun self)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				return false;
			}

			return orig(self);
		}

		private void Revolver_Fire(On.FistVR.Revolver.orig_Fire orig, Revolver self)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				return;
			}

			orig(self);
		}

		private void RevolvingShotgun_Fire(On.FistVR.RevolvingShotgun.orig_Fire orig, RevolvingShotgun self)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				return;
			}

			orig(self);
		}

		private void RollingBlock_Fire(On.FistVR.RollingBlock.orig_Fire orig, RollingBlock self)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				return;
			}

			orig(self);
		}

		private bool BreakActionWeapon_Fire(On.FistVR.BreakActionWeapon.orig_Fire orig, BreakActionWeapon self, int b)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				return false;
			}

			return orig(self, b);
		}

		private bool BoltActionRifle_Fire(On.FistVR.BoltActionRifle.orig_Fire orig, BoltActionRifle self)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				return false;
			}

			return orig(self);
		}

		private void LeverActionFirearm_Fire(On.FistVR.LeverActionFirearm.orig_Fire orig, LeverActionFirearm self)
		{
			if (Failed(self, m => m.Fire, FailureType.Fire))
			{
				self.m_isHammerCocked = false;
				self.PlayAudioEvent(FirearmAudioEventType.HammerHit, 1f);
				return;
			}

			orig(self);
		}
		#endregion

		#region Feed
		private void ClosedBoltWeapon_BeginChamberingRound(On.FistVR.ClosedBoltWeapon.orig_BeginChamberingRound orig, ClosedBoltWeapon self)
		{
			if (Failed(self, m => m.Feed, FailureType.Feed))
			{
				return;
			}

			orig(self);
		}

		private void OpenBoltReceiver_BeginChamberingRound(On.FistVR.OpenBoltReceiver.orig_BeginChamberingRound orig, OpenBoltReceiver self)
		{
			if (Failed(self, m => m.Feed, FailureType.Feed))
			{
				return;
			}

			orig(self);
		}

		private void Handgun_ExtractRound(On.FistVR.Handgun.orig_ExtractRound orig, Handgun self)
		{
			if (Failed(self, m => m.Feed, FailureType.Feed))
			{
				return;
			}

			orig(self);
		}

		private void TubeFedShotgun_ExtractRound(On.FistVR.TubeFedShotgun.orig_ExtractRound orig, TubeFedShotgun self)
		{
			if (Failed(self, m => m.Feed, FailureType.Feed))
			{
				return;
			}

			orig(self);
		}

		private void FVRFireArmChamber_SetRound(On.FistVR.FVRFireArmChamber.orig_SetRound orig, FVRFireArmChamber self, FVRFireArmRound round)
		{
			// TODO: make BoltActionRifle not render ProxyRound on feed failures
			if ((self.Firearm is LeverActionFirearm || self.Firearm is BoltActionRifle) && round != null)
			{
				if (Failed(self.Firearm, m => m.Feed, FailureType.Feed))
				{
					// Add round that will be removed in UpdateLever or UpdateBolt
					self.Firearm.Magazine.AddRound(round, false, true);
					return;
				}
			}

			orig(self, round);
		}
		#endregion

		#region Extract
		private void ClosedBolt_ImpartFiringImpulse(On.FistVR.ClosedBolt.orig_ImpartFiringImpulse orig, ClosedBolt self)
		{
			if (_extractFlag) { return; }
			if (Failed(self.Weapon, m => m.Extract, FailureType.Extract))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		private void OpenBoltReceiverBolt_ImpartFiringImpulse(On.FistVR.OpenBoltReceiverBolt.orig_ImpartFiringImpulse orig, OpenBoltReceiverBolt self)
		{
			if (_extractFlag) { return; }
			if (Failed(self.Receiver, m => m.Extract, FailureType.Extract))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		private void HandgunSlide_ImpartFiringImpulse(On.FistVR.HandgunSlide.orig_ImpartFiringImpulse orig, HandgunSlide self)
		{
			if (_extractFlag) { return; }
			if (Failed(self.Handgun, m => m.Extract, FailureType.Extract))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		private void TubeFedShotgun_EjectExtractedRound(On.FistVR.TubeFedShotgun.orig_EjectExtractedRound orig, TubeFedShotgun self)
		{
			if (_extractFlag) { return; }
			if (Failed(self, m => m.Extract, FailureType.Extract))
			{
				self.RotationInterpSpeed = 2;
				return;
			}

			orig(self);
		}

		private void BreakActionWeapon_PopOutRound(On.FistVR.BreakActionWeapon.orig_PopOutRound orig, BreakActionWeapon self, FVRFireArmChamber chamber)
		{
			if (_extractFlag) { return; }
			if (Failed(self, m => m.Extract, FailureType.Extract))
			{
				return;
			}

			orig(self, chamber);
		}

		private void BreakActionWeapon_PopOutEmpties(On.FistVR.BreakActionWeapon.orig_PopOutEmpties orig, BreakActionWeapon self)
		{
			if (_extractFlag) { return; }
			if (Failed(self, m => m.Extract, FailureType.Extract))
			{
				return;
			}

			orig(self);
		}
		#endregion

		#region LockOpen
		private void Handgun_EngageSlideRelease(On.FistVR.Handgun.orig_EngageSlideRelease orig, Handgun self)
		{
			if (Failed(self, m => m.LockOpen, FailureType.LockOpen))
			{
				return;
			}

			orig(self);
		}

		private void ClosedBolt_LockBolt(On.FistVR.ClosedBolt.orig_LockBolt orig, ClosedBolt self)
		{
			if (Failed(self.Weapon, m => m.LockOpen, FailureType.LockOpen))
			{
				return;
			}

			orig(self);
		}
		#endregion

		#region AccDischarge
		private void HandgunSlide_SlideEvent_ArriveAtFore(On.FistVR.HandgunSlide.orig_SlideEvent_ArriveAtFore orig, HandgunSlide self)
		{
			if (Failed(self.Handgun, m => m.Slamfire, FailureType.AccDischarge))
			{
				self.Handgun.ChamberRound();
				self.Handgun.DropHammer(false);
			}

			orig(self);
		}

		private void ClosedBolt_BoltEvent_ArriveAtFore(On.FistVR.ClosedBolt.orig_BoltEvent_ArriveAtFore orig, ClosedBolt self)
		{
			if (Failed(self.Weapon, m => m.Slamfire, FailureType.AccDischarge))
			{
				self.Weapon.ChamberRound();
				self.Weapon.DropHammer();
			}

			orig(self);
		}

		private void TubeFedShotgunBolt_BoltEvent_ArriveAtFore(On.FistVR.TubeFedShotgunBolt.orig_BoltEvent_ArriveAtFore orig, TubeFedShotgunBolt self)
		{
			if (Failed(self.Shotgun, m => m.Slamfire, FailureType.AccDischarge))
			{
				self.Shotgun.ChamberRound();
				self.Shotgun.ReleaseHammer();
			}

			orig(self);
		}

		private void OpenBoltReceiverBolt_BoltEvent_BoltCaught(On.FistVR.OpenBoltReceiverBolt.orig_BoltEvent_BoltCaught orig, OpenBoltReceiverBolt self)
		{
			if (Failed(self.Receiver, m => m.Slamfire, FailureType.AccDischarge))
			{
				self.Receiver.ReleaseSeer();
			}
			orig(self);
		}
		#endregion

		private IFailure CreateFailureLeaf<TKey>(string name, Func<FailureLeafsConfig, FailureMaskConfig> config, Mapper<FVRFireArm, Option<TKey>> keyFromGun)
		{
			if (Module.Kernel.Get<IAssetReader<Option<Dictionary<TKey, FailureMask>>>>().IsNone)
			{
				Module.Kernel.BindJson<Dictionary<TKey, FailureMask>>();
			}

			var dict = new Dictionary<TKey, FailureMask>();
			var loader = new DictLoader<TKey>(dict);
			DeliFramework.AddAssetLoader(name, loader);

			IFailure failure;
			failure = new DictFailure<TKey>(dict, keyFromGun);
			failure = new FallbackFailure(failure, () => config(Configs.Fallbacks).Mask);
			failure = new MultiplicativeFailure(failure, () => config(Configs.Weights).Mask);

			return failure;
		}
	}
}
