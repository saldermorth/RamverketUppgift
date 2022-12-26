//#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using Assignment_A2_01.Models;
using System.Net;
using System.Net.Http.Json;
using System.Runtime.Caching;

namespace Assignment_A2_01.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();
        CacheItemPolicy policy = new CacheItemPolicy();


        // Your API Key
        readonly string apiKey = "b563e91a171d4ebfa8a8ea256e155aeb";

        public NewsService()
        {
            httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            httpClient.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
            policy.SlidingExpiration = TimeSpan.FromSeconds(61);
        }
        public event EventHandler<string> NewsAvalible;
        protected virtual void OnNewsAvailable(string message)
        {
            NewsAvalible?.Invoke(this, message);
        }


        public async Task<NewsApiData> GetNewsAsync()
        {

#if UseNewsApiSample
            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync("sports");

#else            
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category=sports";
            var CachedNews = MemoryCache.Default.Get("News");
            if (CachedNews is not null)
            {
                NewsAvalible?.Invoke(CachedNews, "News Complete");
                return (NewsApiData)CachedNews;
            }


            // make the http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();





            //Convert Json to Object
            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();
#endif            
            return nd;
        }
    }
}
