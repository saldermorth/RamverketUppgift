using Assignment_A1_03.Exceptions;
using Assignment_A1_03.Models;
using System.Collections.Concurrent;
using System.Net.Http.Json;
using System.Runtime.Caching;

namespace Assignment_A1_03.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();


        //Cache declaration
        ConcurrentDictionary<(double, double, string), Forecast> cachedGeoForecasts = new ConcurrentDictionary<(double, double, string), Forecast>();
        ConcurrentDictionary<(string, string), Forecast> cachedCityForecasts = new ConcurrentDictionary<(string, string), Forecast>();


        // Your API Key
        readonly string apiKey = "91b079387dd769b274c12e2c4e34320e";


        //Event declaration
        public event EventHandler<string> WeatherForecastAvailable;
        protected virtual void OnWeatherForecastAvailable(string message)
        {
            WeatherForecastAvailable?.Invoke(this, message);
        }
        public async Task<Forecast> GetForecastAsync(string City)
        {
            //part of cache code here to check if forecast in Cache
            //generate an event that shows forecast was from cache
            //Your code
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.SlidingExpiration = TimeSpan.FromSeconds(30);


            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code

            MemoryCache.Default.Add("Forecast", forecast, policy);

            bool isCached = MemoryCache.Default.Contains("Forecast");
            if (isCached) Console.WriteLine("Cached value found");


            WeatherForecastAvailable?.Invoke(forecast, "City Forecast Complete");
            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //part of cache code here to check if forecast in Cache
            //generate an event that shows forecast was from cache
            //Your code

            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code
            WeatherForecastAvailable?.Invoke(forecast, "Coordinates Forecast Complete");
            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri)
        {
            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri);

            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {

                throw new NonExistingCityException($"Status code indicates non 200 response");
            }
            WeatherApiData wd = await response.Content.ReadFromJsonAsync<WeatherApiData>();

            //Convert WeatherApiData to Forecast using Linq.
            //Your code
            var forecastSelctedInfo = wd.list.Select(e => new { wd.city, e.dt, e.main.temp, e.wind.speed, e.weather.First().description, e.weather.First().icon }).ToList();


            List<ForecastItem> forecasts = new List<ForecastItem>();

            forecasts.AddRange(forecastSelctedInfo.Select(x
                 => new ForecastItem
                 {
                     DateTime = UnixTimeStampToDateTime(x.dt),
                     Description = x.description,
                     Icon = x.icon,
                     Temperature = x.temp,
                     WindSpeed = x.speed
                 }));


            Forecast forecast = new Forecast
            {
                City = wd.city.name,
                Items = forecasts
            };

            //dummy to compile, replaced by your own dummy code

            return forecast;
        }
        private DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            dateTime = dateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dateTime;
        }
    }
}
