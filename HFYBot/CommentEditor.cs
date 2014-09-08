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
    //Responsible for editing older posts when the author has posted something new.
    class CommentEditor
    {
        //The delay between each editing pass
        public static readonly TimeSpan editFrequency = new TimeSpan(0, 30, 0);

        //The list of pending users, because everyone likes generic lists.
        public static List<RedditUser> pendingUsers = new List<RedditUser>(0);

        //Iterates through comments on posts by users who are marked as pending.
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

        //This is the method called by the thread. It will run until the end of time. Or until the program is closed, on of the two.
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
