namespace WeatherForecastBackend.Interfaces;

public interface IWeatherService
{
    Task<object?> GetWeatherAsync(string location);
}
