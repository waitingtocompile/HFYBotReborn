using System;
using RedditSharp;

using System.Diagnostics;
namespace HFYBot
{
	class Program
	{
		public const string version = "2.0 experimental";

		public static void Main (string[] args)
		{
			Reddit reddit = LogIn();

			Console.Read();
		}

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
