﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoLE_Gauss
{
    internal class GaussElimination
    {
        double[][] sole;//system of linear equations
        Dictionary<int, double> solution { get; set; }
        public bool singularityFlag { get; private set; }
        public GaussElimination(double[][] sole)
        {
            this.sole = sole;
            this.solution = new Dictionary<int, double>();
            singularityFlag = false;
        }
        public double[][] Eliminate()
        {
            var r = Eliminate(this.sole);
            this.singularityFlag = r.Item2;
            return r.Item1;
        }
        public static (double[][],bool) Eliminate(double[][] matrix)//regular elimination
        {
            bool singularityFlag = false;
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
                    if (matrix[i][i] == 0)
                    {
                        singularityFlag = true;
                        for (int j = i; j < matrix.Length - 1; j++)
                        {
                            if (matrix[i][j] != 0)
                            {
                                singularityFlag = false;
                                swapRows(matrix, i, j);
                                break;
                            }
                        }
                        if (singularityFlag)
                        {//actually need just to ignore variable and ajust answer to eqither system has free variable or it's singular 
                            Console.WriteLine("Panic");
                            return (matrix, singularityFlag);
                        }
                    }
                    pivot = matrix[i][i];
                }

                //Normalization 
                for (int j = 0; j < matrix[i].Length; j++)//go through elements of the picked row
                {
                    matrix[i][j] /= pivot;//floating point mantise error

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
            //correct RHS from  floating-point errors
            for(int i = 0; i < matrix.Length; i++)
            {
                matrix[i][matrix[i].Length-1] = Math.Round(matrix[i][matrix[i].Length-1], 5, MidpointRounding.ToEven);
                //matrix[i][j] = Math.Round(matrix[i][j], 5, MidpointRounding.ToEven);// fight 
            }
            return (matrix, singularityFlag);
        }
        public Dictionary<int,double> Solve()
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
            lastVariableSolution = Math.Round(lastVariableSolution, 5, MidpointRounding.ToEven);// fight floating-point errors 
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
                nextVarSolution = Math.Round(nextVarSolution, 5, MidpointRounding.ToEven); // fight floating-point errors 
                res.Add(i, nextVarSolution);

            }
            return res;
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
            for (int i = 0; i < sole[0].Length-1; i++)
            {
                sum += sole[0][i] * sol[i];
            }
            return sum == rhs;
        }
        public bool CheckSolution(Dictionary<int, double> sol)
        {
            foreach (double[] line in sole)
            {
                double rhs = line[sole.Length];
                double sum = 0;
                for (int i = 0; i < sole[0].Length - 1; i++)
                {
                    sum += line[i] * sol[i];
                }
                if (sum != rhs)
                    return false;
            }
            return true;
        }
        public static void swapRows(double[][] matrix, int i1, int i2)
        {//no need to return as it works with references
            double[] t = matrix[i1];
            matrix[i1] = matrix[i2];
            matrix[i2] = t;
            //return matrix;
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
