using project_dijkstra_U4.Folder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;

namespace project_dijkstra_U4.Services
{
    public class MapServices
    {
        MainWindow main = new MainWindow();
        ObjectCreation objcreation = new ObjectCreation();
        public int MapRows = 20;
        public int MapCols = 30;
        public int[,] map;
        public List<City> citiesList = new List<City>();
        public List<City> cities;
        public Ellipse[,] cityEllipses;
        private int citiesToAdd;

        public void CreateMap()
        {
            

            main.gridMap.Children.Clear();

            main.gridMap.RowDefinitions.Clear();
            main.gridMap.ColumnDefinitions.Clear();

            for (int row = 0; row < MapRows; row++)
            {
                main.gridMap.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            }

            for (int col = 0; col < MapCols; col++)
            {
                main.gridMap.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
            }

            for (int row = 0; row < MapRows; row++)
            {
                for (int col = 0; col < MapCols; col++)
                {
                    TextBlock textBlock = new TextBlock
                    {
                        Text = map[row, col] == 1 ? "\u2592" : "\u2593",
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
                    main.gridMap.Children.Add(textBlock);
                }
            }
        }

        public void AddCityToMap(string cityName)
        {
            if (cities.Count >= citiesToAdd)
            {
                return; // No se agregarán más ciudades si ya se ha alcanzado el límite
            }

            Random random = new Random();

            while (true)
            {
                int row = random.Next(MapRows);
                int col = random.Next(MapCols);

                if (map[row, col] == 0 && !CityExistsAt(row, col))
                {
                    if (!citiesList.Any(c => c.Name == cityName))
                    {
                        City city = new City();
                        city.Name = cityName;
                        city.Column = col;
                        city.Row = row;



                        Ellipse cityEllipse = objcreation.CreateCityEllipse(cityName);
                        Grid.SetRow(cityEllipse, row);
                        Grid.SetColumn(cityEllipse, col);

                        main.gridMap.Children.Add(cityEllipse);
                        cityEllipses[row, col] = cityEllipse;
                        // col
                        Point localCoord = cityEllipse.TranslatePoint(new Point(city.Column, city.Row), main.canvasMap);
                        city.xPos = localCoord.X;
                        city.yPos = localCoord.Y;
                        cities.Add(city);
                        citiesList.Add(city);
                    }
                    break;
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

        public void GetCities()
        {
            main.CBox_Origen.ItemsSource = cities;
            main.CBox_Destino.ItemsSource = cities;
            main.CBox_Origen.DisplayMemberPath = "Name";
            main.CBox_Destino.DisplayMemberPath = "Name";
        }

        public void DrawLineBetweenCities(string city1Name, string city2Name)
        {
            City city1 = citiesList.FirstOrDefault(c => c.Name == city1Name);
            City city2 = citiesList.FirstOrDefault(c => c.Name == city2Name);

            if (city1 == null || city2 == null)
            {
                MessageBox.Show("Una o ambas ciudades no existen.");
                return;
            }

            double x1 = city1.xPos * 26.13;
            double y1 = city1.yPos * 23;
            double x2 = city2.xPos * 26.13;
            double y2 = city2.yPos * 23;

            Line line = new Line
            {
                X1 = x1 + 13.06,
                Y1 = y1 + 11.3,
                X2 = x2 + 13.06,
                Y2 = y2 + 11.3,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                StrokeDashArray = new DoubleCollection(new double[] { 2, 2 })
            };

            // TEST DE TEXTO
            TextBlock textBlock = new TextBlock
            {
                Background = Brushes.White,
                Foreground = Brushes.Black,
                Text = city1Name + "_" + city2Name
            };

            Canvas.SetLeft(textBlock, (x1 + x2) / 2);
            Canvas.SetTop(textBlock, (y1 + y2) / 2);

            main.canvasMap.Children.Add(line);
            main.canvasMap.Children.Add(textBlock);
        }
    }
}
