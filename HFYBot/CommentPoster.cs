using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using RedditSharp;
using RedditSharp.Things;

namespace HFYBot
{
    class CommentPoster
    {

        //pulls a predefined number (currently 5) from the subreddit and posts comments on self posts
        public static void MakeCommentPass()
        {
            foreach (Post post in Program.sub.New.Take(5))
            {
                
                if (post.IsSelfPost && post.Comments.Where(com => com.Author == Program.user.Name).Count() < 1)
                {
                    Console.Write("Self post found, \"{0}\", adding comment... ", post.Title);
                    string commentString = generateComment(post);
                    try
                    {
                        Comment com = post.Comment(commentString);
                        Console.WriteLine("Added!");
                    }
                    catch (RateLimitException e)
                    {
                        //for the time being this is a bodged solution, Ideally I would be shoving all of this onto it's own thread
                        Console.Write("\nRate limit hit, retrying in {0}... ", e.TimeToReset);
                        System.Threading.Thread.Sleep(e.TimeToReset);
                        Comment com = post.Comment(commentString);
                        Console.WriteLine("Done!");
                    }
                    lock (CommentEditor.pendingUsers)
                    {
                        CommentEditor.pendingUsers.Add(post.Author);
                    }

                }
            }
        }



        public static string generateComment(Post originPost)
        {
            

            List<Post> allPosts = originPost.Author.Posts.ToList();
            List<Post> relevantPosts = new List<Post>(0);

            foreach (Post post in allPosts)
            {
                if (post.Subreddit == Program.sub.Name && post.IsSelfPost)
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
    }


}
