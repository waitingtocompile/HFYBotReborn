using System;
using RedditSharp;
using RedditSharp.Things;

namespace HFYBot
{
	public abstract class RedditModule
	{
		public readonly string name;

		protected Reddit reddit;
		protected Subreddit sub;

		public bool enabled();

		public ModuleState state{ get; protected set; }

		public RedditModule (string name, Reddit redditInstance, Subreddit sub)
		{
			this.name = name;
			this.sub = sub;
			reddit = redditInstance;
		}


	}

	enum ModuleState{
		Enabled,
		Idle,
		Diabled,
		Crashed
	}
}

