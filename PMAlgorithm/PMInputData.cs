using System;
using System.Globalization;
using System.IO;

class PMInputData
{
    public int N;
    public double[,] A = new double[0, 0];
    public double[] Y0 = [];
    public double Lambda0 = 0;
    public double Delta;
    public double Epsilon;
    public string EpsilonText = "";
    public string DeltaText = "";

    public static PMInputData ReadFrom(string inputFileName)
    {
        if (!File.Exists(inputFileName))
            throw new Exception("Файл input.txt не знайдено.");

        string[] lines = File.ReadAllLines(inputFileName);

        int n = ReadInt(lines, 0, "Некоректно задано розмірність матриці.");

        if (n <= 0)
            throw new Exception("Розмірність матриці має бути додатною.");

        if (lines.Length != n + 4)
            throw new Exception("Некоректний формат файлу. Має бути: n, n рядків матриці, вектор, delta, epsilon.");

        PMInputData data = new PMInputData();
        data.N = n;
        data.A = new double[n, n];

        for (int i = 0; i < n; i++)
        {
            double[] row = ReadVector(lines[i + 1], n, $"Некоректно задано {i + 1}-й рядок матриці.");

            for (int j = 0; j < n; j++)
                data.A[i, j] = row[j];
        }

        data.Y0 = ReadVector(lines[n + 1], n, "Некоректно задано початковий вектор.");

        if (IsZeroVector(data.Y0))
            throw new Exception("Початковий вектор не може бути нульовим.");

        data.DeltaText = lines[n + 2].Trim();
        data.Delta = ReadDouble(data.DeltaText, "Некоректно задано delta.");
        data.EpsilonText = lines[n + 3].Trim();
        data.Epsilon = ReadDouble(data.EpsilonText, "Некоректно задано epsilon.");

        if (data.Delta <= 0)
            throw new Exception("δ має бути додатним числом.");

        if (data.Epsilon <= 0)
            throw new Exception("ε має бути додатним числом.");

        return data;
    }

    public void Print(StreamWriter writer)
    {
        int digits = Formatter.CountDigitsAfterPoint(EpsilonText);

        Formatter.WriteLineTo("", writer);
        Formatter.WriteLineTo($"Матриця A ({N}x{N}):", writer);
        Formatter.PrintMatrix(A, writer);

        Formatter.WriteLineTo("", writer);
        Formatter.WriteLineTo($"y0 = {Formatter.FormatInputVector(Y0)}", writer);
        Formatter.WriteLineTo($"λ0 = {Formatter.FormatInputNumber(Lambda0)}", writer);
        Formatter.WriteLineTo($"δ = {DeltaText}", writer);
        Formatter.WriteLineTo($"ε = {EpsilonText}", writer);
        Formatter.WriteLineTo("", writer);
    }

    private static int ReadInt(string[] lines, int index, string errorMessage)
    {
        if (lines.Length <= index || !int.TryParse(lines[index].Trim(), out int value))
            throw new Exception(errorMessage);

        return value;
    }

    private static double[] ReadVector(string line, int size, string errorMessage)
    {
        string[] parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != size)
            throw new Exception(errorMessage);

        double[] vector = new double[size];

        for (int i = 0; i < size; i++)
            vector[i] = ReadDouble(parts[i], errorMessage);

        return vector;
    }

    private static double ReadDouble(string value, string errorMessage)
    {
        value = value.Trim().Replace(',', '.');

        if (!double.TryParse(value, NumberStyles.Float, CultureInfo.InvariantCulture, out double result))
            throw new Exception(errorMessage);

        return result;
    }

    private static bool IsZeroVector(double[] vector)
    {
        const double eps = 1e-12;

        for (int i = 0; i < vector.Length; i++)
            if (Math.Abs(vector[i]) > eps)
                return false;

        return true;
    }
}