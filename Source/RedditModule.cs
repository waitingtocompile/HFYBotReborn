using System;
using RedditSharp;
using RedditSharp.Things;

namespace HFYBot
{
	/// <summary>
	/// Represents a basic module that uses the reddit API in some way. At present all modules are reddit modules.
	/// </summary>
	public abstract class RedditModule
	{
		/// <summary>
		/// The name of the module used when reffering to that module internally.
		/// </summary>
		public readonly string name;

		/// <summary>
		/// This module's instance of the Reddit API. Typically the one defined in Program.
		/// </summary>
		protected Reddit reddit;
		/// <summary>
		/// The Subreddit the module will operate in. If no specific subreddit is recuires, can be left blank for /r/all.
		/// </summary>
		protected Subreddit sub;

		/// <summary>
		/// Gets or sets the state of the module.
		/// </summary>
		/// <value>The state.</value>
		public ModuleState state{ get; protected set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="HFYBot.RedditModule"/> class.
		/// </summary>
		/// <param name="name">The name of the module</param>
		/// <param name="redditInstance">Reddit API instance</param>
		/// <param name="sub">Target subreddit</param>
		public RedditModule (string name, Reddit redditInstance, Subreddit sub)
		{
			this.name = name;
			this.sub = sub;
			reddit = redditInstance;
		}


	}
	/// <summary>
	/// Enum representing the state of the module.
	/// </summary>
	public enum ModuleState{
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
		Diabled,
		/// <summary>
		/// Representing the module has failed in some way, and should be restarted. Some modules may atempt to restart themselves.
		/// </summary>
		Crashed
	}
}

