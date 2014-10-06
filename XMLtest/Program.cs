using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml;

namespace XMLtest
{
    class Program
    {
        static void Main(string[] args)
        {
            
            string subscriptionFile = "sub.xml";

            XmlDocument doc = new XmlDocument();
            if (!System.IO.File.Exists(subscriptionFile))
            {
                Console.Write("Yay");
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
            }
            else
            {
                Console.Write("Different yay");
                XmlNodeList authors = doc.SelectNodes("/Subscriptions/AuthorSubscriptions/Author");
                foreach (XmlNode author in authors)
                {
                    string name = author.Attributes["name"].Value;
                    XmlNodeList subscribers = author.SelectNodes("/Subscriber");
                    foreach (XmlNode subscriber in subscribers)
                    {
                        
                    }
                }
            }
            doc.Load(subscriptionFile);

            XmlElement element = (XmlElement)doc.SelectSingleNode("/Subscribtions/AuthorSubscriptions");
            XmlElement newElement = doc.CreateElement("Author");
            if (element == null)
            {
                Console.Write(":(");
                Console.Read();
            }
            element.AppendChild(newElement);

            doc.Save(subscriptionFile);
            Console.Read();
        }
    }
}
