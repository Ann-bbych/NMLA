using System;

namespace ThomasAlgorithm
{
    public static class BoundaryValueProblem
    {
        // Крайова задача:
        // y'' - y = 2 - x^2, 0 < x < 1
        // y(0) = 0
        // y(1) = 1
        //
        // Точний розв'язок:
        // y(x) = x^2

        public static double ExactSolution(double x)
        {
            return x * x;
        }

        public static double RightSide(double x)
        {
            return 2.0 - x * x;
        }

        public static double[] BuildGrid(int nodesCount)
        {
            if (nodesCount < 3)
            {
                throw new ArgumentException("Помилка: для крайової задачі кількість вузлів має бути не меншою за 3.");
            }

            double[] grid = new double[nodesCount];
            double h = 1.0 / (nodesCount - 1);

            for (int i = 0; i < nodesCount; i++)
            {
                grid[i] = i * h;
            }

            return grid;
        }

        public static TridiagonalSystem BuildSystem(int nodesCount, double eps)
        {
            if (nodesCount < 3)
            {
                throw new ArgumentException("Помилка: для побудови сіткової СЛАР кількість вузлів має бути не меншою за 3.");
            }

            double[] grid = BuildGrid(nodesCount);

            int internalCount = nodesCount - 2;
            double h = 1.0 / (nodesCount - 1);
            double hSquared = h * h;

            double[] a = new double[internalCount];
            double[] c = new double[internalCount];
            double[] b = new double[internalCount];
            double[] f = new double[internalCount];

            double leftBoundaryValue = ExactSolution(0.0); // y(0) = 0
            double rightBoundaryValue = ExactSolution(1.0); // y(1) = 1

            for (int i = 0; i < internalCount; i++)
            {
                double x = grid[i + 1];

                a[i] = 1.0;
                c[i] = -2.0 - hSquared;
                b[i] = 1.0;
                f[i] = hSquared * RightSide(x);
            }

            a[0] = 0.0;
            f[0] -= leftBoundaryValue;

            b[internalCount - 1] = 0.0;
            f[internalCount - 1] -= rightBoundaryValue;

            return new TridiagonalSystem(internalCount, a, c, b, f, eps);
        }

        public static double[] GetExactValues(double[] grid)
        {
            double[] exactValues = new double[grid.Length];

            for (int i = 0; i < grid.Length; i++)
            {
                exactValues[i] = ExactSolution(grid[i]);
            }

            return exactValues;
        }

        public static double[] BuildFullNumericalSolution(double[] internalSolution)
        {
            double[] fullSolution = new double[internalSolution.Length + 2];

            fullSolution[0] = ExactSolution(0.0);

            for (int i = 0; i < internalSolution.Length; i++)
            {
                fullSolution[i + 1] = internalSolution[i];
            }

            fullSolution[fullSolution.Length - 1] = ExactSolution(1.0);

            return fullSolution;
        }

        public static double[] GetErrors(double[] exactValues, double[] numericalValues)
        {
            if (exactValues == null || numericalValues == null)
            {
                throw new ArgumentException("Помилка: масиви точного і чисельного розв'язків мають бути задані.");
            }

            if (exactValues.Length != numericalValues.Length)
            {
                throw new ArgumentException("Помилка: масиви точного і чисельного розв'язків мають однакову довжину.");
            }

            double[] errors = new double[exactValues.Length];

            for (int i = 0; i < exactValues.Length; i++)
            {
                errors[i] = Math.Abs(exactValues[i] - numericalValues[i]);
            }

            return errors;
        }
    }
}
