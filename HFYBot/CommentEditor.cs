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
        public static List<RedditUser> pendingUsers = new List<RedditUser>(0);

        //Iterates through comments on posts by users who are marked as pending, low stress so can be called (relitaively) frequently
        public static void MakePendingEditPass()
        {
            lock(pendingUsers){
                foreach (RedditUser user in pendingUsers)
                {
                    List<Post> allPosts = user.Posts.ToList();
                    List<Post> relevantPosts = (List<Post>)allPosts.Where(p => p.Subreddit == Program.sub.Name && p.IsSelfPost);

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

        //Iterates through ALL comments, will take a long time and should be used sparingly (i.e on startup or once every x hours)
        public static void MakeGeneralEditPass()
        {
            foreach (Comment comment in Program.user.Comments)
            {
                comment.EditText(CommentPoster.generateComment(findCommentPost(comment)));
            }
        }


        static Post findCommentPost(Comment comment)
        {
            while (true)
            {
                if (comment.Parent.GetType() == typeof(Post))
                {
                    return (Post)comment.Parent;
                }
                else
                {
                    comment = (Comment)comment.Parent;
                }
            }
        }
    }
}
