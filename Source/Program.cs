using System;
using System.Collections.Generic;
using RedditSharp;

using System.Threading;

using HFYBot.Modules;

namespace HFYBot
{
	class Program
	{
		/// <summary>
		/// The version of the program to be displayed to users and the general public.
		/// </summary>
		public const string version = "2.0";

		/// <summary>
		/// The default subreddit the bot will use when acessing reddit. Some modules may use a different subreddit.
		/// </summary>
		#if DEBUG
		public const string defaultSubreddit = "/r/Bottest";
		#else
		public const string defaultSubreddit = "/r/HFY";
		#endif

		/// <summary>
		/// All modules should be in this list. There are many planned methods that are not yet implemented 
		/// </summary>
		static List<Module> modules = new List<Module>();

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
			//UserInterfaceModule UIModule = (UserInterfaceModule)getModuleByName ("UI");
			getModuleByName ("post receiver").setEnabled (true);
			getModuleByName ("UI").setEnabled (true); 
			while (true) //Ugly placeholder to prevent the program exiting while I poke it.
				Thread.Sleep (10000000);
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
		static void LoadCoreModules()
		{
			modules.Add((Module)new UserInterfaceModule ());
			modules.Add((Module)new PostReceiverModule(reddit, reddit.GetSubreddit(defaultSubreddit)));
		}

		/// <summary>
		/// Gets a module by its name.
		/// </summary>
		/// <returns>The maned module.</returns>
		/// <param name="name">Name of the module.</param>
		public static Module getModuleByName(string name)
		{
			foreach (Module module in modules) {
				if(module.name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
				   return module;
			}
			return null;
		}

		/// <summary>
		/// Gets the names of all modules.
		/// </summary>
		/// <returns>The module names.</returns>
		public static string[] getModuleNames()
		{
			modules.TrimExcess();
			string[] names = new string[modules.Count];
			for(int i = 0; i < modules.Count; i++){
				names [i] = modules[i].name;
			}
			return names;
		}
	}
}
