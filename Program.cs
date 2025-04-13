using SoLE_Gauss;
using System.Resources;
using System.IO;
using System.Text;
using System.Globalization;
using System.Diagnostics;
using System.Collections.Generic;


namespace SoLE_Gauss
{
    class Program
    {

        static string path = "MidSoLE2.txt";
        public static void Main()
        {
            //this is for files
            /*
            string rPath = Resources.Path + path;
             SoLE les = new SoLE(rPath);
             double[][] A = les.GetMatrix();
            double[][] B = Matrix.Copy(A);
            double[][] x = //new double[] { 1, 2, 3 };
            {
                new double[] {2, 1, -2},
                new double[] {1, -1, -1},
                new double[] {1, 1, 3}
            };
            */

            Stopwatch stopwatch = new Stopwatch();
            int n = 5000;
            var sole = SoLE.GenerateSolvableSystem(n);
            double[][] A1 = sole.A;
            double[] b = sole.rhs;
            double[][] A = new double[A1.Length][];
            for (int i = 0; i < A.Length; i++ )
            {
                A[i] = A1[i];
                Array.Resize(ref A[i], A[i].Length+1);
                A[i][A[i].Length-1] = b[i];
            }
            //Matrix.printMatrix(A);
            Console.Write("gaussEliminationMultitask: ");
            GaussEliminationMultitask gaussEliminationMultitask = new GaussEliminationMultitask(A, 12);
            stopwatch.Start();
            gaussEliminationMultitask.Eliminate();
            stopwatch.Stop();
            Console.WriteLine("stopwatch: "+stopwatch.ElapsedMilliseconds);
            gaussEliminationMultitask.Solve();
            Console.WriteLine("Solve: done");
            Console.WriteLine("CheckSolution; " + gaussEliminationMultitask.CheckSolution()); 

            Console.Write("gaussElimination: ");
            GaussElimination gaussElimination = new GaussElimination(A);
            stopwatch.Restart();
            gaussElimination.Eliminate();
            stopwatch.Stop();
            Console.WriteLine("stopwatch: " + stopwatch.ElapsedMilliseconds);
            gaussElimination.Solve();
            Console.WriteLine("Solve:done");
            Console.WriteLine("CheckSolution; "+ gaussElimination.CheckSolution());









        }
    }
}



