using System;

namespace CoinGecko.ApiEndPoints
{
    public static class BaseApiEndPointUrl
    {
        public static readonly Uri ApiEndPoint = new("https://api.coingecko.com/api/v3/");
        public static readonly Uri ProApiEndPoint = new("https://pro-api.coingecko.com/api/v3/");
        public static string AddCoinsIdUrl(string id) => "coins/" + id;
    }
}