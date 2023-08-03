using Microsoft.Win32;
using project_dijkstra_U4.Folder;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;
using Path = System.IO.Path;

namespace project_dijkstra_U4
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int citiesToAdd;

        private const int MapRows = 20;
        private const int MapCols = 30;

        private int[,] map;
        private List<City> cities;
        private Ellipse[,] cityEllipses;
        
        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += OnRendering;     // SI VAS A DEBUGUER COMENTA ESTA LÍNEA

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
        }

        private void CreateMap()
        {
            gridMap.Children.Clear();

            gridMap.RowDefinitions.Clear();
            gridMap.ColumnDefinitions.Clear();

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
                    gridMap.Children.Add(textBlock);
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

        private void BTN_Leer_Click(object sender, RoutedEventArgs e)
        {
            string rutaArchivo = TXT_Archivo.Text;

            if (!File.Exists(rutaArchivo))
            {
                MessageBox.Show("El archivo no existe");
                return;
            }

            try
            {
                using (StreamReader lector = new StreamReader(rutaArchivo))
                {
                    ProcessFile(lector);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar leer el archivo: " + ex.Message);
            }
            DibujarRutas();
        }

        private void ProcessFile(StreamReader lector)
        {
            cities.Clear();
            citiesList.Clear();

            string linea = lector.ReadLine();
            if (string.IsNullOrWhiteSpace(linea))
            {
                MessageBox.Show("El archivo está vacío.");
                return;
            }

            if (!int.TryParse(linea, out citiesToAdd))
            {
                MessageBox.Show("El archivo no tiene el formato adecuado");
                return;
            }

            CreateMap();

            while (!lector.EndOfStream)
            {
                ProcessLine(lector.ReadLine());
            }

            GetCities();
        }

        private void ProcessLine(string linea)
        {
            if (string.IsNullOrWhiteSpace(linea))
            {
                return;
            }

            string[] datos = linea.Split(' ');
            if (datos.Length < 3)
            {
                MessageBox.Show($"Formato incorrecto en la línea: {linea}");
                return;
            }

            string ciudad1 = datos[0];
            string ciudad2 = datos[1];
            int distancia;

            if (!int.TryParse(datos[2], out distancia))
            {
                MessageBox.Show($"Error al leer la distancia en la línea: {linea}");
                return;
            }

            AddCityToMap(ciudad1);
            AddCityToMap(ciudad2);

            City city1 = citiesList.FirstOrDefault(c => c.Name == ciudad1);
            City city2 = citiesList.FirstOrDefault(c => c.Name == ciudad2);

            // Agregar la conexión entre las ciudades
            if (city1 != null && city2 != null)
            {
                city1.AddConnection(ciudad1, ciudad2, distancia);
                city2.AddConnection(ciudad2, ciudad1, distancia);
            }
        }

        private List<City> citiesList = new List<City>();

        private void AddCityToMap(string cityName)
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

                        

                        Ellipse cityEllipse = CreateCityEllipse(cityName);
                        Grid.SetRow(cityEllipse, row);
                        Grid.SetColumn(cityEllipse, col);

                        gridMap.Children.Add(cityEllipse);
                        cityEllipses[row, col] = cityEllipse;
                        // col
                        Point localCoord = cityEllipse.TranslatePoint(new Point(city.Column, city.Row), canvasMap);
                        city.xPos = localCoord.X;
                        city.yPos = localCoord.Y;
                        cities.Add(city);
                        citiesList.Add(city);
                    }
                    break;
                }
            }
        }

        private Ellipse CreateCityEllipse(string cityName)
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

        private void CityEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse cityEllipse)
            {
                string cityName = cityEllipse.ToolTip.ToString();

                ToolTip tooltip = new ToolTip
                {
                    Content = cityName,

                };

                cityEllipse.ToolTip = tooltip;
            }
        }

        private void GetCities()
        {
            CBox_Origen.ItemsSource = cities;
            CBox_Destino.ItemsSource = cities;
            CBox_Origen.DisplayMemberPath = "Name";
            CBox_Destino.DisplayMemberPath = "Name";
        }

        private void OnRendering(object sender, EventArgs e)
        {
            Point mousePosition = Mouse.GetPosition(this);

            double windowWidth = ActualWidth;
            double windowHeight = ActualHeight;

            double topBarHeight = SystemParameters.WindowCaptionHeight;

            double centerX = windowWidth / 2;
            double centerY = (windowHeight - topBarHeight + 100) / 2;

            // Hacer coordenadas (0,0) en el centro del mapa
            double relativeX = mousePosition.X - centerX;
            double relativeY = mousePosition.Y - centerY;

            TB_Coords.Text = (relativeY < -240) ? "(X: #, Y: #)" : $"(X: {(int)relativeX}, Y: {(int)relativeY})";
        }

        private void BTN_OpenFileExplorer_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.ShowDialog();

            TXT_Archivo.Text = openFileDialog.FileName;
        }

        private void BTN_NewFile_Click(object sender, RoutedEventArgs e)
        {
            string baseFileName = "CiudadesID";
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path;

            int fileNumber = 1;
            while (true)
            {
                string fileName = $"{baseFileName}({fileNumber}).txt";
                path = Path.Combine(desktop, fileName);

                if (!File.Exists(path))
                {
                    break;
                }

                fileNumber++;
            }

            StringBuilder contentBuilder = new StringBuilder();
            contentBuilder.AppendLine("/**********************************************************************/");
            contentBuilder.AppendLine("/*                         dijkstra-routes-gui                        */");
            contentBuilder.AppendLine("/*                                                                    */");
            contentBuilder.AppendLine("/*              https://github.com/eduqr/dijkstra-routes              */");
            contentBuilder.AppendLine("/**********************************************************************/");
            contentBuilder.AppendLine("/* <total_ciudades>                                                   */");
            contentBuilder.AppendLine("/* <ciudad_1> <ciudad_2> <distancia>                                  */");
            contentBuilder.AppendLine("/*                                                                    */");
            contentBuilder.AppendLine("/* BORRE TODO EL CONTENIDO DE ESTE ARCHIVO Y GUARDE DESPUÉS DE EDITAR */");
            contentBuilder.AppendLine("/**********************************************************************/");

            File.WriteAllText(path, contentBuilder.ToString());

            Process.Start("notepad.exe", path);
            TXT_Archivo.Text = path;
        }

        private void DrawLineBetweenCities(string city1Name, string city2Name, string distance)
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
                StrokeDashArray = new DoubleCollection(new double[] {2, 2})
            };

            canvasMap.Children.Add(line);

            TextBlock textBlock = new TextBlock
            {
                Foreground = Brushes.Black,
                Text = city1Name + "_" + city2Name + "(" + distance + ")",
                FontWeight = FontWeights.Bold,
            };

            Canvas.SetLeft(textBlock, (x1 + x2) / 2);
            Canvas.SetTop(textBlock, (y1 + y2) / 2);
            canvasMap.Children.Add(textBlock);
        }

        private void DibujarRutas()
        {
            canvasMap.Children.Clear();

            string rutaArchivo = TXT_Archivo.Text;
           
            // Hay q acomodar esta shit
            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    MessageBox.Show("El archivo no existe");
                    return;
                }

                StreamReader lector = new StreamReader(rutaArchivo);
                string linea = lector.ReadLine();

                if (linea == null)
                {
                    MessageBox.Show("El archivo está vacío.");
                    lector.Close();
                    return;
                }

                if (int.TryParse(linea, out citiesToAdd))
                {
                    while (!lector.EndOfStream)
                    {
                        linea = lector.ReadLine();

                        if (!string.IsNullOrWhiteSpace(linea))
                        {
                            string[] datos = linea.Split(' ');
                            if (datos.Length >= 3)
                            {
                                string ciudad1 = datos[0];
                                string ciudad2 = datos[1];
                                int distancia;

                                if (int.TryParse(datos[2], out distancia))
                                {
                                    DrawLineBetweenCities(ciudad1, ciudad2, datos[2]);
                                }
                                else
                                {
                                    MessageBox.Show($"Error al leer la distancia en la línea: {linea}");
                                }
                            }
                            else
                            {
                                MessageBox.Show($"Formato incorrecto en la línea: {linea}");
                            }
                        }
                    }
                    lector.Close();
                }
                else
                {
                    MessageBox.Show("El archivo no tiene el formato adecuado");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar leer el archivo: " + ex.Message);
            }
        }

        private void BTN_CalcularRuta_Click(object sender, RoutedEventArgs e)
        {
            // Obtener las ciudades seleccionadas por el usuario
            City origen = (City)CBox_Origen.SelectedItem;
            City destino = (City)CBox_Destino.SelectedItem;

            if (origen == null || destino == null)
            {
                MessageBox.Show("Por favor, selecciona una ciudad de origen y una ciudad de destino.");
                return;
            }

            // Implementar el algoritmo de Dijkstra para encontrar la ruta más corta y la distancia total
            Tuple<List<City>, int> resultadoDijkstra = Dijkstra(origen, destino);
            List<City> rutaMasCorta = resultadoDijkstra.Item1;
            int distanciaTotal = resultadoDijkstra.Item2;

            // Mostrar la ruta resultante y la distancia total en el TextBox
            if (rutaMasCorta != null && rutaMasCorta.Count > 1)
            {
                StringBuilder rutaBuilder = new StringBuilder();
                foreach (var ciudad in rutaMasCorta)
                {
                    rutaBuilder.AppendLine(ciudad.Name);
                }
                
                LB_RutaMasCorta.ItemsSource = rutaMasCorta.Select(ciudad => ciudad.Name);
                LBL_DistanciaMinima.Content = $"Distancia total: {distanciaTotal} km";
                LB_RutaMasCorta.Visibility = Visibility.Visible;
                LBL_DistanciaMinima.Visibility = Visibility.Visible;
                DrawFastestRoute(CBox_Destino.Text);
            }
            else
            {
                LB_RutaMasCorta.ItemsSource = new List<string> { "No se encontró una ruta válida entre las ciudades seleccionadas." };
                LB_RutaMasCorta.Visibility = Visibility.Hidden;
            }
        }
        List<string> route = new List<string>();
        private Tuple<List<City>, int> Dijkstra(City origen, City destino)
        {
            route.Clear();
            Dictionary<City, int> distancias = new Dictionary<City, int>();
            Dictionary<City, City> previos = new Dictionary<City, City>();
            HashSet<City> visitados = new HashSet<City>();

            foreach (var city in citiesList)
            {
                distancias[city] = int.MaxValue;
                previos[city] = null;
            }

            distancias[origen] = 0;

            while (visitados.Count < citiesList.Count)
            {
                City actual = null;
                int minDistancia = int.MaxValue;

                foreach (var city in citiesList)
                {
                    if (!visitados.Contains(city) && distancias[city] < minDistancia)
                    {
                        minDistancia = distancias[city];
                        actual = city;
                    }
                }

                if (actual == null)
                    break;

                visitados.Add(actual);

                foreach (var conexion in actual.Connections)
                {
                    City vecino = citiesList.FirstOrDefault(c => c.Name == conexion.ToCity);

                    if (vecino != null && !visitados.Contains(vecino))
                    {
                        int distanciaACiudadVecina = distancias[actual] + conexion.Distance;
                        if (distanciaACiudadVecina < distancias[vecino])
                        {
                            distancias[vecino] = distanciaACiudadVecina;
                            previos[vecino] = actual;
                        }
                    }
                }
            }

            // Construir la ruta desde el destino hacia el origen
            List<City> ruta = new List<City>();
            City actualCity = destino;
            while (actualCity != null)
            {
                ruta.Insert(0, actualCity);
                route.Insert(0,actualCity.Name);
                actualCity = previos[actualCity];
            }

            // Obtener la distancia total desde el origen hasta el destino
            int distanciaTotal = distancias[destino];

            return Tuple.Create(ruta, distanciaTotal);
        }

        private void DrawFastestRoute(string destino)
        {
            canvasMap.Children.Clear();
            string[] ruta = route.ToArray();
            int i = 0;

            do
            {
                City city1 = citiesList.FirstOrDefault(c => c.Name == ruta[i]);
                City city2 = citiesList.FirstOrDefault(c => c.Name == ruta[i+1]);

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
                    Stroke = Brushes.DarkBlue,
                    StrokeThickness = 2,
                };

                canvasMap.Children.Add(line);

                TextBlock textBlock = new TextBlock
                {
                    Foreground = Brushes.Black,
                    Text = city1.Name + "_" + city2.Name + "(" +(i+1)+")",
                    FontWeight = FontWeights.Bold,
                };

                Canvas.SetLeft(textBlock, (x1 + x2) / 2);
                Canvas.SetTop(textBlock, (y1 + y2) / 2);
                canvasMap.Children.Add(textBlock);
                i++;
            } while (route[i]!=destino);
        }

    }
}
