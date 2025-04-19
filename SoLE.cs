using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Drawing;

namespace SoLE_Gauss
{
    internal class SoLE // System of Linear Equations
    {
        public HashSet<string> variables { get; }
        List<Dictionary<string, double>> coefficientsNamed { get; }//List of coefficients 
        Dictionary<string, double> solutionNamed;
        List<string> rawSoLE;
        List<double[]> lhs; //left-hand side 
        List<double> rhs; //right-hand side 
        Dictionary<int, double> solutionIndexed;// index - coefficient
        string path;
        public SoLE(string path)
        {
            this.path = path;
            this.variables = new HashSet<string>();
            this.solutionNamed = new Dictionary<string, double>();
            this.coefficientsNamed = new List<Dictionary<string, double>>();
            this.lhs = new List<double[]>();
            this.rhs = new List<double>();
            rawSoLE = ReadSoLE(path);
            ParseData();

        }
        public void show()
        {
            for (int i = 0; i < coefficientsNamed.Count; i++)
            {
                var a1 = coefficientsNamed[i];
                string le = "";
                foreach (var a2 in a1)
                {
                    if (!string.IsNullOrEmpty(le) && a2.Value > 0)
                    {
                        le += '+';
                    }
                    if (a2.Value != 0 && a2.Value != 1 && a2.Value != -1)
                    {
                        le += FormatRepeatingDecimal(a2.Value.ToString()) + a2.Key;
                    }
                    else if (a2.Value == 1)
                    {
                        le += a2.Key;
                    }
                    else if (a2.Value == -1)
                    {
                        le += "-" + a2.Key;
                    }
                }
                Console.WriteLine(le + "=" + rhs[i]);
            }
        }

        public double[][] GetMatrix()
        {
            double[][] res = new double[coefficientsNamed.Count][];//coefficients.Count
            for (int i = 0; i < coefficientsNamed.Count; i++)
            {
                var d = coefficientsNamed[i];
                List<double> line = new List<double>();
                foreach (var v in variables)
                {
                    if (d.ContainsKey(v))
                        line.Add(d[v]);
                    else//if line does not contain variable, the variable must be 0, like 0*x does not show up in regular equetions
                        line.Add(0);
                }
                line.Add(rhs[i]);
                res[i] = line.ToArray();
            }
            return res;
        }
        List<string> ReadSoLE(string path)
        {
            try
            {
                return new List<string>(File.ReadAllLines(path));
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("File not found.");
                return new List<string>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return new List<string>();
            }
        }
        void ParseData()
        {//not handling 3*x, only 3x, need an update for that case
            if (rawSoLE == null)
            {
                Console.WriteLine("No data to parse");
                return;
            }
            foreach (string line in rawSoLE)
            {
                if (string.IsNullOrEmpty(line))
                {
                    Console.WriteLine("IsNullOrEmpty triggered, skipping");
                    continue;
                }

                string[] equetionParts = line.Split('=');//[0] - left, [1] - right
                equetionParts[0] = equetionParts[0].Replace("-", "|-");
                equetionParts[0] = equetionParts[0].Replace("+", "|+");
                List<string> tokens = new List<string>(equetionParts[0].Split('|'));
                if (string.IsNullOrEmpty(Regex.Replace(tokens[0], @"\s+", "")))
                    tokens.Remove(tokens[0]);//edge case when line starts with '-'
                Dictionary<string, double> values = new Dictionary<string, double>();
                foreach (string token in tokens)
                {
                    var a = ParseToken(token);
                    variables.Add(a.Key);
                    values[a.Key] = a.Value;
                }
                coefficientsNamed.Add(values);
                rhs.Add(Double.Parse(equetionParts[1]));
            }
        }
        public static bool ValidateToken(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                Console.WriteLine($"IsNullOrEmpty:{token}");
                return false;
            }
            int lastType;
            switch (true)
            {
                case true when IsMathOperand(token[0]):
                    lastType = 1;
                    break;
                case true when Char.IsDigit(token[0]):
                    lastType = 2;
                    break;
                case true when Char.IsLetter(token[0]):
                    lastType = 3;
                    break;
                default:
                    Console.WriteLine($"Unknown character: {token[0]}");
                    if (CharUnicodeInfo.GetUnicodeCategory('+') == CharUnicodeInfo.GetUnicodeCategory(token[0]))
                        Console.WriteLine("Wierd math symbols detected! Format text before letting it in!");
                    return false;
            }
            for (int i = 1; i < token.Length; i++)
            {
                int newType;
                switch (true)
                {
                    case true when IsMathOperand(token[i]):
                        newType = 1;
                        break;
                    case true when Char.IsDigit(token[i]):
                        newType = 2;
                        break;
                    case true when Char.IsLetter(token[i]):
                        newType = 3;
                        break;
                    default:
                        if (CharUnicodeInfo.GetUnicodeCategory('+') == CharUnicodeInfo.GetUnicodeCategory(token[i]))
                            Console.WriteLine("Wierd math symbols detected! Format text before letting it in!");
                        Console.WriteLine($"Unknown character: {token[i]}");
                        return false;
                }
                if (newType < lastType)
                {
                    return false;
                }
                else
                {
                    lastType = newType;
                }
            }
            if (lastType == 1)
            {
                return false;
            }
            return true;
        }
        static public KeyValuePair<string, double> ParseToken(string token)
        {
            Token temp = ParseTokenData(token);

            return new KeyValuePair<string, double>(temp.variable, temp.value * (temp.sign == '-' ? -1 : 1));
        }
        static Token ParseTokenData(string token)
        {
            Token result = new Token();
            //token = token.Trim();//removes sides
            token = Regex.Replace(token, @"\s+", "");//removes all white spaces
            if (!ValidateToken(token))
            {
                Console.WriteLine("Invalid token");
                return result;
            }
            char sign;
            string number = "";
            string variable = "";
            int s = 0;
            char lastChar;
            if (token[0] == '-')
            {
                sign = '-';
                s++;
            }
            else if (token[0] == '+')
            {
                sign = '+';
                s++;
            }
            else
            {
                sign = '+';
            }
            do
            {
                if (Char.IsDigit(token[s]))
                {
                    number += token[s];
                }
                else if (Char.IsLetter(token[s]))
                {
                    variable += token[s];
                }
                else
                {
                    Console.WriteLine($"Unknown token element: {token[s]}");
                }

                lastChar = token[s];
                s++;
            }
            while (s < token.Length);
            if (number.Length < 1) { number = "1"; }
            result.value = Double.Parse(number);
            result.variable = variable;
            result.sign = sign;
            return result;
        }
        public static bool IsMathOperand(char c)
        {//there is different unicode characters that also used as math symbols and some of the very similar
         //like U+002D Hyphen-Minus '-' (standart keyboard minus) and U+2212 Minus Sign'−' 
         //make shure you don't put in wierd symbols like that
            return c == '+' || c == '-' || c == '*' || c == '/'; //|| c == '=';
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
        struct Token
        {
            public char sign;
            public double value;
            public string variable;
        }
        public static (double[][] lhs, double[] rhs, double[] solution) GenerateSolvableSystem(int n, int maxValue = 10)
        {

            Random rand = new Random();
            double[][] A = new double[n][];
            for (int i = 0; i < n; i++)
            {
                A[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    int temp = rand.Next(1, maxValue);
                    temp *= rand.Next(2) == 0 ? -1 : 1;
                    A[i][j] = temp;
                }
            }
            int attempt = 0;
            //matrix must have a determinant
            while (Determinant(A) == 0)
            {
                attempt++;
                Console.WriteLine("\tRegenerating new matrix");
                if (attempt > 100)
                {
                    Console.WriteLine("Attempts to regenerate matrix have run out!");
                    break;
                }
                // Regenerate matrix A until it is non-singular (det != 0)
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        int temp = rand.Next(1, maxValue);
                        temp *= rand.Next(2) == 0 ? -1 : 1;
                        A[i][j] = temp;
                    }
                }
            }
            //Generate the right-hand side
            double[] solution = new double[n];
            for (int i = 0; i < n; i++)
            {
                int temp = rand.Next(1, maxValue);
                temp *= rand.Next(2) == 0 ? -1 : 1;
                solution[i] = temp;  // Random solution values, no double, because of mantisa overflow suffering
            }

            double[] rhs = new double[n];
            for (int i = 0; i < n; i++)
            {
                rhs[i] = 0;
                for (int j = 0; j < n; j++)
                {
                    rhs[i] += A[i][j] * solution[j];
                }
            }
            return (A, rhs, solution);
        }
        public static double[][] GenerateSolvableMatrix(int size, int maxValue = 10)
        {
            Console.Write("Generating system: ");
            var sole = SoLE.GenerateSolvableSystem(size);
            Console.WriteLine("done;");
            double[][] A1 = sole.lhs;
            double[] b = sole.rhs;
            double[][] matrix = new double[A1.Length][];
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i] = A1[i];
                Array.Resize(ref matrix[i], matrix[i].Length + 1);
                matrix[i][matrix[i].Length - 1] = b[i];
            }
            return matrix;
        }

        private static double[][] getSubMatrixDeterminant(double[][] matrix, int rowToRemove, int colToRemove)
        {
            int size = matrix.GetLength(0);
            double[][] subMatrix = new double[size - 1][];
            for (int i = 0; i < subMatrix.Length; i++)
            {
                subMatrix[i] = new double[size - 1];
            }
            int subRow = 0;
            for (int i = 0; i < size; i++)
            {
                if (i == rowToRemove) continue;
                int subCol = 0;
                for (int j = 0; j < size; j++)
                {
                    if (j == colToRemove) continue;
                    subMatrix[subRow][subCol] = matrix[i][j];
                    subCol++;
                }
                subRow++;
            }
            return subMatrix;
        }
        public static double Determinant(double[][] matrix)
        {
            var result = GaussElimination.EliminatePartial(matrix);
            if (result == null)
                return 0;
            double det = result[0][0];
            for (int i = 1; i < result.Length; i++)
            {
                det *= result[i][i];
            }
            return det;
        }
        public void Generate(int n, int maxValue)
        {
            var result = GenerateSolvableSystem(n, maxValue);
            lhs = result.lhs.ToList<double[]>();
            rhs = new List<double>(result.rhs);
            List<double> solutionList = result.solution.ToList();
            solutionIndexed = solutionList
                .Select((value, index) => new { Key = index, Value = value })
                .ToDictionary(item => item.Key, item => item.Value);
        }
    }
}
