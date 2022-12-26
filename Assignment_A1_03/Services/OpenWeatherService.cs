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
        CacheItemPolicy policy = new CacheItemPolicy();

        public OpenWeatherService()
        {
            policy.SlidingExpiration = TimeSpan.FromSeconds(61);
        }

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

        public event EventHandler<string> CachedWeatherForecastAvailable;
        protected virtual void OnCachedWeatherForecastAvailable(string message)
        {
            CachedWeatherForecastAvailable?.Invoke(this, message);
        }


        public async Task<Forecast> GetForecastAsync(string City)
        {
            //part of cache code here to check if forecast in Cache
            //generate an event that shows forecast was from cache
            //Your code

            var CachedForecast = MemoryCache.Default.Get("ForecastCity");
            Console.WriteLine();
            if (CachedForecast != null)
            {
                // invoke cache event 
                Console.WriteLine("Cached value: " + CachedForecast);
                CachedWeatherForecastAvailable?.Invoke(CachedForecast, "City Forecast Complete");
                return (Forecast)CachedForecast;

            }
            else
            {
                Console.WriteLine("Value not found in cache.");
            }


            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?q={City}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code

            MemoryCache.Default.Add("ForecastCity", forecast, policy);
            bool isCached = MemoryCache.Default.Contains("ForecastCity");
            if (isCached) Console.WriteLine("Value Cached");


            WeatherForecastAvailable?.Invoke(forecast, "City Forecast Complete");
            return forecast;

        }
        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //part of cache code here to check if forecast in Cache
            //generate an event that shows forecast was from cache
            //Your code
            var CachedForecast = (Forecast)MemoryCache.Default.Get("ForecastCoor");

            if (CachedForecast is not null)
            {
                // invoke cache event 
                Console.WriteLine("Cached value: " + CachedForecast);

                CachedWeatherForecastAvailable?.Invoke(CachedForecast, "City Forecast Complete");

                return CachedForecast;
            }
            else
            {
                Console.WriteLine("Value not found in cache.");
            }



            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            Forecast forecast = await ReadWebApiAsync(uri);

            //part of event and cache code here
            //generate an event with different message if cached data
            //Your code
            MemoryCache.Default.Add("ForecastCoor", forecast, policy);
            bool isCached = MemoryCache.Default.Contains("ForecastCoor");
            if (isCached) Console.WriteLine("Value Cached");


            WeatherForecastAvailable?.Invoke(forecast, "Coordinates Forecast Complete");
            return forecast;
        }
        private async Task<Forecast> ReadWebApiAsync(string uri) //Todo here makes forcast items
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
