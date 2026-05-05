using System;

namespace JacobiMethod;

internal class JacobiResult
{
    public double[] Solution;
    public double[] Residual; //відхил
    public int Iterations;
    public double DifferenceNorm;
    public double ResidualNorm;
    public bool IsConverged;

    public JacobiResult(int n)
    {
        Solution = new double[n];
        Residual = new double[n];
    }
}

internal class JacobiSolver
{
    public static bool CheckDiagonalDominance(AugmentedMatrix matrix, StreamWriter writer)
    {
        Formatter.WriteLineToBoth("Перевірка достатніх умов збіжності:", writer);
        Formatter.WriteLineToBoth("", writer);

        bool rowsCondition = CheckRowsDominance(matrix, writer);
        Formatter.WriteLineToBoth("", writer);
        bool columnsCondition = CheckColumnsDominance(matrix, writer);

        return rowsCondition && columnsCondition;
    }

    static bool CheckRowsDominance(AugmentedMatrix matrix, StreamWriter writer)
    {
        bool allRows = true;

        Formatter.WriteLineToBoth("1) по рядках:", writer);

        for (int i = 0; i < matrix.N; i++)
        {
            double diagonal = Math.Abs(matrix.A[i, i]);
            double sum = 0;

            Formatter.WriteToBoth($"рядок {i + 1}: |{Formatter.FormatMatrixNumber(matrix.A[i, i])}| > ", writer);

            bool first = true;

            for (int j = 0; j < matrix.N; j++)
            {
                if (j != i)
                {
                    sum += Math.Abs(matrix.A[i, j]);

                    if (!first)
                        Formatter.WriteToBoth(" + ", writer);

                    Formatter.WriteToBoth($"|{Formatter.FormatMatrixNumber(matrix.A[i, j])}|", writer);

                    first = false;
                }
            }

            if (diagonal > sum)
            {
                Formatter.WriteLineToBoth(", виконується", writer);
            }
            else
            {
                Formatter.WriteLineToBoth(", не виконується!", writer);
                allRows = false;
            }
        }

        return allRows;
    }

    static bool CheckColumnsDominance(AugmentedMatrix matrix, StreamWriter writer)
    {
        bool allColumns = true;

        Formatter.WriteLineToBoth("2) по стовпцях:", writer);

        for (int i = 0; i < matrix.N; i++)
        {
            double diagonal = Math.Abs(matrix.A[i, i]);
            double sum = 0;

            Formatter.WriteToBoth($"стовпець {i + 1}: |{Formatter.FormatMatrixNumber(matrix.A[i, i])}| > ", writer);

            bool first = true;

            for (int j = 0; j < matrix.N; j++)
            {
                if (j != i)
                {
                    sum += Math.Abs(matrix.A[j, i]);

                    if (!first)
                        Formatter.WriteToBoth(" + ", writer);

                    Formatter.WriteToBoth($"|{Formatter.FormatMatrixNumber(matrix.A[j, i])}|", writer);

                    first = false;
                }
            }

            if (diagonal > sum)
            {
                Formatter.WriteLineToBoth(", виконується", writer);
            }
            else
            {
                Formatter.WriteLineToBoth(", не виконується!", writer);
                allColumns = false;
            }
        }

        return allColumns;
    }

    public static JacobiResult Solve(AugmentedMatrix matrix, StreamWriter writer)
    {
        int n = matrix.N;

        double[] xOld = new double[n];
        double[] xNew = new double[n];

        Formatter.WriteLineToBoth("Початкове наближення:", writer);
        Formatter.PrintVector("x(0)", xOld, writer);
        Formatter.WriteLineToBoth("", writer);

        Formatter.WriteLineToBoth("Перші наближення:", writer);

        int iteration = 0;
        double differenceNorm = double.MaxValue;

        while (differenceNorm > Constants.eps && iteration<Constants.maxIterations)
        {
            iteration++;

            for (int i = 0; i < n; i++)
            {
                double sum = 0;

                for (int j = 0; j < n; j++)
                {
                    if (j != i)
                        sum += matrix.A[i, j] * xOld[j];
                }

                xNew[i] = (matrix.B[i] - sum) / matrix.A[i, i];
            }

            differenceNorm = CalculateDifferenceNorm(xOld, xNew);

            if (iteration <= Constants.iterationsToPrint)
            {
                Formatter.PrintVector($"x({iteration})", xNew, writer);
            }

            for (int i = 0; i < n; i++)
            {
                xOld[i] = xNew[i];
            }
        }

        Formatter.WriteLineToBoth("", writer);

        JacobiResult result = new JacobiResult(n);

        for (int i = 0; i < n; i++)
        {
            result.Solution[i] = xNew[i];
        }

        result.Iterations = iteration;
        result.DifferenceNorm = differenceNorm;
        result.Residual = CalculateResidual(matrix, xNew);
        result.ResidualNorm = CalculateVectorNorm(result.Residual);
        result.IsConverged = differenceNorm <= Constants.eps;

        return result;
    }

    static double CalculateDifferenceNorm(double[] xOld, double[] xNew)
    {
        double max = 0;

        for (int i = 0; i < xOld.Length; i++)
        {
            double difference = Math.Abs(xNew[i] - xOld[i]);

            if (difference > max)
                max = difference;
        }

        return max;
    }

    static double[] CalculateResidual(AugmentedMatrix matrix, double[] x)
    {
        double[] residual = new double[matrix.N];

        for (int i = 0; i < matrix.N; i++)
        {
            double leftSide = 0;

            for (int j = 0; j < matrix.N; j++)
            {
                leftSide += matrix.A[i, j] * x[j];
            }

            residual[i] = leftSide - matrix.B[i];
        }

        return residual;
    }

    static double CalculateVectorNorm(double[] vector)
    {
        double max = 0;

        for (int i = 0; i < vector.Length; i++)
        {
            double value = Math.Abs(vector[i]);

            if (value > max)
                max = value;
        }

        return max;
    }

    public static void PrintResult(JacobiResult result, StreamWriter writer)
    {
        if (!result.IsConverged)
        {
            Formatter.WriteLineToBoth("Досягнуто максимальної кількості ітерацій.", writer);
            Formatter.WriteLineToBoth("Метод не досяг заданої точності.", writer);
            Formatter.WriteLineToBoth("", writer);
        }

        Formatter.WriteLineToBoth("Наближений розв'язок СЛАР:", writer);
        Formatter.PrintVector("x", result.Solution, writer);
        Formatter.WriteLineToBoth("", writer);

        Formatter.WriteLineToBoth($"Кількість ітерацій: {result.Iterations}", writer);
        Formatter.WriteLineToBoth("", writer);
        Formatter.WriteLineToBoth($"Норма різниці між останніми наближеннями: ", writer);
        Formatter.WriteLineToBoth(
            $"||x(n+1) - x(n)|| = {Formatter.FormatPreciseNumber(result.DifferenceNorm)}",
            writer);
        Formatter.WriteLineToBoth("", writer);
        Formatter.WriteLineToBoth("Відхил:", writer);
        Formatter.PrintVector("Ax - b:", result.Residual, writer);
        Formatter.WriteLineToBoth(
            $"||Ax - b||: {Formatter.FormatPreciseNumber(result.ResidualNorm)}",
            writer);
    }
}