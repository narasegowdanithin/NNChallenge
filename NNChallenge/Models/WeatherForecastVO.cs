using NNChallenge.Interfaces;

namespace NNChallenge.Models
{
    /// <summary>
    /// Implementation of IWeatherForcastVO interface
    /// </summary>
    public class WeatherForecastVO : IWeatherForcastVO
    {
        /// <summary>
        /// Name of the city
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Array of weather forecast entries
        /// </summary>
        public IHourWeatherForecastVO[] HourForecast { get; set; } = [];
    }
}