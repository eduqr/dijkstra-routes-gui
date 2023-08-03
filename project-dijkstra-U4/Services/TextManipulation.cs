using Microsoft.Win32;
using project_dijkstra_U4.Folder;
using project_dijkstra_U4;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace project_dijkstra_U4.Services
{
    public class TextManipulation
    {
        MainWindow main = new MainWindow();
        MapServices maps = new MapServices();
        
        private int citiesToAdd;

        public void ValidateFile(string rutaArchivo)
        {
            if (!File.Exists(rutaArchivo))
            {
                MessageBox.Show("El archivo no existe");
                return;
            }
        }

        public bool ValidateLine(string line)
        {
            if (line == null)
            {
                MessageBox.Show("El archivo está vacío.");
                return false;
            }
            return true;
        }
        public void NewFile(string TextTxtArchivo)
        {
            string baseFileName = "CiudadesID";
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string path;

            int fileNumber = 1;
            while (true)
            {
                string fileName = $"{baseFileName}({fileNumber}).txt";
                path = System.IO.Path.Combine(desktop, fileName);

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
            TextTxtArchivo = path;
        }

        public void OpenFile(string TextTxtArchivo)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Archivos de texto (*.txt)|*.txt";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            openFileDialog.ShowDialog();

            TextTxtArchivo = openFileDialog.FileName;
        }

        public void ReadingButton(string TextTxtArchivo)
        {
            string rutaArchivo = TextTxtArchivo;

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
            maps.cities.Clear();
            maps.citiesList.Clear();

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

            maps.CreateMap();

            while (!lector.EndOfStream)
            {
                ProcessLine(lector.ReadLine());
            }

            maps.GetCities();
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

            maps.AddCityToMap(ciudad1);
            maps.AddCityToMap(ciudad2);
        }

        

    }
}
