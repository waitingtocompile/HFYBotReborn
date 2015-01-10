using System;
using System.Threading;
using System.Deployment;
using System.Collections.Generic;

using System.Diagnostics;

using HFYBot.Modules.UI;

//TODO:general fixing and finishing.

namespace HFYBot.Modules
{
	/// <summary>
	/// User interface module. Allows some control over all modules that allow it.
	/// </summary>
	public class UserInterfaceModule:Module
	{
		/// <summary>
		/// The display thread.
		/// </summary>
		Thread displayThread;

		/// <summary>
		/// The 'stack' of menus. It functions not unlike a program stack, a menu can add a new element to the bottom of the stack, which will become the active menu. Alternitaively the bottom menu can be removed and the next menu up becomes active. The top element, the main menu, cannot be removed, so don't try. Please, you will break things
		/// </summary>
		List<UIMenu> activeMenus = new List<UIMenu> (0);

		MainMenu mainMenu;

		public UserInterfaceModule():base("UI")
		{
			Console.CursorVisible = false;
			mainMenu = new MainMenu (this);
			displayThread = new Thread(new ThreadStart(run));
		}

		/// <summary>
		/// Adds the menu to the bottom of the 'stack', displays it's text and feeds all data input into it.
		/// </summary>
		/// <param name="menu">Menu.</param>
		public void MakeMenuTransition(UIMenu menu){
			if (menu != null) {
				activeMenus.Add (menu);
				Console.Clear ();
				Console.Write (menu.displayText);
			} else
				MenuStepBack ();
		}

		/// <summary>
		/// Removes the final menu from the 'stack' and then transitions to the next menu up.
		/// </summary>
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
				Console.WriteLine ("Waiting for all modules to stop...");
				//TODO: actually wait for modules to stop
				Console.WriteLine ("All modules have stopped. Press any key to exit...");
				Console.Read ();
				Environment.Exit (0);
			} else if (!enabled) {
				state = ModuleState.Enabled;
				enabled = true;
				MakeMenuTransition (mainMenu);
				displayThread.Start ();
			}
		}

		private void run()
		{
			mainMenu.updateText ();
			while (enabled) {
				ConsoleKey key = Console.ReadKey (true).Key;
				if (key.Equals (ConsoleKey.Escape)) {
					MenuStepBack ();
				} else {
					activeMenus [activeMenus.Count - 1].receiveKey (key);
				}
			}
		}

		public static string ModuleStateToString(ModuleState state)
		{
			switch (state) {
			case(ModuleState.Crashed):
				return "Crashed";
			case(ModuleState.Disabled):
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

