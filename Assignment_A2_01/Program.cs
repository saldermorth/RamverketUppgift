using Assignment_A2_01.Services;

namespace Assignment_A2_01
{
    class Program
    {
        static void Main(string[] args)
        {
            NewsService service = new();
            var theNews = service.GetNewsAsync();
            Console.WriteLine(theNews.Result.Articles.First().Title);


        }
    }
}
