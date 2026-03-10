namespace GaussMethod
{
    public static class GaussSolver
    {
        public const double EPS = 1e-9; // 0.000000001
        //для перевірки на близьку до виродженої матрицю
        //для форматованого виводу близького до цілого числа
        //для обчислення похибки при перевірці розв'язку
        public static double[,] Copy(double[,] matrix)
        {
            // GetLength(dimension) - розмір виміру
            // dimension = 0 - кількість рядків, 
            // dimension = 1 - кількість стовпців
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
                        "Матриця вироджена або близька до виродженої.\nТаку НЕ розв'язую.\n\nВизначник:\n0");
                }

                if (maxIndex != currentColumn) // maxIndex != індексу поточного рядка(стовпця)
                {
                    SwapRows(matrix, size, currentColumn, maxIndex);
                    swapCount++;
                }
                // прямий хід
                for (int currentRow = currentColumn + 1; currentRow < size; currentRow++)
                { 
                    double factor = (-1) * matrix[currentRow, currentColumn] / matrix[currentColumn, currentColumn];
                    // те на що множимо верхній рядок, щоб при додаванні став 0 на першому місці у поточному
                    for (int column = currentColumn; column < size + 1; column++)
                    {
                        matrix[currentRow, column] += factor * matrix[currentColumn, column];
                    }
                    matrix[currentRow, currentColumn] = 0; //явний нуль щоб не було похибки
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
                        "Матриця вироджена або близька до виродженої.\nТаку НЕ розв'язую.\n\nВизначник:\n0");
                }

                solution[row] = (matrix[row, size] - sum) / matrix[row, row];
            }

            return solution;
        }

        public static double[] CheckSolution(double[,] originalMatrix, double[] solution, int size)
        { // повертаю обчислену ліву частину щоб потім прирівняти з B
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
        { // працюю з східчастою матрицею після прямого ходу 
            double determinant = 1;

            for (int index = 0; index < size; index++)
            { //домножую діагональні елементи
                determinant *= matrix[index, index];
            }

            if (swapCount % 2 != 0)
            { // якщо p - непарне, визначник змінює знак
                determinant = -determinant;
            }

            return determinant;
        }
    }
}