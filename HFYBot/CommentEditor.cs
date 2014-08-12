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

        //Iterates through comments on posts by users who are marked as pending, low stress so can be called frequently
        public static void MakePendingEditPass()
        {
            lock(pendingUsers){
                foreach (RedditUser user in pendingUsers)
                {
                    //TODO: actual iteration code
                }
                pendingUsers.Clear();
            }
        }

        //Iterates through ALL comments, will take a long time and should be used sparingly (i.e on startup or once every x hours)
        public static void MakeGeneralEditPass()
        {

        }
    }
}
