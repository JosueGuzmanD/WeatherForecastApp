using WeatherForecastBackend.Interfaces;

namespace WeatherForecastBackend.Services;

using System.Net.Http.Json;

public class WeatherService : IWeatherService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public WeatherService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public async Task<object?> GetWeatherAsync(string location)
    {
        var apiKey = _configuration["OpenWeatherMap:ApiKey"];
        var client = _httpClientFactory.CreateClient();

        var response = await client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={apiKey}&units=metric");

        if (!response.IsSuccessStatusCode)
        {
            return null;
        }

        var weatherData = await response.Content.ReadFromJsonAsync<object>();
        return weatherData;
    }
}
