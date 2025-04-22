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
        _config.location_api_key = location_api_key;
        _config.places_api_key = places_api_key;
    }

    [Fact]
    public async Task TestOKCodeStatus()
    {
        var places_res = await LocationApi.GetPlaces("moscow", _client, _config);
        Assert.Equal((int)HttpStatusCode.OK, (int)places_res.StatusCode);
    }
    [Fact]
    public async Task TestBadCodeStatus()
    {
        var places_res = await LocationApi.GetPlaces(String.Empty, _client, _config);
        Assert.Equal((int)HttpStatusCode.BadRequest, (int)places_res.StatusCode);
    }
    [Fact]
    public async Task TestOKCodeStatusAPI2()
    {
        var sights = await SightsApi.GetSights("-117.000165", "46.7323875", _client, _config, "1000");
        Assert.Equal((int)HttpStatusCode.OK, (int)sights.StatusCode);
    }
    [Fact]
    public async Task TestBadCodeStatusAPI2()
    {
        var sights = await SightsApi.GetSights(String.Empty, "46.7323875", _client, _config, "1000");
        Assert.Equal((int)HttpStatusCode.BadRequest, (int)sights.StatusCode);
    }

}