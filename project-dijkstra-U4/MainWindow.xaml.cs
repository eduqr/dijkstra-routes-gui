using Microsoft.Win32;
using project_dijkstra_U4.Folder;
using System;
using System.Collections.Generic;
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
        private int citiesToAdd; // Declarar citiesToAdd como una variable miembro de la clase

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
            gridMap.Children.Clear(); // Limpiamos el contenido existente antes de actualizar el mapa

            gridMap.RowDefinitions.Clear(); // Limpiamos las definiciones de filas
            gridMap.ColumnDefinitions.Clear(); // Limpiamos las definiciones de columnas

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
            cities.Clear();
            citiesList.Clear();
            string nombreArchivo = TXT_Archivo.Text;
            string escritorio = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            string rutaArchivo = Path.Combine(escritorio, nombreArchivo);

            try
            {
                if (!File.Exists(rutaArchivo))
                {
                    MessageBox.Show("El archivo no existe en la ruta especificada.");
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

                if (int.TryParse(linea, out citiesToAdd)) //Pasamos el valor de la linea a la variable citiesToAdd
                {
                    CreateMap();

                    while (!lector.EndOfStream)
                    {
                        linea = lector.ReadLine();
                        if (!string.IsNullOrWhiteSpace(linea)) // Verificar que la línea no esté vacía ni sea nula
                        {
                            string[] datos = linea.Split(' ');
                            if (datos.Length >= 3) // Verificar que la línea contenga al menos tres elementos
                            {
                                string ciudad1 = datos[0];
                                string ciudad2 = datos[1];
                                int distancia;

                                if (int.TryParse(datos[2], out distancia))
                                {
                                    //lógica para crear conexiones entre las ciudades basadas en las distancias

                                    // Luego, si es posible, agrega las ciudades al mapa
                                    AddCityToMap(ciudad1);
                                    AddCityToMap(ciudad2);
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
                    MessageBox.Show("El archivo no tiene un formato válido para el número de ciudades.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar leer el archivo: " + ex.Message);
            }
        }

        private List<City> citiesList = new List<City>();

        private void AddCityToMap(string cityName)
        {
            Random random = new Random();

            while (cities.Count < citiesToAdd)
            {
                int row = random.Next(MapRows);
                int col = random.Next(MapCols);

                if (map[row, col] == 0 && !CityExistsAt(row, col))
                {
                    // Verificar si la ciudad ya existe en la lista de ciudades
                    if (!citiesList.Any(c => c.Name == cityName))
                    {
                        City city = new City(cityName, row, col);
                        cities.Add(city);
                        citiesList.Add(city);

                        Ellipse cityEllipse = new Ellipse
                        {
                            Width = 10,
                            Height = 10,
                            Fill = Brushes.Red // Punto rojo
                        };

                        // Añadir el nombre de la ciudad como un atributo del Ellipse
                        cityEllipse.ToolTip = cityName;

                        

                        Grid.SetRow(cityEllipse, row);
                        Grid.SetColumn(cityEllipse, col);
                        gridMap.Children.Add(cityEllipse);
                        cityEllipses[row, col] = cityEllipse;
                    }
                    break;
                }
            }
        }

        private void CityEllipse_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is Ellipse cityEllipse)
            {
                string cityName = cityEllipse.ToolTip.ToString();

                // Mostrar la leyenda cerca del punto rojo
                ToolTip tooltip = new ToolTip
                {
                    Content = cityName,
                    
                };
                // Establecer el ToolTip en el Ellipse para mostrar la leyenda cerca del punto rojo
                cityEllipse.ToolTip = tooltip;
            }
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
    }
}
