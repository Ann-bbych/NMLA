using System;

namespace ThomasAlgorithm
{
    public static class LeftSweepSolver
    {
        /*
        перевірка умов коректності лівої прогонки:
        |C_i| > 0,  i = 0, ..., N - 1
        |A_i| > 0, i = 1, ..., N - 1  
        |B_i| > 0, i = 0, ..., N - 2
        
        |C_0| >= |B_0|
        |C_i| >= |A_i| + |B_i|,  i = 1, ..., N - 2
        |C_(N-1)| >= |A_(N-1)|

        !!! хоча б 1 з 4-6 має бути строга
        */
        public static bool CheckNecessaryConditions(TridiagonalSystem system, double eps, out string message)
        {
            bool hasStrictInequality = false;

            // чи головна діагональ не нульова:
            for (int i = 0; i < system.N; i++)
            {
                if (Math.Abs(system.C[i]) < eps)
                {
                    message = $"На головній діагоналі є нульовий елемент: |C[{i}]| = 0.";
                    return false;
                }
            }
            // чи нижня діагональ не нульова:
            for (int i = 1; i < system.N; i++) // i=1, бо A[0] має бути 0
            {
                if (Math.Abs(system.A[i]) < eps)
                {
                    message = $"На нижній діагоналі є нульовий елемент: |A[{i}]| = 0.";
                    return false;
                }
            }
            // чи верхня діагональ не нульова:
            for (int i = 0; i < system.N - 1; i++) // до i=N-2, бо B[N-1] має бути 0
            {
                if (Math.Abs(system.B[i]) < eps)
                {
                    message = $"На верхній діагоналі є нульовий елемент: |B[{i}]| = 0.";
                    return false;
                }
            }


            if (Math.Abs(system.C[0]) + eps < Math.Abs(system.B[0]))
            { // додано eps для коректного порівняння чисел типу double
                message = "Не виконується: |C[0]| >= |B[0]|.";
                return false;
            }
            // + перевірка на строгість
            if (Math.Abs(system.C[0]) > Math.Abs(system.B[0]) + eps)
            {
                hasStrictInequality = true;
            }

            for (int i = 1; i < system.N - 1; i++)
            {
                if (Math.Abs(system.C[i]) + eps < Math.Abs(system.A[i]) + Math.Abs(system.B[i]))
                {
                    message = $"Не виконується: |C[{i}]| >= |A[{i}]| + |B[{i}]|.";
                    return false;
                }
                // + перевірка на строгість
                if (Math.Abs(system.C[i]) > Math.Abs(system.A[i]) + Math.Abs(system.B[i]) + eps)
                {
                    hasStrictInequality = true;
                }
            }

            if (Math.Abs(system.C[system.N - 1]) + eps < Math.Abs(system.A[system.N - 1]))
            {
                message = $"Не виконується: |C[{system.N - 1}]| >= |A[{system.N - 1}]|.";
                return false;
            }
            // + перевірка на строгість
            if (Math.Abs(system.C[system.N - 1]) > Math.Abs(system.A[system.N - 1]) + eps)
            {
                hasStrictInequality = true;
            }


            if (!hasStrictInequality)
            {
                message = "Немає хоча б 1 строгої нерівності серед умов діагонального переважання.";
                return false;
            }

            message = "Усі необхідні умови виконуються.";
            return true;
        }


        public static double[] Solve(TridiagonalSystem system, double eps)
        {
            double[] ksi;
            double[] eta;

            FindSweepCoefficients(system, eps, out ksi, out eta);

            return BuildSolution(system, ksi, eta, eps);
        }

        // обчислення коефіцієнтів лівої прогонки ксі, ета:
        // + зупинка при діленні на нуль
        private static void FindSweepCoefficients(TridiagonalSystem system, double eps, out double[] ksi, out double[] eta)
        {
            ksi = new double[system.N];
            eta = new double[system.N];

            if (Math.Abs(system.C[system.N - 1]) < eps)
            {
                throw new InvalidOperationException(
                    $"Помилка: C[{system.N - 1}] = 0, тому неможливо почати ліву прогонку.");
            }

            ksi[system.N - 1] = -system.A[system.N - 1] / system.C[system.N - 1];
            eta[system.N - 1] = system.F[system.N - 1] / system.C[system.N - 1];

            for (int i = system.N - 2; i >= 1; i--)
            {
                double denominator = system.C[i] + system.B[i] * ksi[i + 1];

                if (Math.Abs(denominator) < eps)
                {
                    throw new InvalidOperationException(
                        $"Помилка: під час обчислення коефіцієнтів лівої прогонки знаменник C[{i}] + B[{i}] * ksi[{i + 1}] = 0.");
                }

                ksi[i] = -system.A[i] / denominator;
                eta[i] = (system.F[i] - system.B[i] * eta[i + 1]) / denominator;
            }
        }

        // знаходження розв'язку - вектор y:
        // + зупинка при діленні на нуль
        private static double[] BuildSolution(TridiagonalSystem system, double[] ksi, double[] eta, double eps)
        {
            double[] y = new double[system.N];

            double denominator = system.C[0] + system.B[0] * ksi[1];

            if (Math.Abs(denominator) < eps) 
            {
                throw new InvalidOperationException(
                    "Помилка: під час обчислення y0 знаменник C[0] + B[0] * ksi[1] = 0.");
            }

            y[0] = (system.F[0] - system.B[0] * eta[1]) / denominator;

            for (int i = 1; i < system.N; i++)
            {
                y[i] = ksi[i] * y[i - 1] + eta[i];
            }

            return y;
        }
    }
}
