using Turis.Location;
using Turis.Sights;
using DotNetEnv;

namespace Turis;

internal static class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
        var builder = WebApplication.CreateBuilder(args);
        builder.Configuration.AddEnvironmentVariables();

        var app = builder.Build();

        // Загружаем конфиги
        var locationApiKey = app.Configuration["LOCATION_API_KEY"];
        var placesApiKey = app.Configuration["PLACES_API_KEY"];

        if (string.IsNullOrEmpty(locationApiKey) || string.IsNullOrEmpty(placesApiKey))
        {
            Console.WriteLine("API keys are missing!");
            return;
        }

        var config = new Config { locationApiKey = locationApiKey, placesApiKey = placesApiKey };
        
        // Настройка параметров сервера
        
        var host = app.Configuration["HOST"];
        var port = app.Configuration["PORT"];

        if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
        {
            Console.WriteLine("host or port is missing!");
            return;
        }

        var serverConfig = new ServerConfig { host = host, port = port };
        
        // инициализация http клиента
        var httpClient = new HttpClient();

        // Обработка GET /places?location=Москва
        app.MapGet("/places", async (HttpContext context) =>
        {
            var location = context.Request.Query["location"].FirstOrDefault();
            if (string.IsNullOrEmpty(location))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var placesRes = await LocationApi.GetPlaces(location, httpClient, config);
            var responseString = await placesRes.Content.ReadAsStringAsync();

            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(responseString);
        });

        // Обработка GET /places/sights?radius=1000&lng=...&lat=...
        app.MapGet("/places/sights", async (HttpContext context) =>
        {
            var radius = context.Request.Query["radius"].FirstOrDefault();
            var lng = context.Request.Query["lng"].FirstOrDefault()?.Replace(',', '.');
            var lat = context.Request.Query["lat"].FirstOrDefault()?.Replace(',', '.');

            if (string.IsNullOrEmpty(lng) || string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(radius))
            {
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            var sights = await SightsApi.GetSights(lng, lat, httpClient, config, radius);
            var responseString = await sights.Content.ReadAsStringAsync();

            context.Response.ContentType = "text/plain; charset=utf-8";
            await context.Response.WriteAsync(responseString);
        });

        app.Run($"http://{serverConfig.host}:{serverConfig.port}/");
    }
}