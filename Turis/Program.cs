using Turis.Location;
using Turis.Sights;
using DotNetEnv;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Turis;

internal static class Program
{
    public static void Main(string[] args)
    {
        try
        {
            Env.Load();
            var builder = WebApplication.CreateBuilder(args);
            
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();
            builder.Logging.AddDebug();
            builder.Configuration.AddEnvironmentVariables();
            
            var locationApiKey = builder.Configuration["LOCATION_API_KEY"];
            var placesApiKey = builder.Configuration["PLACES_API_KEY"];

            if (string.IsNullOrEmpty(locationApiKey) || string.IsNullOrEmpty(placesApiKey))
            {
                throw new Exception("API keys are missing!");
            }

            var config = new Config { locationApiKey = locationApiKey, placesApiKey = placesApiKey };
            ConfigureServices(builder.Services, config);
            
            var app = builder.Build();
            
            // Настройка параметров сервера
            var host = builder.Configuration["HOST"];
            var port = builder.Configuration["PORT"];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(port))
            {
                throw new Exception("Host or port is missing!");
            }

            var serverConfig = new ServerConfig { host = host, port = port };
            app.Logger.LogInformation($"Server url: http://{serverConfig.host}:{serverConfig.port}");
            
            ConfigureApp(app);
            
            app.Run($"http://{serverConfig.host}:{serverConfig.port}/");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fatal error: {ex}");
        }
    }
    private static void ConfigureServices(IServiceCollection services, Config config)
    {
        services.AddControllers();
        services.AddHttpClient();
        services.AddSingleton(config);
            
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
    private static void ConfigureApp(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // app.UseHttpsRedirection();
        // app.UseAuthorization();
        app.MapControllers();
    }
}