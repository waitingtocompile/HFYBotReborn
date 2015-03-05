using System;
using HFYBot;

namespace HFYBot
{
	/// <summary>
	/// Represents a basic module. All module inherit this base class.
	/// </summary>
	public abstract class Module
	{
		public static ModuleManager moduleManager;

		public MessageType[] permittedMessgaeTypes{ get; protected set;}

		/// <summary>
		/// The name of the module used when reffering to that module internally.
		/// </summary>
		public readonly string name;

		/// <summary>
		/// Internal flag to tell whether the module should be enabled. Allows module to finish a pass where relevant before stopping.
		/// </summary>
		protected bool enabled = false;

		/// <summary>
		/// Gets or sets the state of the module.
		/// </summary>
		/// <value>The state.</value>
		public ModuleState state{ get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HFYBot.Module"/> class.
		/// </summary>
		/// <param name="name">Name of the module.</param>
		public Module (string name)
		{
			this.name = name;
			state = ModuleState.Disabled;
		}

		/// <summary>
		/// Sets whether the module is enabled. This must be overrriden in children.
		/// </summary>
		/// <param name="b">The intended state of the module.</param>
		public abstract void setEnabled(bool b);

		/// <summary>
		/// Called by the ModuleManager when when a sutiable messgae has been broadcast.
		/// </summary>
		/// <param name="messageType">The type of message being sent</param>
		/// <param name="messgaeData">Data associated with the message</param>
		public abstract void RecieveMessage(MessageType messageType, Object[] messageData);
	}

	/// <summary>
	/// Enum representing the state of the module.
	/// </summary>
	public enum ModuleState
	{
		/// <summary>
		/// Representing the module being both enabled and running.
		/// </summary>
		Enabled,
		/// <summary>
		/// Representing the module being enabled, but not currently operating. Typically waiting for a time to elapse or an order from another module.
		/// </summary>
		Idle,
		/// <summary>
		/// Representing the module has been disabled and will not respond to external input.
		/// </summary>
		Disabled,
		/// <summary>
		/// Representing the module has failed in some way, and should be restarted. Some modules may atempt to restart themselves.
		/// </summary>
		Crashed
	}
}

