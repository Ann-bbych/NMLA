using System;
using System.IO;

class Verifier
{
    public static VerificationResult Verify(PMInputData data, PMResult result)
    { // чи виконується Ax ≈ λx
        VerificationResult verification = new VerificationResult();

        verification.Ax = Multiply(data.A, result.X); // обчислюємо Ax
        verification.LambdaX = MultiplyByNumber(result.X, result.Lambda); // обчислюємо λx
        verification.ResidualNorm = FindResidualNorm(verification.Ax, verification.LambdaX); 
        // обчислюємо ||Ax - λx|| - максимальну різницю між компонентами 

        return verification;
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

    private static double[] MultiplyByNumber(double[] x, double number)
    {
        double[] result = new double[x.Length];

        for (int i = 0; i < x.Length; i++)
            result[i] = number * x[i];

        return result;
    }

    private static double FindResidualNorm(double[] first, double[] second)
    {
        double max = 0;

        for (int i = 0; i < first.Length; i++)
        {
            double difference = Math.Abs(first[i] - second[i]);

            if (difference > max)
                max = difference;
        }

        return max;
    }
}

class VerificationResult
{
    public double[] Ax = [];
    public double[] LambdaX = [];
    public double ResidualNorm;

    public void Print(StreamWriter writer, int digits)
    {
        Formatter.WriteLineTo("Перевірка правильності:", writer);
        Formatter.WriteLineTo($"Ax = {Formatter.FormatCheckVector(Ax, digits)}", writer);
        Formatter.WriteLineTo($"λx = {Formatter.FormatCheckVector(LambdaX, digits)}", writer);
        Formatter.WriteLineTo($"||Ax - λx|| = {Formatter.FormatCheckNumber(ResidualNorm, digits)}", writer);
    }
}