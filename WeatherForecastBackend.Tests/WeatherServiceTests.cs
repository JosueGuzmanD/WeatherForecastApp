using System.Net;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using WeatherForecastBackend.Services;

namespace WeatherForecastBackend.Tests;

public class WeatherServiceTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
    private readonly IConfiguration _configuration;
    private readonly WeatherService _weatherService;

    public WeatherServiceTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _httpMessageHandlerMock = new Mock<HttpMessageHandler>();

        var configData = new Dictionary<string, string>
        {
            { "OpenWeatherMap:ApiKey", "fake_api_key" }
        };
        _configuration = new ConfigurationBuilder().AddInMemoryCollection(configData).Build();

        var httpClient = new HttpClient(_httpMessageHandlerMock.Object);
        _httpClientFactoryMock.Setup(factory => factory.CreateClient(It.IsAny<string>())).Returns(httpClient);

        _weatherService = new WeatherService(_httpClientFactoryMock.Object, _configuration);
    }
    

    [Fact]
    public async Task GetWeatherAsync_ReturnsNull_WhenResponseIsUnsuccessful()
    {
        // Arrange
        var responseMessage = new HttpResponseMessage
        {
            StatusCode = HttpStatusCode.NotFound
        };

        _httpMessageHandlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(responseMessage);

        // Act
        var result = await _weatherService.GetWeatherAsync("UnknownLocation");

        // Assert
        Assert.Null(result);
    }
}