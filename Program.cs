using SoLE_Gauss;
using System.Resources;
using System.IO;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;
using System.Net.Http.Headers;
using Microsoft.Win32.SafeHandles;
using System.Drawing;
using System.Threading;


namespace SoLE_Gauss
{
    class Program
    {
        static string fileName = "GaussMultitask";
        static string fileType = ".csv";
        public static void Main()
        {
            
            string fullFileName, relativePath;
            List<Report> reportList = new List<Report>();
            Stopwatch stopwatch = new Stopwatch();
            for (int size = 4500; size <= 5000; size += 500)
            {
                for (int threads = 2; threads <= 12; threads += 2)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine($"size:{size}; threads:{threads}; iteration: {i}");
                        var A = SoLE.GenerateSolvableMatrix(size);
                        Console.Write("gaussEliminationMultitask: ");
                        GaussEliminationMultitask gaussEliminationMultitask = new GaussEliminationMultitask(A, threads);
                        stopwatch.Restart();
                        gaussEliminationMultitask.Eliminate();
                        stopwatch.Stop();
                        Console.WriteLine("done;");
                        TimeSpan timeGaussMultitask = stopwatch.Elapsed;
                        reportList.Add(new Report(size, threads, timeGaussMultitask));
                        Console.WriteLine();
                    }
                }

                fullFileName = fileName + "_" + size + fileType;
                relativePath = Resources.Path + fullFileName;
                Logger.WriteCsv(relativePath, reportList);
                Console.WriteLine("Report added;");
                Console.WriteLine();
                reportList.Clear();
            }
        }
    }
}



