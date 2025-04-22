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
using DotNetEnv;
using System.Collections;

namespace Turis
{
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
                Env.Load();

                string? location_api_key = Environment.GetEnvironmentVariable("LOCATION_API_KEY") ?? Env.GetString("LOCATION_API_KEY");
                string? places_api_key = Environment.GetEnvironmentVariable("PLACES_API_KEY") ?? Env.GetString("PLACES_API_KEY");

                if (location_api_key == null || places_api_key == null)
                {
                    Console.WriteLine("api keys - NULL");
                    return;
                }

                Config config = new Config();
                config.location_api_key = location_api_key;
                config.places_api_key = places_api_key;

                string? host = Environment.GetEnvironmentVariable("HOST") ?? Env.GetString("HOST", "default");
                string? port = Environment.GetEnvironmentVariable("PORT") ?? Env.GetString("PORT");

                if (host == null)
                {
                    Console.WriteLine("host == NULL");
                    return;
                }
                if (port == null)
                {
                    Console.WriteLine("port == NULL");
                    return;
                }

                ServerConfig serverConfig = new ServerConfig();
                serverConfig.port = port;
                serverConfig.host = host;

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
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}