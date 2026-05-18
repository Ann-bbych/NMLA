using System;
using System.Collections.Generic;
using System.IO;

class PMSolver
{
    public static PMResult Solve(PMInputData data)
    {
        PMResult result = new PMResult();

        double[] x = Normalise(data.Y0);
        double[] previousLambdas = CreateInitialLambdas(data.N, data.Lambda0);

        int iteration = 0;

        while (true)
        {
            double[] y = Multiply(data.A, x);

            double[] currentLambdas = FindLambdaComponents(x, y, data.Delta);
            double[] lambdaDifferences = FindLambdaDifferences(currentLambdas, previousLambdas);

            double lambdaAvg = FindLambdaAvg(currentLambdas);
            double maxLambdaDifference = FindMaxValid(lambdaDifferences);

            double[] nextX = Normalise(y);

            iteration++;

            if (iteration <= PMResult.maxPrintedIterations)
            {
                result.FirstIterations.Add(new IterationInfo
                {
                    Number = iteration,
                    Lambda = lambdaAvg,
                    X = (double[])nextX.Clone(),
                    LambdaDifferences = (double[])lambdaDifferences.Clone(),
                    MaxLambdaDifference = maxLambdaDifference
                });
            }

            x = nextX;
            previousLambdas = currentLambdas;

            if (maxLambdaDifference <= data.Epsilon)
            {
                result.Lambda = lambdaAvg;
                result.X = x;
                result.Iterations = iteration;
                result.LambdaDifferences = lambdaDifferences;
                result.MaxLambdaDifference = maxLambdaDifference;

                return result;
            }

            if (iteration > PMResult.maxIterations)
                throw new Exception("Перевищено максимальну кількість ітерацій.");
        }
    }

    private static double[] CreateInitialLambdas(int size, double lambda0)
    {
        double[] lambdas = new double[size];

        for (int i = 0; i < size; i++)
            lambdas[i] = lambda0;

        return lambdas;
    }

    private static double[] Multiply(double[,] A, double[] x)
    {
        int n = x.Length;
        double[] result = new double[n];

        for (int i = 0; i < n; i++)
            for (int j = 0; j < n; j++)
                result[i] += A[i, j] * x[j];

        return result;
    }

    private static double FindNorm(double[] vector)
    {
        double max = Math.Abs(vector[0]);

        for (int i = 1; i < vector.Length; i++)
            if (Math.Abs(vector[i]) > max)
                max = Math.Abs(vector[i]);

        return max;
    }

    private static double[] Normalise(double[] vector)
    {
        double norm = FindNorm(vector);

        if (norm == 0)
            throw new Exception("Неможливо нормувати нульовий вектор.");

        double[] result = new double[vector.Length];

        for (int i = 0; i < vector.Length; i++)
            result[i] = vector[i] / norm;

        return result;
    }

    private static double[] FindLambdaComponents(double[] previousX, double[] currentY, double delta)
    {
        double[] lambdas = new double[previousX.Length];
        int count = 0;

        for (int i = 0; i < previousX.Length; i++)
        {
            if (Math.Abs(previousX[i]) >= delta)
            {
                lambdas[i] = currentY[i] / previousX[i];
                count++;
            }
            else
            {
                lambdas[i] = double.NaN;
            }
        }

        if (count == 0)
            throw new Exception("Неможливо обчислити λ: множина S порожня.");

        return lambdas;
    }

    private static double[] FindLambdaDifferences(double[] currentLambdas, double[] previousLambdas)
    {
        double[] differences = new double[currentLambdas.Length];

        for (int i = 0; i < currentLambdas.Length; i++)
        {
            if (double.IsNaN(currentLambdas[i]) || double.IsNaN(previousLambdas[i]))
                differences[i] = double.NaN;
            else
                differences[i] = Math.Abs(currentLambdas[i] - previousLambdas[i]);
        }

        return differences;
    }

    private static double FindLambdaAvg(double[] lambdas)
    {
        double sum = 0;
        int count = 0;

        for (int i = 0; i < lambdas.Length; i++)
        {
            if (!double.IsNaN(lambdas[i]))
            {
                sum += lambdas[i];
                count++;
            }
        }

        if (count == 0)
            throw new Exception("Неможливо обчислити середнє λ: немає допустимих компонент.");

        return sum / count;
    }

    private static double FindMaxValid(double[] values)
    {
        bool found = false;
        double max = 0;

        for (int i = 0; i < values.Length; i++)
        {
            if (!double.IsNaN(values[i]))
            {
                if (!found || values[i] > max)
                    max = values[i];

                found = true;
            }
        }

        if (!found)
            throw new Exception("Неможливо перевірити критерій зупинки: немає допустимих Δλ.");

        return max;
    }
}

class PMResult
{
    public const int maxPrintedIterations = 10;
    public const int maxIterations = 10000;

    public double Lambda;
    public double[] X = [];
    public int Iterations;
    public double[] LambdaDifferences = [];
    public double MaxLambdaDifference;

    public List<IterationInfo> FirstIterations = [];

    public void Print(StreamWriter writer, int digits)
    {
        Formatter.WriteLineTo("Перші ітерації PM-алгоритму:", writer);
        Formatter.WriteLineTo("", writer);

        foreach (IterationInfo info in FirstIterations)
        {
            Formatter.WriteLineTo($"k = {info.Number}:", writer);
            Formatter.WriteLineTo($"λ = {Formatter.FormatResultNumber(info.Lambda, digits)}", writer);
            Formatter.WriteLineTo($"Δλ = {Formatter.FormatVector(info.LambdaDifferences, digits)}", writer);
            Formatter.WriteLineTo($"max Δλ = {Formatter.FormatResultNumber(info.MaxLambdaDifference, digits)}", writer);
            Formatter.WriteLineTo($"x = {Formatter.FormatVector(info.X, digits)}", writer);
            Formatter.WriteLineTo("", writer);
        }

        Formatter.WriteLineTo("Результат:", writer);
        Formatter.WriteLineTo($"λ = {Formatter.FormatResultNumber(Lambda, digits)}", writer);
        Formatter.WriteLineTo($"x = {Formatter.FormatVector(X, digits)}", writer);
        Formatter.WriteLineTo($"Кількість ітерацій: {Iterations}", writer);
        Formatter.WriteLineTo($"Δλ = {Formatter.FormatVector(LambdaDifferences, digits)}", writer);
        Formatter.WriteLineTo($"max Δλ = {Formatter.FormatResultNumber(MaxLambdaDifference, digits)}", writer);
        Formatter.WriteLineTo("", writer);
    }
}

class IterationInfo
{
    public int Number;
    public double Lambda;
    public double[] X = [];
    public double[] LambdaDifferences = [];
    public double MaxLambdaDifference;
}