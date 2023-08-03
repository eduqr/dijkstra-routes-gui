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

        public double xPos { get; set; }
        public double yPos { get; set; }
        public List<Connection> Connections { get; } = new List<Connection>();

        public void AddConnection(string fromCity, string toCity, int distance)
        {
            Connection connection = new Connection(fromCity, toCity, distance);
            Connections.Add(connection);
        }
        //public City(string name, int row, int column, int xpos, int ypos)
        //{
        //    Name = name;
        //    Row = row;
        //    Column = column;
        //    xPos = xpos; 
        //    yPos = ypos;
        //}
    }
}
