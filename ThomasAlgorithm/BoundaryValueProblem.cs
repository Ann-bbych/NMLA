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

        public static TridiagonalSystem BuildSystem(double eps)
        { // готова отримана методом сіток СЛАР для крайової задачі
            int n = 4;

            double[] a = { 0.0, 1.0, 1.0, 1.0 };
            double[] c = { -2.04, -2.04, -2.04, -2.04 };
            double[] b = { 1.0, 1.0, 1.0, 0.0 };
            double[] f = { 0.0784, 0.0736, 0.0656, -0.9456 };

            return new TridiagonalSystem(n, a, c, b, f, eps);
        }

        public static double[] GetExactSolutionVector()
        { // відомий вектор точних розв'язків
            return new double[] { 0.04, 0.16, 0.36, 0.64 };
        }

        public static double[] GetErrors(double[] exactValues, double[] numericalValues)
        { // обчислення абсолютних похибок між точним і чисельним розв'язками
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