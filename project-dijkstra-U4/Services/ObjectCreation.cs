using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;

namespace project_dijkstra_U4.Services
{
    public class ObjectCreation
    {
        public Ellipse CreateCityEllipse(string cityName)
        {
            Ellipse cityEllipse = new Ellipse
            {
                Width = 20,
                Height = 20,
                Fill = Brushes.Red,
                ToolTip = cityName
            };
            return cityEllipse;
        }
    }
}
