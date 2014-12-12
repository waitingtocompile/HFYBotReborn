using System;


namespace HFYBot.Modules.UI
{
	/// <summary>
	/// The main menu. exactly what it sounds like. Think of it like a gatekeeper.
	/// </summary>
	public class MainMenu:UIMenu
	{
		public MainMenu (HFYBot.Modules.UserInterfaceModule module):base(module)
		{
			displayText = "";
			lazy = new LazyMan (module);
			exitConf = new ExitConfMenu (module);
		}

		public override void receiveKey (ConsoleKey key)
		{
			switch (key) {
			case(ConsoleKey.D1):
				module.MakeMenuTransition (lazy);
				break;
			case(ConsoleKey.D2):
				updateText();
				break;
			case(ConsoleKey.D3):
				module.MakeMenuTransition (exitConf);
				break;
			}
		}

		LazyMan lazy;
		ExitConfMenu exitConf;
	}
}

