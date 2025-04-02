using SoLE_Gauss;
using System.Resources;
using System.IO;
using System.Text;
string path = "SimpleSoLE1.txt";

double[][] a =
{
  new double[] {1,1,6 },
  new double[] { 1,-1,4}
    };
GaussElimination gaussElimination = new GaussElimination(a);
double[][] b = gaussElimination.Eliminate();
for (int i = 0; i < b.Length; i++)
{
    for (int j = 0; j < b[i].Length; j++)
    {
        Console.Write(b[i][j]+"\t");
    }
    Console.WriteLine();    
}

var c = gaussElimination.Solve();
var c1 = c.ToArray();
for (int i = 0; i < c1.Length; i++)
{
   Console.Write($"Var: {i};\t Solution: {c[i]}");
    Console.WriteLine();
}


/*SoLE simple = new SoLE(Resources.Path + path);
simple.show();





/*

*/