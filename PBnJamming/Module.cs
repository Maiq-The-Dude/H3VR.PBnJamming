using ADepIn;

namespace PBnJamming
{
	internal class Module : IEntryModule<Module>
	{
		public static IServiceKernel Kernel { get; private set; }

		public void Load(IServiceKernel kernel)
		{
			Kernel = kernel;
		}
	}
}
