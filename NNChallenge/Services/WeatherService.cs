using System.Text.Json;
using System.Text.Json.Serialization;
using NNChallenge.Interfaces;
using NNChallenge.Models;

namespace NNChallenge.Services
{
    public interface IWeatherService
    {
        Task<IWeatherForcastVO> GetForecastAsync(string city);
    }

    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private const string ApiKey = "898147f83a734b7dbaa95705211612";
        private const string BaseUrl = "https://api.weatherapi.com/v1/forecast.json";

        public WeatherService()
        {
            _httpClient = new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);
        }

        public async Task<IWeatherForcastVO> GetForecastAsync(string city)
        {
            try
            {
                var url = $"{BaseUrl}?key={ApiKey}&q={city}&days=3&aqi=no&alerts=no";
                var response = await _httpClient.GetStringAsync(url);
                
                // Use System.Text.Json for deserialization
                var apiResponse = JsonSerializer.Deserialize<WeatherApiResponse>(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (apiResponse?.Location == null || apiResponse.Forecast?.Forecastday == null)
                    throw new Exception("Invalid response from weather API");

                return ConvertToForecastVO(apiResponse);
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fetch weather data: {ex.Message}");
            }
        }

        private IWeatherForcastVO ConvertToForecastVO(WeatherApiResponse apiResponse)
        {
            var hourlyForecasts = new List<IHourWeatherForecastVO>();

            foreach (var forecastDay in apiResponse.Forecast.Forecastday)
            {
                foreach (var hour in forecastDay.Hour)
                {
                    hourlyForecasts.Add(new HourWeatherForecastVO
                    {
                        Date = DateTime.Parse(hour.Time),
                        TeperatureCelcius = (float)hour.TempC,
                        TeperatureFahrenheit = (float)hour.TempF,
                        ForecastPitureURL = hour.Condition.Icon
                    });
                }
            }

            return new WeatherForecastVO
            {
                City = apiResponse.Location?.Name ?? "Unknown City",
                HourForecast = hourlyForecasts.ToArray()
            };
        }

        private class WeatherApiResponse
        {
            public LocationData? Location { get; set; }
            public ForecastData? Forecast { get; set; }
        }

        private class LocationData
        {
            public string Name { get; set; } = string.Empty;
            public string Country { get; set; } = string.Empty;
        }

        private class ForecastData
        {
            public List<ForecastDayData>? Forecastday { get; set; }
        }

        private class ForecastDayData
        {
            public string? Date { get; set; }
            public List<HourData>? Hour { get; set; }
        }

        private class HourData
        {
            public string Time { get; set; } = string.Empty;
            
            [JsonPropertyName("temp_c")]
            public double TempC { get; set; }
            
            [JsonPropertyName("temp_f")] 
            public double TempF { get; set; }
            
            public ConditionData? Condition { get; set; }
        }

        private class ConditionData
        {
            public string Text { get; set; } = string.Empty;
            public string Icon { get; set; } = string.Empty;
        }
    }
}