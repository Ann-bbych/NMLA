using System;
using System.IO;

namespace LUmethod
{
    internal class Program
    {
        static void Main(string[] args)
        {   // правильно знайти і створити файли у папці проекту:
            string projectDirectory = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", ".."));

            string inputFileName = Path.Combine(projectDirectory, "input.txt");
            string outputFileName = Path.Combine(projectDirectory, "output.txt");

            using StreamWriter writer = new StreamWriter(outputFileName);

            try
            {
                double[,] a;
                double[] b;
                int n;

                MatrixTools.ReadFromFile(inputFileName, out a, out b, out n);

                double[,] originalA = MatrixTools.CopyMatrix(a, n);
                double[] originalB = MatrixTools.CopyVector(b, n);

                MatrixTools.WriteLineTo(writer, "Початкова матриця A:");
                MatrixTools.PrintMatrix(a, n, writer);

                MatrixTools.WriteLineTo(writer, "Вектор b:");
                MatrixTools.PrintVector(b, n, writer);

                bool canBuildLU = LUSolver.CheckLeadingMinors(a, n);

                if (!canBuildLU)
                {
                    MatrixTools.WriteLineTo(writer, "LU-розклад неможливий.");
                    MatrixTools.WriteLineTo(writer, "Один із головних мінорів дорівнює нулю.");
                    return;
                }

                MatrixTools.WriteLineTo(writer, "LU-розклад можливий. Усі головні мінори ненульові.");
                MatrixTools.WriteLineTo(writer, "");

                double[,] l;
                double[,] u;
                LUSolver.BuildLU(a, n, out l, out u);

                MatrixTools.WriteLineTo(writer, "L:");
                MatrixTools.PrintMatrix(l, n, writer);

                MatrixTools.WriteLineTo(writer, "U:");
                MatrixTools.PrintMatrix(u, n, writer);


                double[] y = LUSolver.SolveLowerSystem(l, b, n);
                double[] x = LUSolver.SolveUpperSystem(u, y, n);

                LUSolver.PrintSolutionProcess(y, x, n, writer);

                MatrixTools.WriteLineTo(writer, "Визначник матриці:");
                // вже разом із перевіркою і точним виводом чисел:
                double determinant = LUSolver.FindDeterminant(u, n);
                string detLine = "det(A) = ";

                for (int i = 0; i < n; i++)
                {
                    string part = MatrixTools.FormatNumberExact(u[i, i]);

                    if (i == 0)
                    {
                        detLine += part;
                    }
                    else
                    {
                        detLine += " * " + part;
                    }
                }

                detLine += " = " + MatrixTools.FormatNumber(determinant);

                MatrixTools.WriteLineTo(writer, detLine);
                MatrixTools.WriteLineTo(writer, "");

                LUSolver.PrintDecompositionCheck(originalA, l, u, n, writer);
                LUSolver.PrintSolutionCheck(originalA, x, originalB, n, writer);
            }
            catch (Exception ex)
            {
                MatrixTools.WriteLineTo(writer, "Помилка: " + ex.Message);
            }
        }
    }
}