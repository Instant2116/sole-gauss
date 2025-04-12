using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using System.

namespace SoLE_Gauss
{
    internal class Matrix
    {
        double[][] data;
        public Matrix (double[][] data)
        {
            this.data = data;
        }
        public static void printMatrix(double[][] A)
        {
            for (int i = 0; i < A.Length; i++)
            {
                Console.WriteLine(String.Join(";", A[i].DefaultIfEmpty()));
            }
        }
        
        public static double[][] getMatrixRaw(uint x, uint y)//i j
        {
            Random rnd = new Random();
            double[][] matrix = new double[y][];
            for (int i = 0; i < y; i++)
            {
                matrix[i] = new double[x];
                for (int j = 0; j < x; j++)
                {
                    matrix[i][j] = rnd.NextDouble()*98+1;//1-99
                }
            }
            return matrix;
        }
        public static Matrix getMatrix(uint i, uint j)
        {
            return new Matrix(getMatrixRaw(i,j));
        }
        public static double[][] Copy(double[][] A)//return a copy
        {
            double[][] B = new double[A.Length][];
            for (int i = 0; i < A.Length; i++)
            {
                B[i] = new double [A[i].Length];
                A[i].CopyTo(B[i], 0);
            }
            return B;
        }
        public static Matrix Copy(Matrix A)//return a copy
        {
            return new Matrix(Copy(A.data));
        }
        public static double[][] getSubMatrix(double[][] A, uint indexI, uint indexJ, uint size)//return a square sub-matrix from main matrix, 
        {
            if (A == null || A.First() == null)
            {
                throw new ArgumentNullException("getSubMatrix: matrix or matrix.First() is null");
            }
            if(A.Length < indexI+size || A.First().Length < indexJ+size)
            {
                throw new ArgumentOutOfRangeException("getSubMatrix: index is out of range");
            }
            double[][] B = new double[size][];
            for (uint i = 0; i < size; i++)
            {
                B[i] = new double[size];
                for (uint j = 0; j < size; j++)
                {
                    B[i][j] = A[i+indexI][j+indexJ];
                }
            }
            return B;
        }
        public static Matrix getSubMatrix(Matrix A, uint indexI, uint indexJ, uint size)
        {
            return new Matrix(Matrix.getSubMatrix(A.data, indexI, indexJ, size));
        }
        public static double[][] removeRow(double[][] A, uint indexI)
        {
            if (A == null || A.First() == null)
            {
                throw new ArgumentNullException("removeRow: matrix or matrix.First() is null");
            }
            double[][] B = new double[A.Length-1][];
            for (int i = 0; i < A.Length - 1; i++)
            {
                if (i == indexI)
                    continue;
                B[i] = new double[A[i].Length]; 
                A[i].CopyTo(B, 0);
            }
            return B;
        }
        public static double[][] removeColumn(double[][] A, uint indexJ)
        {
            double[][] B = new double[A.Length][];
            for (int i = 0; i < A.Length; i++)
            {
                B[i] = new double[A[i].Length - 1];
                for (int j = 0; j < A[i].Length; j++)
                {
                    if (i == indexJ)
                        continue;
                    B[i][j] = A[i][j];
                }
            }
            return new double[A.Length][];
        }
    }
}
