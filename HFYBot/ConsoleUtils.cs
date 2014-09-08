using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HFYBot
{

    //This is just a class to hold any methods associated with the console. Most of the code here is less that perfect.
    class ConsoleUtils
    {
        //Gives a nice looking timestamp to go at the start of lines
        public static string TimeStamp
        {
            get
            {
                return "[" + DateTime.Now.ToString("HH:mm:ss") + "]";
            }
        }

        //A nice method of reading test input from the console. If showChars is false it will show asterisks in place of the actual text (good for passwords)
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
