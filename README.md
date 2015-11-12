HFYBotReborn version 2.1
========================

This bot is used by /r/HFY to scan for recent posts (configurable) and add a comment consisting of recent posts by the same author. If the author is new and this is their first submission to /r/HFY, then the comment will indicate as such.

It works by logging into a specific account and reading the last set of comments made by the bot; the parent post ID is parsed out, and the last NN days (configurable, defaults to 60, 7 is typical) of posts to the sub are read. Any that have the proper tags (either [OC] or [PI] in the title or flair) that are not in the most-recent comments by the bot will get a comment added.

The bot has basically been re-written to remove the unused modules and run as a command-line program. One of the major design restrictions was that it *must* run under Windows XP and be triggered by a Windows Scheduler - as such, it must be compatible with .NET 3.5. This forced a pull of the RedditSharp library which was then modified to remove the .NET 4.5 features that prevented the program from running (async functions and default parms). These have either been replaced with appropriate "thunker" functions or if-def�d out.

The RedditSharp library has also been modified to add additional functionality that helps with the performance of HFYBot, allowing to post a comment without re-loading the entire submission or scanning it.


The directory 3rdParty contains the dll libraries from HtmlAgilityPack, Newtonsoft.Json, and System.Threading that allows this version of RedditSharp to run. They are all 2.0 or 3.5 .NET version compatible.

The .sln and .csproj files are Visual Studio 2008 versions, which allows for a more broadly compatible 3.5 build than 2015.


       -user=username        username of the account to run this bot under (required).
       -pass=password        password for the account to run this bot under (required).
       -oauthid=id           client id as generated by https://www.reddit.com/prefs/apps (optional).
       -oauthsecret=secret   client secret as generated by https://www.reddit.com/prefs/apps (optional).
       -sub=subbedit         subbedit to process (optional, defaults to HFY).
       -maxcount=max         max # of posts to scan (optional, defaults to 500).
       -maxdays=max          max # of days to scan into the past for unprocessed posts (optional, defaults to 60).
       -fastcutoff           if set, then only considers posts made after the last HFY bot comment.
       -reallypost           if set, the posts are actually made; if not set, then dumped to console.
   -v, -verbose              if set, then progress lines are displayed as the posts and comments are built.
   -q, -quiet                if set, then as little output as possible is generated.
   -h, -help                 show this help.
