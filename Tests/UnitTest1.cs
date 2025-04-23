using DotNetEnv;
using System.Net;
using Xunit;
namespace Turis.Tests;
using System;
using System.Net.Http;
using Turis.Location;
using Turis.Sights;

public class ProgramTest
{
    private readonly HttpClient _client;
    private readonly Config _config;

    public ProgramTest()
    {
        _client = new HttpClient();

        Env.Load();
        string? location_api_key = Environment.GetEnvironmentVariable("LOCATION_API_KEY") ?? Env.GetString("LOCATION_API_KEY");
        string? places_api_key = Environment.GetEnvironmentVariable("PLACES_API_KEY") ?? Env.GetString("PLACES_API_KEY");

        _config = new Config();
        _config.locationApiKey = location_api_key;
        _config.placesApiKey = places_api_key;
    }

    [Fact]
    public async Task TestOkCodeStatus()
    {
        var context = new DefaultHttpContext();
        var placesRes = await LocationApi.GetPlaces("moscow", _client, _config);
        var responseString = await placesRes.Content.ReadAsStringAsync();

        context.Response.ContentType = "text/plain; charset=utf-8";
        context.Response.StatusCode = (int)placesRes.StatusCode;
        await context.Response.WriteAsync(responseString);
        
        Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
    }
    [Fact]
    public async Task TestBadCodeStatus()
    {
        var places_res = await LocationApi.GetPlaces(String.Empty, _client, _config);
        Assert.Equal((int)HttpStatusCode.BadRequest, (int)places_res.StatusCode);
    }
    [Fact]
    public async Task TestOkCodeStatusApi2()
    {
        var context = new DefaultHttpContext();
        var sights = await SightsApi.GetSights("-117.000165", "46.7323875", _client, _config, "1000");
        var responseString = await sights.Content.ReadAsStringAsync();

        context.Response.ContentType = "text/plain; charset=utf-8";
        context.Response.StatusCode = (int)sights.StatusCode;
        await context.Response.WriteAsync(responseString);
        
        Assert.Equal((int)HttpStatusCode.OK, context.Response.StatusCode);
    }
    [Fact]
    public async Task TestBadCodeStatusApi2()
    {
        var sights = await SightsApi.GetSights(String.Empty, "46.7323875", _client, _config, "1000");
        Assert.Equal((int)HttpStatusCode.BadRequest, (int)sights.StatusCode);
    }
    [Fact]
    public void ServerConfig_CanBeCreatedWithDefaultConstructor()
    {
        // Act
        var config = new ServerConfig();
        
        // Assert
        Assert.NotNull(config);
        Assert.Null(config.host);
        Assert.Null(config.port);
    }

    [Fact]
    public void ServerConfig_CanBeCreatedWithProperties()
    {
        // Arrange
        const string testHost = "localhost";
        const string testPort = "8080";
        
        // Act
        var config = new ServerConfig
        {
            host = testHost,
            port = testPort
        };
        
        // Assert
        Assert.NotNull(config);
        Assert.Equal(testHost, config.host);
        Assert.Equal(testPort, config.port);
    }
    [Fact]
    public void Config_CanBeCreatedWithDefaultConstructor()
    {
        // Act
        var config = new Config();
        
        // Assert
        Assert.NotNull(config);
        Assert.Null(config.locationApiKey);
        Assert.Null(config.placesApiKey);
    }

    [Fact]
    public void Config_CanBeCreatedWithProperties()
    {
        // Arrange
        const string testLocationApiKey = "api1";
        const string testPlacesApiKey = "api2";
        
        // Act
        var config = new Config
        {
            locationApiKey = testLocationApiKey,
            placesApiKey = testPlacesApiKey
        };
        
        // Assert
        Assert.NotNull(config);
        Assert.Equal(testLocationApiKey, config.locationApiKey);
        Assert.Equal(testPlacesApiKey, config.placesApiKey);
    }

    [Fact]
    public void BuilderLoggerAppNotNull()
    {
        var builder = WebApplication.CreateBuilder();
            
        // Добавляем логгирование
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        
        var app = builder.Build();
        
        var logger = app.Logger; // Получаем логгер
        
        Assert.NotNull(builder);
        Assert.NotNull(app);
        Assert.NotNull(logger);
    }
    [Fact]
    public void EnvValuesNotNull()
    {
        Env.Load();
        var builder = WebApplication.CreateBuilder();
 
        builder.Configuration.AddEnvironmentVariables();

        var app = builder.Build();

        // Загружаем конфиги
        var locationApiKey = app.Configuration["LOCATION_API_KEY"];
        var placesApiKey = app.Configuration["PLACES_API_KEY"];
        
        var config = new Config { locationApiKey = locationApiKey, placesApiKey = placesApiKey };
        
        Assert.NotNull(locationApiKey);
        Assert.NotNull(placesApiKey);
        Assert.NotNull(config);
    }
}