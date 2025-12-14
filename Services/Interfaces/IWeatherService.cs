using Final.Models;
using Final.Models.Weatherbit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Services.Interfaces
{
    public interface IWeatherService
    {
        Task<List<WeatherDay>> Get7DaysAsync(Region region, string token, string lang);
    }
}
