using ADepIn;
using FistVR;

namespace PBnJamming
{
	public interface IFailure
	{
		Option<FailureMask> this[FVRFireArmChamber chamber] { get; }
	}
}
