using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;
using SimpleUI.Models;

namespace SimpleUI.DataAccess
{
    public interface IWeatherData
    {
        [Get("/WeatherForecast")]
        Task<List<WeatherModel>> GetWeather();
    }
}
