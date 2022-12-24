using Assignment_A1_01.Models;
using System.Net.Http.Json;

namespace Assignment_A1_01.Services
{
    public class OpenWeatherService
    {
        HttpClient httpClient = new HttpClient();

        // Your API Key
        readonly string apiKey = "91b079387dd769b274c12e2c4e34320e";

        public async Task<Forecast> GetForecastAsync(double latitude, double longitude)
        {
            //https://openweathermap.org/current
            var language = System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;
            var uri = $"https://api.openweathermap.org/data/2.5/forecast?lat={latitude}&lon={longitude}&units=metric&lang={language}&appid={apiKey}";

            //Read the response from the WebApi
            HttpResponseMessage response = await httpClient.GetAsync(uri);
            response.EnsureSuccessStatusCode();
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

            long unixTime = 1661870592;
            DateTime dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(unixTime);


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
