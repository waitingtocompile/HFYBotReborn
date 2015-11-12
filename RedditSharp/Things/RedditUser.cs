using System;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RedditSharp.Things
{
   public class RedditUser : Thing
   {
      private const string OverviewUrl = "/user/{0}.json";
      private const string CommentsUrl = "/user/{0}/comments.json";
      private const string LinksUrl = "/user/{0}/submitted.json";
      private const string SubscribedSubredditsUrl = "/subreddits/mine.json";
      private const string LikedUrl = "/user/{0}/liked.json";
      private const string DislikedUrl = "/user/{0}/disliked.json";

      private const int MAX_LIMIT = 100;

      public RedditUser Init(Reddit reddit, JToken json, IWebAgent webAgent)
      {
         CommonInit(reddit, json, webAgent);
         JsonConvert.PopulateObject(json["name"] == null ? json["data"].ToString() : json.ToString(), this,
             reddit.JsonSerializerSettings);
         return this;
      }

#if (_HAS_ASYNC_)
     public async Task<RedditUser> InitAsync(Reddit reddit, JToken json, IWebAgent webAgent)
     {
         CommonInit(reddit, json, webAgent);
         await Task.Factory.StartNew(() => JsonConvert.PopulateObject(json["name"] == null ? json["data"].ToString() : json.ToString(), this,
             reddit.JsonSerializerSettings));
         return this;
      }
#endif

      private void CommonInit(Reddit reddit, JToken json, IWebAgent webAgent)
      {
         base.Init(json);
         Reddit = reddit;
         WebAgent = webAgent;
      }

      [JsonIgnore]
      protected Reddit Reddit { get; set; }

      [JsonIgnore]
      protected IWebAgent WebAgent { get; set; }

      [JsonProperty("name")]
      public string Name { get; set; }

      [JsonProperty("is_gold")]
      public bool HasGold { get; set; }

      [JsonProperty("is_mod")]
      public bool IsModerator { get; set; }

      [JsonProperty("link_karma")]
      public int LinkKarma { get; set; }

      [JsonProperty("comment_karma")]
      public int CommentKarma { get; set; }

      [JsonProperty("created")]
      [JsonConverter(typeof(UnixTimestampConverter))]
      public DateTime Created { get; set; }

      public Listing<VotableThing> Overview
      {
         get
         {
            return new Listing<VotableThing>(Reddit, string.Format(OverviewUrl, Name), WebAgent);
         }
      }

      public Listing<Post> LikedPosts
      {
         get
         {
            return new Listing<Post>(Reddit, string.Format(LikedUrl, Name), WebAgent);
         }
      }

      public Listing<Post> DislikedPosts
      {
         get
         {
            return new Listing<Post>(Reddit, string.Format(DislikedUrl, Name), WebAgent);
         }
      }

      public Listing<Comment> Comments
      {
         get
         {
            return new Listing<Comment>(Reddit, string.Format(CommentsUrl, Name), WebAgent);
         }
      }

      public Listing<Post> Posts
      {
         get
         {
            return new Listing<Post>(Reddit, string.Format(LinksUrl, Name), WebAgent);
         }
      }

      public Listing<Subreddit> SubscribedSubreddits
      {
         get
         {
            return new Listing<Subreddit>(Reddit, SubscribedSubredditsUrl, WebAgent);
         }
      }

      /// <summary>
      /// Get a listing of comments and posts from the user sorted by newest-first since their first comment, in batches of 25.
      /// </summary>
      /// <returns>The listing of comments and posts requested.</returns>
      public Listing<VotableThing> GetOverview()
      {
         return GetOverview(Sort.New, 25, FromTime.All);
      }

      /// <summary>
      /// Get a listing of comments and posts from the user sorted by <paramref name="sorting"/> since their first comment, in batches of 25.
      /// </summary>
      /// <param name="sorting">How to sort the comments and posts (hot, new, top, controversial).</param>
      /// <returns>The listing of comments and posts requested.</returns>
      public Listing<VotableThing> GetOverview(Sort sorting)
      {
         return GetOverview(sorting, 25, FromTime.All);
      }

      /// <summary>
      /// Get a listing of comments and posts from the user sorted by <paramref name="sorting"/> since their first comment, limited to <paramref name="limit"/>.
      /// </summary>
      /// <param name="sorting">How to sort the comments and posts (hot, new, top, controversial).</param>
      /// <param name="limit">How many comments and posts to fetch per request. Max is 100.</param>
      /// <returns>The listing of comments and posts requested.</returns>
      public Listing<VotableThing> GetOverview(Sort sorting, int limit)
      {
         return GetOverview(sorting, limit, FromTime.All);
      }
      
      /// <summary>
      /// Get a listing of comments and posts from the user sorted by <paramref name="sorting"/>, from time <paramref name="fromTime"/>
      /// and limited to <paramref name="limit"/>.
      /// </summary>
      /// <param name="sorting">How to sort the comments and posts (hot, new, top, controversial).</param>
      /// <param name="limit">How many comments and posts to fetch per request. Max is 100.</param>
      /// <param name="fromTime">What time frame of comments and posts to show (hour, day, week, month, year, all).</param>
      /// <returns>The listing of comments and posts requested.</returns>
      public Listing<VotableThing> GetOverview(Sort sorting, int limit, FromTime fromTime)
      {
         if ((limit < 1) || (limit > MAX_LIMIT))
            throw new ArgumentOutOfRangeException("limit", "Valid range: [1," + MAX_LIMIT + "]");
         string overviewUrl = string.Format(OverviewUrl, Name);
         overviewUrl += string.Format("?sort={0}&limit={1}&t={2}", Enum.GetName(typeof(Sort), sorting), limit, Enum.GetName(typeof(FromTime), fromTime));

         return new Listing<VotableThing>(Reddit, overviewUrl, WebAgent);
      }

 
      /// <summary>
      /// Get a listing of comments from the user sorted by newest-first since their first comment, in batches of 25.
      /// </summary>
      /// <returns>The listing of comments requested.</returns>
      public Listing<Comment> GetComments()
      {
         return GetComments(Sort.New, 25, FromTime.All);
      }

      /// <summary>
      /// Get a listing of comments from the user sorted by <paramref name="sorting"/> since their first comment, in batches of 25.
      /// </summary>
      /// <param name="sorting">How to sort the comments (hot, new, top, controversial).</param>
      /// <returns>The listing of comments requested.</returns>
      public Listing<Comment> GetComments(Sort sorting)
      {
         return GetComments(sorting, 25, FromTime.All);
      }

      /// <summary>
      /// Get a listing of comments from the user sorted by <paramref name="sorting"/> since their first comment, limited to <paramref name="limit"/>.
      /// </summary>
      /// <param name="sorting">How to sort the comments (hot, new, top, controversial).</param>
      /// <param name="limit">How many comments to fetch per request. Max is 100.</param>
      /// <returns>The listing of comments requested.</returns>
      public Listing<Comment> GetComments(Sort sorting, int limit)
      {
         return GetComments(sorting, limit, FromTime.All);
      }
      
      /// <summary>
      /// Get a listing of comments from the user sorted by <paramref name="sorting"/>, from time <paramref name="fromTime"/>
      /// and limited to <paramref name="limit"/>.
      /// </summary>
      /// <param name="sorting">How to sort the comments (hot, new, top, controversial).</param>
      /// <param name="limit">How many comments to fetch per request. Max is 100.</param>
      /// <param name="fromTime">What time frame of comments to show (hour, day, week, month, year, all).</param>
      /// <returns>The listing of comments requested.</returns>
      public Listing<Comment> GetComments(Sort sorting, int limit, FromTime fromTime)
      {
         if ((limit < 1) || (limit > MAX_LIMIT))
            throw new ArgumentOutOfRangeException("limit", "Valid range: [1," + MAX_LIMIT + "]");
         string commentsUrl = string.Format(CommentsUrl, Name);
         commentsUrl += string.Format("?sort={0}&limit={1}&t={2}", Enum.GetName(typeof(Sort), sorting), limit, Enum.GetName(typeof(FromTime), fromTime));

         return new Listing<Comment>(Reddit, commentsUrl, WebAgent);
      }

      /// <summary>
      /// Get a listing of posts from the user sorted by newest-first since their first posts, in batches of 25.
      /// </summary>
      /// <returns>The listing of posts requested.</returns>
      public Listing<Post> GetPosts()
      {
         return GetPosts(Sort.New, 25, FromTime.All);
      }

      /// <summary>
      /// Get a listing of posts from the user sorted by <paramref name="sorting"/> since their first post, in batches of 25.
      /// </summary>
      /// <param name="sorting">How to sort the posts (hot, new, top, controversial).</param>
      /// <returns>The listing of posts requested.</returns>
      public Listing<Post> GetPosts(Sort sorting)
      {
         return GetPosts(sorting, 25, FromTime.All);
      }

      /// <summary>
      /// Get a listing of posts from the user sorted by <paramref name="sorting"/> since their first post, limited to <paramref name="limit"/>.
      /// </summary>
      /// <param name="sorting">How to sort the posts (hot, new, top, controversial).</param>
      /// <param name="limit">How many posts to fetch per request. Max is 100.</param>
      /// <returns>The listing of posts requested.</returns>
      public Listing<Post> GetPosts(Sort sorting, int limit)
      {
         return GetPosts(sorting, limit, FromTime.All);
      }
      
      /// <summary>
      /// Get a listing of posts from the user sorted by <paramref name="sorting"/>, from time <paramref name="fromTime"/>
      /// and limited to <paramref name="limit"/>.
      /// </summary>
      /// <param name="sorting">How to sort the posts (hot, new, top, controversial).</param>
      /// <param name="limit">How many posts to fetch per request. Max is 100.</param>
      /// <param name="fromTime">What time frame of posts to show (hour, day, week, month, year, all).</param>
      /// <returns>The listing of posts requested.</returns>
      public Listing<Post> GetPosts(Sort sorting, int limit, FromTime fromTime)
      {
         if ((limit < 1) || (limit > 100))
            throw new ArgumentOutOfRangeException("limit", "Valid range: [1,100]");
         string linksUrl = string.Format(LinksUrl, Name);
         linksUrl += string.Format("?sort={0}&limit={1}&t={2}", Enum.GetName(typeof(Sort), sorting), limit, Enum.GetName(typeof(FromTime), fromTime));

         return new Listing<Post>(Reddit, linksUrl, WebAgent);
      }


#region StaticGetPostsHelpers

      // these functions are designed to be called from the Reddit class which lets you directly access a user's posts without loading the user record completely.
      public static Listing<Post> GetPosts(Reddit reddit, IWebAgent webagent, string name)
      {
         return GetPosts(reddit, webagent, name, Sort.New, 25, FromTime.All);
      }

      public static Listing<Post> GetPosts(Reddit reddit, IWebAgent webagent, string name, Sort sorting)
      {
         return GetPosts(reddit, webagent, name, sorting, 25, FromTime.All);
      }

      public static Listing<Post> GetPosts(Reddit reddit, IWebAgent webagent, string name, Sort sorting, int limit)
      {
         return GetPosts(reddit, webagent, name, sorting, limit, FromTime.All);
      }

      public static Listing<Post> GetPosts(Reddit reddit, IWebAgent webagent, string name, Sort sorting, int limit, FromTime fromTime)
      {
         if ((limit < 1) || (limit > 100))
            throw new ArgumentOutOfRangeException("limit", "Valid range: [1,100]");
         string linksUrl = string.Format(LinksUrl, name);
         linksUrl += string.Format("?sort={0}&limit={1}&t={2}", Enum.GetName(typeof(Sort), sorting), limit, Enum.GetName(typeof(FromTime), fromTime));

         return new Listing<Post>(reddit, linksUrl, webagent);
      }

#endregion



      public override string ToString()
      {
         return Name;
      }
   }

   public enum Sort
   {
      New,
      Hot,
      Top,
      Controversial
   }

   public enum FromTime
   {
      All,
      Year,
      Month,
      Week,
      Day,
      Hour
   }
}
