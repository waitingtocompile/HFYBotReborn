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
        public static List<RedditUser> pendingUsers = new List<RedditUser>()

        public static void MakeEditPass()
        {

            //This is unfinished, it currently only deletes comments of score less than -1, and isn't very good at that anyway.
            //foreach (Comment comment in user.Comments)
            //{
            //    if (comment.Upvotes - comment.Downvotes < 0)
            //    {
            //        //Console.WriteLine("Comment on {0} removed, score less than 0", comment.Author);
            //        //comment.Remove();
            //    }
            //}
        }
    }
}
