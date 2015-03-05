using System;
using System.Threading;
using System.Collections.Generic;

using RedditSharp;
using RedditSharp.Things;

#if DEBUG
using System.Diagnostics;
#endif

namespace HFYBot.Modules
{
	/// <summary>
	/// Module that deals with new posts on the subreddit.
	/// </summary>
	public class PostReceiverModule : RedditModule
	{
		/// <summary>
		/// The thread on which the module operates.
		/// </summary>
		Thread thread;

		/// <summary>
		/// The time to wait between passes
		/// </summary>
		TimeSpan waitTime = new TimeSpan (0, 5, 0);

		/// <summary>
		/// Initializes a new instance of the <see cref="HFYBot.Modules.PostReceiverModule"/> class.
		/// </summary>
		/// <param name="reddit">Reddit API instance</param>
		/// <param name="sub">Target subreddit</param>
		public PostReceiverModule (Reddit reddit, Subreddit sub) :base("post receiver", reddit, sub)
		{
			thread = new Thread(new ThreadStart(run));
			permittedMessgaeTypes = new MessageType[0];
		}

		/// <summary>
		/// Make a pass through a number of new posts
		/// </summary>
		/// <param name="postCount">The number of posts to check</param>
		void MakePass(int postCount)
		{

			//TODO: Optimise post recieveing code. 
			try{
				List<RedditUser> pendingEdits = new List<RedditUser>(0);

				IEnumerable<Post> posts = sub.New.GetListing(postCount);
				foreach(Post post in posts){
					if(isOC(post) && !processed(post)){
						if(!pendingEdits.Contains(post.Author)) pendingEdits.Add(post.Author);
						post.Comment("Please wait...");
					}
				}

				foreach(RedditUser user in pendingEdits){
					string comment = generateCommentText(user);
					foreach(Post post in user.Posts){
						if(isOC(post)){
							foreach(Comment comm in post.Comments)
								if(comm.Author.Equals(reddit.User.Name))
									comm.EditText(comment);
						}
					}
				}

			} catch (System.Net.WebException) {
				state = ModuleState.Crashed;
			}
		}

		/// <summary>
		/// Generates the comment text.
		/// </summary>
		/// <returns>The comment text.</returns>
		/// <param name="user">User to generate text for.</param>
		string generateCommentText(RedditUser user)
		{
			int count = 0;
			Listing<Post> allPosts = user.GetPosts(Sort.New);
			List<Post> availiblePosts = new List<Post>(0);
			foreach (Post post in allPosts) {
				#if DEBUG
				Debug.Write("post " + post.Title + ", " + post.Subreddit);
				#endif
				if (post.Subreddit.Equals (sub.Name) && isOC (post)) {
					#if DEBUG
					Debug.WriteLine(" Included");
					#endif
					count++;
					if (availiblePosts.Count < 25)
						availiblePosts.Add (post);
				}
				#if DEBUG
				else Debug.Write("\n");
				#endif
			}

			string comm;

			if (availiblePosts.Count > 1) {
				comm = "There are " + count + " stories by [u/" + user.Name + "](http://reddit.com/u/" + user.Name + ") Including:";
				foreach (Post p in availiblePosts) {
					comm += "\n\n* [" + p.Title + "](" + p.Url + ")";
				}
			} else {
				comm = "There are no other stories by [u/" + user.Name + "](http://reddit.com/u/" + user.Name+ ")";
			}

			comm += "\n\nThis list was automatically generated by HFYBotReborn version "
				+ Program.version
				+". Please contact /u/KaiserMagnus if you have any queries. This bot is [open source](https://github.com/waitingtocompile/HFYBotReborn).";
			return comm;
		}
		/// <summary>
		/// Checks if post is OC
		/// </summary>
		/// <returns><c>true</c>, if is OC, <c>false</c> otherwise.</returns>
		/// <param name="post">Post to check</param>
		bool isOC(Post post)
		{
			if (post.Equals (null))
				return false;
			if (post.Title.ToUpperInvariant ().Contains ("[OC]"))
				return true;
			if (!string.IsNullOrEmpty (post.LinkFlairText) && post.LinkFlairText.ToUpperInvariant ().Equals ("OC"))
				return true;
			return false;
		}

		/// <summary>
		/// Checks if post has been processed
		/// </summary>
		/// <param name="post">Post to check</param>
		bool processed(Post post)
		{
			foreach (Comment com in post.Comments) {
				if (com.Author.Equals (reddit.User.Name))
					return true;
			}
			return false;
		}

		/// <summary>
		/// Run this instance. Intended to be started within the thread.
		/// </summary>
		void run()
		{
			state = ModuleState.Enabled;
			MakePass(40);
			while (enabled) {
				state = ModuleState.Idle;
				Thread.Sleep (waitTime);
				state = ModuleState.Enabled;
				MakePass(5);
				if (state.Equals (ModuleState.Crashed)) {
					Thread.Sleep (10000);
					state = ModuleState.Enabled;
					MakePass(5);
					if (state.Equals (ModuleState.Crashed)) {
						Thread.Sleep (120000);
						state = ModuleState.Enabled;
						MakePass(5);
						if(state.Equals(ModuleState.Crashed))
						   break;
					}
				}
			}
			if(state != ModuleState.Crashed)
				state = ModuleState.Disabled;
		}

		/// <summary>
		/// Sets whether the module is enabled. Will wait until the current pass is finished.
		/// </summary>
		/// <param name="b">Intended state.</param>
		public override void setEnabled(bool b)
		{
			if (!b && state.Equals (ModuleState.Idle)) {
				thread.Abort ();
				enabled = false;
			} else {
				enabled = b;
				thread.Start ();
			}
		}

		public override void RecieveMessage (MessageType messageType, object[] messageData)
		{
			return; //needs not respond to messages, since it will receive none.
		}
	}
}

