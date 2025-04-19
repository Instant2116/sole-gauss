using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;

namespace SoLE_Gauss
{
    internal class Logger
    {
        string Path {  get; set; }
        public Logger(string filePath) {
        this.Path = filePath;   
        }
        private void WriteCsv(List<Report> reports)
        {

        }
        public static void WriteCsv(string filePath, List<Report> reports)
        {
            try
            {
                using (var writer = new StreamWriter(filePath))
                using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                {
                    csv.WriteRecords(reports);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing CSV file: {ex.Message}");
            }
        }
    }
}
