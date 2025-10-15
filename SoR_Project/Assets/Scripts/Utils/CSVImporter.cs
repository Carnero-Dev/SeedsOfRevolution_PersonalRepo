using System.IO;
using System;
/// <summary>
/// Clase que se encarga de importar un CSV y almacena su información en un String que devuelve la función ImportCSV.
/// Author: Carlos Carnero Cabrera
/// </summary>
public class CSVImporter
{
    public static string[,] ImportCSV(String path) {
        string[] lines = File.ReadAllLines(path);
        int rows = lines.Length;
        int cols = lines[0].Split(",").Length;

        string[,] result = new string[rows, cols];
        for (int r = 0; r < rows; r++) {
            string[] values = lines[r].Split(",");
            for (int c = 0; c < cols; c++) {
                result[r, c] = values[c];
            }
        }
        return result;
    }
}
