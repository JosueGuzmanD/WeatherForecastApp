using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using WeatherForecastBackend.Interfaces;

namespace WeatherForecastBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController : ControllerBase
{
    private readonly IWeatherService _weatherService;

    public WeatherController(IWeatherService weatherService)
    {
        _weatherService = weatherService;
    }

    [HttpGet("{location}")]
    public async Task<IActionResult> GetWeather(string location)
    {
        try
        {
            var weatherData = await _weatherService.GetWeatherAsync(location);

            if (weatherData == null)
            {
                return NotFound("Weather data not found for the specified location.");
            }

            return Ok(weatherData);
        }
        catch (HttpRequestException ex)
        {
            return StatusCode(503, $"Service unavailable: {ex.Message}");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }
}