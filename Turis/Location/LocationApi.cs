namespace Turis.Location;
public static class LocationApi
{
    public static Task<HttpResponseMessage> GetPlaces(string locName, HttpClient httpClient, Config config)
    {
        return httpClient.GetAsync($"https://graphhopper.com/api/1/geocode?q={locName}&locale=ru&limit=10&reverse=false&debug=false&provider=default&key={config.locationApiKey}");
    }
}
