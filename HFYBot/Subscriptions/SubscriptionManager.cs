using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

using RedditSharp.Things;
using System.Diagnostics;

namespace HFYBot.Subscriptions
{
    //Deals with users directly asking the bot to do things. I know right, so needy!
    static class SubscriptionManager
    {
        public static readonly string subscriptionFile = "subscriptions.xml";

        static SortedList<string, SubscribedAuthor> subscribedAuthors = new SortedList<string, SubscribedAuthor>(0);

        static XmlDocument doc = new XmlDocument();

        static TimeSpan waitTime = new TimeSpan(0, 10, 0);

        //Checks the bot's inbox for new mail/comments.
        public static void checkInbox()
        {
            if (Program.user.HasMail)
            {
                foreach (PrivateMessage message in Program.user.UnreadMessages)
                {
                    Debug.Write(message.Body);

                    string content = message.Body;
                    if(content.Substring(0, 6).Equals("HFYBot", StringComparison.InvariantCultureIgnoreCase)){
                        //string content = message.Subject;
                        string[] tokens = content.Split(' ');
                        for (int i = 0; i < tokens.Length; i++)
                            tokens[i] = tokens[i].ToLowerInvariant();

                        switch (tokens[1])
                        {
                            case("subscribe"):
                                try
                                {
                                    Program.redditInstance.GetUser(tokens[2]);
                                    addSubscriber(tokens[2], message.Author);
                                    message.Reply("Your have now been subscribed to " + tokens[2] + ", you will be messaged when they post new content. See here(beta is beta) for more options.");
                                    //ayy llamo
                                }
                                catch (System.Net.WebException e)
                                {
                                    message.Reply("I can't find a user by that name. Did you do something wrong?");
                                }
                                break;
                                
                            case("checksubscriptions"):
                                List<String> authors = checkSubscriptions(message.Author);
                                if (authors.Count == 0)
                                    message.Reply("You aren't subscribed to anyone!");
                                else
                                {
                                    string reply = "You are subscribed to:";
                                    foreach (string s in authors)
                                        reply += "\n\n" + s;
                                }
                                break;

                            case("checksubscribers"):
                                List<String> subs = checkSubscribers(message.Author);
                                if (subs.Count == 0)
                                    message.Reply("You have no subscribers!");
                                else
                                {
                                    string reply = "Your subscribers are:";
                                    foreach (string s in subs)
                                        reply += "\n\n" + s;
                                }
                                break;

                            case("unsubscribe"):
                                try{
                                    if (removeSubscriber(tokens[2], message.Author))
                                        message.Reply("Your have now been unsubscribed from " + tokens[2] + ", you will no longer be messaged when they post new content. See here(TODO) for more options.");
                                        else message.Reply("You don't seem to be subscribed to someone by that name. Did you do mis-spell their name (you can check you subscriptions by messaging me with:\n\n    HFYBot checkSubscriptions");
                                } catch (IndexOutOfRangeException e){
                                    message.Reply("I can't unsubscibe you unless you tell me who.");
                                }
                                
                                break;

                            default:
                                message.Reply("My automated systems have no reponse to that. There is a list here (LOLNOPE, beta is beta)");
                                break;
                        }
                        message.SetAsRead();
                    }
                }
            }
        }

        public static void loadSubscriptionsFromFile()
        {
            if (!System.IO.File.Exists(subscriptionFile))
            {
                System.IO.File.Create(subscriptionFile).Dispose();
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
                doc.Load(subscriptionFile);
            }
            else
            {
                doc.Load(subscriptionFile);
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

        static void optimiseFile()
        {
            foreach (SubscribedAuthor author in subscribedAuthors.Values)
            {
                if (author.subscribers.Count == 0)
                    getAuthorNode(author.author).RemoveAll();
            }
            doc.Save(subscriptionFile);
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
            loadSubscriptionsFromFile();
            for (; ; )
            {
                checkInbox();
                Console.WriteLine(ConsoleUtils.TimeStamp + " MailCheck finished. User has {0} unread messages remaining", Program.user.UnreadMessages.Count().ToString());
                optimiseFile();
                Thread.Sleep(waitTime);
            }

        }
    }


    //Structure representing an author one or more users have subscibed to
    struct SubscribedAuthor
    {

        public List<string> subscribers;
        public readonly string author;


        public SubscribedAuthor(string name)
        {
            author = name;
            subscribers = new List<string>(0);
        }

    }
}
