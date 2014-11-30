using System;
using RedditSharp;
using RedditSharp.Things;

namespace HFYBot
{
	/// <summary>
	/// Represents a basic module that uses the reddit API in some way.
	/// </summary>
	public abstract class RedditModule : Module
	{
		/// <summary>
		/// This module's instance of the Reddit API. Typically the one defined in Program.
		/// </summary>
		protected Reddit reddit;

		/// <summary>
		/// The Subreddit the module will operate in. If no specific subreddit is recuires, can be left blank for /r/all.
		/// </summary>
		protected Subreddit sub;

		/// <summary>
		/// Initializes a new instance of the <see cref="HFYBot.RedditModule"/> class.
		/// </summary>
		/// <param name="name">The name of the module</param>
		/// <param name="redditInstance">Reddit API instance</param>
		/// <param name="sub">Target subreddit</param>
		public RedditModule (string name, Reddit redditInstance, Subreddit sub):base(name)
		{
			this.sub = sub;
			reddit = redditInstance;
		}
	}
}

