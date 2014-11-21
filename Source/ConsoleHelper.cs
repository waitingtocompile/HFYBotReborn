using System;

namespace HFYBot
{
	/// <summary>
	/// Provides some useful console related functions.
	/// </summary>
	public class ConsoleHelper
	{
		/// <summary>
		/// Request a sting from the user
		/// </summary>
		/// <returns>The string input</returns>
		/// <param name="descriptor">DA description of the string to show to the user.</param>
		/// <param name="hideChars">If set to <c>true</c> hide chars (useful for passwords).</param>
		public static string RequestString(string descriptor, bool hideChars = false){
			Console.CursorVisible = true;
			Console.Write(descriptor + ": ");
			string s = "";
			while (true)
			{
				ConsoleKeyInfo keyInfo = Console.ReadKey(hideChars);
				if (keyInfo.Key.Equals (ConsoleKey.Enter))
					return s;
				else if (keyInfo.Key.Equals(ConsoleKey.Backspace))
				{
					s = s.Substring(0, s.Length-1);
					if (hideChars)
						Console.Write (keyInfo.KeyChar);
				}
				else
				{
					s += keyInfo.KeyChar;
				}
			}
		}
	}
}

