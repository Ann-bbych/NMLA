using System;

namespace GaussMethod
{
    public static class GaussSolver
    {
        public const double EPS = 1e-9;

        public static double[,] Copy(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int columns = matrix.GetLength(1);

            double[,] copyMatrix = new double[rows, columns];

            for (int row = 0; row < rows; row++)
            {
                for (int column = 0; column < columns; column++)
                {
                    copyMatrix[row, column] = matrix[row, column];
                }
            }

            return copyMatrix;
        }

        public static void ForwardElimination(double[,] matrix, int size, ref int swapCount)
        {
            for (int currentColumn = 0; currentColumn < size; currentColumn++)
            {
                int maxIndex = FindMaxModElemIndex(matrix, size, currentColumn);

                if (Math.Abs(matrix[maxIndex, currentColumn]) < EPS)
                {
                    throw new Exception(
                        "Матриця вироджена або близька до виродженої. Розв’язок методом Гауса неможливий.");
                }

                if (maxIndex != currentColumn)
                {
                    SwapRows(matrix, size, currentColumn, maxIndex);
                    swapCount++;
                }

                for (int currentRow = currentColumn + 1; currentRow < size; currentRow++)
                {
                    double factor = matrix[currentRow, currentColumn] / matrix[currentColumn, currentColumn];

                    for (int column = currentColumn; column < size + 1; column++)
                    {
                        matrix[currentRow, column] -= factor * matrix[currentColumn, column];
                    }

                    matrix[currentRow, currentColumn] = 0;
                }
            }
        }

        public static int FindMaxModElemIndex(double[,] matrix, int size, int currentColumn)
        {
            int maxIndex = currentColumn;
            double maxValue = Math.Abs(matrix[currentColumn, currentColumn]);

            for (int row = currentColumn + 1; row < size; row++)
            {
                double currentValue = Math.Abs(matrix[row, currentColumn]);

                if (currentValue > maxValue)
                {
                    maxValue = currentValue;
                    maxIndex = row;
                }
            }

            return maxIndex;
        }

        public static void SwapRows(double[,] matrix, int size, int firstRow, int secondRow)
        {
            for (int column = 0; column < size + 1; column++)
            {
                double temp = matrix[firstRow, column];
                matrix[firstRow, column] = matrix[secondRow, column];
                matrix[secondRow, column] = temp;
            }
        }

        public static double[] BackSubstitution(double[,] matrix, int size)
        {
            double[] solution = new double[size];

            for (int row = size - 1; row >= 0; row--)
            {
                double sum = 0;

                for (int column = row + 1; column < size; column++)
                {
                    sum += matrix[row, column] * solution[column];
                }

                if (Math.Abs(matrix[row, row]) < EPS)
                {
                    throw new Exception(
                        "Матриця вироджена або близька до виродженої. Розв’язок методом Гауса неможливий.");
                }

                solution[row] = (matrix[row, size] - sum) / matrix[row, row];
            }

            return solution;
        }

        public static double[] CheckSolution(double[,] originalMatrix, double[] solution, int size)
        {
            double[] result = new double[size];

            for (int row = 0; row < size; row++)
            {
                result[row] = 0;

                for (int column = 0; column < size; column++)
                {
                    result[row] += originalMatrix[row, column] * solution[column];
                }
            }

            return result;
        }

        public static double CalculateDeterminant(double[,] matrix, int size, int swapCount)
        {
            double determinant = 1;

            for (int index = 0; index < size; index++)
            {
                determinant *= matrix[index, index];
            }

            if (swapCount % 2 != 0)
            {
                determinant = -determinant;
            }

            return determinant;
        }
    }
}