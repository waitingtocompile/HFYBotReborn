using System;
using RedditSharp;

namespace HFYBot
{
	public abstract class RedditModule : BotModule
	{
		protected Reddit reddit;

		public RedditModule (string name, Reddit redditInstance) : base(name)
		{
			reddit = redditInstance;
		}


	}
}

