using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Turis.Location
{
    public class LocationApi
    {
        static public Task<HttpResponseMessage> GetPlaces(string loc_name, HttpClient httpClient, Config config)
        {
            return httpClient.GetAsync($"https://graphhopper.com/api/1/geocode?q={loc_name}&locale=ru&limit=10&reverse=false&debug=false&provider=default&key={config.location_api_key}");
        }
    }
}
