using Assignment_A1_02.Models;
using Assignment_A1_02.Services;

namespace Assignment_A1_02
{
    class Program
    {
        static bool ForecastForCityRecived = false;
        static bool ForecastForCoordinatesRecived = false;
        static Forecast forecastCity;
        static Forecast forecastCoordinates;
        static string UsersChoosenCity = "Miami";


        static async Task Main(string[] args)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Gray;
            OpenWeatherService service = new OpenWeatherService();


            //Register the event
            //Your Code
            service.WeatherForecastAvailable += OnCoordintesWeatherForecastAvailable;



            Task<Forecast>[] tasks = { null, null };
            Exception exception = null;
            try
            {
                double latitude = 59.5086798659495;
                double longitude = 18.2654625932976;

                //Create the two tasks and wait for comletion
                tasks[0] = service.GetForecastAsync(latitude, longitude);
                tasks[1] = service.GetForecastAsync(UsersChoosenCity); // Validation


                Task.WaitAll(tasks[0], tasks[1]);
            }
            catch (Exception ex)
            {
                //exception = ex;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error : " + ex.Message + "\nCity Entered: " + UsersChoosenCity);
                Console.ForegroundColor = ConsoleColor.Gray;


            }

            foreach (var task in tasks)
            {
                PrintForecasts();

                //How to deal with successful and fault tasks
                //Your Code
            }
        }
        static void OnCoordintesWeatherForecastAvailable(object sender, string forecast)
        {

            if (forecast.Contains("City"))
            {
                ForecastForCityRecived = true;
                forecastCity = (Forecast)sender;
                Thread.Sleep(5000);
                Console.WriteLine($"Forecast recived");

                // Progressbar();

            }


            if (forecast.Contains("Coordinates"))
            {
                ForecastForCoordinatesRecived = true;
                forecastCoordinates = (Forecast)sender;
                var forelist = forecastCoordinates.Items.GroupBy(x => x.DateTime.Date);
                Thread.Sleep(5000);
                Console.WriteLine($"Forecast for coordinates recived ");
                // Progressbar();
            }




        }
        public static void PrintForecasts()
        {
            //Give color based on weather
            // Print Group by
            if (ForecastForCityRecived && ForecastForCoordinatesRecived)
            {
                Console.WriteLine($"Weather forecast for {forecastCity.City}"); // Triggers twice
                //Console.WriteLine("{0,-20}{1,-15}{2,-15}{3,-20}{4}", "DateTime", "Temperature", "WindSpeed", "Description", "Icon");
                forecastCity.Items.ForEach(x => Console.WriteLine(x));

                Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++");

                Console.WriteLine($"Weather forecast for {forecastCoordinates.City}");
                //Console.WriteLine("{0,-20}{1,-15}{2,-15}{3,-20}{4}", "DateTime", "Temperature", "WindSpeed", "Description", "Icon");
                forecastCoordinates.Items.ForEach(x => Console.WriteLine(x));

                Console.WriteLine("++++++++++++++++++++++++++++++++++++++++++++++++++");
                ForecastForCoordinatesRecived = false;
                ForecastForCityRecived = false;

                Environment.Exit(1);

            }
        }
        public static void Progressbar()
        {
            for (int i = 0; i < 100; i++)
            {
                Console.Write("+");
                Thread.Sleep(10);
            }
        }
        //Event handler declaration
        //Your Code
    }
}