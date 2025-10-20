namespace NNChallenge.Interfaces
{
    public interface IHourWeatherForecastVO
    {
        /// <summary>
        /// date of forecast
        /// </summary>
        DateTime Date { get; }
        /// <summary>
        /// temerature in Celcius
        /// </summary>
        float TeperatureCelcius { get; }
        /// <summary>
        /// Temperture in Fahrenheit
        /// </summary>
        float TeperatureFahrenheit { get; }
        /// <summary>
        /// url for picture
        /// </summary>
        string ForecastPitureURL { get; }
    }
}