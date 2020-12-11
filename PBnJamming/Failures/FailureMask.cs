using System;
using Valve.Newtonsoft.Json;

namespace PBnJamming.Failures
{
	public readonly struct FailureMask : IEquatable<FailureMask>
	{
		public static FailureMask Unit { get; } = new FailureMask(1, 1, 1, 1, 1);

		public readonly float Fire;
		public readonly float Feed;
		public readonly float Extract;
		public readonly float LockOpen;
		public readonly float Discharge;

		[JsonConstructor]
		public FailureMask(float fire, float feed, float extract, float lockOpen, float discharge)
		{
			Fire = fire;
			Feed = feed;
			Extract = extract;
			LockOpen = lockOpen;
			Discharge = discharge;
		}

		public float this[FailureType type] =>
			type switch
			{
				FailureType.Fire => Fire,
				FailureType.Feed => Feed,
				FailureType.Extract => Extract,
				FailureType.LockOpen => LockOpen,
				FailureType.Discharge => Discharge,
				_ => throw new ArgumentOutOfRangeException()
			};

		public static FailureMask operator +(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire + b.Fire,
				a.Feed + b.Feed,
				a.Extract + b.Extract,
				a.LockOpen + b.LockOpen,
				a.Discharge + b.Discharge
			);
		}

		public static FailureMask operator -(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire - b.Fire,
				a.Feed - b.Feed,
				a.Extract - b.Extract,
				a.LockOpen - b.LockOpen,
				a.Discharge - b.Discharge
			);
		}

		public static FailureMask operator *(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire * b.Fire,
				a.Feed * b.Feed,
				a.Extract * b.Extract,
				a.LockOpen * b.LockOpen,
				a.Discharge * b.Discharge
			);
		}

		public static FailureMask operator /(FailureMask a, FailureMask b)
		{
			return new FailureMask(
				a.Fire / b.Fire,
				a.Feed / b.Feed,
				a.Extract / b.Extract,
				a.LockOpen / b.LockOpen,
				a.Discharge / b.Discharge
			);
		}

		public static FailureMask operator -(FailureMask a)
		{
			return new FailureMask(
				-a.Fire,
				-a.Feed,
				-a.Extract,
				-a.LockOpen,
				-a.Discharge
			);
		}

		public static FailureMask operator *(FailureMask a, float b)
		{
			return new FailureMask(
				a.Fire * b,
				a.Feed * b,
				a.Extract * b,
				a.LockOpen * b,
				a.Discharge * b
			);
		}

		public static FailureMask operator *(float a, FailureMask b)
		{
			return b * a;
		}

		public static FailureMask operator /(FailureMask a, float b)
		{
			return a * (1 / b);
		}

		public static FailureMask operator /(float a, FailureMask b)
		{
			return b / a;
		}

		public bool Equals(FailureMask other)
		{
			return Fire == other.Fire && Feed == other.Feed && Extract == other.Extract && LockOpen == other.LockOpen && Discharge == other.Discharge;
		}

		public override bool Equals(object obj)
		{
			return obj is FailureMask other && Equals(other);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				var hashCode = Fire.GetHashCode();
				hashCode = (hashCode * 397) ^ Feed.GetHashCode();
				hashCode = (hashCode * 397) ^ Extract.GetHashCode();
				hashCode = (hashCode * 397) ^ LockOpen.GetHashCode();
				hashCode = (hashCode * 397) ^ Discharge.GetHashCode();
				return hashCode;
			}
		}

		public static bool operator ==(FailureMask a, FailureMask b)
		{
			return a.Equals(b);
		}

		public static bool operator !=(FailureMask a, FailureMask b)
		{
			return !(a == b);
		}
	}
}
