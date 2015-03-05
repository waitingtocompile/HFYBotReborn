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

		static ModuleManager moduleManager;

		/// <summary>
		/// The entry point of the program, where the program control starts and ends.
		/// </summary>
		/// <param name="args">The command-line arguments.</param>
		public static void Main (string[] args)
		{
			moduleManager = new ModuleManager(LogIn());

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
	}
}
