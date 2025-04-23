namespace Turis.Sights;
public static class SightsApi
{
    public static Task<HttpResponseMessage> GetSights(string lon, string lat, HttpClient httpClient, Config config, string radius)
    {
        return httpClient.GetAsync($"https://api.opentripmap.com/0.1/ru/places/radius?radius={radius}&lon={lon}&lat={lat}&apikey={config.placesApiKey}");
    }
}