using Final.Models;
using Final.Models.Weatherbit;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Final.Services.Interfaces;

namespace Final.Services
{
    public class WeatherbitService : IWeatherService, IDisposable
    {
        private readonly ApiClient _client;

        public WeatherbitService()
        {
            _client = new ApiClient("https://api.weatherbit.io/v2.0");
        }

        public async Task<List<WeatherDay>> Get7DaysAsync(Region region, string token, string lang)
        {
            
            string lat = region.Latitude.ToString(CultureInfo.InvariantCulture);
            string lon = region.Longitude.ToString(CultureInfo.InvariantCulture);

            string endpoint =
                $"/forecast/daily?lat={lat}&lon={lon}&days=7&lang={lang}&key={token}";

            string json = await _client.RequeteGetAsync(endpoint);

            JObject jsonObj = JObject.Parse(json);
            JToken? dataToken = jsonObj["data"];

            List<WeatherDay> jours = dataToken?.ToObject<List<WeatherDay>>() ?? new List<WeatherDay>();

            return jours;
        }

        public void Dispose() => _client.Dispose();
    }
}
