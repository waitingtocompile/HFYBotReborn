using System;
using System.Threading;
using System.Deployment;

namespace HFYBot
{
	/// <summary>
	/// User interface module. Allows some control over all modules that allow it.
	/// </summary>
	public class UserInterfaceModule:Module
	{

		Thread displayThread;

		public UserInterfaceModule():base("UI")
		{
			displayThread = new Thread(new ThreadStart(run));
			enabled = true;
		}

		public override void setEnabled(bool b)
		{
			if (!b) {
				Console.Clear();
				Console.Write("Waiting for all modules to stop...");
				//TODO actually wait for modules to stop
				Console.Write("All modules have stopped. Press any key to exit...");
				Console.Read ();
				Environment.Exit(0);
			}
		}

		private void run()
		{

		}

		public static string ModuleStateToString(ModuleState state)
		{
			switch (state) {
			case(ModuleState.Crashed):
				return "Crashed";
			case(ModuleState.Diabled):
				return "Disabled";
			case(ModuleState.Enabled):
				return "Enabled";
			case(ModuleState.Idle):
				return "Idle";
			default:
				return "Unknown";
			}
		}
	}
}

