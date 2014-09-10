using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HFYBot.Subscriptions
{
    //Class representing an author one or more users have subscibed to
    class SubscribedAuthor
    {
        public const string SUBSCRIPTIONS_FILE_PATH = "subscriptions.xml";

        XmlWriterSettings writerSettings = new XmlWriterSettings();

        public List<string> subscribers = new List<string>(0);
        string author;


        public SubscribedAuthor(string name)
        {
            writerSettings = new XmlWriterSettings();
            writerSettings.Indent = true;
            writerSettings.WriteEndDocumentOnClose = true;
        }

    }
}
