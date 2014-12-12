using System;
using HFYBot.Modules;

namespace HFYBot.Modules.UI
{
	public class LazyMan:UIMenu
	{
		public LazyMan (UserInterfaceModule module):base(module)
		{
			displayText = "Not Implemented. Press a key to go back.";
		}

		public override void receiveKey (ConsoleKey key)
		{
			module.MenuStepBack ();
		}
	}
}

