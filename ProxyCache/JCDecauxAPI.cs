using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ProxyCache
{
    public static class JCDecauxAPI
    {
        private const string API_KEY = "7109d50b09d48cbb216a7365c27620f25bea3d3c";
        private const string URL = "https://api.jcdecaux.com/vls/v3/";
        private static readonly HttpClient _client = new HttpClient();
        private static readonly ProxyCache<string> _cache = new ProxyCache<string>(RetrieveDataAsync);

        private static async Task<string> RetrieveDataAsync(string cacheItemName)
        {
            Console.WriteLine("Requesting data for key : " + cacheItemName);
            var uri = URL + "stations?apiKey=" + API_KEY;
            if (cacheItemName != "all")
            {
                var val = cacheItemName.Split('_');
                uri = URL + "stations/" + val[1] + "?contract=" + val[0] + "&apiKey=" + API_KEY;
            }
            var response = await _client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public static async Task<string> GetStationsAsync(string nameOfItem) => await _cache.Get(nameOfItem, 60);

        public static async Task<string> GetAllStationsAsync() => await _cache.Get("all");

    }
}