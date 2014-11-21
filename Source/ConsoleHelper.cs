using System;

namespace HFYBot
{
	public class ConsoleHelper
	{
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

