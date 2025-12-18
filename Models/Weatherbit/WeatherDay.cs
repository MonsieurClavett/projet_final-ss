using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Final.Models.Weatherbit
{
    public class WeatherDay
    {
        public string datetime { get; set; } = "";  
        public double temp { get; set; }
        public double min_temp { get; set; }
        public double max_temp { get; set; }
        public WeatherInfo weather { get; set; } = new();
    }

    public class WeatherInfo
    {
        public string description { get; set; } = "";
        public string icon { get; set; } = "";
        public string IconUrl => $"https://www.weatherbit.io/static/img/icons/{icon}.png";
    
    }
}
