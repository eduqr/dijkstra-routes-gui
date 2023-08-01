using project_dijkstra_U4.Folder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace project_dijkstra_U4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int MapRows = 20;
        private const int MapCols = 30;

        private int[,] map;
        private List<City> cities;
        private Ellipse[,] cityEllipses;

        public MainWindow()
        {
            InitializeComponent();

            map = new int[MapRows, MapCols];
            cityEllipses = new Ellipse[MapRows, MapCols];
            cities = new List<City>();

            Random random = new Random();
            for (int row = 0; row < MapRows; row++)
            {
                for (int col = 0; col < MapCols; col++)
                {
                    map[row, col] = random.Next(3);
                }
            }

            CreateMap();
            AddCities();
        }

        private void CreateMap()
        {
            for (int row = 0; row < MapRows; row++)
            {
                gridMap.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int col = 0; col < MapCols; col++)
            {
                gridMap.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int row = 0; row < MapRows; row++)
            {
                for (int col = 0; col < MapCols; col++)
                {
                    TextBlock textBlock = new TextBlock
                    {
                        Text = map[row, col] == 1 ? "\u2592" : "\u2593", // \u2593   // \u2588
                        FontSize = 40,
                        Foreground = map[row, col] == 1 ? Brushes.Blue : Brushes.LightGreen,
                        Padding = new Thickness(0),
                        Margin = new Thickness(0),
                        TextAlignment = TextAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };

                    Grid.SetRow(textBlock, row);
                    Grid.SetColumn(textBlock, col);
                    gridMap.Children.Add(textBlock);
                }
            }
        }

        private void AddCities()
        {
            Random random = new Random();
            int citiesToAdd = 10; // Número total de ciudades (se va a tener que cambiar por lo del txt)

            while (cities.Count < citiesToAdd)
            {
                int row = random.Next(MapRows);
                int col = random.Next(MapCols);

                if (map[row, col] == 0 && !CityExistsAt(row, col))
                {
                    City city = new City("Ciudad " + (cities.Count + 1), row, col);
                    cities.Add(city);

                    Ellipse cityEllipse = new Ellipse
                    {
                        Width = 10,
                        Height = 10,
                        Fill = Brushes.Red // Punto rojo
                    };

                    Grid.SetRow(cityEllipse, row);
                    Grid.SetColumn(cityEllipse, col);
                    gridMap.Children.Add(cityEllipse);
                    cityEllipses[row, col] = cityEllipse;
                }
            }
        }

        private bool CityExistsAt(int row, int col)
        {
            foreach (var city in cities)
            {
                if (city.Row == row && city.Column == col)
                    return true;
            }
            return false;
        }
    }
}
