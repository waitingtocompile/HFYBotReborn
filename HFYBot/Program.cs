﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Security.Authentication;

using System.Diagnostics;

using RedditSharp;
using RedditSharp.Things;

namespace HFYBot
{
    //As the name would suggest, the program itself. Simply holds the main method and a handful of general values.
    class Program
    {
        //The version number to be shown in the footer
        public const string version = "Release 1.1";

        //Shown at the bottom oof each comment below a horizontal line to provide information about the bot.
        public static string footer
        {
            get
            {
                return "This comment was automatically generated by HFYBotReloaded version "
                    + Program.version
                    + ". If You think that this bot is malfunctioning or have any questions about the bot please contact [u/KaiserMagnus](http://reddit.com/u/kaisermagnus)."
                    + "\n\nThis bot is open source and can be located [here](https://github.com/waitingtocompile/HFYBotReborn)";
            }
        }

        
        public static Reddit redditInstance
        {
            private set;
            get;
        }
        public static AuthenticatedUser user
        {
            private set;
            get;
        }
        public static Subreddit sub
        {
            private set;
            get;
        }

        //Pair of threads, I will merge these in one or two commits time. All things aside from the login are done on these threads
        static Thread editThread;
        static Thread postThread;

        //Fairly unremarkable main method. Deals with the login process then starts the two threads and lets them do the rest.
        static void Main(string[] args)
        {
            Console.WriteLine("Please input Reddit credentials (don't worry, I won't steal them)");
            redditInstance = new Reddit();
            attemptLogin();
            Console.Clear();
            Console.WriteLine(ConsoleUtils.TimeStamp + " Login sucsessful, user has " + user.UnreadMessages.Count().ToString() + " unread messages");
            sub = redditInstance.GetSubreddit("/r/HFY");

            editThread = new Thread(new ThreadStart(CommentEditor.Run));
            postThread = new Thread(new ThreadStart(CommentPoster.Run));

            postThread.Start();
            editThread.Start();
            
        }


        
        //Resposible for actually logging in. If sucsessfull it will allow the program to proceeed, if not it re prompts. This is probably not a good way to do things.
        static void attemptLogin()
        {
            try
            {
                string name = ConsoleUtils.readString("Username", true);
                string pass = ConsoleUtils.readString("Password", false);
                user = redditInstance.LogIn(name, pass);
            }
            catch (AuthenticationException)
            {
                Console.Clear();
                Console.WriteLine("Login credentials refused, please retry");
                attemptLogin();
            }
        }
    }
}
