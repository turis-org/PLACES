using Turis.Location;
using Turis.Sights;
using DotNetEnv;

namespace Turis;

internal static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            
            // Добавляем логгирование
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            
            builder.Configuration.AddEnvironmentVariables();

            var app = builder.Build();
            var logger = app.Logger; // Получаем логгер

            // Загружаем конфиги
            var locationApiKey = app.Configuration["LOCATION_API_KEY"];
            var placesApiKey = app.Configuration["PLACES_API_KEY"];

            if (string.IsNullOrEmpty(locationApiKey) || string.IsNullOrEmpty(placesApiKey))
            {
                logger.LogError("API keys are missing!");
                return;
            }

            var config = new Config { locationApiKey = locationApiKey, placesApiKey = placesApiKey };
            
            // Настройка параметров сервера
            var host = app.Configuration["HOST"];
            var port = app.Configuration["PORT"];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
            {
                logger.LogError("Host or port is missing!");
                return;
            }

            var serverConfig = new ServerConfig { host = host, port = port };
            logger.LogInformation($"Server url: http://{serverConfig.host}:{serverConfig.port}");
            
            // Инициализация http клиента
            var httpClient = new HttpClient();

            // Обработка GET /places?location=Москва
            app.MapGet("/places", async (HttpContext context) =>
            {
                var location = context.Request.Query["location"].FirstOrDefault();
                if (string.IsNullOrEmpty(location))
                {
                    logger.LogWarning("Empty location parameter received");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                logger.LogInformation($"Request for location: {location}");
                
                try
                {
                    var placesRes = await LocationApi.GetPlaces(location, httpClient, config);
                    var responseString = await placesRes.Content.ReadAsStringAsync();

                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync(responseString);
                    logger.LogInformation($"Successfully processed request for {location}");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error processing location request for {location}");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Internal server error");
                }
            });

            // Обработка GET /places/sights?radius=1000&lng=-117.000165&lat=46.7323875
            app.MapGet("/places/sights", async (HttpContext context) =>
            {
                var radius = context.Request.Query["radius"].FirstOrDefault();
                var lng = context.Request.Query["lng"].FirstOrDefault()?.Replace(',', '.');
                var lat = context.Request.Query["lat"].FirstOrDefault()?.Replace(',', '.');

                if (string.IsNullOrEmpty(lng) || string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(radius))
                {
                    logger.LogWarning("Invalid parameters for sights request");
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    return;
                }

                logger.LogInformation($"Request for sights at lat: {lat}, lng: {lng}, radius: {radius}");
                
                try
                {
                    var sights = await SightsApi.GetSights(lng, lat, httpClient, config, radius);
                    var responseString = await sights.Content.ReadAsStringAsync();

                    context.Response.ContentType = "text/plain; charset=utf-8";
                    await context.Response.WriteAsync(responseString);
                    logger.LogInformation($"Successfully processed sights request");
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, $"Error processing sights request");
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsync("Internal server error");
                }
            });

            app.Run($"http://{serverConfig.host}:{serverConfig.port}/");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex}");
        }
    }
}