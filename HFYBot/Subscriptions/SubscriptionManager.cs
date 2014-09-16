using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace HFYBot.Subscriptions
{
    //Deals with users directly asking the bot to do things. I know right, so needy!
    static class SubscriptionManager
    {
        public static readonly string subscriptionFile = "subscriptions.xml";

        static SortedList<string, SubscribedAuthor> subscribedAuthors = new SortedList<string, SubscribedAuthor>(0);

        //Checks the bot's inbox for new mail/comments.
        public static void checkInbox()
        {

        }

        public static void loadSubscriptionsFromFile()
        {
            if (System.IO.File.Exists(subscriptionFile))
            {
                System.IO.File.Create(subscriptionFile);
                XmlWriterSettings set = new XmlWriterSettings();
                XmlWriter writer = XmlWriter.Create(subscriptionFile, set);
                writer.WriteStartDocument();
                writer.WriteStartElement("Subscribtions");
                writer.WriteStartElement("AuthorSubscriptions");
                writer.WriteEndElement();
                writer.WriteEndElement();
                writer.WriteEndDocument();
                writer.Flush();
                writer.Close();
            }
            else
            {
                XmlDocument doc = new XmlDocument();
                doc.LoadXml(subscriptionFile);
                XmlNodeList authors = doc.SelectNodes("/Subscriptions/AuthorSubscriptions/Author");
                foreach (XmlNode author in authors)
                {
                    string name = author.Attributes["name"].Value;
                    SubscribedAuthor sub = new SubscribedAuthor(author.Attributes["name"].Value);
                    subscribedAuthors.Add(name, sub);
                    XmlNodeList subscribers = author.SelectNodes("/Subscriber");
                    foreach (XmlNode subscriber in subscribers)
                    {
                        sub.subscribers.Add(subscriber.InnerText);
                    }
                }
            }
            
        }

        public static void addSubscriber(string author, string subscriber)
        {

        }

        public static bool removeSubscriber(string author, string subscriber)
        {


            return false;
        }

        public static string[] checkSubscribers(string author)
        {

            return null;
        }

        public static string[] checkSubscriptions(string user)
        {

            return null;
        }
            
    }
}
