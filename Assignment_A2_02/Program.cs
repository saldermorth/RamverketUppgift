using Assignment_A2_02.Models;
using Assignment_A2_02.Services;

namespace Assignment_A2_02
{
    class Program
    {
        static void Main(string[] args)
        {
            NewsService service = new();

            Task<News>[] tasks = { null, null, null, null };

            Exception exception = null;
            try
            {
                NewsCategory Category = new NewsCategory();
                Random rnd = new Random();
                var values = (NewsCategory[])Enum.GetValues(typeof(NewsCategory));
                var randomArticle = values[rnd.Next(values.Length)];

                //Create the two tasks and wait for comletion
                tasks[0] = service.GetNewsAsync(randomArticle);
                // tasks[1] = service.GetForecastAsync("Miami");

                Task.WaitAll(tasks[0], tasks[1]);

                //tasks[2] = service.GetForecastAsync(latitude, longitude);
                //tasks[3] = service.GetForecastAsync("Miami");

                //Wait and confirm we get an event showing cahced data avaialable
                Task.WaitAll(tasks[2], tasks[3]);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error : " + ex.Message + "\nCity Entered: ");
                Console.ForegroundColor = ConsoleColor.Gray;

            }

            foreach (var task in tasks)
            {
                PrintArticles();
                //How to deal with successful and fault tasks
                //Your Code
            }

            //NewsCategory Category = new NewsCategory();
            //Category = NewsCategory.science;
            //var theNews = service.GetNewsAsync(Category);

            //Console.WriteLine(theNews.Result.Articles.First().Title);

        }
        static void PrintArticles()
        {

        }
    }
}
