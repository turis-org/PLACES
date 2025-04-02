using System;
using System.Diagnostics;
using System.Text.Json;
using Turis;
using System.Net.Http;

using Turis.Location;
using Turis.Sights;
using System.Text;
using System.Net;
using System.Numerics;
class Program
{
    private static async void HttpHandler(HttpListenerContext context, HttpClient httpClient, Config config)
    {

        HttpListenerRequest request = context.Request;
        HttpListenerResponse response = context.Response;
        if (request == null || request.Url == null)
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.Close();
            return;
        }

        if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/places")
        {
            // Получаем параметры из URL (например: /places?location=Москва)
            var queryParams = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
            if (queryParams == null)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Close();
                return;
            }

            string? location = queryParams["location"]; // Название локации

            if (string.IsNullOrEmpty(location))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Close();
                return;
            }

            var places_res = await LocationApi.GetPlaces(location, httpClient, config);
            string places_string = await places_res.Content.ReadAsStringAsync();

            string responseString = places_string;

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentType = "text/plain; charset=utf-8";
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }

        else if (request.HttpMethod == "GET" && request.Url.AbsolutePath == "/places/sights")
        {
            // Получаем параметры из URL (например: /places/sights?radius=1000&lng=-117.000165&lat=46.7323875)
            var queryParams = System.Web.HttpUtility.ParseQueryString(request.Url.Query);
            if (queryParams == null)
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Close();
                return;
            }

            string? radius = queryParams["radius"];
            string? lng = queryParams["lng"];
            string? lat = queryParams["lat"];

            if (string.IsNullOrEmpty(lng) || string.IsNullOrEmpty(lat) || string.IsNullOrEmpty(radius))
            {
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Close();
                return;
            }
            var sights = await SightsApi.GetSights(lng.Replace(',', '.'), lat.Replace(',', '.'), httpClient, config, radius);
            string sights_string = await sights.Content.ReadAsStringAsync();

            string responseString = sights_string;

            byte[] buffer = Encoding.UTF8.GetBytes(responseString);

            response.ContentType = "text/plain; charset=utf-8";
            response.ContentLength64 = buffer.Length;
            await response.OutputStream.WriteAsync(buffer, 0, buffer.Length);
        }
        else
        {
            response.StatusCode = (int)HttpStatusCode.BadRequest;
            response.Close();
            return;
        }
    }
    static void Main(string[] args)
    {
        try
        {
            string json_config = File.ReadAllText("secrets.json");
            Config? config = JsonSerializer.Deserialize<Config>(json_config);
            if (config == null)
            {
                Console.WriteLine("Config file is NULL");
                return;
            }

            string server_config = File.ReadAllText("server_config.json");
            ServerConfig? serverConfig = JsonSerializer.Deserialize<ServerConfig>(server_config);
            if (serverConfig == null)
            {
                Console.WriteLine("Server config file is NULL");
                return;
            }

            if (serverConfig.host == null || serverConfig.port == null)
            {
                Console.WriteLine("Host or port is NULL");
                return;
            }

            var listener = new HttpListener();
            listener.Prefixes.Add($"http://{serverConfig.host}:{serverConfig.port}/");
            listener.Start();
            Console.WriteLine($"Server started on http://{serverConfig.host}:{serverConfig.port}/");

            HttpClient httpClient = new HttpClient();

            while (true)
            {
                HttpListenerContext context = listener.GetContext();
                HttpHandler(context, httpClient, config);
            }
        }
        catch (IOException ex)
        {
            Console.WriteLine("Ошибка чтения файла конфигурации. " + ex.Message);
        }
    }
}