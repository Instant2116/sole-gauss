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
            Stopwatch stopwatch = new Stopwatch();
            //this is for files
            /*
            string rPath = Resources.Path + path;
             SoLE les = new SoLE(rPath);
             double[][] A = les.GetMatrix();
            double[][] B = Matrix.Copy(A);
            LinearSystem linearSystem = new LinearSystem();
           */

            double[][] x = //new double[] { 1, 2, 3 };
            {
                new double[] {1,2,3},
                new double[] {4,5,6},
                new double[] {7,8,9},
            };

            int n = 3; // Size of the system (e.g., 3x3)
            var sole = SoLE.GenerateSolvableSystem(3);
            int[][] a = sole.A;
            double[][] A1 = a.Select(row => row.Select(x => (double)x).ToArray()).ToArray();
            double[] b = sole.b;
            double[][] A = new double[A1.Length][];
            for ( int i = 0; i < A.Length; i++ )
            {
                A[i] = A1[i];
                Array.Resize(ref A[i], A[i].Length+1);
                A[i][A[i].Length-1] = b[i];
            }
            Console.WriteLine("Coefficient Matrix (A):");
            Matrix.printMatrix(A);

            Console.WriteLine("Right-hand side vector (b):");
            Console.WriteLine(String.Join(";", b));
            GaussElimination gaussElimination = new GaussElimination(A);
            gaussElimination.Eliminate();
            gaussElimination.Solve();
            Console.WriteLine("CheckSolution; "+ gaussElimination.CheckSolution());
            gaussElimination.showSolution();

            /*
            double [][] B = LinearSystem.getSubMatrixDeterminant(A);

            Console.WriteLine("A");
            Matrix.printMatrix(A);
            Console.WriteLine("B");
            Matrix.printMatrix(B);
            Console.WriteLine(LinearSystem.Determinant(A));*/



            Console.WriteLine();
            //Gaus 
            /*
            var sysL = LinearSystem.GenerateSolvableSystem(1000);
            double[][] A1 = sysL.A;
            double[] rhs = sysL.b;
            double[][] A = new double[A1.Length+1][];
            A[A1.Length + 1] = rhs;
            Console.WriteLine("gaussElimination");
            GaussElimination gaussElimination = new GaussElimination(A);
            gaussElimination.Eliminate();
            gaussElimination.showSoLE();
            gaussElimination.Solve();
            gaussElimination.showSolution();
            Console.WriteLine("singularityFlag: " + gaussElimination.singularityFlag);
            Console.WriteLine("CheckSolution: " + gaussElimination.CheckSolution());
            Console.WriteLine("showSolution:\n "); gaussElimination.showSolution();*/








        }
    }
}



