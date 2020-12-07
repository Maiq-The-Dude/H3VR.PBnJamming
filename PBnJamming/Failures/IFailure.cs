using FistVR;

namespace PBnJamming
{
	public interface IFailure
	{
		FailureMask this[FVRFireArm gun] { get; }
	}
}
