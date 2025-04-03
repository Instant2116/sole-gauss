using SoLE_Gauss;
using System.Resources;
using System.IO;
using System.Text;
using System.Globalization;


namespace SoLE_Gauss
{
    class Program
    {
        static double[][] a =
        {
            new double[] {1,0,1,6},
            new double[] {0,-3,1,7},
            new double[] {2,1,3,15}
};
        static double[][] a1 =
        {
            new double[] {2,1,-2,3},
            new double[] {1,-1,-1,0},
            new double[] {1,1,3,12}
};
        static string path = "SimpleSoLE4.txt";
        public static void Main()
        {

            string rPath = Resources.Path + path;
            SoLE les = new SoLE(rPath);
            double[][] m = les.GetMatrix();

            foreach (var r in m)
            {
                foreach (var n in r)
                {
                    Console.Write(n + ";");
                }
                Console.WriteLine();
            }
            GaussElimination gaussElimination = new GaussElimination(m);
            gaussElimination.Eliminate();
            var sol = gaussElimination.Solve();
            Console.WriteLine($"Solution fit as result: {gaussElimination.CheckSolutionQuick(sol)}");
            foreach (var i  in sol)
            {
                Console.WriteLine($"var: {i.Key};\tval: {i.Value}");
            }

            /*
            GaussElimination gaussElimination = new GaussElimination(a);
            double[][] b = gaussElimination.Eliminate();
            for (int i = 0; i < b.Length; i++)
            {
                for (int j = 0; j < b[i].Length; j++)
                {
                    Console.Write(FormatRepeatingDecimal(b[i][j].ToString()) + "\t");
                }
                Console.WriteLine();
            }

            var c = gaussElimination.Solve();
            var c1 = c.ToArray();
            for (int i = 0; i < c1.Length; i++)
            {
                Console.Write($"Var: {i};\t Solution: {FormatRepeatingDecimal(c[i].ToString())}");
                Console.WriteLine();
            }
            */


        }
    }
}






/*

/*

*/

