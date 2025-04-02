using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoLE_Gauss
{
    internal class GaussElimination
    {
        double[][] matrix;
        public GaussElimination(double[][] matrix)
        {
            this.matrix = matrix;
        }
        public double[][] Eliminate()//regular elimination
        {
            //Pivot Selection
            for (int i = 0; i < matrix.Length - 1; i++) //go through rows; 
            {//- 1 because the last variable does not need elimination
                double pivot;
                //pick pivot
                if (matrix[i][i] != 0)
                {//all good
                    pivot = matrix[i][i];
                }
                else
                {//pivot is 0, need rows swapping 
                    //swap
                    pivot = matrix[i][i];
                }

                //Normalization 
                for (int j = 0; j < matrix[i].Length; j++)//go through elements of the picked row
                {
                    matrix[i][j] /= pivot;
                }
                //Gaussian Elimination
                for (int m = 0; m < matrix.Length; m++)//go through rest of rows
                {
                    if (m == i) //do not self-eliminate
                        continue;
                    double coeficient = matrix[m][i]; // pivot; // do not need division, because pivot is always 1;
                    for (int n = 0; n < matrix[m].Length; n++) //go through elements of other rows
                    {
                        matrix[m][n] -= matrix[i][n] * coeficient;
                    }
                }
            }
            return matrix;
        }
        public Dictionary<int, double> Solve()
        {//only after all eliminations
            Dictionary<int, double> res = new Dictionary<int, double>();
            int a = matrix.Length;
            int b = matrix[0].Length;
            double lastVariableCoeficient = matrix[matrix.Length - 1][matrix[0].Length-1];
            double lastVariableRHS = matrix[matrix.Length-1][matrix[0].Length-1];
            double lastVariableSolution = lastVariableRHS / lastVariableCoeficient;
            res.Add(matrix.Length - 1, lastVariableSolution);
            for (int i = matrix.Length - 2; i > -1; i--) //-2 to skip the last row, as we already solved it
            {//shifting through rows solving LE
                double nextRHS = matrix[i][matrix[0].Length-1];
                for (int j = i+1;j < matrix[i].Length - 1; j++)//fit known variables
                {//will cause an error if res[j] doesn't have a solution for variable
                    nextRHS -= res[j] * matrix[i][j]; //carry whatever is const on left to right
                }
                double nextVarCoeficient = matrix[i][i];
                double nextVarSolution = nextRHS / nextVarCoeficient;
                res.Add(i, nextVarSolution);
            }
            return res;
        }
    }
}

