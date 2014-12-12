using System;

namespace HFYBot.Modules.UI
{
	/// <summary>
	/// Represents a menu for the UI.
	/// </summary>
	public abstract class UIMenu
	{
		/// <summary>
		/// What is actually displayed when this menu is active.
		/// </summary>
		/// <value>The display text.</value>
		public string displayText{ get; protected set;}

		/// <summary>
		/// Called whenever the user presses a key.
		/// </summary>
		/// <param name="key">Key.</param>
		public abstract void receiveKey (ConsoleKey key);

		/// <summary>
		/// A reference to the UserInterfaceModule in order to make tansitions. Yes it is strongly coupled. So sue me.
		/// </summary>
		protected UserInterfaceModule module;

		/// <summary>
		/// Updates the text. Used for menus that display information that may become out of date.
		/// </summary>
		public virtual void updateText(){

		}

		public UIMenu(UserInterfaceModule module){
			this.module = module;
		}
	}
}

