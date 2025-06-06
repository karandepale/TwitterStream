using WebApi.DBEntityFramework;
using WebApi.Interfaces;

namespace WebApi.Wrapper
{
    public class TweetDashboardWrapper : ITweetDashboardWrapper
    {
        private readonly AppDbContext _dbContext;
        public TweetDashboardWrapper(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        //public List<string> GetTokens(string tweetUID)
        //{
        //    var tokenList = new List<string>();

        //    try
        //    {
        //        var userRecord = _dbContext.PersonalProjectTable
        //            .FirstOrDefault(u => u.TwitterUserId == tweetUID);

        //        if (userRecord != null)
        //        {
        //            tokenList.Add(userRecord.AccessToken ?? "");
        //            tokenList.Add(userRecord.RefreshToken ?? "");
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error fetching tokens: " + ex.Message);
        //    }

        //    return tokenList;
        //}

        public List<string> GetTokens(string tweetUID)
        {
            var tokenList = new List<string>();

            try
            {
                // Determine whether to filter by tweetUID or not
                var userRecord = string.IsNullOrWhiteSpace(tweetUID)
                    ? _dbContext.PersonalProjectTable.FirstOrDefault() // No filter
                    : _dbContext.PersonalProjectTable.FirstOrDefault(u => u.TwitterUserId == tweetUID); // Filter by UID

                if (userRecord != null)
                {
                    tokenList.Add(userRecord.AccessToken ?? "");
                    tokenList.Add(userRecord.RefreshToken ?? "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching tokens: " + ex.Message);
            }

            return tokenList;
        }



    }
}
