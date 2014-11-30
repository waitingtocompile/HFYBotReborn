using System;
using System.Collections.Generic;
using RedditSharp;

using HFYBot.Modules;

namespace HFYBot
{
	class Program
	{
		/// <summary>
		/// The version of the program to be displayed to users and the general public.
		/// </summary>
		public const string version = "2.0 experimental";

		/// <summary>
		/// The default subreddit the bot will use when acessing reddit. Some modules may use a different subreddit.
		/// </summary>
		public const string defaultSubreddit = "/r/bottest";

		/// <summary>
		/// All modules should be in this list. There are many planned methods that are not yet implemented 
		/// </summary>
		static List<RedditModule> Modules = new List<RedditModule>();

		/// <summary>
		/// The instance of the Reddit API that the bot uses. Individual modules can (in theory) have their own though will generally use this.
		/// </summary>
		static Reddit reddit;

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			reddit = LogIn();
			LoadCoreModules();
			((PostReceiverModule)getModuleByName ("post receiver")).setEnabled (true);
			//TODO: actual code to execute
			Console.Read();
		}

		/// <summary>
		/// Prompts the user to log in. Will ask again if details are incorrect. Will end the program if it cannot connect to reddit.
		/// </summary>
		/// <returns>The Instance of the Reddit API logged into</returns>
		static Reddit LogIn()
		{
			while (true) {
				Console.WriteLine("Please log in to Reddit");
				try{
					Reddit reddit = new Reddit(ConsoleHelper.RequestString("Username"), ConsoleHelper.RequestString("Password", true));
					return reddit;
				} catch (System.Security.Authentication.AuthenticationException) {
					Console.WriteLine("\n\nLogin refused, please try again.");
				} catch (System.Net.WebException) {
					Console.WriteLine("\n\nNetwork error when connecting to reddit. Please check your connection");
					Console.Write("Press any key...");
					Console.Read();
					System.Environment.Exit (0);
				}
			}

		}

		/// <summary>
		/// Loads the core modules. Other modules can be dynamically loaded later
		/// </summary>
		static void LoadCoreModules(){
			Modules.Add(new PostReceiverModule(reddit, defaultSubreddit));
		}

		/// <summary>
		/// Gets a module by its name.
		/// </summary>
		/// <returns>The maned module.</returns>
		/// <param name="name">Name of the module.</param>
		public static RedditModule getModuleByName(string name){
			foreach (RedditModule module in Modules) {
				if(module.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				   return module;
			}
		}
	}
}
