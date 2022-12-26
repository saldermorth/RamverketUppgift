//#define UseNewsApiSample  // Remove or undefine to use your own code to read live data

using Assignment_A2_02.Models;
using System.Net;
using System.Net.Http.Json;
namespace Assignment_A2_02.Services
{
    public class NewsService
    {
        HttpClient httpClient = new HttpClient();

        // Your API Key
        readonly string apiKey = "b563e91a171d4ebfa8a8ea256e155aeb";

        public NewsService()
        {
            httpClient = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate });
            httpClient.DefaultRequestHeaders.Add("user-agent", "News-API-csharp/0.1");
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        public async Task<News> GetNewsAsync(NewsCategory category)
        {
#if UseNewsApiSample      
            NewsApiData nd = await NewsApiSampleData.GetNewsApiSampleAsync(category.ToString());

#else
            //https://newsapi.org/docs/endpoints/top-headlines
            var uri = $"https://newsapi.org/v2/top-headlines?country=se&category={category}";

            // make the http request
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = await httpClient.SendAsync(httpRequest);
            response.EnsureSuccessStatusCode();

            //Convert Json to Object
            NewsApiData nd = await response.Content.ReadFromJsonAsync<NewsApiData>();

#endif
            //Todo - Date time is now?
            var NewsSelected = nd.Articles.Select(x => new { x.Title, x.Description, x.Url, x.UrlToImage });
            List<NewsItem> ni = new();
            ni.AddRange(NewsSelected.Select(x => new NewsItem
            {
                DateTime = DateTime.Now,
                Title = x.Title,
                Description = x.Description,
                Url = x.Url,
                UrlToImage = x.UrlToImage,

            }));


            var news = new News
            {
                Category = category,
                Articles = ni
            }; //dummy to compile, replaced by your code
            return news;
        }
    }
}
