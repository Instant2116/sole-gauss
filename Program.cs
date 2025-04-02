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
        string path = "SimpleSoLE1.txt";
        public static void Main()
        {
            GaussElimination.swapRows(a1, 1, 2);
            for (int i = 0; i < a1.Length; i++)
            {
                for (int j = 0; j < a1[i].Length; j++)
                {
                    Console.Write(a1[i][j] + " ");
                }
                Console.WriteLine();
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


        public static string FormatRepeatingDecimal(string s)
        {
            string separator = NumberFormatInfo.CurrentInfo.CurrencyDecimalSeparator;// it can be ',' or '.' or what ever
            if (s.Contains(separator))
            {
                string ss = s.ToString().Split(separator)[1];
                if (ss[0] == ss[1] && ss[1] == ss[2])
                {
                    ss = $".({ss[0]})";
                }
                ss = s.ToString().Split(',')[0] + ss;
                return ss;
            }
            else
            {
                return s;
            }
        }
    }
}





/*

/*SoLE simple = new SoLE(Resources.Path + path);
simple.show();





/*

*/

