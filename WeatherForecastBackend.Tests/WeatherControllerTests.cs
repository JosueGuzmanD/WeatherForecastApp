using Microsoft.AspNetCore.Mvc;
using Moq;
using WeatherForecastBackend.Controllers;
using WeatherForecastBackend.Interfaces;

namespace WeatherForecastBackend.Tests;

public class WeatherControllerTests
{
    private readonly Mock<IWeatherService> _weatherServiceMock;
    private readonly WeatherController _controller;

    public WeatherControllerTests()
    {
        _weatherServiceMock = new Mock<IWeatherService>();
        _controller = new WeatherController(_weatherServiceMock.Object);
    }

    [Fact]
    public async Task GetWeather_ReturnsOk_WhenDataIsFound()
    {
        // Arrange
        var location = "London";
        var fakeWeatherData = new { Temperature = 20, Condition = "Sunny" };
        _weatherServiceMock
            .Setup(service => service.GetWeatherAsync(location))
            .ReturnsAsync(fakeWeatherData);

        // Act
        var result = await _controller.GetWeather(location);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(fakeWeatherData, okResult.Value);
    }

    [Fact]
    public async Task GetWeather_ReturnsNotFound_WhenDataIsNotFound()
    {
        // Arrange
        var location = "UnknownLocation";
        _weatherServiceMock
            .Setup(service => service.GetWeatherAsync(location))
            .ReturnsAsync((object)null);

        // Act
        var result = await _controller.GetWeather(location);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task GetWeather_ReturnsServiceUnavailable_OnHttpRequestException()
    {
        // Arrange
        var location = "London";
        _weatherServiceMock
            .Setup(service => service.GetWeatherAsync(location))
            .ThrowsAsync(new HttpRequestException("API down"));

        // Act
        var result = await _controller.GetWeather(location);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(503, statusCodeResult.StatusCode);
        Assert.Contains("Service unavailable", statusCodeResult.Value.ToString());
    }

    [Fact]
    public async Task GetWeather_ReturnsInternalServerError_OnGenericException()
    {
        // Arrange
        var location = "London";
        _weatherServiceMock
            .Setup(service => service.GetWeatherAsync(location))
            .ThrowsAsync(new Exception("Something went wrong"));

        // Act
        var result = await _controller.GetWeather(location);

        // Assert
        var statusCodeResult = Assert.IsType<ObjectResult>(result);
        Assert.Equal(500, statusCodeResult.StatusCode);
        Assert.Contains("Internal server error", statusCodeResult.Value.ToString());
    }
}