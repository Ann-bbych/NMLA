using System;
using System.IO;

namespace LUmethod
{
    public static class LUSolver
    {
        // поріг точності для обчислень (похибка double)
        private const double eps = 1e-9; // 0.000000001

        public static bool CheckLeadingMinors(double[,] a, int n)
        {
            for (int size = 1; size <= n; size++)
            {
                double[,] minor = MatrixTools.BuildLeadingMinor(a, size);
                double determinant = MatrixTools.DeterminantByGauss(minor, size);
                // обчислюю визначники для мінорів методом гаусса
                if (Math.Abs(determinant) < eps)
                {
                    return false;
                }
            }

            return true;
        }

        public static void BuildLU(double[,] a, int n, out double[,] l, out double[,] u)
        { // спосіб 1 (на гол. діаг. L одиниці) 
            l = new double[n, n];
            u = new double[n, n];

            for (int i = 0; i < n; i++)
            { 
                l[i, i] = 1.0;
            }

            for (int i = 0; i < n; i++)
            {
                // рахую елементи U: над головною діагоналлю і на ній (j >= i)
                for (int j = i; j < n; j++)
                {
                    double sum = 0.0;

                    // при i=0 цикл попускається:
                    for (int k = 0; k < i; k++)
                    {
                        sum += l[i, k] * u[k, j];
                    }//

                    u[i, j] = a[i, j] - sum;
                }
                // якщо на головінй діагоналі 0:
                if (Math.Abs(u[i, i]) < eps)
                {
                    throw new Exception("LU-розклад неможливо виконати.\nПід час побудови розкладу отримано нульовий діагональний елемент.");
                }
                // рахую елементи L: під головною діагоналлю (j > i)
                for (int j = i + 1; j < n; j++)
                {
                    double sum = 0.0;

                    for (int k = 0; k < i; k++)
                    {
                        sum += l[j, k] * u[k, i];
                    }

                    l[j, i] = (a[j, i] - sum) / u[i, i];
                }
            }
        }

        public static double[] SolveLowerSystem(double[,] l, double[] b, int n)
        {
            double[] y = new double[n];

            for (int i = 0; i < n; i++)
            {
                double sum = 0.0;
                // при i=0 цикл попускається:
                for (int j = 0; j < i; j++)
                {
                    sum += l[i, j] * y[j];
                }//

                if (Math.Abs(l[i, i]) < eps)
                {
                    throw new Exception("Неможливо розв'язати систему Ly = b, оскільки діагональний елемент матриці L дорівнює нулю.");
                }

                y[i] = (b[i] - sum) / l[i, i];
            }

            return y;
        }

        public static double[] SolveUpperSystem(double[,] u, double[] y, int n)
        {
            double[] x = new double[n];

            for (int i = n - 1; i >= 0; i--)
            {
                double sum = 0.0;
                // при i = n-1 цикл попускається:
                for (int j = i + 1; j < n; j++)
                {
                    sum += u[i, j] * x[j];
                }//

                if (Math.Abs(u[i, i]) < eps)
                {
                    throw new Exception("Неможливо розв'язати систему Ux = y, оскільки діагональний елемент матриці U дорівнює нулю.");
                }

                x[i] = (y[i] - sum) / u[i, i];
            }

            return x;
        }
        public static void PrintSolutionProcess(double[] y, double[] x, int n, StreamWriter writer)
        {
            MatrixTools.WriteLineTo(writer, "Розв'язання СЛАР:");
            MatrixTools.WriteLineTo(writer, "A x = b");
            MatrixTools.WriteLineTo(writer, "L U x = b");
            MatrixTools.WriteLineTo(writer, "L y = b, U x = y");
            MatrixTools.WriteLineTo(writer, "");

            MatrixTools.WriteLineTo(writer, "Крок 1: розв'язуємо L y = b");
            MatrixTools.WriteLineTo(writer, "Проміжний вектор y:");

            for (int i = 0; i < n; i++)
            {
                MatrixTools.WriteLineTo(writer, "y" + (i + 1) + " = " + MatrixTools.FormatNumber(y[i]));
            }

            MatrixTools.WriteLineTo(writer, "");

            MatrixTools.WriteLineTo(writer, "Крок 2: розв'язуємо U x = y");
            MatrixTools.WriteLineTo(writer, "Розв'язок (вектор x):");

            for (int i = 0; i < n; i++)
            {
                MatrixTools.WriteLineTo(writer, "x" + (i + 1) + " = " + MatrixTools.FormatNumber(x[i]));
            }

            MatrixTools.WriteLineTo(writer, "");
        }
        public static double FindDeterminant(double[,] u, int n)
        {
            double determinant = 1.0;

            for (int i = 0; i < n; i++)
            {
                determinant *= u[i, i];
            }

            return determinant;
        }

        public static void PrintSolutionCheck(double[,] a, double[] x, double[] b, int n, StreamWriter writer)
        {
            MatrixTools.WriteLineTo(writer, "Перевірка розв'язку A * x = b:");
            MatrixTools.WriteLineTo(writer, "");

            bool isCorrect = true;
            // перевіряю чи ліва частина р-ня = правій частині (b)
            // відразу виводжу обчислення в line
            for (int i = 0; i < n; i++)
            {
                double leftValue = 0.0; 
                string line = ""; 

                for (int j = 0; j < n; j++)
                {
                    double product = a[i, j] * x[j];
                    leftValue += product;

                    string coefficient = MatrixTools.FormatNumber(Math.Abs(a[i, j])); // модуль !!!! (для правильного знаку)
                    string variableValue = MatrixTools.FormatNumber(x[j]);
                    string part = coefficient + " * " + variableValue;

                    if (j == 0)
                    {
                        if (a[i, j] < 0)
                        {
                            line += "- " + part;
                        }
                        else
                        {
                            line += part;
                        }
                    }
                    else
                    {
                        if (a[i, j] < 0)
                        {
                            line += " - " + part;
                        }
                        else
                        {
                            line += " + " + part;
                        }
                    }
                }

                line += " = " + MatrixTools.FormatNumber(leftValue) +
                        " | b" + (i + 1) + " = " + MatrixTools.FormatNumber(b[i]);

                MatrixTools.WriteLineTo(writer, line);

                if (Math.Abs(leftValue - b[i]) > eps)
                {
                    isCorrect = false;
                }
            }

            if (isCorrect)
            {
                MatrixTools.WriteLineTo(writer, "\nПеревірка пройдена.");
            }
            else
            {
                MatrixTools.WriteLineTo(writer, "\nПеревірка не пройдена.");
            }

            MatrixTools.WriteLineTo(writer, "");
        }

        public static void PrintDecompositionCheck(double[,] a, double[,] l, double[,] u, int n, StreamWriter writer)
        {
            double[,] lu = MatrixTools.MultiplyMatrices(l, u, n);

            MatrixTools.WriteLineTo(writer, "Перевірка розкладу A = L * U:");
            MatrixTools.WriteLineTo(writer, "");
            MatrixTools.WriteLineTo(writer, "Матриця L * U:");
            MatrixTools.PrintMatrix(lu, n, writer);

            MatrixTools.WriteLineTo(writer, "Початкова матриця A:");
            MatrixTools.PrintMatrix(a, n, writer);

            bool isCorrect = true;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    if (Math.Abs(lu[i, j] - a[i, j]) > eps)
                    {
                        isCorrect = false;
                    }
                }
            }

            if (isCorrect)
            {
                MatrixTools.WriteLineTo(writer, "Перевірка пройдена.");
            }
            else
            {
                MatrixTools.WriteLineTo(writer, "Перевірка не пройдена.");
            }

            MatrixTools.WriteLineTo(writer, "");
        }
    }
}