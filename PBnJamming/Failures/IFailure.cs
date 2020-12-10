using ADepIn;
using FistVR;

namespace PBnJamming.Failures
{
	public interface IFailure
	{
		Option<FailureMask> this[FVRFireArm gun] { get; }
	}
}
