using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;

namespace SoLE_Gauss
{
    internal class GaussEliminationParalell
    {
        /// <summary>
        /// in Elemination process the elemination itself is the most time consuming process (~97%)
        /// Paralell implementation is better than asyncrhonus, because of thread usage
        /// Paralell implementation is better than multitasking is better because tasks queued with low priority flag as background tasks
        /// Parallel.For uses greaddy tactic to let other threads steal work from busy threads, which optimizes the load, while segmentation of work for threads perform worse
        /// </summary>
        double[][] matrix;//system of linear equations
        double[][] soleOriginal;
        Dictionary<int, double> solution;
        //bool singularityFlag;
        public int Threads { get; private set; }
        public int Delimiter { get; set; }
        public int SegmentSize { get; private set; }
        static int marginOfTolerance = 8;

        public GaussEliminationParalell(double[][] matrix, int threads)
        {
            soleOriginal = Matrix.Copy(matrix);
            this.matrix = Matrix.Copy(matrix); ;
            solution = new Dictionary<int, double>();
            Threads = threads;
            //Segmentation
            Delimiter = Math.Min(threads, soleOriginal.Length);
            Delimiter = getDivider(Delimiter, soleOriginal.Length); // even split, like matrix size 10, and 9 proc, so ...
            this.SegmentSize = (int)(soleOriginal.Length / Delimiter);
        }
        public double[][] Eliminate()
        {
            return Eliminate(this.matrix, this.Threads);
        }
        public double[][] EliminateSegmented(uint segmentationCoeficient = 1) // segmentationCoeficient: 1 => segments = threads number, 2 => segments x 2 = threads etc.
        {//perform worse, than static method, because Paralell.For uses greaddy strategy, which can not be performed when matrix is already splitted in segments
            //local segmentation
            int localSegmentSize;
            if (segmentationCoeficient != 1)
                localSegmentSize = (int) (soleOriginal.Length / getDivider(this.Threads * (int)segmentationCoeficient, soleOriginal.Length));
            else
                localSegmentSize = SegmentSize;
            
            for (int i = 0; i < matrix.Length; i++)
            {
                //Pivot Selection
                double pivot = matrix[i][i];
                int index = i;
                //Pick pivot partial pivoting
                for (int I = i; I < matrix.Length; I++)
                {
                    if (Math.Abs(matrix[I][i]) > Math.Abs(pivot))
                    {
                        pivot = matrix[I][i];
                        index = I;
                    }
                }
                swapRows(matrix, i, index);

                //technicly can lock on matrix[i] or matrix[m],
                //but they aquire access to pointer of a subarray and never intervene each other
                //Normalization 
                for (int j = 0; j < matrix[i].Length; j++)//go through elements of the picked row
                {
                    matrix[i][j] /= pivot;//floating point mantise error

                }
                //Gaussian Elimination
                // Parallel.For use greaddy strategy, where free threads steal work from busy threads
                //because of this splitting matrix into segments for each thread to process will perform worse
                Parallel.For(0, Delimiter, new ParallelOptions { MaxDegreeOfParallelism = Threads },
                    m =>
                    {
                        for (int c = 0; c < SegmentSize; c++)
                        {
                            int s = m * SegmentSize + c;//currently processed index of a row
                            if (s != i) //do not self-eliminate
                            {
                                double coeficient = matrix[m * SegmentSize + c][i]; // pivot;
                                for (int n = 0; n < matrix[m * SegmentSize + c].Length; n++) //go through elements of other rows
                                    matrix[m * SegmentSize + c][n] -= matrix[i][n] * coeficient;
                            }
                        }
                    });

            }
            //correct RHS from  floating-point errors
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i][matrix[i].Length - 1] = Math.Round(matrix[i][matrix[i].Length - 1], marginOfTolerance, MidpointRounding.ToEven);
            }
            return matrix;
        }

        public static double[][] Eliminate(double[][] matrix, int Threads)//paralell elimination
        {
           
            for (int i = 0; i < matrix.Length; i++)
            {
                //Pivot Selection
                double pivot = matrix[i][i];
                int index = i;
                //Pick pivot partial pivoting
                for (int I = i; I < matrix.Length; I++)
                {
                    if (Math.Abs(matrix[I][i]) > Math.Abs(pivot))
                    { //pick pivot
                        pivot = matrix[I][i];
                        index = I;
                    }
                }
                swapRows(matrix, i, index);
                //technicly can lock on matrix[i] or matrix[m],
                //but they aquire access to pointer of a subarray and never intervene each other
                //Normalization 
                for (int j = 0; j < matrix[i].Length; j++)//go through elements of the picked row
                {
                    matrix[i][j] /= pivot;//floating point mantise error

                }
                //Gaussian Elimination
                Parallel.For(0, matrix.Length, new ParallelOptions { MaxDegreeOfParallelism = Threads },
                    m =>
                    {
                        if (m != i) //do not self-eliminate
                        {
                            double coeficient = matrix[m][i]; // pivot; // do not need division, because pivot is always 1;
                            for (int n = 0; n < matrix[m].Length; n++) //go through elements of other rows
                                matrix[m][n] -= matrix[i][n] * coeficient;
                        }
                    });

            }
            //correct RHS from  floating-point errors
            for (int i = 0; i < matrix.Length; i++)
            {
                matrix[i][matrix[i].Length - 1] = Math.Round(matrix[i][matrix[i].Length - 1], marginOfTolerance, MidpointRounding.ToEven);
            }
            return matrix;
        }

        public Dictionary<int, double> Solve()
        {
            solution = Solve(this.matrix);
            return solution;
        }
        public Dictionary<int, double> Solve(double[][] matrix)
        {//only after all eliminations
            Dictionary<int, double> res = new Dictionary<int, double>();
            int a = matrix.Length;
            int b = matrix[0].Length;
            double lastVariableCoeficient = matrix[matrix.Length - 1][matrix[0].Length - 2];//-2 because -1 is last element, and we want last variable coeficient which is pre last element
            double lastVariableRHS = matrix[matrix.Length - 1][matrix[0].Length - 1];
            double lastVariableSolution = lastVariableRHS / lastVariableCoeficient;
            lastVariableSolution = Math.Round(lastVariableSolution, marginOfTolerance, MidpointRounding.ToEven);// fight floating-point errors 
            res.Add(matrix.Length - 1, lastVariableSolution);
            for (int i = matrix.Length - 2; i > -1; i--) //-2 to skip the last row, as we already solved it
            {//shifting through rows solving LE
                double nextRHS = matrix[i][matrix[0].Length - 1];
                for (int j = i + 1; j < matrix[i].Length - 1; j++)//fit known variables
                {//will cause an error if res[j] doesn't have a solution for variable,
                 //which can happen when we have a free variable, look up pivot picking
                    nextRHS -= res[j] * matrix[i][j]; //carry whatever is const on left to right
                }
                double nextVarCoeficient = matrix[i][i];
                double nextVarSolution = nextRHS / nextVarCoeficient;
                nextVarSolution = Math.Round(nextVarSolution, marginOfTolerance, MidpointRounding.ToEven); // fight floating-point errors 
                res.Add(i, nextVarSolution);

            }
            return res;
        }

        public static void swapRows(double[][] matrix, int i1, int i2)
        {//no need to return as it works with references
            double[] t = matrix[i1];
            matrix[i1] = matrix[i2];
            matrix[i2] = t;
            //return matrix;
        }

        public bool CheckSolution()
        {
            return CheckSolution(this.solution);
        }

        public bool CheckSolution(Dictionary<int, double> sol)
        {
            foreach (double[] line in soleOriginal)
            {
                double rhs = line[soleOriginal.Length];
                double sum = 0;
                for (int i = 0; i < soleOriginal[0].Length - 1; i++)
                {
                    sol[i] = Math.Round(sol[i], marginOfTolerance, MidpointRounding.ToEven);
                    sum += line[i] * sol[i];
                }
                sum = Math.Round(sum, marginOfTolerance - 3, MidpointRounding.ToEven);
                if (sum != rhs)
                {
                    Console.WriteLine($"{sum} != {rhs};");
                    return false;
                }
            }
            return true;
        }
        public void showSoLE()
        {
            for (int i = 0; i < matrix.Length; i++)
            {
                Console.WriteLine("[" + String.Join(";", matrix[i]) + "]");
            }
        }
        public void showSolution()
        {
            foreach (var i in solution)
            {
                Console.WriteLine($"var: {i.Key};\tval: {i.Value}");
            }
        }
        public static int getDivider(int threads, int size)
        {//for equal load
            if (threads <= size)
            {
                int delimiter = threads;
                while (size % delimiter != 0)
                {
                    delimiter++;
                }
                return delimiter;
            }
            return size;
        }

        public List<double[][]> Segmentize(double[][] matrix, int segmentSize)
        {
            List<double[][]> result = new List<double[][]>();
            for (int i = 0; i <= matrix.Length; i += segmentSize)
            {
                double[][] segment = new double[segmentSize][];
                for (int j = 0; j < segmentSize; j++)
                {
                    segment[i][j] = matrix[i][j];
                }
            }


            return result;
        }

    }
}
