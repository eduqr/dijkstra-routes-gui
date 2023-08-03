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
            CompositionTarget.Rendering += OnRendering;

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
                        City city = new City(cityName, row, col);
                        cities.Add(city);
                        citiesList.Add(city);

                        Ellipse cityEllipse = CreateCityEllipse(cityName);
                        Grid.SetRow(cityEllipse, row);
                        Grid.SetColumn(cityEllipse, col);
                        gridMap.Children.Add(cityEllipse);
                        cityEllipses[row, col] = cityEllipse;
                    }
                    break;
                }
            }
        }

        private Ellipse CreateCityEllipse(string cityName)
        {
            Ellipse cityEllipse = new Ellipse
            {
                Width = 10,
                Height = 10,
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
    }
}
