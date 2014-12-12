using System;
using System.Threading;
using System.Deployment;
using System.Collections.Generic;

using HFYBot.Modules.UI;

namespace HFYBot.Modules
{
	/// <summary>
	/// User interface module. Allows some control over all modules that allow it.
	/// </summary>
	public class UserInterfaceModule:Module
	{

		Thread displayThread;

		List<UIMenu> activeMenus = new List<UIMenu> (0);

		MainMenu mainMenu;

		public UserInterfaceModule():base("UI")
		{
			Console.CursorVisible = false;
			mainMenu = new MainMenu (this);
		}

		public void MakeMenuTransition(UIMenu menu){
			if (menu != null) {
				activeMenus.Add (menu);
				Console.Clear ();
				Console.Write (menu.displayText);
			} else
				MenuStepBack ();
		}

		public void MenuStepBack(){
			if (activeMenus.Count == 1)
				return;
			activeMenus.RemoveAt (activeMenus.Count - 1);
			activeMenus.TrimExcess ();
			Console.Clear ();
			Console.Write (activeMenus[activeMenus.Count - 1].displayText);
		}

		public override void setEnabled(bool b)
		{
			if (!b) {
				Console.Clear ();
				Console.Write ("Waiting for all modules to stop...");
				//TODO: actually wait for modules to stop
				Console.Write ("All modules have stopped. Press any key to exit...");
				Console.Read ();
				Environment.Exit (0);
			} else if (!enabled) {
				MakeMenuTransition (mainMenu);
				displayThread = new Thread(new ThreadStart(run));
			}
		}

		private void run()
		{
			Console.ReadKey();
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

