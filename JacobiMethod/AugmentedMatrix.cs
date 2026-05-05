using System;
using System.Globalization;

namespace JacobiMethod;

internal class AugmentedMatrix
{
    public int N;
    public double[,] A;
    public double[] B;

    public AugmentedMatrix(int n)
    {
        N = n;
        A = new double[n, n];
        B = new double[n];
    }

    public static AugmentedMatrix ReadFromFile(string fileName)
    {
        if (!File.Exists(fileName))
            throw new Exception("Файл input.txt не знайдено.");

        string[] lines = File.ReadAllLines(fileName);

        if (lines.Length == 0)
            throw new Exception("Файл input.txt порожній.");

        if (!int.TryParse(lines[0].Trim(), out int n))
            throw new Exception("У першому рядку файлу має бути розмір матриці - ціле число n.");

        if (n <= 0)
            throw new Exception("Розмір матриці n має бути додатним.");

        if (lines.Length < n + 1)
            throw new Exception("У файлі недостатньо рядків для заданої розширеної матриці.");

        if (lines.Length > n + 1)
            throw new Exception("У файлі є зайві рядки після розширеної матриці.");

        AugmentedMatrix matrix = new AugmentedMatrix(n);

        for (int i = 0; i < n; i++)
        {
            string[] parts = lines[i + 1].Split(
                new char[] { ' ', '\t' },
                StringSplitOptions.RemoveEmptyEntries);

            if (parts.Length != n + 1)
                throw new Exception($"У рядку {i + 2} має бути рівно {n + 1} чисел.");

            for (int j = 0; j < n; j++)
            {
                matrix.A[i, j] = ParseDouble(parts[j], i + 2);
            }

            matrix.B[i] = ParseDouble(parts[n], i + 2);
        }

        for (int i = 0; i < n; i++)
        {
            if (Math.Abs(matrix.A[i, i]) < Constants.eps)
                throw new Exception($"Діагональний елемент a[{i + 1},{i + 1}] дорівнює нулю або дуже близький до нуля.");
        }

        return matrix;
    }

    static double ParseDouble(string text, int lineNumber)
    {
        text = text.Replace(',', '.');

        if (!double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out double value))
            throw new Exception($"У рядку {lineNumber} є значення, яке не є числом.");

        return value;
    }

    public void Print(StreamWriter writer)
    {
        Formatter.WriteLineToBoth("Початкова розширена матриця СЛАР:", writer);
        Formatter.WriteLineToBoth("", writer);

        for (int i = 0; i < N; i++)
        {
            Formatter.WriteToBoth("[ ", writer);

            for (int j = 0; j < N; j++)
            {
                Formatter.WriteToBoth(Formatter.FormatMatrixNumber(A[i, j]).PadLeft(8), writer);
            }

            Formatter.WriteToBoth(" | ", writer);
            Formatter.WriteToBoth(Formatter.FormatMatrixNumber(B[i]).PadLeft(8), writer);
            Formatter.WriteLineToBoth(" ]", writer);
        }

        Formatter.WriteLineToBoth("", writer);
    }

    public void CheckDegenerate()
    {
        double determinant = CalculateDeterminant();

        if (Math.Abs(determinant) < Constants.eps)
            throw new Exception("Матриця є виродженою або близькою до виродженої. Роботу програми зупинено.");
    }

    double CalculateDeterminant()
    {
        double[,] copy = new double[N, N];

        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                copy[i, j] = A[i, j];
            }
        }

        double determinant = 1;

        for (int k = 0; k < N; k++)
        {
            int maxRow = k;

            for (int i = k + 1; i < N; i++)
            {
                if (Math.Abs(copy[i, k]) > Math.Abs(copy[maxRow, k]))
                    maxRow = i;
            }

            if (Math.Abs(copy[maxRow, k]) < 1e-12)
                return 0;

            if (maxRow != k)
            {
                for (int j = 0; j < N; j++)
                {
                    double temp = copy[k, j];
                    copy[k, j] = copy[maxRow, j];
                    copy[maxRow, j] = temp;
                }

                determinant *= -1;
            }

            determinant *= copy[k, k];

            for (int i = k + 1; i < N; i++)
            {
                double factor = copy[i, k] / copy[k, k];

                for (int j = k; j < N; j++)
                {
                    copy[i, j] -= factor * copy[k, j];
                }
            }
        }

        return determinant;
    }
}