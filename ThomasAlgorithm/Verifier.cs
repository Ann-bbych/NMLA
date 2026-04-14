using System;
using System.IO;
using System.Text;

namespace ThomasAlgorithm
{
    public static class Verifier
    {
        public static void PrintVerification(TridiagonalSystem system, double[] solution, StreamWriter writer, double eps)
        {
            Formatter.WriteLineToBoth("Перевірка правильності розв'язку:", writer);
            Formatter.WriteLineToBoth(string.Empty, writer);

            bool isCorrect = true;

            for (int i = 0; i < system.N; i++)
            {
                double leftValue = GetLeftSideValue(system, solution, i); // рахую значення лівої частини рівняння 
                string line = BuildVerificationLine(system, solution, i, leftValue, eps); // формую повний і-ий рядок системи

                Formatter.WriteLineToBoth(line, writer);

                if (Math.Abs(leftValue - system.F[i]) > eps) // перевіряю правильність обчислених значень і відповідних F[i]
                {
                    isCorrect = false;
                }
            }

            Formatter.WriteLineToBoth(string.Empty, writer);

            if (isCorrect)
            {
                Formatter.WriteLineToBoth("Перевірка пройдена.", writer);
            }
            else
            {
                Formatter.WriteLineToBoth("Перевірка не пройдена.", writer);
            }

            Formatter.WriteLineToBoth(string.Empty, writer);
        }

        private static double GetLeftSideValue(TridiagonalSystem system, double[] solution, int rowIndex)
        {
            double value = 0.0;

            if (rowIndex > 0) // A_i * y_(i-1)
            {
                value += system.A[rowIndex] * solution[rowIndex - 1];
            }

            value += system.C[rowIndex] * solution[rowIndex]; // C_i * y_i

            if (rowIndex < system.N - 1) // B_i * y_(i+1)
            {
                value += system.B[rowIndex] * solution[rowIndex + 1];
            }

            return value;
        }

        private static string BuildVerificationLine(TridiagonalSystem system, double[] solution, int rowIndex, double leftValue, double eps)
        {
            StringBuilder builder = new StringBuilder();

            if (rowIndex > 0)
            {
                AppendTerm(builder, system.A[rowIndex], solution[rowIndex - 1], true, eps);
            }

            AppendTerm(builder, system.C[rowIndex], solution[rowIndex], builder.Length == 0, eps);

            if (rowIndex < system.N - 1)
            {
                AppendTerm(builder, system.B[rowIndex], solution[rowIndex + 1], builder.Length == 0, eps);
            }

            builder.Append(" = ");
            builder.Append(Formatter.FormatNumber(leftValue, eps));
            builder.Append(" | ");
            builder.Append(Formatter.FormatNumber(system.F[rowIndex], eps));

            return builder.ToString();
        }

        private static void AppendTerm(StringBuilder builder, double coefficient, double value, bool isFirst, double eps)
        {
            if (Math.Abs(coefficient) < eps)
            {
                return;
            }

            if (isFirst)
            {
                if (coefficient < 0)
                {
                    builder.Append("-");
                }
            }
            else
            {
                if (coefficient < 0)
                {
                    builder.Append(" - ");
                }
                else
                {
                    builder.Append(" + ");
                }
            }

            builder.Append(Formatter.FormatNumber(Math.Abs(coefficient), eps));
            builder.Append(" * ");
            builder.Append(Formatter.FormatNumber(value, eps));
        }
    }
}
