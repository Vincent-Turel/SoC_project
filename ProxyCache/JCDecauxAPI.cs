using System.Net.Http;
using System.Runtime.Remoting.Proxies;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProxyCache
{
    public class JCDecauxAPI
    {
        private static readonly HttpClient _client = new HttpClient();
        private static readonly ProxyCache<string> _cache = new ProxyCache<string>(RetrieveDataAsync);
        private static async Task<string> RetrieveDataAsync(string cacheItemName)
        {
            if (cacheItemName == "all")
            {
                return await _client.GetStringAsync("https://api.jcdecaux.com/vls/v3/stations?apiKey=7109d50b09d48cbb216a7365c27620f25bea3d3c");
            }
            var val = cacheItemName.Split('_');
            return await _client.GetStringAsync("https://api.jcdecaux.com/vls/v3/stations/" + val[0] + "?contract=" + val[1] + "&apiKey=7109d50b09d48cbb216a7365c27620f25bea3d3c");
        }

        public static async Task<string> GetStationsAsync(string nameOfItem) => await _cache.Get(nameOfItem);
    }
}