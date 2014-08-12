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
    //As the name would suggest, the program itself. At the moment everything is sitting in main, which while bad practisce was quick and easy. Once I start breaking things up into their own threads this would be less of a problem.
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

            //This is a placeholder for response to existing comments. At present it just prints information on the screen. Ultimately it should be checking the comment's score and deleting if appropriate.
            foreach (Comment comment in user.Comments)
            {
                Console.WriteLine("{0}      {1}, {2}, {3}", comment.Body, comment.Upvotes, comment.Downvotes, comment.Upvotes - comment.Downvotes);
            }

            //This segment is dedicating to placing the actual comments. It is currently first iteration, so is mostly placeholder, has no checks for if there is already a comment etc.
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
                        //for the time being this is a bodged solution, Ideally I would be shoving all of this onto it's own thread
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
