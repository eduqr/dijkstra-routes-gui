using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_dijkstra_U4.Folder
{
    public class Connection
    {
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public int Distance { get; set; }

        public Connection(string fromCity, string toCity, int distance)
        {
            FromCity = fromCity;
            ToCity = toCity;
            Distance = distance;
        }
    }
}
