using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFYBot
{
    class ConsoleUtils
    {
        public static string readString(string descriptor, bool showChars)
        {
            Console.Write("\n" + descriptor + ": ");
            string readString = "";
            while (true)
            {

                ConsoleKeyInfo key = Console.ReadKey(true);
                if (key.Key.Equals(ConsoleKey.Enter)) return readString;
                else if (key.Key.Equals(ConsoleKey.Backspace) && readString.Length > 0)
                {
                    Console.Write("\b \b");
                    readString = readString.Substring(0, readString.Length - 1);
                }
                else
                {
                    if (showChars)
                        Console.Write(key.KeyChar);
                    else
                        Console.Write("*");
                    readString += key.KeyChar;
                }
            }
        }


    }
}
