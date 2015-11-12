namespace RedditSharp
{
   public class CaptchaResponse
   {
      public readonly string Answer;

      public bool Cancel { get { return string.IsNullOrEmpty(Answer); } }

      public CaptchaResponse()
      {
         Answer = null;
      }

      public CaptchaResponse(string answer)
      {
         Answer = answer;
      }
   }
}
