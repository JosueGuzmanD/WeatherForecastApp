using Microsoft.AspNetCore.Mvc;

namespace WeatherForecastBackend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WeatherController: ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public WeatherController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    
    [HttpGet("{Location}")]
    public async Task<IActionResult> GetWeather(string location)
    {
        var apiKey = _configuration["OpenWeatherMap:ApiKey"];
        var client = _httpClientFactory.CreateClient();
        var response = await client.GetAsync($"https://api.openweathermap.org/data/2.5/weather?q={location}&appid={apiKey}&units=metric");

        if (!response.IsSuccessStatusCode)
        {
            return BadRequest("Error fetching weather data.");
        }

        var data = await response.Content.ReadAsStringAsync();
        return Ok(data);
    }
    
    
}