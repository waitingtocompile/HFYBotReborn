using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Authentication;

using RedditSharp;
using RedditSharp.Things;

namespace HFYBot
{
    class Program
    {
        public static Reddit redditInstance
        {
            private set;
            get;
        }
        public static AuthenticatedUser user
        {
            private set;
            get;
        }
        public static Subreddit sub
        {
            private set;
            get;
        }


        static void Main(string[] args)
        {
            Console.WriteLine("Please input Reddit credentials (don't worry, I won't steal them)");
            redditInstance = new Reddit();
            attemptLogin();
            Console.Clear();
            Console.WriteLine("Login sucsessful, user has " + user.UnreadMessages.Count().ToString() + " unread messages");
            sub = redditInstance.GetSubreddit("/r/MagnusTest");

            foreach (Post post in sub.New.Take(25))
            {
                Console.WriteLine("{0}, by {1} with score of {2}", post.Title, post.AuthorName, post.Score.ToString());
            }
            Console.Write("Press Any Key...");
            Console.Read();
            
        }

        static void attemptLogin()
        {
            try
            {
                string name = ConsoleUtils.readString("Username", true);
                string pass = ConsoleUtils.readString("Password", false);
                user = redditInstance.LogIn(name, pass);
            }
            catch (AuthenticationException)
            {
                Console.Clear();
                Console.WriteLine("Login credentials refused, please retry");
                attemptLogin();
            }
        }
    }
}
