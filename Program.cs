using SoLE_Gauss;
using System.Diagnostics;


namespace SoLE_Gauss
{
    class Program
    {
        static string fileName1 = "ResultsSequential";
        static string fileName2 = "ResultsParalell";
        static string fileType = ".csv";
        public static void Main()
        {

            string fullFileName1, fullFileName2, relativePath1, relativePath2;
            List<Report> reportList1 = new List<Report>();
            List<Report> reportList2 = new List<Report>();
            Stopwatch stopwatch = new Stopwatch();

            for (int size = 1000; size <= 5000; size += 500)
            {
                for (int threads = 2; threads <= 12; threads += 2)
                {
                    for (int i = 0; i < 10; i++)
                    {
                        Console.WriteLine($"size:{size}; threads:{threads}; iteration: {i}");
                        var A = SoLE.GenerateSolvableMatrix(size);

                        Console.Write("gaussEliminationParalell: ");
                        GaussEliminationParalell gaussEliminationParalell = new GaussEliminationParalell(A, threads);
                        stopwatch.Restart();
                        gaussEliminationParalell.Eliminate();
                        stopwatch.Stop();
                        Console.WriteLine("done;");
                        TimeSpan timeGaussMultitask = stopwatch.Elapsed;
                        reportList1.Add(new Report(size, threads, timeGaussMultitask));

                        Console.Write("gaussEliminationSequential: ");
                        GaussElimination gaussElimination = new GaussElimination(A);
                        stopwatch.Restart();
                        gaussElimination.Eliminate();
                        stopwatch.Stop();
                        Console.WriteLine("done;");
                        TimeSpan gaussEliminationSequential = stopwatch.Elapsed;
                        reportList2.Add(new Report(size, threads, timeGaussMultitask));
                        Console.WriteLine();
                    }
                }

                fullFileName1 = fileName1 + "_" + size + fileType;
                fullFileName2 = fileName2 + "_" + size + fileType;
                relativePath1 = Resources.Path + fullFileName1;
                relativePath2 = Resources.Path + fullFileName2;
                Logger.WriteCsv(relativePath1, reportList1);
                Logger.WriteCsv(relativePath2, reportList2);
                Console.WriteLine("Reports were added;");
                Console.WriteLine();
                reportList1.Clear();
                reportList2.Clear();
            }
        }
    }
}



