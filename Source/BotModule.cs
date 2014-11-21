using System;

namespace HFYBot
{
	public abstract class BotModule
	{
		public readonly string name;

		public ModuleState state
		{
			get;
			protected set;
		}

		public BotModule (string name)
		{
			this.name = name;
		}

		public abstract bool ChangeState(ModuleState targetState);
	}


	public enum ModuleState
	{
		Running,
		Idle,
		Disabled,
		NoConnection,
		Crashed
	}
}

