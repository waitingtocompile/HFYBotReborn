using System;

namespace HFYBot.Modules.UI
{
	public class ExitConfMenu:UIMenu
	{
		public ExitConfMenu (UserInterfaceModule module):base(module)
		{
			displayText = "Are you sure you wish to halt the bot?\n\nPress Enter to confirm. Press any other key to go back.";
		}

		public override void receiveKey (ConsoleKey key)
		{
			if (key.Equals (ConsoleKey.Enter))
				module.setEnabled (false);
			else
				module.MenuStepBack ();
		}
	}
}

