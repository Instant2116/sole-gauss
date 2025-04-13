using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoLE_Gauss
{
    internal class GaussEliminationMultitask
    {

        double[][] sole;//system of linear equations
        double[][] soleOriginal;
        Dictionary<int, double> solution;
        bool singularityFlag;
        public int Threads { get; set; }
        static int marginOfTolerance = 8;

        public GaussEliminationMultitask(double[][] matrix, int threads)
        {
            soleOriginal = Matrix.Copy(matrix);
            singularityFlag = false;
            this.sole = Matrix.Copy(matrix); ;
            solution = new Dictionary<int, double>();
            Threads = threads;
        }
        public double[][] Eliminate()
        {
            return Eliminate(this.sole);
        }

        public double[][] Eliminate(double[][] matrix)//regular elimination
        {
            //Pivot Selection
            for (int i = 0; i < matrix.Length; i++)
            {
                double pivot;
                //- 1 because the last variable does not need elimination
                //pick pivot
                if (matrix[i][i] != 0)
                {//all good
                    pivot = matrix[i][i];
                }
                else
                {//pivot is 0, need rows swapping 
                    //swap
                    if (matrix[i][i] == 0)
                    {
                        //matrix is always solvable (c)Stetsenko
                        bool singularityFlag = true; 
                        for (int j = i; j < matrix.Length - 1; j++)
                        {
                            if (matrix[i][j] != 0)
                            {
                                //singularityFlag = false;
                                swapRows(matrix, i, j);
                                break;
                            }
                        }
                        if (singularityFlag)
                        {//actually need just to ignore variable and ajust answer to eqither system has free variable or it's singular 
                            this.singularityFlag = true;
                            Console.WriteLine("Panic - singularity");
                            return matrix;
                        }
                    }
                    pivot = matrix[i][i];
                }
                //technicly can lock on matrix[i] or matrix[m],
                //but they aquire access to pointer of a subarray and never intervene each other
                //Normalization 
                Parallel.For(0, matrix[i].Length, new ParallelOptions { MaxDegreeOfParallelism = this.Threads },
                    j =>//go through elements of the picked row
                {
                    matrix[i][j] /= pivot;
                    //matrix[i][j] = Math.Round(matrix[i][j], 5, MidpointRounding.ToEven); // fight floating-point errors 
                });
                //Gaussian Elimination
                Parallel.For(0, matrix.Length, new ParallelOptions { MaxDegreeOfParallelism = this.Threads },
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
            solution = Solve(this.sole);
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
        public bool CheckSolutionQuick()
        {
            return CheckSolutionQuick(this.solution);
        }
        public bool CheckSolution()
        {
            return CheckSolution(this.solution);
        }
        public bool CheckSolutionQuick(Dictionary<int, double> sol)
        {
            double rhs = sole[0][sole.Length];
            double sum = 0;
            for (int i = 0; i < sole[0].Length - 1; i++)
            {
                sum += sole[0][i] * sol[i];
            }
            return sum == rhs;
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
                sum = Math.Round(sum, marginOfTolerance, MidpointRounding.ToEven);
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
            for (int i = 0; i < sole.Length; i++)
            {
                Console.WriteLine("[" + String.Join(";", sole[i]) + "]");
            }
        }
        public void showSolution()
        {
            foreach (var i in solution)
            {
                Console.WriteLine($"var: {i.Key};\tval: {i.Value}");
            }
        }
    }
}
