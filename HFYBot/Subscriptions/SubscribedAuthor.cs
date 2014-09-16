using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace HFYBot.Subscriptions
{
    //Class representing an author one or more users have subscibed to
    class SubscribedAuthor
    {

        public List<string> subscribers = new List<string>(0);
        string author { public get; private set; }


        public SubscribedAuthor(string name)
        {
            author = name;
        }

    }
}
