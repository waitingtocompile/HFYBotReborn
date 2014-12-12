using System;

namespace HFYBot.Modules.UI
{
	public abstract class UIMenu
	{
		public string displayText{ get; protected set;}

		public abstract void receiveKey (ConsoleKey key);

		protected UserInterfaceModule module;

		public virtual void updateText(){

		}

		public UIMenu(UserInterfaceModule module){
			this.module = module;
		}
	}
}

