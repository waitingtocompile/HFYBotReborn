using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

using RedditSharp;
using RedditSharp.Things;

namespace HFYBot
{
    class CommentEditor
    {
        public static readonly TimeSpan editFrequency = new TimeSpan(0, 30, 0);


        public static List<RedditUser> pendingUsers = new List<RedditUser>(0);

        //Iterates through comments on posts by users who are marked as pending, low stress so can be called (relitaively) frequently
        public static void MakePendingEditPass()
        {
            lock(pendingUsers){
                foreach (RedditUser user in pendingUsers)
                {
                    List<Post> allPosts = user.Posts.ToList();
                    List<Post> relevantPosts = new List<Post>();
                    foreach (Post post in allPosts)
                    {
                        if (post.Subreddit == Program.sub.Name && post.IsSelfPost)
                            relevantPosts.Add(post);
                    }

                    foreach (Post post in relevantPosts)
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
                pendingUsers.Clear();
            }
        }

        //Iterates through ALL comments, will take a long time and should be used sparingly (i.e on startup or once every x hours) currently disabled.
        public static void MakeGeneralEditPass()
        {
            foreach (Comment comment in Program.user.Comments)
            {
                //comment.EditText(CommentPoster.generateComment(comment.));
            }
        }


        public static void Run()
        {
            Thread.Sleep(new TimeSpan(0, 10, 0));
            for (; ; )
            {
                Console.WriteLine(ConsoleUtils.TimeStamp + " Beginning Comment edit pass");
                MakePendingEditPass();
                Console.WriteLine(ConsoleUtils.TimeStamp + " Comment edit pass completed");
                Thread.Sleep(editFrequency);
            }
        }
    }
}
