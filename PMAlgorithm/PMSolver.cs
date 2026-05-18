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
            // ітерація:
            double[] y = Multiply(data.A, x); // y^(k) = A * x^(k-1)

            double[] currentLambdas = FindLambdaComponents(x, y, data.Delta);
            // будуємо S = { i : |x_i| >= δ } і обчислюємо λ_i = y_i / x_i для i ∈ S

            double[] lambdaDifferences = FindLambdaDifferences(currentLambdas, previousLambdas);
            // обчислюємо Δλ_i = |λ_i - λ_i^(k-1)| для i ∈ S

            double lambdaAvg = FindLambdaAvg(currentLambdas);
            // обчислюємо середнє λ^(k) = (1 / |S|) * Σ λ_i для i ∈ S

            double maxLambdaDifference = FindMaxValid(lambdaDifferences);
            // перевірка критерію зупинки: шукаємо найбільшу різниця λ (якщо вона ≤ ε, то інші точно ≤ ε)

            double[] nextX = Normalise(y); //власний вектор наступної ітерації
            // x^(k) = y^(k) / ||y^(k)||

            iteration++;

            // зберігаємо перші ітерації для виводу
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

            // перевірка критерію зупинки: порівнюємо максимальну різницю λ з ε
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
    { // визначає множину S і рахує покомпонентні λᵢ
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
    { // рахує покомпонентні різниці Δλᵢ
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
    { // рахує середнє значення λ за допустимими компонентами
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
    { // шукає найбільшу різницю Δλ (якщо вона ≤ ε, то інші точно ≤ ε)
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
    public const int maxPrintedIterations = 10; // кількість ітерацій які будуть виводитись
    public const int maxIterations = 10000; // щоб не було нескінченного циклу

    public double Lambda; // власне значення
    public double[] X = []; // власний вектор (нормований)
    public int Iterations; // кількість ітерацій
    public double[] LambdaDifferences = []; // масив покомпонентних різниць λ на останній ітерації
    public double MaxLambdaDifference; // найбільша різниця λ на останній ітерації (для перевірки правильності)

    public List<IterationInfo> FirstIterations = []; // масив інформації про перші ітерації 

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