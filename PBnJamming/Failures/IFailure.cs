using ADepIn;
using FistVR;

namespace PBnJamming
{
	public interface IFailure
	{
		Option<FailureMask> this[FVRFireArm gun] { get; }
	}
}
