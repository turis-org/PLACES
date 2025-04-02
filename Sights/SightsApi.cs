using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace Turis.Sights
{
    public class SightsApi
    {
        static public Task<HttpResponseMessage> GetSights(string lon, string lat, HttpClient httpClient, Config config, string radius)
        {
            return httpClient.GetAsync($"https://api.opentripmap.com/0.1/ru/places/radius?radius={radius}&lon={lon}&lat={lat}&apikey={config.places_api_key}");
        }
    }
}
