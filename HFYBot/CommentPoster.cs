using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using RedditSharp;
using RedditSharp.Things;

namespace HFYBot
{
    //Responsible for posting of new comments on OC. All editing is left to the comment editor.
    class CommentPoster
    {
        //The delay between each posting pass
        static readonly TimeSpan postFrequency = new TimeSpan(0, 30, 0);


        //pulls a predefined number (currently 10) from the subreddit and posts comments on self posts
        static void MakeCommentPass()
        {
            SortedList<RedditUser, string> pendingEdits = new SortedList<RedditUser, string>(0);

            //I know, the number to pull is hardcoded. I should fix that at some point. Or you could, this project is open source after all (hint hint)
            foreach (Post post in Program.sub.New.Take(10))
            {
                
                if (checkPostElegibility(post) && post.Comments.Where(com => com.Author == Program.user.Name).Count() == 0)
                {
                    Console.Write(ConsoleUtils.TimeStamp + " Self post found, \"{0}\", adding comment... ", post.Title);
                    string commentString = generateComment(post);
                    try
                    {
                        Comment com = post.Comment(commentString);
                        Console.WriteLine("Added!");
                    }
                    catch (RateLimitException e)
                    {
                        Console.Write("\nRate limit hit, retrying in {0}... ", e.TimeToReset);
                        System.Threading.Thread.Sleep(e.TimeToReset);
                        Comment com = post.Comment(commentString);
                        Console.WriteLine("Done!");
                    }

                    List<string> users = Subscriptions.SubscriptionManager.checkSubscriptions(post.AuthorName);

                    if (users != null)
                    {
                        string messageString = Subscriptions.SubscriptionManager.generateSubscriptionMessage(post);
                        foreach (string name in users)
                        {
                            Program.redditInstance.ComposePrivateMessage("HFYBot Subscription service", messageString, name);
                        }
                    }

                    if (pendingEdits.Keys.Contains(post.Author))
                        pendingEdits[post.Author] = commentString;
                    else
                        pendingEdits.Add(post.Author, commentString);

                }
            }
            for (int i = 0; i < pendingEdits.Count; i++)
            {
                makeMassEdit(pendingEdits.Keys[i], pendingEdits.Values[i]);
            }

        }

        static void makeMassEdit(RedditUser user, string commentString)
        {
            List<Post> allPosts = user.Posts.ToList();

            foreach (Post post in allPosts)
            {
                if (checkPostElegibility(post))
                {
                    foreach (Comment comment in post.Comments)
                    {
                        if (comment.Author == Program.user.Name)
                        {
                            comment.EditText(CommentPoster.generateComment(post));
                        }
                    }
                }
            }
        }

        static bool checkPostElegibility(Post post)
        {
            return (post.Subreddit == Program.sub.Name && post.IsSelfPost && (post.LinkFlairText == "OC" | post.Title.Substring(0, 4).Equals("[OC]", StringComparison.InvariantCultureIgnoreCase)));
        }

        //Generates actual text for comments. It will never list more than 20 links, otherwise it would hit the character limit on /u/battletoad's posts. This method is also used by the CommentEditor to avoid code duplication.
        static string generateComment(Post originPost)
        {
            List<Post> allPosts = originPost.Author.Posts.ToList();
            List<Post> relevantPosts = new List<Post>(0);

            foreach (Post post in allPosts)
            {
                if (checkPostElegibility((post)))
                    relevantPosts.Add(post);
            }

            if (relevantPosts.Count <= 1)
            {
                return "[u/" + originPost.Author.Name + "](http://reddit.com/u/"
                    + originPost.Author.Name
                    + ") has not yet posted any other stories\n\n---\n\n" + Program.footer;
            }
            else
            {
                string comment = "There are " + relevantPosts.Count.ToString() + " stories by [u/" + originPost.Author.Name + "](http://reddit.com/u/" + originPost.Author.Name + ") including:\n\n---------\n\n";
                for (int i = 0; i < relevantPosts.Count && i < 20; i++)
                {
                    if (relevantPosts[i] != originPost)
                        comment += "\n\n* [" + relevantPosts[i].Title + "](" + relevantPosts[i].Url + ")";
                }
                comment += "\n\n---\n\n" + Program.footer;
                return comment;
            }
        }

        //This is the method called by the thread. It will run until the end of time. Or until the program is closed, one of the two.
        public static void Run()
        {
            for (; ; )
            {
                Console.WriteLine(ConsoleUtils.TimeStamp + " Beginning comment posting pass");
                MakeCommentPass();
                Console.WriteLine(ConsoleUtils.TimeStamp + " Comment posting Pass completed");

                Thread.Sleep(postFrequency);
            }
        }
    }
}
