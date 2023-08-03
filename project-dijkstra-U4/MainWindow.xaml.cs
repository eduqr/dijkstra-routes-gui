using Microsoft.Win32;
using project_dijkstra_U4.Folder;
using project_dijkstra_U4.Services;
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
        TextManipulation text = new TextManipulation();
        MapServices maps = new MapServices();

        public MainWindow()
        {
            InitializeComponent();
            CompositionTarget.Rendering += OnRendering;     // SI VAS A DEBUGUER COMENTA ESTA LÍNEA

            maps.map = new int[maps.MapRows, maps.MapCols];
            maps.cities = new List<City>();

            Random random = new Random();
            for (int row = 0; row < maps.MapRows; row++)
            {
                for (int col = 0; col < maps.MapCols; col++)
                {
                    maps.map[row, col] = random.Next(3);
                }
            }

            maps.CreateMap();
        }

        private void BTN_Leer_Click(object sender, RoutedEventArgs e)
        {
            text.ReadingButton(TXT_Archivo.Text);
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
            text.OpenFile(TXT_Archivo.Text);
        }

        private void BTN_NewFile_Click(object sender, RoutedEventArgs e)
        {
            text.NewFile(TXT_Archivo.Text);
        }

        

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            int citiesToAdd;
            canvasMap.Children.Clear();
            // Hay q acomodar esta shit
            try
            {
                text.ValidateFile(TXT_Archivo.Text);

                StreamReader lector = new StreamReader(TXT_Archivo.Text);
                string linea = lector.ReadLine();

                if(!text.ValidateLine(linea))
                    lector.Close();

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
                                    maps.DrawLineBetweenCities(ciudad1, ciudad2);
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
                }
                else
                {
                    lector.Close();
                    MessageBox.Show("El archivo no tiene el formato adecuado");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al intentar leer el archivo: " + ex.Message);
            }

        }
    }
}
