using NNChallenge.Interfaces;

namespace NNChallenge.Models
{
    /// <summary>
    /// Implementation of IHourWeatherForecastVO interface
    /// </summary>
    public class HourWeatherForecastVO : IHourWeatherForecastVO
    {
        /// <summary>
        /// date of forecast
        /// </summary>
        public DateTime Date { get; set; }
        
        /// <summary>
        /// temperature in Celcius
        /// </summary>
        public float TeperatureCelcius { get; set; }
        
        /// <summary>
        /// Temperature in Fahrenheit
        /// </summary>
        public float TeperatureFahrenheit { get; set; }

        /// <summary>
        /// url for picture
        /// </summary>
        public string ForecastPitureURL { get; set; } = string.Empty;
    }
}