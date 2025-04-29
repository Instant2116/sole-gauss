using SoLE_Gauss;
using System.Diagnostics;


namespace SoLE_Gauss
{
    class Program
    {
        static string fileName1 = "ResultsSequential";
        static string fileName2 = "ResultsParalell";
        //static string fileName3 = "ResultsSegmented";
        static string fileType = ".csv";
        public static void Main()
        {
            List<Report> reportList1 = new List<Report>();//Sequential
            List<Report> reportList2 = new List<Report>();//Paralell
            //List<Report> reportList3 = new List<Report>();//Segmented
            Stopwatch stopwatch = new Stopwatch();

            string testSoLEFile1 = "SimpleSoLE2.txt";
            SoLE soLE1 = new SoLE(Resources.Path + testSoLEFile1);
            GaussElimination gaussEliminationDemonstration = new GaussElimination(soLE1.GetMatrix());
            gaussEliminationDemonstration.Eliminate();
            gaussEliminationDemonstration.Solve();
            Console.WriteLine("SoLE:");
            soLE1.show();
            Console.WriteLine();
            Console.WriteLine("Solution:");
            gaussEliminationDemonstration.showSolution();
            Console.WriteLine();
            Console.WriteLine("CheckSolution: " + gaussEliminationDemonstration.CheckSolution());

            string testSoLEFile2 = "MidSoLE2.txt";
            SoLE soLE2 = new SoLE(Resources.Path + testSoLEFile2);
            GaussEliminationParalell gaussEliminationParalellDemonstration = new GaussEliminationParalell(soLE2.GetMatrix(), 10);
            gaussEliminationParalellDemonstration.Eliminate();
            gaussEliminationParalellDemonstration.Solve();
            Console.WriteLine("SoLE:");
            soLE2.show();
            Console.WriteLine();
            Console.WriteLine("Solution:");
            gaussEliminationParalellDemonstration.showSolution();
            Console.WriteLine();
            Console.WriteLine("CheckSolution: " + gaussEliminationParalellDemonstration.CheckSolution());


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
                        GaussElimination gaussEliminationSequential = new GaussElimination(A);
                        stopwatch.Restart();
                        gaussEliminationSequential.Eliminate();
                        stopwatch.Stop();
                        Console.WriteLine("done;");
                        TimeSpan gaussEliminationSequentialTime = stopwatch.Elapsed;
                        reportList2.Add(new Report(size, threads, timeGaussMultitask));
                        Console.WriteLine();
                        /*
                        for (int j = 1; j < 10; j++)
                        {
                            Console.Write("gaussEliminationSegmented: ");
                            GaussEliminationParalell gaussEliminationSegmented = new GaussEliminationParalell(A, threads);
                            stopwatch.Restart();
                            gaussEliminationSegmented.EliminateSegmented((uint)j);
                            stopwatch.Stop();
                            Console.WriteLine("done;");
                            TimeSpan gaussEliminationSegmentedTime = stopwatch.Elapsed;
                            reportList3.Add(new Report(size, threads, gaussEliminationSegmentedTime));
                            Console.WriteLine();
                        }*/

                    }
                }
                string fullFileName1 = fileName1 + "_" + size + fileType;
                string fullFileName2 = fileName2 + "_" + size + fileType;
                //string fullFileName3 = fileName3 + "_" + size + fileType;
                string relativePath1 = Resources.Path + fullFileName1;
                string relativePath2 = Resources.Path + fullFileName2;
                //string relativePath3 = Resources.Path + fullFileName3;
                Logger.WriteCsv(relativePath1, reportList1);
                Logger.WriteCsv(relativePath2, reportList2);
                //Logger.WriteCsv(relativePath3, reportList3);
                Console.WriteLine("Reports were added;");
                Console.WriteLine();
                reportList1.Clear();
                reportList2.Clear();
                //reportList3.Clear();
            }
        }
    }
}



