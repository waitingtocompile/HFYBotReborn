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

            //This is unfinished, it currently only deletes comments of score less than -1, and isn't very good at that anyway.
            //foreach (Comment comment in user.Comments)
            //{
            //    if (comment.Upvotes - comment.Downvotes < 0)
            //    {
            //        //Console.WriteLine("Comment on {0} removed, score less than 0", comment.Author);
            //        //comment.Remove();
            //    }
            //}

            //This segment is dedicating to placing the actual comments. It is currently first iteration, so is mostly placeholder, has no checks for if there is already a comment etc.
            foreach (Post post in sub.New.Take(5))
            {
                if (post.IsSelfPost && post.Comments.Where(com => com.Author == user.Name).Count() < 1)
                {
                    Console.Write("Self post found, \"{0}\", adding comment...", post.Title);
                    string commentString = generateComment(post);
                    try
                    {
                        Comment com = post.Comment(commentString);
                        Console.WriteLine("  added!");
                    }
                    catch (RateLimitException e)
                    {
                        //for the time being this is a bodged solution, Ideally I would be shoving all of this onto it's own thread
                        Console.Write("\nRate limit hit, retrying in {0}... ", e.TimeToReset);
                        System.Threading.Thread.Sleep(e.TimeToReset);
                        Comment com = post.Comment(commentString);
                        Console.WriteLine("Done!");
                    }
                    
                }
            }

            

            Console.Write("Press Any Key...");
            Console.Read();
            
        }


        static string generateComment(Post originPost)
        {
            List<Post> allPosts = originPost.Author.Posts.ToList();
            List<Post> relevantPosts = new List<Post>(0);

            foreach (Post post in allPosts)
            {
                if (post.Subreddit == sub.Name && post.IsSelfPost)
                    relevantPosts.Add(post);
            }

            if (relevantPosts.Count <= 1)
            {
                return "[u/" + originPost.Author.Name + "](www.reddit.com/u/" + originPost.Author.Name + ") has not yet posted any other stories";
            }
            else
            {
                string comment = "There are " + relevantPosts.Count.ToString() + " stories by [u/" + originPost.Author.Name + "](http://reddit.com/u/" + originPost.Author.Name + ") including:\n\n---------\n\n";
                for (int i = 0; i < relevantPosts.Count && i < 20; i++)
                {
                    if (relevantPosts[i] != originPost)
                        comment += "\n\n* [" + relevantPosts[i].Title + "](" + relevantPosts[i].Url + ")";
                }
                return comment;
            }
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
