using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turis.Location
{
    public class Point
    {
        public double lng { get; set; }
        public double lat { get; set; }

    }
    public class Place
    {
        public string? name { get; set; }
        public string? country { get; set; }
        public string? city { get; set; }
        public Point? point { get; set; }
        public string? osm_value { get; set; }
    }
}
