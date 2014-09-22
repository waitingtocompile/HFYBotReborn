using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace HFYBot.Subscriptions
{
    //Deals with users directly asking the bot to do things. I know right, so needy!
    static class SubscriptionManager
    {
        public static readonly string subscriptionFile = "subscriptions.xml";

        static SortedList<string, SubscribedAuthor> subscribedAuthors = new SortedList<string, SubscribedAuthor>(0);

        static XmlDocument doc = new XmlDocument();

        //Checks the bot's inbox for new mail/comments.
        public static void checkInbox()
        {
            
        }

        public static void loadSubscriptionsFromFile()
        {
            if (!System.IO.File.Exists(subscriptionFile))
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

        static XmlNode getAuthorNode(string authorName){
            foreach (XmlNode node in doc.SelectNodes("/Subscriptions/AuthorSubscriptions/Author")){
                if(node.Attributes["name"].Value.Equals(authorName, StringComparison.InvariantCultureIgnoreCase))
                    return node;
            }
            XmlNode listingNode = doc.SelectSingleNode("/Subscriptions/AuthorSubscriptions");
            XmlElement newNode = doc.CreateElement("Author");
            newNode.SetAttribute("name", authorName);
            listingNode.AppendChild((XmlNode)newNode);
            return (XmlNode)newNode;
        }

        public static void addSubscriber(string authorName, string subscriber)
        {
            SubscribedAuthor author;
            XmlNode node = getAuthorNode(authorName);
            if (!subscribedAuthors.TryGetValue(authorName, out author))
            {
                author = new SubscribedAuthor(authorName);
                subscribedAuthors.Add(authorName, author);
            }
            author.subscribers.Add(subscriber);
            XmlElement subscriberElement = doc.CreateElement("Subscriber");
            subscriberElement.InnerText = subscriber;
            node.AppendChild((XmlNode)subscriberElement);
            doc.Save(subscriptionFile);
        }

        public static bool removeSubscriber(string authorName, string subscriber)
        {
            SubscribedAuthor author;
            if (subscribedAuthors.TryGetValue("author", out author))
            {
                if (author.subscribers.Contains(subscriber))
                {
                    author.subscribers.Remove(subscriber);
                    XmlNode authorNode = getAuthorNode(authorName);
                    foreach (XmlNode SubscriberNode in authorNode.SelectNodes("/Subscriber"))
                    {
                        if (SubscriberNode.InnerText.Equals(subscriber, StringComparison.InvariantCultureIgnoreCase))
                        {
                            authorNode.RemoveChild(SubscriberNode);
                            break;
                        }
                    }
                    return true;
                }
            }
            return false;
        }

        public static List<string> checkSubscribers(string authorName)
        {
            SubscribedAuthor author;
            if (subscribedAuthors.TryGetValue(authorName, out author))
            {
                return author.subscribers;
            }
            return null;
        }

        public static List<string> checkSubscriptions(string user)
        {
            List<string> subscriptions = new List<string>(0);
            foreach (SubscribedAuthor author in subscribedAuthors.Values)
            {
                if(author.subscribers.Contains(user)) subscriptions.Add(author.author);
            }
            return subscriptions;
        }



        public static void Run()
        {
            doc.LoadXml(subscriptionFile);
            loadSubscriptionsFromFile();
        }
    }


    //Structure representing an author one or more users have subscibed to
    private struct SubscribedAuthor
    {

        public List<string> subscribers = new List<string>(0);
        public readonly string author;


        public SubscribedAuthor(string name)
        {
            author = name;
        }

    }
}
