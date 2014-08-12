using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Authentication;

using System.Diagnostics;

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
            sub = redditInstance.GetSubreddit("/r/Bottest");


            foreach (Comment comment in user.Comments)
            {
                Console.WriteLine("{0}      {1}, {2}, {3}", comment.Body, comment.Upvotes, comment.Downvotes, comment.Upvotes - comment.Downvotes);
            }

            foreach (Post post in sub.New.Take(5))
            {
                if (post.IsSelfPost)
                {
                    Console.Write("Self post found, \"{0}\", adding comment...", post.Title);
                    try
                    {
                        Comment com = post.Comment("This is an early development bot. It *should* delete on -1 votes. I'm kind of still working on this bit. Feel free to downvote it anyway.");
                        Console.WriteLine("  added!");
                    }
                    catch (RateLimitException e)
                    {
                        
                        Console.Write("\nRate limit hit, retrying in {0}... ", e.TimeToReset);
                        System.Threading.Thread.Sleep(e.TimeToReset);
                        Comment com = post.Comment("This is an early development bot. It *should* delete on -1 votes. I'm kind of still working on this bit. Feel free to downvote it anyway.");
                        Console.WriteLine("Done!");
                    }
                    
                }
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
