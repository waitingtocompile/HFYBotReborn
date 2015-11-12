using System;
using System.Threading;
using System.Collections.Generic;

using RedditSharp;
using RedditSharp.Things;

#if DEBUG
using System.Diagnostics;
#endif

namespace HFYBot
{
   /// <summary>
   /// Module that deals with new posts on the subreddit.
   /// </summary>
   public class ProcessNewPosts
   {
      /// <summary>
      /// This module's instance of the Reddit API. Typically the one defined in Program.
      /// </summary>
      protected Reddit reddit;

      /// <summary>
      /// The Subreddit the module will operate in. If no specific subreddit is requires, can be left blank for /r/all.
      /// </summary>
      protected Subreddit sub;

      /// <summary>
      /// Initializes a new instance of the ProcessNewPosts class.
      /// </summary>
      /// <param name="reddit">Reddit API instance</param>
      /// <param name="sub">Target subreddit</param>
      public ProcessNewPosts(Reddit _reddit, Subreddit _sub)
      {
         reddit = _reddit;
         sub = _sub;
      }

      /// <summary>
      /// Scan for new posts and add comment linking to author's other works.
      /// This works by pulling the HFY bot's comments and keeping a hash map of the url ids for the parent subjects (optimize later: cache these locally).
      /// Then it gets a list of the post titles and url/ids. Any that are not in the list are added to the list of stuff to process.
      /// </summary>
      /// <param name="maxCounttoGet">The number of posts to check</param>
      public void Run(int maxCounttoGet, int maxAgeInDays, bool fastcutoff, bool reallypost, bool verbose)
      {
         Dictionary<string, int> botHasAlreadyPostedToThese = new Dictionary<string, int>();   // this will be the ones that we don't care about and don't need to check any more
         // at the most we will care about the most recent 500 posts (which is a LOT), but we also do a cutoff of 90 days (later revise down to 30 days or less)

         DateTime mostRecentlyMadeCommentDT = new DateTime();  // if the -fastcutoff flag is passed, then this is used. Note that if the bot fails or the network drops during processing, then this will cause older posts to be missed.
         DateTime maxAgeToLook = DateTime.Now.AddDays(-maxAgeInDays);

         const int maxBotCommentsToBuffer = 10000;

         try
         {
            if (verbose)
            {
               Console.WriteLine("");
               Console.WriteLine("-----------------------------------------------------------------");
               Console.WriteLine("Reading existing bot comments and building list of previous work.");
               Console.WriteLine("-----------------------------------------------------------------");
               Console.WriteLine("");
            }

            Listing<Comment> botComments = reddit.User.GetComments(Sort.New, 100);
            int count = 0;
            foreach (Comment comment in botComments)
            {
               if (count == 0)
                  mostRecentlyMadeCommentDT = comment.Created; // the first one will be the most recently made one; we only need to check the comments in posts made or edited AFTER this time and date.

               ++count;

               if (verbose)
                  Console.WriteLine("#{0}:{1} (parentID={2}, date={3}", count, comment.LinkTitle, comment.ParentId, comment.Created);

               string parentID = comment.ParentId; // parentID will be "t3_3ph64o" for example - the t3_ needs to be removed
               if (parentID.StartsWith("t3_"))
                  parentID = parentID.Remove(0, 3);
               botHasAlreadyPostedToThese[parentID] = 1;

               if (count >= maxBotCommentsToBuffer)
                  break;
               // we have to key off all of the comments so we know which posts we have done (which may be a boatload)
//                if (comment.Created < maxAgeToLook)
//                   break;   // we've looked at enough, we're never going to consider posts older than this anyways (plus Reddit doesn't allow us to comment on posts older than 90 days or so)
            }
         }
         catch (System.Net.WebException we)
         {
            Console.WriteLine("Unable to read bot comments, network is possibly down {0}.", we.ToString());
            return;
         }

         // Now get the list of post from Reddit in the sub, and check to see if we've done enough.
         // We will stop once we start getting posts that are older than the last time we ran (taken from the most-recent comment the bot made), or we hit the maxCounttoGet limit.
         // We pre-filter out any post that doesn't have either [OC] in the TITLE or [OC] in the FLAIR.
         // We pre-filter out any post that has ALREADY been processed by our bot, in that the post ID value is already in the botHasAlreadyPostedToThese hash map.
         try
         {
            if (verbose)
            {
               Console.WriteLine("");
               Console.WriteLine("--------------------------------------------------------------------------");
               Console.WriteLine("Reading recent posts from sub and checking against previous work and [OC].");
               Console.WriteLine("--------------------------------------------------------------------------");
               Console.WriteLine("");
            }

            Dictionary<string, string> authorComments = new Dictionary<string, string>(0); // authorname->comment block

            int count = 0;

            IEnumerable<Post> posts = sub.New.GetListing(maxCounttoGet); // post count should be fairly large and be cut off by a given date.
            foreach (Post post in posts)
            {
               if (!post.Equals(null))
               {
                  ++count;

                  if (verbose)
                     Console.WriteLine("#{0}: title={1}, id={2}, createdOn={3}", count, post.Title, post.Id, post.Created);

                  post.SelfText = null;   // attempt to reduce space usage by dereffing the big text blocks
                  post.SelfTextHtml = null;

                  // if the json value for Edited is false, then the post was never edit and the DateTime value for post.Edited will be 1/1/1970. We just want the most-recent time between created and edited.
                  DateTime whenTouched = post.Created;
                  if (post.Edited > whenTouched)
                     whenTouched = post.Edited;

                  // Now check if this post was done *before* the last time the HFYBOT ran. If so, then it's already been scanned and either found lacking or processed in some other way.
                  // Downside: if the bot fails or the network craps out before this then there may be a big fat block of stuff that gets missed.
                  if (fastcutoff && (whenTouched < mostRecentlyMadeCommentDT))
                  {
                     if (verbose)
                        Console.WriteLine("Post {0} is older than the last time bot succesfully ran, breaking early.", post.Title);
                     break;
                  }

                  if (whenTouched < maxAgeToLook)
                  {
                     if (verbose)
                        Console.WriteLine("Reached age limit on posts, exiting the scan.", post.Title);
                     break;
                  }

                  if (botHasAlreadyPostedToThese.ContainsKey(post.Id))
                  {
                     if (verbose)
                        Console.WriteLine("Post {0} already has a comment by this bot user, skipping.", post.Title);
                     continue;   // skip to the next one, we've already processed this (if fastcutoff set, then this will probably not trigger)
                  }

                  if (isOC(post))
                  {
                     // this post has not been visited by the bot and we will want to process it. Make the comment block if we haven't already and then post it.
                     string authorName = post.AuthorName;
                     if (!authorComments.ContainsKey(authorName))
                     {
                        authorComments[authorName] = generateCommentText(authorName);
                     }

                     string commentText = authorComments[authorName];

                     if (commentText!="")
                     {
                        if (reallypost)
                        {
                           if (verbose)
                           {
                              Console.WriteLine("--------------------------------------------------------------------------");
                              Console.WriteLine("Adding comment to {0}:\n{1}", post.Title, commentText);
                              Console.WriteLine("--------------------------------------------------------------------------");
                           }
                           else
                              Console.WriteLine("Adding comment to {0}", post.Title);
                           
                           post.Comment(commentText);
                        }
                        else
                        {
                           Console.WriteLine("--------------------------------------------------------------------------");
                           Console.WriteLine("Would have added comment {0}:\n{1}", post.Title, commentText);
                           Console.WriteLine("--------------------------------------------------------------------------");
                        }
                     }
                  }
               }
            }
         }
         catch (System.Net.WebException we)
         {
            Console.WriteLine("Unable to read the posts from the sub, network is possibly down {0}.", we.ToString());
         }
      }

      /// <summary>
      /// Generates the comment text, with at most 25 linked items from the author.
      /// Header will include a total count and a signature line.
      /// </summary>
      /// <returns>The comment text.</returns>
      /// <param name="user">User to generate text for.</param>
      string generateCommentText(string username)
      {
         int count = 0;
         string comm = "";
         Listing<Post> allPosts = reddit.GetPostsByUser(username, Sort.New, 100);

         foreach (Post post in allPosts)
         {
            if (post.SubredditName.Equals(sub.Name) && isOC(post))
            {
               if (count < 25)
                  comm += "\n\n* [" + post.Title + "](" + post.Url + ")";
               post.SelfText = null;   // attempt to reduce space usage by dereffing the big text blocks
               post.SelfTextHtml = null;

               ++count;
            }
         }

         if (count > 1)
         {
            string leadin = "There are " + count + " stories by " + userLink(username) + ", including:";
            comm = leadin + comm;
         }
         else
         {
            comm = "There are no other stories by " + userLink(username) + " at this time.";
         }

         comm += "\n\nThis list was automatically generated by HFYBotReborn version "
            + Program.version
            + ". Please contact " + botmailLink("KaiserMagnus") + " or " + botmailLink("j1xwnbsr") + " if you have any queries. This bot is [open source](https://github.com/waitingtocompile/HFYBotReborn).";
         return comm;
      }

      string userLink(string username)
      {
         return "[" + username + "](https://reddit.com/u/" + username + ")";
      }

      string botmailLink(string username)
      {
         return "[" + username+ "](https://www.reddit.com/message/compose?to=" + username + "&subject=HFYBot&message=)";
      }

      /// <summary>
      /// Checks if post is OC or PI (either has OC/PI flair or [OC]/[PI] as part of the title)
      /// OC = Orginal Content; PI = Prompt Inspired
      /// </summary>
      /// <returns><c>true</c>, if is OC, <c>false</c> otherwise.</returns>
      /// <param name="post">Post to check</param>
      bool isOC(Post post)
      {
         if (post.Equals(null))
            return false;
         if (post.Title.ToUpperInvariant().Contains("[OC]") || post.Title.ToUpperInvariant().Contains("[PI]"))
            return true;
         if (!string.IsNullOrEmpty(post.LinkFlairText) && (post.LinkFlairText.ToUpperInvariant().Equals("OC") || post.LinkFlairText.ToUpperInvariant().Equals("PI")))
            return true;
         return false;
      }

   }
}

