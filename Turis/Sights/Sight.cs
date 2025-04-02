using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Turis.Sights
{
    public class Properties
    {
        public string? xid { get; set; }
        public string? name { get; set; }
        public double dist { get; set; }
        public string? kinds { get; set; }
    }
    public class Sight
    {
        public string? id { get; set; }
        public Properties? properties { get; set; }
    }
}
