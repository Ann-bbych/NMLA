using System;
using System.Globalization;
using System.IO;

namespace LUmethod
{
    public static class MatrixTools
    {
        // поріг точності для обчислень (похибка double)
        private const double eps = 1e-9; // 0.000000001

        public static void ReadFromFile(string inputFileName, out double[,] a, out double[] b, out int n)
        {
            if (!File.Exists(inputFileName))
            {
                throw new Exception("Файл input.txt не знайдено.");
            }

            string[] lines = File.ReadAllLines(inputFileName);

            if (lines.Length == 0)
            {
                throw new Exception("Файл input.txt порожній.");
            }

            string firstLine = lines[0].Trim();

            if (!int.TryParse(firstLine, out n) || n <= 0)
            {
                throw new Exception("Некоректно задано розмір матриці.");
            }

            if (lines.Length < n + 2)
            {
                throw new Exception("Недостатньо даних у файлі input.txt.");
            }

            a = new double[n, n];
            b = new double[n];

            for (int i = 0; i < n; i++)
            {
                string line = lines[i + 1].Trim(); // беру рядок з елементами

                if (line == "")
                {
                    throw new Exception("Один із рядків матриці A порожній.");
                }
                // ділю його на елементи, додаю в parts
                string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

                if (parts.Length != n) 
                {
                    throw new Exception("Кількість елементів у рядку матриці A не дорівнює n.");
                }

                for (int j = 0; j < n; j++)
                { // якщо вийшло, перетворюю елементи в double і записую в матрицю
                    if (!double.TryParse(parts[j], NumberStyles.Any, CultureInfo.InvariantCulture, out a[i, j]))
                    {
                        throw new Exception("Некоректне число у матриці A.");
                    }
                }
            }

            string bLine = lines[n + 1].Trim();

            if (bLine == "")
            {
                throw new Exception("Рядок вектора b порожній.");
            }

            string[] bParts = bLine.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

            if (bParts.Length != n)
            {
                throw new Exception("Кількість елементів у векторі b не дорівнює n.");
            }

            for (int i = 0; i < n; i++)
            {
                if (!double.TryParse(bParts[i], NumberStyles.Any, CultureInfo.InvariantCulture, out b[i]))
                {
                    throw new Exception("Некоректне число у векторі b.");
                }
            }
        }

        public static void WriteTo(StreamWriter writer, string text)
        { // виводжу на консоль і  у файл 
            Console.Write(text);
            writer.Write(text);
        }

        public static void WriteLineTo(StreamWriter writer, string text)
        { // виводжу на консоль і  у файл  + переходжу на новий рядок
            Console.WriteLine(text);
            writer.WriteLine(text);
        }

        public static string FormatNumber(double value)
        { // десяткові - округлення до сотих, цілі - без крапки
            if (Math.Abs(value) < eps)
            { //явний 0
                value = 0.0;
            }

            double roundedToInteger = Math.Round(value);

            if (Math.Abs(value - roundedToInteger) < eps)
            {
                return ((long)roundedToInteger).ToString(CultureInfo.InvariantCulture);
            }

            return value.ToString("0.00", CultureInfo.InvariantCulture);
        }

        public static string FormatNumberExact(double value)
        { // для точного виводу елементів у обчисленні визначника
            // але без проблеми з похибками double і з правильними цілими

            if (Math.Abs(value) < eps)
            { // явний 0
                value = 0.0;
            }

            double rounded = Math.Round(value);

            if (Math.Abs(value - rounded) < eps)
            {
                return ((long)rounded).ToString(System.Globalization.CultureInfo.InvariantCulture);
            }

            return value.ToString("0.###############", CultureInfo.InvariantCulture);
        }

        public static void PrintMatrix(double[,] matrix, int n, StreamWriter writer)
        {
            for (int i = 0; i < n; i++)
            {
                string line = "";

                for (int j = 0; j < n; j++)
                {
                    string current = FormatNumber(matrix[i, j]).PadLeft(10);

                    line += current;
                }

                WriteLineTo(writer, line);
            }

            WriteLineTo(writer, "");
        }

        public static void PrintVector(double[] vector, int n, StreamWriter writer)
        {
            for (int i = 0; i < n; i++)
            {
                WriteLineTo(writer, FormatNumber(vector[i]));
            }

            WriteLineTo(writer, "");
        }

        public static double[,] CopyMatrix(double[,] source, int n)
        {
            double[,] copy = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    copy[i, j] = source[i, j];
                }
            }

            return copy;
        }

        public static double[] CopyVector(double[] source, int n)
        {
            double[] copy = new double[n];

            for (int i = 0; i < n; i++)
            {
                copy[i] = source[i];
            }

            return copy;
        }

        public static double[,] MultiplyMatrices(double[,] first, double[,] second, int n)
        {
            double[,] result = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double sum = 0.0;

                    for (int k = 0; k < n; k++)
                    {
                        sum += first[i, k] * second[k, j];
                    }

                    result[i, j] = sum;
                }
            }

            return result;
        }

        public static double[] MultiplyMatrixByVector(double[,] matrix, double[] vector, int n)
        {
            double[] result = new double[n];

            for (int i = 0; i < n; i++)
            {
                double sum = 0.0;

                for (int j = 0; j < n; j++)
                {
                    sum += matrix[i, j] * vector[j];
                }

                result[i] = sum;
            }

            return result;
        }

        public static double[,] BuildLeadingMinor(double[,] a, int size)
        { // size змінний, починаємо з верхнього лівого фрагмента
            double[,] minor = new double[size, size];

            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    minor[i, j] = a[i, j];
                }
            }

            return minor;
        }

        public static double DeterminantByGauss(double[,] matrix, int n)
        {
            double[,] temp = CopyMatrix(matrix, n);
            double determinant = 1.0;
            int swapCount = 0;

            for (int k = 0; k < n; k++)
            {
                int pivotRow = k;

                for (int i = k; i < n; i++)
                {
                    if (Math.Abs(temp[i, k]) > Math.Abs(temp[pivotRow, k]))
                    {
                        pivotRow = i;
                    }
                }

                if (Math.Abs(temp[pivotRow, k]) < eps)
                {
                    return 0.0;
                }

                if (pivotRow != k)
                {
                    for (int j = 0; j < n; j++)
                    {
                        double t = temp[k, j];
                        temp[k, j] = temp[pivotRow, j];
                        temp[pivotRow, j] = t;
                    }

                    swapCount++;
                }

                for (int i = k + 1; i < n; i++)
                {
                    double factor = temp[i, k] / temp[k, k];

                    for (int j = k; j < n; j++)
                    {
                        temp[i, j] -= factor * temp[k, j];
                    }
                }
            }

            for (int i = 0; i < n; i++)
            {
                determinant *= temp[i, i];
            }

            if (swapCount % 2 != 0)
            {
                determinant = -determinant;
            }

            return determinant;
        }
    }
}
