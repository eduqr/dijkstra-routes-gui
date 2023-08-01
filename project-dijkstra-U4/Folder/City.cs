using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace project_dijkstra_U4.Folder
{
    public class City
    {
        public string Name { get; set; }
        public int Row { get; set; }
        public int Column { get; set; }

        public City(string name, int row, int column)
        {
            Name = name;
            Row = row;
            Column = column;
        }
    }
}
