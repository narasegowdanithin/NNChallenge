namespace NNChallenge.Interfaces
{
    public interface IWeatherForcastVO
    {
        /// <summary>
        /// Name of the city
        /// </summary>
        string City { get; }
        /// <summary>
        /// Array of weather forecast etries
        /// </summary>
        IHourWeatherForecastVO[] HourForecast { get; }
    }

    
}
