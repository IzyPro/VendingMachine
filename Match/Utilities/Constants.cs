using Microsoft.Extensions.Caching.Distributed;

namespace Match.Utilities
{
    public class Constants
    {
        //RESPONSE CODES
        public static string SuccessResponseCode = "00";
        public static string BadRequestResponseCode = "99";

        // CREATED BY
        public static string DefaultCreator = "SYSTEM";

        // ACCEPTED DENOMINATIONS
        public static int[] AcceptedCoins = new int[] { 100, 50, 20, 10, 5 };

        //ROLES
        public static string BUYERROLE = "Buyer";

        // AUTH & CACHE
        public static int tokenExpiryInMinutes = 60;
        public static DistributedCacheEntryOptions cacheOptions = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromMinutes(tokenExpiryInMinutes));
    }
}
